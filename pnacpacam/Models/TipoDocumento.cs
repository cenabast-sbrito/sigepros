using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace pnacpacam.Models
{
    public class TipoDocumento
    {

        public string tvalor { get;set ;}
        public string significado { get; set; }

        public List<TipoDocumento> GetTipoDocumentos()
        {
            string sql = "  SELECT [TValor], [Significado] FROM [dbo].[SB_Referencias] WHERE Sistema = 'FARMACIA' and Variable = 'Documento' and Atributo = 'TipoDocumento';";
            sql = "  SELECT [codigoTipoDocumento], [TipoDocumento] FROM [dbo].[inventarioTipoDocumento] ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<TipoDocumento> listaDocumentos = new List<TipoDocumento>();
            TipoDocumento doc = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    doc = new TipoDocumento();
                    doc.tvalor = reader["codigoTipoDocumento"].ToString();
                    doc.significado = reader["TipoDocumento"].ToString();
                    listaDocumentos.Add(doc);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            finally
            {
                // Cierro la Conexión.
                con.Close();
            }

            return listaDocumentos;
        }

    }

}