
using pnacpacam.Models;
using PNacPacam;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static PNacPacam.Despachos;

namespace pnacpacam.Controllers
{
    [Authorize]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class DocumentosController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 1️⃣ Validar sesión ASP.NET
            if (Session == null || Session.IsNewSession)
            {
                CerrarSesion(filterContext);
                return;
            }

            // 2️⃣ Validar Forms Authentication
            if (!Request.IsAuthenticated || User == null || !User.Identity.IsAuthenticated)
            {
                CerrarSesion(filterContext);
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        private void CerrarSesion(ActionExecutingContext filterContext)
        {
            // Limpiar sesión
            Session.Clear();
            Session.Abandon();

            // Cerrar autenticación
            FormsAuthentication.SignOut();

            // Eliminar cookie
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName)
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(cookie);
            }

            // Redirección a Login
            filterContext.Result = RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }
        // POST: Documentos/SubirDocumento: sube un documento adjunto (guia, nota credito o debito) a la carpeta del proveedor en el servidor PNAC-PACAM-PNI


        public JsonResult subirDocumento(HttpPostedFileBase documentoUpload, string _rutProveedorUpload, string _facturaLista, string _tipoDocumento)
        {
            string Mensaje = string.Empty;
            string rutaDestino = string.Empty;
            string urlBaseDocumentos = string.Empty;
            string nombreArchivo = string.Empty;

            try
            {
                // Requisito: factura debe existir
                if (Factura._existeFacturaProveedor(_rutProveedorUpload, _facturaLista) <= 0)
                    return Json(new { message = "No ha sido hallada la factura del proveedor; es requisito obligatorio para subir el documento.", status = false });

                if (documentoUpload == null || documentoUpload.ContentLength <= 0)
                    return Json(new { message = "Debe seleccionar un archivo.", status = false });

                // Validación de extensión y content-type (defensa en profundidad)
                var extension = Path.GetExtension(documentoUpload.FileName);
                if (!string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
                    return Json(new { message = "Solo se permiten archivos PDF (extensión .pdf).", status = false });

                // Algunos navegadores usan application/octet-stream; validamos lista permitida
                var contentTypesPermitidos = new[] { "application/pdf", "application/octet-stream" };
                if (!contentTypesPermitidos.Contains(documentoUpload.ContentType, StringComparer.OrdinalIgnoreCase))
                    return Json(new { message = "Tipo de contenido no permitido. Solo PDF.", status = false });

                // Tamaño máximo opcional (en MB) desde config (por ejemplo 20MB)
                ////            int maxMb = 20;
                ////            int.TryParse(ConfigurationManager.AppSettings["MaxUploadMB"], out maxMb);
                ////            long maxBytes = maxMb * 1024L * 1024L;
                ////            if (documentoUpload.ContentLength > maxBytes)
                ////                return Json(new { message = $"El archivo supera el tamaño máximo permitido de {maxMb} MB.", status = false });

                // Resolver ruta base por ambiente y tipo
                string ambiente = ConfigurationManager.AppSettings["Ambiente"];
                if (_tipoDocumento != "FC") // guía/nota crédito/nota débito
                {
                    urlBaseDocumentos = (ambiente == "Production")
                        ? ConfigurationManager.AppSettings["urlBaseIODocumentos"].ToString() + _rutProveedorUpload
                        : ConfigurationManager.AppSettings["urlBaseIODocumentos_TEST"].ToString() + _rutProveedorUpload;
                }
                else // factura de comisión
                {
                    urlBaseDocumentos = (ambiente == "Production")
                        ? ConfigurationManager.AppSettings["urlBaseIOFacturaComision"].ToString()
                        : ConfigurationManager.AppSettings["urlBaseIOFacturaComision_TEST"].ToString();
                }

                // Asegurar directorio destino
                rutaDestino = urlBaseDocumentos;
                if (!Directory.Exists(rutaDestino))
                    Directory.CreateDirectory(rutaDestino);

                // Sanitizar nombre base original
                string nombreOriginal = Path.GetFileName(documentoUpload.FileName);
                nombreOriginal = string.IsNullOrWhiteSpace(nombreOriginal) ? "documento.pdf" : nombreOriginal;

                // Remover caracteres inválidos
                foreach (var c in Path.GetInvalidFileNameChars())
                    nombreOriginal = nombreOriginal.Replace(c, '_');

                // Definir nombre objetivo
                if (_tipoDocumento != "FC")
                {
                    // Estructura: {TIPO}_{FACTURA}_{original}.pdf
                    nombreArchivo = $"{_tipoDocumento}_{_facturaLista}_{nombreOriginal}";
                }
                else
                {
                    // Estructura para FC: prefijo + rut + _ + factura + .pdf
                    string prefijo = ConfigurationManager.AppSettings["prefijoNombreComision"]?.ToString() ?? "FC_";
                    nombreArchivo = $"{prefijo}{_rutProveedorUpload}_{_facturaLista}.pdf";
                }

                string rutaCompleta = Path.Combine(rutaDestino, nombreArchivo);

                // Guardar con sobrescritura:
                // FileMode.Create => crea si no existe o sobrescribe si existe
                using (var fileStream = new FileStream(rutaCompleta, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    documentoUpload.InputStream.CopyTo(fileStream);
                }

                Mensaje = $"Archivo subido correctamente a: {rutaCompleta}";
                return Json(new { message = Mensaje, status = true });
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al subir el archivo: {ex.Message}";
                return Json(new { message = Mensaje, status = false });
            }
        }
        public JsonResult subirDocumentoOLD(HttpPostedFileBase documentoUpload, string _rutProveedorUpload, string _facturaLista, string _tipoDocumento)
        {
            string Mensaje = string.Empty;
            string rutaDestino = string.Empty;
            var urlBaseDocumentosAdjuntos = "";
            var urlBaseFacturaComision = "";
            string urlBaseDocumentos = string.Empty;
            string nombreArchivo = string.Empty;
            try
            {

                Despachos despachos = null;

                // requisito de upload de documento es que exista la factura en la BD
                if (Factura._existeFacturaProveedor(_rutProveedorUpload, _facturaLista) <= 0) return Json(new { message = "No ha sido hallada la factura del proveedor, la factura es requisito obligatorio para subir el documento.", status = false });

                string ambiente = ConfigurationManager.AppSettings["Ambiente"];
                if (_tipoDocumento != "FC") // es guia, cota credito o debito
                {
                    urlBaseDocumentos = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseIODocumentos"].ToString() + _rutProveedorUpload :
                        ConfigurationManager.AppSettings["urlBaseIODocumentos_TEST"].ToString() + _rutProveedorUpload;
                }
                else // es factura de comision
                {
                    urlBaseDocumentos = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseIOFacturaComision"].ToString() : // + _rutProveedorUpload :
                       ConfigurationManager.AppSettings["urlBaseIOFacturaComision_TEST"].ToString(); // + _rutProveedorUpload;
                }

                if (documentoUpload != null && documentoUpload.ContentLength > 0)
                {
                    // Validar que sea un PDF
                    var extension = Path.GetExtension(documentoUpload.FileName);
                    if (!string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        return Json(new { message = "Solo se permiten archivos PDF.", status = false });

                    }
                    // Ruta donde guardar el archivo 
                    rutaDestino = urlBaseDocumentos;

                    // solo para guias, notas de credito o debito: Crear el directorio si no existe
                    if (_tipoDocumento != "FC") // es guia, nota credito o debito
                    {
                        if (!Directory.Exists(rutaDestino))
                        {
                            Directory.CreateDirectory(rutaDestino);
                        }
                    }

                    if (_tipoDocumento != "FC")
                        nombreArchivo = _tipoDocumento + "_" + _facturaLista + "_" + Path.GetFileName(documentoUpload.FileName);
                    else
                        nombreArchivo = ConfigurationManager.AppSettings["prefijoNombreComision"].ToString() + _rutProveedorUpload + "_" + _facturaLista + ".pdf";

                    string rutaCompleta = Path.Combine(rutaDestino, nombreArchivo);

                    // Guardar archivo
                    documentoUpload.SaveAs(rutaCompleta);

                    Mensaje = $"Archivo subido correctamente a: {rutaCompleta}";
                    return Json(new { message = Mensaje, status = true });
                }
                else
                {
                    Mensaje = "Debe seleccionar un archivo.";
                    return Json(new { message = Mensaje, status = false });
                }
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al subir el archivo: {ex.Message}";
                return Json(new { message = Mensaje, status = true });
            }
        }

        [HttpGet]
        public JsonResult getDocumentosAdjuntos(string rut, string factura)
        {

            Movimiento doc = new Movimiento();
            if (doc.ExisteDocumentoAdjunto(rut, factura) != 1) return Json(new { state = false, message = "Sin Documentos" }, JsonRequestBehavior.AllowGet);

            try
            {


                string ambiente = ConfigurationManager.AppSettings["Ambiente"];

                var urlBase = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseIODocumentos"].ToString() + rut :
                    ConfigurationManager.AppSettings["urlBaseIODocumentos_TEST"].ToString() + rut;

                var urlDA = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseLinkDocumentos"].ToString() :
                   ConfigurationManager.AppSettings["urlBaseLinkDocumentos_TEST"].ToString();

                string folder = urlBase;
                Archivo archivo = new Archivo();
                List<Archivo> archivos = new List<Archivo>();

                string fileFind = "???" + factura + "*.pdf";

                var archivosLista = Directory.GetFiles(folder, fileFind)
                    .Select(Path.GetFileName)
                    .OrderBy(f => f)
                    .Select(f => new Archivo { Nombre = f })
                    .ToList();

                var modelo = new Archivos
                {
                    Url = urlBase + int.Parse(rut) + "/procesados/",
                    UrlDA = urlDA + int.Parse(rut) + "/",
                    Nombres = archivosLista
                };

                return Json(new { state = true, archivos = modelo }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { state = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public JsonResult getFacturaComision(string rut, string factura)
        {
            try
            {

                string ambiente = ConfigurationManager.AppSettings["Ambiente"];

                var urlBase = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseIOFacturaComision"].ToString() + rut :
                    ConfigurationManager.AppSettings["urlBaseIOFacturaComision_TEST"].ToString() + rut;


                string folder = urlBase;
                string nombreArchivo = ConfigurationManager.AppSettings["prefijoNombreComision"].ToString() + rut + "_" + factura + ".pdf";
                string rutaCompleta = Path.Combine(urlBase, nombreArchivo);


                Movimiento doc = new Movimiento();
                if (doc.existeFacturaComision(rut, factura) == 1) return Json(new { state = true, archivo = nombreArchivo }, JsonRequestBehavior.AllowGet);
                else return Json(new { state = false, archivo = "" }, JsonRequestBehavior.AllowGet);

                //    Archivo archivo = new Archivo();
                //List<Archivo> archivos = new List<Archivo>();

                //string fileFind = "???" + factura + "*.pdf";

                //var archivosLista = Directory.GetFiles(folder, fileFind)
                //    .Select(Path.GetFileName)
                //    .OrderBy(f => f)
                //    .Select(f => new Archivo { Nombre = f })
                //    .ToList();

                //var modelo = new Archivos
                //{
                //    Url = urlBase + int.Parse(rut) + "/procesados/",
                //    Nombres = archivosLista
                //};


            }
            catch (Exception e)
            {
                return Json(new { state = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        //public Boolean existeDocumentoAdjunto(string rutProveedor, string factura)
        //{
        //    return true;
        //    Movimiento doc = new Movimiento();
        //    if (doc.existeFacturaComision(rutProveedor, factura) == 1)
        //    {
        //        return true;
        //    }
        //    return false;

        //}

    }
}
