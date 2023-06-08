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
    public class DistribucionDetalle
    {
        public int IdDistribucionDetalle { get; set; }
        [Required]
        [Range(300000000, 3999999999)]
        public int Doc_Cenabast { get; set; }
        [StringLength(30, ErrorMessage = "El largo de Articulo no puede ser mayor a 30")]
        public string Articulo { get; set; }
        [StringLength(30, ErrorMessage = "El largo de Lote no puede ser mayor a 30")]
        public string Lote { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Debe ser un valor entre {1} y {2}.")]
        public int Cantidad { get; set; }
        public string FechaCreacion { get; set; }
        public string FechaActualizacion { get; set; }

        public int Ingresar(List<DistribucionDetalle> detalle)
        {
            int i = 0;
            string sql = "";

            sql = "insert into DistribucionDetalle( Doc_Cenabast, Articulo, Lote, Cantidad, FechaCreacion, FechaActualizacion)  values ( @Doc_Cenabast, @Articulo, @Lote, @Cantidad, GETDATE(), GETDATE() )";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);

            con.Open();
            try
            {
                foreach (DistribucionDetalle d in detalle)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Doc_Cenabast", d.Doc_Cenabast);
                    cmd.Parameters.AddWithValue("@Articulo", d.Articulo);
                    cmd.Parameters.AddWithValue("@Lote", d.Lote);
                    cmd.Parameters.AddWithValue("@Cantidad", d.Cantidad);
                    i = i +cmd.ExecuteNonQuery();
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

        public DistribucionDetalle Obtener(int id)
        {
            string sql = "select IdDistribucionDetalle, Doc_Cenabast, Articulo, Lote, Cantidad, FechaCreacion, FechaActualizacion from DistribucionDetalle where IdDistribucionDetalle = @IdDistribucionDetalle ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@IdDistribucionDetalle", id);
            SqlDataReader reader;
            DistribucionDetalle detalle = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    detalle = new DistribucionDetalle();
                    detalle.IdDistribucionDetalle = Convert.ToInt32(reader["IdDistribucionDetalle"].ToString());
                    detalle.Doc_Cenabast = int.Parse(reader["Doc_Cenabast"].ToString());
                    detalle.Articulo = reader["Articulo"].ToString();
                    detalle.Lote = reader["Lote"].ToString();
                    detalle.Cantidad = int.Parse(reader["Cantidad"].ToString());
                    detalle.FechaCreacion = reader["FechaCreacion"].ToString();
                    detalle.FechaActualizacion = reader["FechaActualizacion"].ToString();
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

            return detalle;
        }

        public List<DistribucionDetalle> ObtenerDetalles(int docVenta)
        {
            string sql = "select IdDistribucionDetalle, Doc_Cenabast, Articulo, Lote, Cantidad, FechaCreacion, FechaActualizacion from DistribucionDetalle where Doc_Cenabast = @docVenta ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@docVenta", docVenta);
            SqlDataReader reader;
            List<DistribucionDetalle> lista = new List<DistribucionDetalle>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    DistribucionDetalle detalle = new DistribucionDetalle();
                    detalle.IdDistribucionDetalle = Convert.ToInt32(reader["IdDistribucionDetalle"].ToString());
                    detalle.Doc_Cenabast = int.Parse(reader["Doc_Cenabast"].ToString());
                    detalle.Articulo = reader["Articulo"].ToString();
                    detalle.Lote = reader["Lote"].ToString();
                    detalle.Cantidad = int.Parse(reader["Cantidad"].ToString());
                    detalle.FechaCreacion = reader["FechaCreacion"].ToString();
                    detalle.FechaActualizacion = reader["FechaActualizacion"].ToString();
                    lista.Add(detalle);
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

        public int Actualizar(int docVenta, List<DistribucionDetalle> detalles)
        {
            int i = 0;
            string sql = "";
            sql = "update DistribucionDetalle set  Articulo = @Articulo, Lote = @Lote, Cantidad = @Cantidad, FechaActualizacion = GETDATE() where IdDistribucionDetalle = @IdDistribucionDetalle and Doc_Cenabast = @docVenta ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            
            con.Open();
            try
            {
                foreach (DistribucionDetalle d in detalles)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Articulo", d.Articulo);
                    cmd.Parameters.AddWithValue("@Lote", d.Lote);
                    cmd.Parameters.AddWithValue("@Cantidad", d.Cantidad);
                    cmd.Parameters.AddWithValue("@IdDistribucionDetalle", d.IdDistribucionDetalle);
                    cmd.Parameters.AddWithValue("@docVenta", docVenta);
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
            string sql = "delete DistribucionDetalle where IdDistribucionDetalle = @IdDistribucionDetalle ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@IdDistribucionDetalle", id);
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