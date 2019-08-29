using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerFinal.Modelo
{
    public class ControlaServidor
    {
        Dictionary<string, Socket> dicionarioConexao = new Dictionary<string, Socket>();
        List<string> listaKeys = new List<string>();
        Queue<string> filaMensagemChat = new Queue<string>();
        Queue<string> filaMensagemConexao = new Queue<string>();
        public void LigaServidor()
        {
            ControlaXml oControlaXml = new ControlaXml();
            List<Cliente> listaClientesAutenticados = new List<Cliente>();
            TcpListener servidor = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);

            oControlaXml.ExcreveXML();
            listaClientesAutenticados = oControlaXml.LeXML();

            Console.WriteLine("Servidor ligado");

            while (true)
            {
                try
                {
                    servidor.Start();

                    Socket socketConexao = servidor.AcceptSocket();
                    NetworkStream socketStream = new NetworkStream(socketConexao);
                    BinaryReader le = new BinaryReader(socketStream);
                    BinaryWriter escreve = new BinaryWriter(socketStream);

                    string mensagemCliente = le.ReadString().ToLowerInvariant();
                    string[] mensagemQuebrada = mensagemCliente.Split('|');
                    string tipoMensagem = mensagemQuebrada[0];

                    if (tipoMensagem == "login")
                    {
                        string nomeCliente = mensagemQuebrada[1];
                        Autenticacao oAutenticacao = new Autenticacao();
                        bool resultadoAutenticacao = oAutenticacao.ValidaAutenticacao(nomeCliente);

                        if (resultadoAutenticacao)
                        {
                            if (VerificaStatusCliente(listaClientesAutenticados, nomeCliente))
                            {
                                dicionarioConexao.Add(nomeCliente, socketConexao);
                                listaKeys.Add(nomeCliente);
                                escreve.Write("autenticado");
                                Console.WriteLine(nomeCliente + " está online");

                                socketConexao = null;
                                socketConexao = null;
                                le = null;
                                escreve = null;
                            }
                            else
                            {
                                escreve.Write("Você já está logado");
                            }
                        }
                        else
                        {
                            escreve.Write("não autenticado");
                            socketConexao.Close();
                            socketStream.Close();
                            le.Close();
                            escreve.Close();
                        }
                    }
                    servidor.Stop();
                }
                catch
                {
                    continue;
                }
            }
        }
        public bool VerificaStatusCliente(List<Cliente> listaCliente, string nomeCliente)
        {
            Cliente clienteAutenticado = listaCliente.FirstOrDefault(ca => ca.Apelido == nomeCliente);

            if (clienteAutenticado.Logado == false)
            {
                clienteAutenticado.Logado = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        public void EscutaConexao(Socket socketConexaoParametro)
        {
            NetworkStream networkStream = new NetworkStream(socketConexaoParametro);
            BinaryReader leAtual = new BinaryReader(networkStream);

            while (true)
            {
                if (leAtual != null)
                {
                    //ESTRUTURA MENSAGEM
                    //remetente|tipo de mensagem|conteúdo|destinatário
                    string mensagemAtual = leAtual.ReadString().ToLowerInvariant();
                    string[] mensagemAtualQuebrada = mensagemAtual.Split('|');
                    string tipoMensagem = mensagemAtualQuebrada[1];

                    //O CARA QUE ORGANIZA A FILA TEM QUE FICAR AQUIII BURRO
                    //NA THREAD QUE ESCUTA BURRO

                    if (mensagemAtual != null)
                    {
                        if (tipoMensagem == "conexao")
                        {
                            filaMensagemConexao.Enqueue(mensagemAtual);
                            mensagemAtual = null;
                        }
                        else if (tipoMensagem == "chat")
                        {
                            filaMensagemChat.Enqueue(mensagemAtual);
                            mensagemAtual = null;
                        }
                    }
                }
            }
        }
        public void ExecutaThreadParametrizada()
        {
            Dictionary<string, Thread> dicionarioThread = new Dictionary<string, Thread>();

            while (true)
            {
                if (dicionarioConexao.Count > 0 && listaKeys.Count == dicionarioConexao.Count)
                {
                    int contador = 0;

                    for (; contador < listaKeys.Count; contador++)
                    {
                        string keyAtual = listaKeys[contador];
                        bool jaExiste = dicionarioThread.ContainsKey(keyAtual);

                        if (jaExiste == false)
                        {
                            Thread novaThread = new Thread(new ThreadStart(() => EscutaConexao(dicionarioConexao[keyAtual])));
                            novaThread.Name = "Escuta usuário: " + keyAtual;

                            dicionarioThread.Add(keyAtual, novaThread);
                            dicionarioThread.Last().Value.Start();
                        }
                        Thread.Sleep(500);
                    }
                }
            }
        }
        public void DescarregaMensagens()
        {
            while (true)
            {
                //OHHHH FII DUMA VEIA, O CARA QUE ORGANIZA A FILA TEM QUE
                //FICAR NA THREAD QUE ESCUTA BURRO

                //if (mensagemAtual != null)
                //{
                //    filaMensagem.Enqueue(mensagemAtual);
                //    mensagemAtual = null;
                //}
                if (filaMensagemConexao.Count > 0)
                {
                    string[] mensagemQuebrada = filaMensagemConexao.Dequeue().Split('|');
                    Mensagem oMensagem = new Mensagem(mensagemQuebrada[0], mensagemQuebrada[1], mensagemQuebrada[2], mensagemQuebrada[3]);

                    NetworkStream networkStream = new NetworkStream(dicionarioConexao[oMensagem.destinatario]);
                    BinaryWriter escreve = new BinaryWriter(networkStream);
                    escreve.Write("ping");
                }

                if (filaMensagemChat.Count > 0)
                {
                    string[] mensagemQuebrada = filaMensagemChat.Dequeue().Split('|');
                    Mensagem oMensagem = new Mensagem(mensagemQuebrada[0], mensagemQuebrada[1], mensagemQuebrada[2], mensagemQuebrada[3]);

                    NetworkStream networkStream = new NetworkStream(dicionarioConexao[oMensagem.destinatario]);
                    BinaryWriter escreve = new BinaryWriter(networkStream);
                    escreve.Write("Mensagem de " + oMensagem.remetente + ": " + oMensagem.conteudo);
                }
            }
        }

        //public void TrocaConexao()
        //{
        //    int contador2 = 0;
        //    while (true)
        //    {
        //        if (dicionarioConexao.Count > 0 && listaKeys.Count == dicionarioConexao.Count)
        //        {
        //            int contador = 0;
        //            for (; contador < dicionarioConexao.Count; contador++, contador2++)
        //            {
        //                //Console.WriteLine("Escutando " + listaKeys[contador] + " " + contador2);

        //                networkStreamAtual = new NetworkStream(dicionarioConexao[listaKeys[contador]]);
        //                leAtual = new BinaryReader(networkStreamAtual);
        //                clienteAtual = listaKeys[contador];
        //                Thread.Sleep(300);
        //            }
        //        }
        //    }
        //}
    }
}
