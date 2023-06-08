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
    public class DistribucionMovimiento
    {

        public int IdDistribucionMovimiento { get; set; }
        [Required]
        [Range(300000000, 3999999999)]
        public int Doc_Cenabast { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        [Required]
        public int DescMovimiento { get; set; }
        public string RecibidoPor { get; set; }
        public string FechaCreacion { get; set; }
        public string FechaActualizacion { get; set; }
        public int Ingresar(List<DistribucionMovimiento> movimientos)
        {
            int i = 0;
            string sql = "";
            
            sql = "insert into distribucionMovimiento ( Doc_Cenabast, Fecha, Hora, DescMovimiento, RecibidoPor, FechaCreacion, FechaActualizacion) values ( @Doc_Cenabast, @Fecha, @Hora, @DescMovimiento, @RecibidoPor, CONVERT(varchar,GETDATE(),126), CONVERT(varchar,GETDATE(),126) )";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.CommandType = CommandType.Text;
            con.Open();
            try
            {
                foreach (DistribucionMovimiento m in movimientos)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Doc_Cenabast", m.Doc_Cenabast);
                    cmd.Parameters.AddWithValue("@Fecha", m.Fecha);
                    cmd.Parameters.AddWithValue("@Hora", m.Hora );
                    cmd.Parameters.AddWithValue("@DescMovimiento", m.DescMovimiento);
                    cmd.Parameters.AddWithValue("@RecibidoPor", m.RecibidoPor);
                    i = i + cmd.ExecuteNonQuery();
                }
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

        public int Ingresar(DistribucionMovimiento movimiento)
        {
            int i = 0;
            string sql = "insert into distribucionMovimiento ( Doc_Cenabast, Fecha, Hora, DescMovimiento, RecibidoPor, FechaCreacion, FechaActualizacion) values ( @Doc_Cenabast, @Fecha, @Hora, @DescMovimiento, @RecibidoPor, CONVERT(varchar,GETDATE(),126), CONVERT(varchar,GETDATE(),126) )";
            
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Doc_Cenabast", movimiento.Doc_Cenabast);
            cmd.Parameters.AddWithValue("@Fecha", movimiento.Fecha);
            cmd.Parameters.AddWithValue("@Hora", movimiento.Hora);
            cmd.Parameters.AddWithValue("@DescMovimiento", movimiento.DescMovimiento);
            cmd.Parameters.AddWithValue("@RecibidoPor", movimiento.RecibidoPor);

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

        public DistribucionMovimiento Obtener(int id)
        {
            string sql = "select IdDistribucionMovimiento, Doc_Cenabast, Fecha, Hora, DescMovimiento, RecibidoPor, FechaCreacion, FechaActualizacion from distribucionMovimiento where IdDistribucionMovimiento = @IdDistribucionMovimiento ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@IdDistribucionMovimiento", id);
            SqlDataReader reader;
            DistribucionMovimiento movimiento = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    movimiento = new DistribucionMovimiento();
                    movimiento.IdDistribucionMovimiento = Convert.ToInt32(reader["IdDistribucionMovimiento"].ToString());
                    movimiento.Doc_Cenabast = int.Parse(reader["Doc_Cenabast"].ToString());
                    movimiento.Fecha = reader["Fecha"].ToString();
                    movimiento.Hora = reader["Hora"].ToString();
                    movimiento.DescMovimiento = int.Parse(reader["DescMovimiento"].ToString());
                    movimiento.RecibidoPor = reader["RecibidoPor"].ToString();
                    movimiento.FechaCreacion = reader["FechaCreacion"].ToString();
                    movimiento.FechaActualizacion = reader["FechaActualizacion"].ToString();
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

            return movimiento;
        }

        public List<DistribucionMovimiento> ObtenerMovimientos(int docVenta)
        {
            string sql = "select IdDistribucionMovimiento, Doc_Cenabast, Fecha, Hora, DescMovimiento, RecibidoPor, FechaCreacion, FechaActualizacion from DistribucionMovimiento where Doc_Cenabast = @docVenta ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@docVenta", docVenta);
            SqlDataReader reader;
            List<DistribucionMovimiento> lista = new List<DistribucionMovimiento>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    DistribucionMovimiento movimiento = new DistribucionMovimiento();
                    movimiento.IdDistribucionMovimiento = Convert.ToInt32(reader["IdDistribucionMovimiento"].ToString());
                    movimiento.Doc_Cenabast = int.Parse(reader["Doc_Cenabast"].ToString());
                    movimiento.Fecha = reader["Fecha"].ToString();
                    movimiento.Hora = reader["Hora"].ToString();
                    movimiento.DescMovimiento = int.Parse(reader["DescMovimiento"].ToString());
                    movimiento.RecibidoPor = reader["RecibidoPor"].ToString();
                    movimiento.FechaCreacion = reader["FechaCreacion"].ToString();
                    movimiento.FechaActualizacion = reader["FechaActualizacion"].ToString();
                    lista.Add(movimiento);
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

        public int Actualizar(int docVenta, List<DistribucionMovimiento> movimientos)
        {
            int i = 0;
            string sql = "";

            sql = "update DistribucionMovimiento set Fecha = @Fecha , Hora = @Hora, DescMovimiento = @DescMovimiento, RecibidoPor = @RecibidoPor, FechaActualizacion = GETDATE() where IdDistribucionMovimiento = @IdDistribucionMovimiento and Doc_Cenabast = @docVenta ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            try
            {
                foreach (DistribucionMovimiento m in movimientos)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Fecha", Convert.ToDateTime(m.Fecha));
                    cmd.Parameters.AddWithValue("@Hora", Convert.ToDateTime(m.Hora));
                    cmd.Parameters.AddWithValue("@DescMovimiento", m.DescMovimiento);
                    cmd.Parameters.AddWithValue("@RecibidoPor", m.RecibidoPor);
                    cmd.Parameters.AddWithValue("@docVenta", docVenta);
                    cmd.Parameters.AddWithValue("@IdDistribucionMovimiento", m.IdDistribucionMovimiento);
                    i = i + cmd.ExecuteNonQuery();
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

            return i;
        }

        public int Eliminar(int id)
        {

            int i = 0;
            string sql = "delete DistribucionMovimiento where IdDistribucionMovimiento = @IdDistribucionMovimiento ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@IdDistribucionMovimiento", id);
            con.Open();
            try
            {
                i = cmd.ExecuteNonQuery();
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

            return i;
        }
    }

    
}