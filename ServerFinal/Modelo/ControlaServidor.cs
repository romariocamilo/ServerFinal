using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerFinal.Modelo
{
    public class ControlaServidor
    {
        public void LigaServidor()
        {
            ControlaXml oControlaXml = new ControlaXml();
            List<Cliente> listaClientesAutenticados = new List<Cliente>();
            Dictionary<string, Socket> dicionarioConexao = new Dictionary<string, Socket>();
            TcpListener servidor = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);

            oControlaXml.ExcreveXML();
            listaClientesAutenticados = oControlaXml.LeXML();

            Console.WriteLine("Servidor ligado");

            while (true)
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
                            escreve.Write("autenticado");
                            Console.WriteLine(nomeCliente + " está online");
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
                    }
                }
                servidor.Stop();
            }
        }

        static public bool VerificaStatusCliente(List<Cliente> listaCliente, string nomeCliente)
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
    }
}
