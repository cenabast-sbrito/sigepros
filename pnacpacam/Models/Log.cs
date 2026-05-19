using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace PNacPacam
{
    public class Log
    {
        private string RutUsuario { get; set; }
        private string RutProveedor { get; set; }
        private int NFactura { get; set; }
        private string FAccion { get; set; }
        private string CEstado_Actual { get; set; }
        private string CEstado_Nuevo { get; set; }
        public int addLog(string rutUsuario, string rutProveedor, int NFactura, string estado_actual, string estado_nuevo)
        {
            int logId = -1;
            string sql = "insert into [LogAcciones] (RutUsuario,RutProveedor,NFactura,FAccion,CEstado_Actual, CEstado_Nuevo) values ( @Usuario, @Proveedor, @NFactura, CONVERT(varchar,GETDATE(),126), @estado_actual, @estado_nuevo)";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Usuario", rutUsuario);
            cmd.Parameters.AddWithValue("@Proveedor", rutProveedor);
            cmd.Parameters.AddWithValue("@NFactura", NFactura);
            cmd.Parameters.AddWithValue("@estado_actual", estado_actual);
            cmd.Parameters.AddWithValue("@estado_nuevo", estado_nuevo);
            con.Open();
            try
            {
                logId = cmd.ExecuteNonQuery();
                if (logId > 0)
                {
                    return logId;
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


        public List<Log> getLog(int rut)
        {
            string sql = "select RutUsuario,RutProveedor,NFactura,FAccio,CEstado_Actual,CEstado_Nuevo from [LogAcciones] where RutProveedor = @rut";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@rut", rut);
            SqlDataReader reader;
            Log log = null;
            List<Log> logs = new List<Log>();

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    log = new Log();
                    log.RutUsuario = reader["RutUsuario"].ToString();
                    log.RutProveedor = reader["RutProveedor"].ToString();
                    log.NFactura = int.Parse(reader["NFactura"].ToString());
                    log.FAccion = reader["FAccion"].ToString();
                    log.CEstado_Actual = reader["CEstado_Actual"].ToString();
                    log.CEstado_Nuevo = reader["CEstado_Nuevo"].ToString();
                    logs.Add(log);
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
            return logs;
        }

        public string getVar(string metodo)
        {

            var archivoConfiguracionMensajes = Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "configMensaje.json");
            StreamReader documentoConfiguracionMensajes = new StreamReader(archivoConfiguracionMensajes);
            var jsonString = documentoConfiguracionMensajes.ReadToEnd();
            var mensajes = JsonConvert.DeserializeObject<List<mensajesAPI>>(jsonString);

            foreach (var x in mensajes)
            {
                if (x.metodo == metodo)
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











