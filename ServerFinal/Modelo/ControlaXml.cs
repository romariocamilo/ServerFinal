using System;
using System.Collections.Generic;
using System.Text;

namespace ServerFinal.Modelo
{
    public class ControlaXml
    {
        string caminhoArquivo = Environment.CurrentDirectory + "\\clientes.xml";
        
        //Cria lista de usuários autenticados
        public void ExcreveXML()
        {
            List<Cliente> listaClientes = new List<Cliente>();

            for (int contador = 1; contador <= 3; contador++)
            {
                switch (contador)
                {
                    case 1:
                        Cliente novoCliente = new Cliente(contador, "romario", false);
                        listaClientes.Add(novoCliente);
                        break;
                    case 2:
                        Cliente novoCliente2 = new Cliente(contador, "ricardo", false);
                        listaClientes.Add(novoCliente2);
                        break;
                    default:
                        Cliente novoCliente3 = new Cliente(contador, "herbert", false);
                        listaClientes.Add(novoCliente3);
                        break;
                }
            }

            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<Cliente>));
            System.IO.FileStream file = System.IO.File.Create(@caminhoArquivo);
            writer.Serialize(file, listaClientes);
            file.Close();
        }

        //Lê lista de usuário autenticados
        public List<Cliente> LeXML()
        {
            System.Xml.Serialization.XmlSerializer leitura = new System.Xml.Serialization.XmlSerializer(typeof(List<Cliente>));
            System.IO.StreamReader file = new System.IO.StreamReader(caminhoArquivo);
            List<Cliente> listaClientes = (List<Cliente>)leitura.Deserialize(file);
            file.Close();

            return listaClientes;
        }
    }
}
