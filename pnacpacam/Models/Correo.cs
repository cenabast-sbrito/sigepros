using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;

namespace pnacpacam.Models
{
    public class Correo
    {

        public int notificar(string rutUsuario, string rutProveedor, string factura, string estado)
        {
            int i = 0;
            //string sql = @"
            //insert into Notificaciones ( RutUsuario, RutProveedor , NFactura, FNotificacion, BLeido, TMensaje ) 
            //values ( @rutUsuario, @rutProveedor,@factura, CONVERT(varchar,GETDATE(),126), 0 , @estado ) 
            //";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand("proc_Notificacion_Agregar @rutUsuario,@rutProveedor,@factura,@estado", con);
            cmd.Parameters.AddWithValue("@rutUsuario", rutUsuario);
            cmd.Parameters.AddWithValue("@rutProveedor", rutProveedor);
            cmd.Parameters.AddWithValue("@factura", factura);
            cmd.Parameters.AddWithValue("@estado", estado);
            con.Open();
            try
            {
                i = cmd.ExecuteNonQuery();
            }
            catch (Exception)
            { i = 0; }
            return i;
        }
        public bool enviaCorreo(string nFactura, string NombreCreador, string emailDestino, string NombreDestino, string path, string motivo)
        {
            MailMessage mail = new MailMessage();
            bool i = false;

            try
            {

                string cabMensaje = "Estimad@ <strong>" + NombreDestino + "</strong>: ";
                string materia = "<br><br>" + motivo + " correspondiente a la factura <strong>" + nFactura + " </strong>, ";
                string detMensaje = "el cual se ha asignado a ud. por <strong>" + NombreCreador + "</strong>. ";
                string pieMensaje = "<br><br>Para revisar mas detalles del CDC, debe acceder a la plataforma en la siguiente dirección:";

                string body = "";
                mail.From = new MailAddress("no-reply@cenabast.cl");
                //mail.To.Add(new MailAddress(emailDestino));
                mail.To.Add(new MailAddress("sbrito@cenabast.cl"));
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                string subject = "Sistema de notificación - pnacpacam";
                mail.Subject = subject;
                body = System.IO.File.ReadAllText(path);
                body = body.Replace("@titulo", "Plataforma Control y Seguimiento del pnacpacam en Bienestar");
                body = body.Replace("@cabecera", "");
                body = body.Replace("@cuerpo", cabMensaje + materia + detMensaje + pieMensaje);
                body = body.Replace("@link", "http://pnacpacam.cenabast.cl");
                mail.Body = body;

                SmtpClient client = new SmtpClient("192.168.7.27");
                client.Port = 25;
                client.EnableSsl = false;
                client.Send(mail);
                i = true;

                Console.WriteLine("Correo enviado");

            }
            catch (Exception e)
            {
                Console.WriteLine("Ha ocurrido un error : " + e.Message + " " + e.TargetSite.Name);

            }

            return i;
        }
        public bool enviarCorreoRechazo(string Nombre, string NombreCreador, string emailCreador, string NombreGestor, string path, string motivo, string observacion)
        {
            MailMessage mail = new MailMessage();
            bool i = false;

            try
            {

                string cabMensaje = "Estimad@ <strong>" + NombreGestor + "</strong>: ";
                string materia = "<br><br>" + motivo + " con nombre <strong>" + Nombre + " </strong>, ";
                string detMensaje = "el cual se ha asignado a ud. por <strong>" + NombreCreador + "</strong>. <br><br>Observaciones: '" + observacion + "'. ";
                string pieMensaje = "<br><br> Para revisar mas detalles del CDC, debe acceder a la plataforma en la siguiente dirección:";

                string body = "";
                mail.From = new MailAddress("no-reply@cenabast.cl");
                mail.To.Add(new MailAddress(emailCreador));
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                string subject = "Sistema de notificación - pnacpacam";
                mail.Subject = subject;
                body = System.IO.File.ReadAllText(path);
                body = body.Replace("@titulo", "Plataforma Control y Seguimiento del pnacpacam en Bienestar ");
                body = body.Replace("@cabecera", "");
                body = body.Replace("@cuerpo", cabMensaje + materia + detMensaje + pieMensaje);
                body = body.Replace("@link", "http://testInventario.cenabast.cl");
                mail.Body = body;

                SmtpClient client = new SmtpClient("192.168.7.27");
                client.Port = 25;
                client.EnableSsl = false;
                client.Send(mail);
                i = true;

            }
            catch (Exception e)
            {
                Console.WriteLine("Ha ocurrido un error : " + e.Message + " " + e.TargetSite.Name);

            }

            return i;
        }




    }

}