using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace pnacpacam
{
    public class Notificacion
    {
        public string IdNotificacion { get; set; }
        public string RutUsuario { get; set; }
        public string RutProveedor { get; set; }
        public string FNotificacion { get; set; }
        public string NFactura { get; set; }
        public string BLeido { get; set; }
        public string TMensaje { get; set; }
        public string FCreacion { get; set; }
        public string FActualizacion { get; set; }
        public List<Notificacion> getNotificaciones(string Rut)
        {
            string sql = "select IdNotificacion, RutUsuario, RutProveedor, NFactura, FNotificacion, BLeido, TMensaje, FCreacion, FActualizacion from Notificaciones where RutUsuario = @Rut order by FNotificacion desc";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Rut", Rut);
            SqlDataReader reader;
            List<Notificacion> lista = new List<Notificacion>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Notificacion notificacion = new Notificacion();
                    notificacion.IdNotificacion = reader["IdNotificacion"].ToString();
                    notificacion.RutUsuario = reader["RutUsuario"].ToString();
                    notificacion.RutProveedor = reader["RutProveedor"].ToString();
                    notificacion.NFactura = reader["NFactura"].ToString();
                    notificacion.FNotificacion = reader["FNotificacion"].ToString();
                    notificacion.BLeido = reader["BLeido"].ToString();
                    notificacion.TMensaje = reader["TMensaje"].ToString();
                    notificacion.FCreacion = reader["FCreacion"].ToString();
                    notificacion.FActualizacion = reader["FActualizacion"].ToString();
                    lista.Add(notificacion);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }

            return lista;
        }
        //public int postNotificacion(Notificacion notificacion)
        //{
        //    int i = 0;
        //    string sql = "insert into Notificaciones ( RutUsuario, RutProveedor, FNotificacion, BLeido, TMensaje, FCreacion ) values ( @RutUsuario, CONVERT(varchar,GETDATE(),126), @BLeido, @TMensaje, CONVERT(varchar,GETDATE(),126) )";

        //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
        //    SqlCommand cmd = new SqlCommand(sql, con);
        //    cmd.Parameters.AddWithValue("@RutUsuario", notificacion.RutUsuario);
        //    cmd.Parameters.AddWithValue("@RutProveedor", notificacion.RutProveedor);
        //    cmd.Parameters.AddWithValue("@BLeido", notificacion.BLeido);
        //    cmd.Parameters.AddWithValue("@TMensaje", notificacion.TMensaje);

        //    con.Open();
        //    try
        //    {
        //        i = cmd.ExecuteNonQuery();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error: " + ex.ToString());
        //        i = 0;
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }

        //    return i;
        //}
        public Notificacion getNotificacion(int id)
        {
            string sql = "select IdNotificacion, RutUsuario, RutProveedor, NFactura, FNotificacion, BLeido, TMensaje from Notificaciones where IdNotificacion = @Id ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@IdNotificacion", id);
            SqlDataReader reader;
            Notificacion notificacion = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    notificacion.IdNotificacion = reader["IdNotificacion"].ToString();
                    notificacion.RutUsuario = reader["RutUsuario"].ToString();
                    notificacion.RutProveedor = reader["RutProveedor"].ToString();
                    notificacion.NFactura = reader["NFactura"].ToString();
                    notificacion.FNotificacion = reader["FNotificacion"].ToString();
                    notificacion.BLeido = reader["BLeido"].ToString();
                    notificacion.TMensaje = reader["TMensaje"].ToString();
                    notificacion.FCreacion = reader["FCreacion"].ToString();
                    notificacion.FActualizacion = reader["FActualizacion"].ToString();
                    notificacion = new Notificacion();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }

            return notificacion;
        }
        public int putNotificacionLeida(int Id, List<Notificacion> notificacion)
        {
            int i = 0;
            string sql = "";

            sql = "update DistribucionMovimiento set BLeido = @BLeido, FActualizacion = GETDATE() where IdNotificacion = @Id ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            try
            {
                foreach (Notificacion m in notificacion)
                {
                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("@BLeido", m.BLeido);
                    i = i + cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }

            return i;
        }
        public int putNotificacionRecibida(int Id, List<Notificacion> notificacion)
        {
            int i = 0;
            string sql = "";

            sql = "update Notificaciones set FNotificacion = GETDATE() where IdNotificacion = @Id ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            try
            {
                foreach (Notificacion m in notificacion)
                {
                    i = i + cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }

            return i;
        }
        public int deleteNotificacion(int id)
        {

            int i = 0;
            string sql = "delete Notificaciones where IdNotificacion = @Id ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@IdNotificacion", id);
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
                con.Close();
            }

            return i;
        }
    }


}