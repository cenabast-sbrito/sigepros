using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace pnacpacam.Models
{
    public class Periodo
    {

        public int PeriodoActivo { get; set; }
        public int Activo { get; set; }

        public Periodo() { }

        public Periodo GetPeriodo(string stringConexion)
        {
            Periodo per = null;
            string sql = "SELECT [Periodo], [Activo] FROM [dbo].[Periodo] where [Activo] = 1;";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[stringConexion].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
//            cmd.Parameters.AddWithValue("@Rut", Rut);
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    per = new Periodo();
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    per.PeriodoActivo = int.Parse(reader["Periodo"].ToString());
                    per.Activo = 1;

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
            return per;
        }
    }
}