using ServerFinal.Modelo;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ServerFinal
{
    class Program
    {
        static void Main(string[] args)
        {
            ControlaServidor oControlaServidor = new ControlaServidor();

            Thread ligaServidor = new Thread(oControlaServidor.LigaServidor);
            ligaServidor.Name = "ligaServidor";
            ligaServidor.Start();

            Thread.Sleep(2000);
            Thread escutaConexoes = new Thread(oControlaServidor.ExecutaThreadParametrizada);
            escutaConexoes.Name = "escutaConexoes";
            escutaConexoes.Start();

            //Thread trocaConexao = new Thread(oControlaServidor.TrocaConexao);
            //trocaConexao.Start();

            Thread descarregaMensagens = new Thread(oControlaServidor.DescarregaMensagens);
            descarregaMensagens.Name = "descarregaMensagens";
            descarregaMensagens.Start();
        }
    }
}
