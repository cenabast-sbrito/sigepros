using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace pnacpacam.Models
{
    public class Referencia
    {


        public string Abreviatura { get; set; }
        public string Descripcion { get; set; }
        public string TValor { get; set; }
        public string NValor { get; set; }
        public string DeberHaberNeutro { get; set; }
        public string enabled { get; set; }

        /*        public string Sistema { get; set; }
                public string Variable { get; set; }
                public string Atributo { get; set; }
                public string TablaReferenciada { get; set; }
                public string TValor { get; set; }
                public string Significado { get; set; }
                public string NValor { get; set; }
                public string DeberHaberNeutro { get; set; }*/
        public string FechaActualizacion { get; set; }
        public string FechaCreacion { get; set; }

        public Referencia getReferencia(string abreviatura)
        {
            string sql = "  SELECT [Abreviatura],[Descripcion],[TValor],[NValor],[FechaActualizacion],[FechaCreacion],[DeberHaberNeutro],[enabled] " +
                         "    FROM [dbo].[inventarioConfiguracion] where Abreviatura ='@abreviatura' ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@abreviatura", abreviatura);
            Referencia referencia = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    referencia = new Referencia();

                    referencia.Abreviatura = reader["Abreviatura"].ToString();
                    referencia.Descripcion = reader["Descripcion"].ToString();
                    referencia.TValor = reader["TValor"].ToString();
                    referencia.NValor = reader["NValor"].ToString();
                    referencia.FechaActualizacion = reader["FechaActualizacion"].ToString();
                    referencia.FechaCreacion = reader["FechaCreacion"].ToString();
                    referencia.DeberHaberNeutro = reader["DeberHaberNeutro"].ToString();
                    referencia.enabled = reader["enabled"].ToString();

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
            return referencia;
        }
        public List<Referencia> getReferencias()
        {
            string sql = "  SELECT [Abreviatura],[Descripcion],[TValor],[NValor],[FechaActualizacion],[FechaCreacion],[DeberHaberNeutro],[enabled] " +
                         "    FROM [dbo].[inventarioConfiguracion] ";


            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<Referencia> referencias = new List<Referencia>();
            Referencia referencia = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    referencia = new Referencia();

                    referencia.Abreviatura = reader["Abreviatura"].ToString();
                    referencia.Descripcion = reader["Descripcion"].ToString();
                    referencia.TValor = reader["TValor"].ToString();
                    referencia.NValor = reader["NValor"].ToString();
                    referencia.FechaActualizacion = reader["FechaActualizacion"].ToString();
                    referencia.FechaCreacion = reader["FechaCreacion"].ToString();
                    referencia.DeberHaberNeutro = reader["DeberHaberNeutro"].ToString();
                    referencia.enabled = reader["enabled"].ToString();

                    referencias.Add(referencia);

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

            return referencias;
        }

        public int Ingresar(Referencia referencia)
        {

            int i = 0;
            string sql = "insert into Referencias ([Sistema],[Variable],[Atributo],[TablaReferenciada],[TValor],[Significado],[NValor], [FechaActualizacion],[FechaCreacion]) " +
                " values ( @Sistema,@Variable,@Atributo,@TablaReferenciada,@TValor,@Significado,@NValor, GETDATE(), GETDATE())";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["primary"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("Abreviatura", referencia.Abreviatura);
            cmd.Parameters.AddWithValue("Descripcion", referencia.Descripcion);
            cmd.Parameters.AddWithValue("TValor", referencia.TValor);
            cmd.Parameters.AddWithValue("NValor", referencia.NValor);
            cmd.Parameters.AddWithValue("FechaActualizacion", referencia.FechaActualizacion);
            cmd.Parameters.AddWithValue("FechaCreacion", referencia.FechaCreacion);
            cmd.Parameters.AddWithValue("DeberHaberNeutro", referencia.DeberHaberNeutro);
            cmd.Parameters.AddWithValue("enabled", referencia.enabled);

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
                con.Close();
            }

            return i;

        }

        public int putReferencia(int IdDocumento, string Contenido)
        {
            int i = 0;
            string sql = "";

            sql = "Update Documento_Adjunto set Contenido = @Contenido  where IdDocumento = @IdDocumento; ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["primary"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@IdDocumento", IdDocumento);
            cmd.Parameters.AddWithValue("@Contenido", Contenido);
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
                con.Close();
            }

            return i;
        }


        public int Delete(int IdDocumento)
        {
            int i = 0;
            string sql = "";

            sql = "delete from Documento_Adjunto where IdDocumento = @IdDocumento; ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["primary"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@IdDocumento", IdDocumento);
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
                con.Close();
            }

            return i;
        }


    }


}