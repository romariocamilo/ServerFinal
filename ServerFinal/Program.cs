using ServerFinal.Modelo;
using System;
using System.Collections.Generic;

namespace ServerFinal
{
    class Program
    {
        static void Main(string[] args)
        {
            ControlaServidor oControlaServidor = new ControlaServidor();
            oControlaServidor.LigaServidor();
        }
    }
}
