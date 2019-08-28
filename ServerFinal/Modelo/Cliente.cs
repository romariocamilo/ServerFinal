using System;
using System.Collections.Generic;
using System.Text;

namespace ServerFinal.Modelo
{
    [Serializable]
    public class Cliente
    {
        public int ClienteId { get; set; }
        public string Apelido { get; set; }
        public bool Logado { get; set; }
        public Cliente()
        {

        }
        public Cliente(int ClienteId, string Apelido, bool Logado)
        {
            this.ClienteId = ClienteId;
            this.Apelido = Apelido;
            this.Logado = Logado;
        }
    }
}
