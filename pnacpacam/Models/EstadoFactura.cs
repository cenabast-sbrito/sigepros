using pnacpacam.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace pnacpacam
{
    public class Estado
    {
        public string CEstado { get; set; }
        public string TEstado { get; set; }
        public string TEstadoTiempoPresente { get; set; }
        public List<Estado> _getEstados(string rol)
        {


            string esHistorico = "";
            string sql = @"
                   SELECT CEstado, TEstado, TEstadoTiempoPresente FROM [dbo].[Estado]  
                    WHERE 
                        (
                               (@rol IN ('MIN','MIP') AND @esHistorico = '' AND CEstado IN (4,5,6))
                            OR (@rol IN ('ADM','ADP') AND @esHistorico = '' AND CEstado IN (0,1,2))
                            OR (@rol IN ('CON','COP') AND @esHistorico = '' AND CEstado IN (3,11))
                            OR (@rol IN ('MIN','ADM','MIP','ADP') AND @esHistorico = '1' AND CEstado IN (7,8,9,10))
                        )
            ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            Estado prov = null;
            List<Estado> lista = new List<Estado>();

            cmd.Parameters.AddWithValue("@rol", rol);
            cmd.Parameters.AddWithValue("@esHistorico", esHistorico);

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        prov = new Estado();
                        prov.CEstado = reader["CEstado"].ToString();
                        prov.TEstado = reader["TEstado"].ToString();
                        prov.TEstadoTiempoPresente = reader["TEstadoTiempoPresente"].ToString();

                        lista.Add(prov);
                    }
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

    }

    public class EstadoFactura
    {
        public string RutProveedor { get; set; }
        public string NFactura { get; set; }
        public string EnRevisionCenabast { get; set; }
        public string DerivadoAContratos { get; set; }
        public string ListoParaRevisionMinsal { get; set; }
        public string EnRevisionMinsal { get; set; }
        public string Rechazado { get; set; }
        public string Aceptado { get; set; }
        public string AceptadoConObservacion { get; set; }
        public string EnGestionPago { get; set; }
        public string Expediente { get; set; }
        public string Certificado { get; set; }
        public string FacturaAdjunta { get; set; }
        public EstadoFactura putEstado(string rutusuario, string rutProveedor, string NFactura, string campo, string obs)
        {
            string mje = "";
            string updateCampo = "";
            string insertCampo = "";

            switch (campo)
            {
                case "1": insertCampo = "[EnRevisionCenabast]"; updateCampo = "[EnRevisionCenabast] = CONVERT(varchar,GETDATE(),126) "; break;
                case "2": insertCampo = "[DevueltoContrato]"; updateCampo = "[DevueltoContrato] = CONVERT(varchar,GETDATE(),126) "; break;
                case "3": insertCampo = "[EnviarContrato]"; updateCampo = "[EnviarContrato] = CONVERT(varchar,GETDATE(),126) "; break;
                case "4": insertCampo = "[EnviarMinsal]"; updateCampo = "[EnviarMinsal] = CONVERT(varchar,GETDATE(),126) "; break;
                case "5": insertCampo = "[EnRevisionMinsal]"; updateCampo = "[EnRevisionMinsal] = CONVERT(varchar,GETDATE(),126) "; break;
                case "6": insertCampo = "[Rechazado]"; updateCampo = "[Rechazado] = CONVERT(varchar,GETDATE(),126) "; break;
                case "7": insertCampo = "[Aceptado]"; updateCampo = "[Aceptado] = CONVERT(varchar,GETDATE(),126) "; break;
                case "8": insertCampo = "[AceptadoObservacion]"; updateCampo = "[AceptadoObservacion] = CONVERT(varchar,GETDATE(),126) "; break;
                case "9": insertCampo = "[EnGestionPago]"; updateCampo = "[EnGestionPago] = CONVERT(varchar,GETDATE(),126) "; break;
                case "10": insertCampo = "[Cerrado]"; updateCampo = "[Cerrado] = CONVERT(varchar,GETDATE(),126) "; break;
                case "11": insertCampo = "[OKContrato]"; updateCampo = "[OKContrato] = CONVERT(varchar,GETDATE(),126) "; break;
                case "Expediente": insertCampo = "[Expediente]"; updateCampo = "[Expediente] = CONVERT(varchar,GETDATE(),126) "; break;
                default: insertCampo = "[EnRevisionCenabast]"; updateCampo = "[EnRevisionCenabast] = CONVERT(varchar,GETDATE(),126) "; break;
            }

            string updateCampoEstado = campo != "Expediente" ? ", NEstado = @NEstado " : "";

            // 🔹 Un solo bloque SQL que maneja inserción o actualización
            string sql = $@"
            IF EXISTS (SELECT 1 FROM EstadoFactura WHERE RutProveedor = @RutProveedor AND NFactura = @NFactura)
            BEGIN
                UPDATE EstadoFactura
                SET FEstado = CONVERT(varchar,GETDATE(),126),
                    TObservacion = @Obs
                    {updateCampoEstado},
                    {updateCampo}
                WHERE RutProveedor = @RutProveedor AND NFactura = @NFactura;
            END
            ELSE
            BEGIN
                INSERT INTO EstadoFactura (RutProveedor, NFactura, {(campo != "Expediente" ? "NEstado," : "")} {insertCampo}, FEstado, TObservacion)
                VALUES (@RutProveedor, @NFactura, {(campo != "Expediente" ? "@NEstado," : "")} CONVERT(varchar,GETDATE(),126), CONVERT(varchar,GETDATE(),126), @Obs);
            END";

            int i = 0;

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@RutProveedor", rutProveedor);
                cmd.Parameters.AddWithValue("@NFactura", NFactura);
                cmd.Parameters.AddWithValue("@Obs", obs ?? "");
                if (campo != "Expediente") cmd.Parameters.AddWithValue("@NEstado", campo);

                con.Open();
                i = cmd.ExecuteNonQuery();
            }

            // 🔹 Enviar correo solo si hubo acción
            if (i > 0)
            {
                Correo correo = new Correo();
                mje = getMensajeNotificacion(campo, NFactura);
                correo.notificar(rutusuario, rutProveedor, NFactura, mje);
            }

            return null;
        }

        public EstadoFactura putEstadoOLD(string rutusuario, string rutProveedor, string NFactura, string campo, string obs)
        {
            string mje = "";

            string updateCampo = "";
            string insertCampo = "";

            switch (campo)
            {
                case "1":
                    insertCampo = "[EnRevisionCenabast]";
                    updateCampo = "[EnRevisionCenabast]      = CONVERT(varchar,GETDATE(),126) ";
                    break;

                case "2":
                    insertCampo = "[DevueltoContrato]";
                    updateCampo = "[DevueltoContrato]        = CONVERT(varchar,GETDATE(),126) ";
                    break;

                case "3":
                    insertCampo = "[EnviarContrato]";
                    updateCampo = "[EnviarContrato]          = CONVERT(varchar,GETDATE(),126) ";
                    break;

                case "4":
                    insertCampo = "[EnviarMinsal]";
                    updateCampo = "[EnviarMinsal]            = CONVERT(varchar,GETDATE(),126) ";
                    break;

                case "5":
                    insertCampo = "[EnRevisionMinsal]";
                    updateCampo = "[EnRevisionMinsal]        = CONVERT(varchar,GETDATE(),126) ";
                    break;

                case "6":
                    insertCampo = "[Rechazado]";
                    updateCampo = "[Rechazado]               = CONVERT(varchar,GETDATE(),126) ";
                    break;

                case "7":
                    insertCampo = "[Aceptado]";
                    updateCampo = "[Aceptado]                = CONVERT(varchar,GETDATE(),126) ";
                    break;

                case "8":
                    insertCampo = "[AceptadoObservacion]";
                    updateCampo = "[AceptadoObservacion]     = CONVERT(varchar,GETDATE(),126) ";
                    break;

                case "9":
                    insertCampo = "[EnGestionPago]";
                    updateCampo = "[EnGestionPago]           = CONVERT(varchar,GETDATE(),126) ";
                    break;

                case "10":
                    insertCampo = "[Cerrado]";
                    updateCampo = "[Cerrado]                 = CONVERT(varchar,GETDATE(),126) ";
                    break;

                case "11":
                    insertCampo = "[OKContrato]";
                    updateCampo = "[OKContrato]              = CONVERT(varchar,GETDATE(),126) ";
                    break;
                case "Expediente":
                    insertCampo = "[Expediente]";
                    updateCampo = "[Expediente]              = CONVERT(varchar,GETDATE(),126) ";
                    break;

                default:
                    insertCampo = "[EnRevisionCenabast]";
                    updateCampo = "[EnRevisionCenabast] = CONVERT(varchar,GETDATE(),126) ";
                    break;
            }

            string updateCampoEstado = "";
            if (!(campo == "Expediente"))
            {
                updateCampoEstado = ", NEstado = @NEstado ";
            }


            int i = 0;
            string sql = "";
            if (campo == "Expediente")
            {
                sql = "insert into EstadoFactura (RutProveedor,NFactura,Expediente) values ( @RutProveedor, @NFactura, CONVERT(varchar,GETDATE(),126))";
            }
            else
            {
                sql = "insert into EstadoFactura (RutProveedor,NFactura,NEstado " +
                             " ," + insertCampo + " , FEstado, TObservacion ) values ( @RutProveedor, @NFactura, @NEstado, " +
                             " CONVERT(varchar,GETDATE(),126), CONVERT(varchar,GETDATE(),126), '" + obs + "')";
            }

            string sqlupd = "update EstadoFactura " +
                            "   set FEstado  = CONVERT(varchar,GETDATE(),126) " +
                            "      , TObservacion = '" + obs + "'" +

                                   updateCampoEstado + " ," + updateCampo +

                            " where RutProveedor = @RutProveedor and NFactura = @NFactura ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RutProveedor", rutProveedor);
            cmd.Parameters.AddWithValue("@NFactura", NFactura);
            if (!(campo == "Expediente")) cmd.Parameters.AddWithValue("@NEstado", campo);
            con.Open();
            try
            {
                i = cmd.ExecuteNonQuery();
                if (i > 0)
                {

                    Correo correo = new Correo();
                    mje = getMensajeNotificacion(campo, NFactura);
                    i = correo.notificar(rutusuario, rutProveedor, NFactura, mje);

                }
            }
            catch (Exception ex)
            {
                SqlCommand cmdupd = new SqlCommand(sqlupd, con);
                cmdupd.Parameters.AddWithValue("@RutProveedor", rutProveedor);
                cmdupd.Parameters.AddWithValue("@NFactura", NFactura);
                cmdupd.Parameters.AddWithValue("@NEstado", campo);
                try
                {
                    i = cmdupd.ExecuteNonQuery();
                    if (i > 0)
                    {

                        Correo correo = new Correo();
                        mje = getMensajeNotificacion(campo, NFactura);
                        i = correo.notificar(rutusuario, rutProveedor, NFactura, mje);

                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error: " + ex.ToString());
                    i = 0;
                }
            }
            finally
            {
                con.Close();
            }

            return null;
        }
        public string getMensajeNotificacion(string cestado, string NFactura)
        {
            string mensaje = "";
            string sql = "SELECT [TMensaje] FROM [dbo].[Estado] where [CEstado] = @cestado";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@cestado", cestado);

            SqlDataReader reader;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    mensaje = reader["tmensaje"].ToString();
                    mensaje = mensaje.Replace("{factura}", NFactura);

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
            return mensaje;


        }
    }
}