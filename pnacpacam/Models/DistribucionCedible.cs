using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace pnacpacam
{
    public class DistribucionCedible
    {
        public int IdDistribucionCedible { get; set; }
        [Required]
        [Range(300000000, 399999999)]
        public int Doc_Cenabast { get; set; }
        [Required]
        public string Rut_Proveedor { get; set; }
        [Required]
        public string Documento { get; set; }
        public string FechaCreacion { get; set; }
        public string FechaActualizacion { get; set; }

        public int IngresarCedible(DistribucionCedible cedible)
        {
            int i = 0;
            string sql = "insert into DistribucionCedible (Doc_Cenabast, Rut_Proveedor, Documento, FechaCreacion, FechaActualizacion) values ( @Doc_Cenabast, @Rut_Proveedor, @Documento, CONVERT(varchar,GETDATE(),126), CONVERT(varchar,GETDATE(),126))";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Doc_Cenabast", cedible.Doc_Cenabast);
            cmd.Parameters.AddWithValue("@Rut_Proveedor", cedible.Rut_Proveedor);
            cmd.Parameters.AddWithValue("@Documento", cedible.Documento);
            con.Open();
            try
            {
                i = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                i = 0;
            }
            finally
            {
                // Cierro la Conexión.
                con.Close();
            }

            return i;
        }

        public DistribucionCedible Obtener(int Doc_Cenabast)
        {
            string sql = "select  IdDistribucionCedible, Doc_Cenabast, Documento, Rut_Proveedor, FechaCreacion, FechaActualizacion from DistribucionCedible where Doc_Cenabast = @Doc_Cenabast ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Doc_Cenabast", Doc_Cenabast);
            SqlDataReader reader;
            DistribucionCedible cedible = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    cedible = new DistribucionCedible();
                    cedible.IdDistribucionCedible = int.Parse(reader["IdDistribucionCedible"].ToString());
                    cedible.Doc_Cenabast = int.Parse(reader["Doc_Cenabast"].ToString());
                    cedible.Rut_Proveedor = reader["Rut_Proveedor"].ToString();
                    cedible.Documento = reader["Documento"].ToString();
                    cedible.FechaCreacion = reader["FechaCreacion"].ToString();
                    cedible.FechaActualizacion = reader["FechaActualizacion"].ToString();
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

            return cedible;
        }

        public List<DistribucionCedible> ObtenerCedibles()
        {
            string sql = "select  IdDistribucionCedible, Doc_Cenabast, Documento, Rut_Proveedor, FechaCreacion, FechaActualizacion from DistribucionCedible order by IdDistribucionCedible desc";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader;
            List<DistribucionCedible> lista = new List<DistribucionCedible>();
            DistribucionCedible cedible = null;

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    cedible = new DistribucionCedible();
                    cedible.IdDistribucionCedible = int.Parse(reader["IdDistribucionCedible"].ToString());
                    cedible.Doc_Cenabast = int.Parse(reader["Doc_Cenabast"].ToString());
                    cedible.Rut_Proveedor = reader["Rut_Proveedor"].ToString();
                    cedible.Documento = reader["Documento"].ToString();
                    cedible.FechaCreacion = reader["FechaCreacion"].ToString();
                    cedible.FechaActualizacion = reader["FechaActualizacion"].ToString();
                    lista.Add(cedible);
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

            return lista;
        }

        public bool IsBase64(string base64String)
        {
            if (base64String.Replace(" ", "").Length % 4 != 0)
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            return false;
        }


    }
}