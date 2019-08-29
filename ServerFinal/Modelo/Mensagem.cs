using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerFinal.Modelo
{
    public class Mensagem
    {
        public string remetente { get; set; }
        public string tipo { get; set; }
        public string conteudo { get; set; }
        public string destinatario { get; set; }

        public Mensagem()
        {

        }
        public Mensagem(string remetente, string tipo, string conteudo, string destinatario)
        {
            this.remetente = remetente;
            this.tipo = tipo;
            this.conteudo = conteudo;
            this.destinatario = destinatario;
        }
        public void TrataMensagem(Queue<string> filaMensagemConexao, Queue<string> filaMensagemChat, Queue<string> filaMensagemRequisicao, Dictionary<string, Socket> dicionarioConexao, List<Cliente> listaClientes, Dictionary<string, Thread> dicionarioThread, List<string> listaKeys)
        {
            string[] mensagemQuebrada = null;
            NetworkStream networkStream;
            BinaryWriter escreve;

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
                    try
                    {
                        mensagemQuebrada = filaMensagemConexao.Dequeue().Split('|');
                        Mensagem oMensagem = new Mensagem(mensagemQuebrada[0], mensagemQuebrada[1], mensagemQuebrada[2], mensagemQuebrada[3]);

                        networkStream = new NetworkStream(dicionarioConexao[oMensagem.destinatario]);
                        escreve = new BinaryWriter(networkStream);
                        escreve.Write("ping");
                    }
                    catch (Exception ex)
                    {
                        ControlaServidor oControlaServidor = new ControlaServidor();
                        oControlaServidor.RemoveConexao(dicionarioConexao, listaClientes, listaKeys, dicionarioThread, mensagemQuebrada[0]);
                    }
                }

                if (filaMensagemChat.Count > 0)
                {
                    mensagemQuebrada = filaMensagemChat.Dequeue().Split('|');
                    Mensagem oMensagem = new Mensagem(mensagemQuebrada[0], mensagemQuebrada[1], mensagemQuebrada[2], mensagemQuebrada[3]);
                    //NetworkStream networkStream;
                    //BinaryWriter escreve;

                    if (oMensagem.destinatario == "all")
                    {
                        for (int contador = 0; contador < dicionarioConexao.Count; contador++)
                        {
                            networkStream = new NetworkStream(dicionarioConexao[listaKeys[contador]]);
                            escreve = new BinaryWriter(networkStream);
                            string remetente = oMensagem.remetente;

                            if (remetente != listaKeys[contador])
                            {
                                escreve.Write("Mensagem geral de " + oMensagem.remetente + ": " + oMensagem.conteudo);
                            }
                        }
                    }
                    else
                    {
                        if (ValidaDestinatario(dicionarioConexao, oMensagem.destinatario))
                        {
                            networkStream = new NetworkStream(dicionarioConexao[oMensagem.destinatario]);
                            escreve = new BinaryWriter(networkStream);

                            if (oMensagem.remetente != oMensagem.destinatario)
                            {
                                escreve.Write("Mensagem de " + oMensagem.remetente + ": " + oMensagem.conteudo);
                            }
                            else
                            {
                                escreve.Write("Mensagem pra você mesmo não dá jovenzinho");
                            }
                        }
                        else
                        {
                            networkStream = new NetworkStream(dicionarioConexao[oMensagem.remetente]);
                            escreve = new BinaryWriter(networkStream);
                            escreve.Write("Esse usuário não existe");
                        }
                    }
                }

                if (filaMensagemRequisicao.Count > 0)
                {
                    mensagemQuebrada = filaMensagemRequisicao.Dequeue().Split('|');
                    Mensagem oMensagem = new Mensagem(mensagemQuebrada[0], mensagemQuebrada[1], mensagemQuebrada[2], mensagemQuebrada[3]);

                    string conteudo = oMensagem.conteudo.Replace(" ", "");

                    networkStream = new NetworkStream(dicionarioConexao[oMensagem.remetente]);
                    escreve = new BinaryWriter(networkStream);

                    if (conteudo == "usuariosonline")
                    {
                        //networkStream = new NetworkStream(dicionarioConexao[oMensagem.remetente]);
                        //escreve = new BinaryWriter(networkStream);

                        string usuarioOnline = "Usuários online: ";

                        for (int contador = 0; contador < listaKeys.Count; contador++)
                        {
                            if (listaKeys[contador] == listaKeys.Last())
                            {
                                usuarioOnline = usuarioOnline + listaKeys[contador] + ". ";
                            }
                            else
                            {
                                usuarioOnline = usuarioOnline + listaKeys[contador] + " , ";
                            }
                        }
                        escreve.Write(usuarioOnline);
                    }
                    else
                    {
                        //networkStream = new NetworkStream(dicionarioConexao[oMensagem.remetente]);
                        //escreve = new BinaryWriter(networkStream);
                        escreve.Write("Essa requisição não está em nossos servidores");
                    }

                }
            }
        }
        public bool ValidaDestinatario(Dictionary<string, Socket> dicionarioConexao, string keyConexao)
        {
            if (dicionarioConexao.ContainsKey(keyConexao))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
