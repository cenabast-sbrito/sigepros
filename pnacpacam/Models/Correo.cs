using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace pnacpacam.Models
{
    public class Correo
    {
        public bool enviaCorreo(string NombreConvenio, string NombreCreador, string emailDestino, string NombreDestino, string path, string motivo)
        {
            MailMessage mail = new MailMessage();
            bool i = false;

            try
            {

                string cabMensaje = "Estimad@ <strong>" + NombreDestino + "</strong>: ";
                string materia = "<br><br>" + motivo + " correspondiente al <strong>" + NombreConvenio + " </strong>, ";
                string detMensaje = "el cual se ha asignado a ud. por <strong>" + NombreCreador + "</strong>. ";
                string pieMensaje = "<br><br>Para revisar mas detalles del CDC, debe acceder a la plataforma en la siguiente dirección:";

                string body = "";
                mail.From = new MailAddress("no-reply@cenabast.cl");
                mail.To.Add(new MailAddress(emailDestino));
                //mail.To.Add(new MailAddress("rsalazar@cenabast.cl"));
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                string subject = "Sistema de notificación - pnacpacam";
                mail.Subject = subject;
                body = System.IO.File.ReadAllText(path);
                body = body.Replace("@titulo", "Plataforma Control y Seguimiento del pnacpacam en Bienestar");
                body = body.Replace("@cabecera", "");
                body = body.Replace("@cuerpo", cabMensaje + materia + detMensaje + pieMensaje);
                body = body.Replace("@link", "http://pnacpacam.cenabast.cl");
                //body = body.Replace("@link", "http://testInventario.cenabast.cl");
                mail.Body = body;

                SmtpClient client = new SmtpClient("10.8.0.27");
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

                SmtpClient client = new SmtpClient("10.8.0.27");
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