using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static PNacPacam.Despachos;

namespace pnacpacam.Models
{
    public class Programa
    {
        public int cprograma {  get; set; }
        public string tprograma { get; set; }
        public List<Programa> getListProgramas()
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);

            SqlCommand cmd = new SqlCommand("select cprograma, tprograma_breve from programa", con);

            SqlDataReader reader;

            Programa programa = null;
            List<Programa> programas = new List<Programa>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    programa = new Programa();
                    programa.cprograma = int.Parse(reader["cprograma"].ToString());
                    programa.tprograma = reader["tprograma_breve"].ToString();
                    programas.Add(programa);
                }
            }
            catch (SqlException sex)
            {
                Console.WriteLine("Error: " + sex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return programas;
        }

    }
}
