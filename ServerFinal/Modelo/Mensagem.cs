using System;
using System.Collections.Generic;
using System.Text;

namespace ServerFinal.Modelo
{
    public class Mensagem
    {
        public string remetente { get; set; }
        public string tipo { get; set; }
        public string conteudo { get; set; }
        public string destinatario { get; set; }

        public Mensagem(string remetente, string tipo, string conteudo, string destinatario)
        {
            this.remetente = remetente;
            this.tipo = tipo;
            this.conteudo = conteudo;
            this.destinatario = destinatario;
        }
    }
}
