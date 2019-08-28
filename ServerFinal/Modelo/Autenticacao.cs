using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerFinal.Modelo
{
    public class Autenticacao
    {
        public bool ValidaAutenticacao(string nomeCliente)
        {
            ControlaXml oControlaXml = new ControlaXml();
            List<Cliente> listaClientesAutenticados = oControlaXml.LeXML();

            Cliente nomeClienteAutenticado = listaClientesAutenticados.FirstOrDefault(lca => lca.Apelido == nomeCliente);

            if (nomeClienteAutenticado != null)
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
