using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Xml.XPath;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;
using WebApi.Areas.HelpPage;
using Newtonsoft.Json;
using System.IO;
using System.Web.DynamicData;
using System.Web.Http.Results;
using System.Web.Helpers;

namespace WebApi.Models
{
    /// <summary>
    /// Esta es la clase base de distribucion que representa la informacion logistica necesaria para tener claridad de la distribucion
    /// </summary> 
    public class Log
    {
        private int Id { get; set; } //IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
        private string Ip { get; set; } // varchar] (15) NULL,
	    private string Usuario { get; set; } // ][varchar] (10) NULL,
	    private string Fecha   { get; set; } // ][datetime] NULL,
        private string Accion { get; set; } // ][varchar] (100) NULL,
        private string Metodo { get; set; } // ][varchar] (100) NULL,
        /*
                        [Required]
                        public List<LogDetalle> LogDetalle { get; set; }
                */
        /// <summary>
        /// Registra en BD un Objeto Log  y retorna el identificador con el que lo ha guardado
        /// </summary> 
        public int addLog( string usuario, string accion, string metodo)
        {
            //int i = 0; 
            int logId = -1; int res=0; 
            string sql = "insert into APILog (Ip,Usuario,Fecha,Accion, Metodo) values ( @Ip, @Usuario, CONVERT(varchar,GETDATE(),126), @Accion, @Metodo)";
            string sqlId = "SELECT @@IDENTITY AS 'Identity'";
            SqlDataReader reader;
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlCommand cmdId = new SqlCommand(sqlId, con);
            cmd.Parameters.AddWithValue("@Ip", HttpContext.Current.Request.UserHostAddress);
            cmd.Parameters.AddWithValue("@Usuario", usuario);
            cmd.Parameters.AddWithValue("@Accion", accion);
            cmd.Parameters.AddWithValue("@Metodo", metodo);
            /* if (string.IsNullOrEmpty(distribucion.Fecha_Fac)) cmd.Parameters.AddWithValue("@Fecha_Fac", DBNull.Value); else cmd.Parameters.AddWithValue("@Fecha_Fac", distribucion.Fecha_Fac); */
            con.Open();
            try
            {
                res = cmd.ExecuteNonQuery();
                reader = cmdId.ExecuteReader();
                if (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    
                    logId = int.Parse(reader["Identity"].ToString());



                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                logId = -1;
            }
            finally
            {
                con.Close();
            }
            return logId;
        }
        public int addLogDetalle(int idLog, string Mensaje)
        {
            if (idLog==0) return 0; 
            int logId = 0;
            string sql = "insert into APILogDetalle (idLog,Mensaje) values ( @idLog, @Mensaje )";
            SqlDataReader reader;
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@idLog", idLog);
            cmd.Parameters.AddWithValue("@Mensaje", Mensaje);
            con.Open();
            try
            {
                logId = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                logId = -1;
            }
            finally
            {
                con.Close();
            }
            return logId;
        }
        /// <summary>
        /// Obtiene un Objeto Distribucion a BBDD
        /// </summary> 
        public Log getLog(int id)
        {
            string sql = "select Id,Ip,Usuario,Fecha,Accion,Metodo from APILog where Id = @id";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);
            SqlDataReader reader;
            Log log = null;
//            LogDetalle det = new LogDetalle();

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    log = new Log();
                    log.Id = int.Parse(reader["Id"].ToString());
                    log.Ip = reader["Ip"].ToString();
                    log.Usuario = reader["Usuario"].ToString();
                    log.Fecha = reader["Fecha"].ToString();
                    log.Accion = reader["Accion"].ToString();
                    log.Metodo = reader["Metodo"].ToString();
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
            return log;
        }
        /// <summary>
        /// Obtener el ultimo Id generado debe ser ejecutado inmediatamente despues del insert
        /// </summary>
        /// <returns></returns>
        public int getLastId()
        {
            string sql = "SELECT @@IDENTITY AS 'Identity'";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            int logId = 0;
            //            LogDetalle det = new LogDetalle();

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    logId = int.Parse(reader["Identity"].ToString());
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
            return logId;
        }
        public string getVar(string metodo)
        {

            var archivoConfiguracionMensajes = Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "configMensaje.json");
            StreamReader documentoConfiguracionMensajes = new StreamReader(archivoConfiguracionMensajes);
            var jsonString = documentoConfiguracionMensajes.ReadToEnd();
            var mensajes = JsonConvert.DeserializeObject<List<mensajesAPI>>(jsonString);

            foreach (var x in mensajes)
            {
                if ( x.metodo==metodo)
                {
                    return x.mensaje;
                }
            }
            return null;

        }
    }
    public class mensajesAPI
    {
        public string metodo { get; set; }
        public string mensaje { get; set; }
        public string status { get; set; }

    }
}











