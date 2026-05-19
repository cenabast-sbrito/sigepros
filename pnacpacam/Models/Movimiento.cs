using Aspose.Pdf.Facades;
using Aspose.Pdf.Operators;
using DocumentFormat.OpenXml.Office.Word;
using pnacpacam;
using pnacpacam.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;


namespace PNacPacam
{
    public class Movimiento
    {
        public string resultado { get; set; }

        
        public string crearExpediente(string rut, string factura)
        {
            var ambiente = ConfigurationManager.AppSettings["Ambiente"].ToString();

            var urlBaseCedibles = (ambiente=="Production")? ConfigurationManager.AppSettings["urlBaseCedibles"].ToString()     : ConfigurationManager.AppSettings["urlBaseCedibles_TEST"].ToString();
            var urlBaseDestino  = (ambiente=="Production")? ConfigurationManager.AppSettings["urlBaseIOExpedientes"].ToString(): ConfigurationManager.AppSettings["urlBaseIOExpedientes_TEST"].ToString();
            var urlCargaSapPro  = (ambiente=="Production")? ConfigurationManager.AppSettings["urlCargaSapPro"].ToString()      : ConfigurationManager.AppSettings["urlCargaSapPro_TEST"].ToString()      ; // UNC también

            List<Movimiento> lista = new List<Movimiento>();

            string backupDir, sourceDir, bckupDirFull, doc_venta, fileName, NombreExpediente;
            Despachos despacho = new Despachos();

            var listaDespachos = despacho.getDespachosByFactura(rut, factura, true);
            string[] dire = new string[listaDespachos.Count];
            int ind = 0;
            String res = "";

            foreach (var despa in listaDespachos)
            {
                doc_venta = despa.DocumentoVenta;


                /*
                 * seleccionar el ultimo documento correcto cargado
                 */

                // -------------------------------------------
                // 1) RUTA UNC DEL DIRECTORIO "procesados"
                // -------------------------------------------
                string carpetaRemota = Path.Combine(urlCargaSapPro, rut, "procesados") + "\\";

                if (!Directory.Exists(carpetaRemota))
                {
                    return $"Error: No existe la carpeta remota: {carpetaRemota}";
                }

                // -------------------------------------------
                // 2) ENCONTRAR PDF CORRECTO (doc_venta + mayor sufijo)
                // -------------------------------------------
                string[] archivos = Directory.GetFiles(carpetaRemota, doc_venta + "*.pdf");

                if (archivos.Length == 0)
                {
                    return $"Error: No se encontraron archivos PDF para {doc_venta} en {carpetaRemota}";
                }

                string archivoSeleccionado = archivos
                    .Select(a =>
                    {
                        string nombre = Path.GetFileNameWithoutExtension(a);
                        string resto = nombre.Substring(doc_venta.Length);

                        int n = 0;
                        int.TryParse(resto, out n);
                        return new { archivo = a, num = n };
                    })
                    .OrderBy(x => x.num)
                    .Last()
                    .archivo;

                // Archivo PDF real
                sourceDir = archivoSeleccionado;


                string nombreArchivoSeleccionado = Path.GetFileName(archivoSeleccionado);












                fileName = rut + "_" + doc_venta + ".pdf";

                backupDir = urlBaseDestino + fileName;
                sourceDir = @urlBaseCedibles + rut + "/procesados/" + doc_venta + ".pdf";
                sourceDir = @urlBaseCedibles + rut + "/procesados/" + nombreArchivoSeleccionado ;

                dire[ind] = backupDir;
                ind++;

                byte[] fileData;
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        fileData = client.DownloadData(sourceDir);
                    }
                }
                catch (WebException e)
                {
                    ind = 0;
                    return "... creando expediente de factura " + factura +
                           ", Ocurrió un error al descargar PDF desde: " + sourceDir +
                           ", detalle: " + e.Message;
                }

                try
                {
                    using (FileStream fileStream = new FileStream(backupDir, FileMode.Create))
                    {
                        fileStream.Write(fileData, 0, fileData.Length);
                        fileStream.Flush();
                    }
                }
                catch (Exception e)
                {
                    ind = 0;
                    return "... creando expediente de factura " + factura +
                           ", error al guardar PDF " + backupDir +
                           ", detalle: " + e.Message;
                }

                res = "OK";
            }


            // -------------------------------
            // VALIDACIÓN PREVIA DE ARCHIVOS PDF
            // -------------------------------
            for (int i = 0; i < dire.Length; i++)
            {
                string file = dire[i];

                // Validación 1: existe el archivo
                if (!File.Exists(file))
                {
                    return $"Error: El archivo PDF no existe: {file}";
                }

                // Validación 2: tamaño mínimo razonable
                var info = new FileInfo(file);
                if (info.Length < 1000)  // < 1 KB = muy sospechoso
                {
                    return $"Error: El PDF está vacío o corrupto: {file}";
                }

                // Validación 3: intentar abrir con Aspose
                try
                {
                    using (var pdfDoc = new Aspose.Pdf.Document(file))
                    {
                        // Si abre aquí, el PDF es válido
                    }
                }
                catch (Exception ex)
                {
                    return $"Error: El archivo PDF es inválido o está dañado: {file}. Detalle: {ex.Message}";
                }
            }


            // -------------------------------
            // CONCATENACIÓN DE PDFS
            // -------------------------------
            try
            {
                if (ind > 0)
                {
                    NombreExpediente = rut + "_" + factura + "_FULL.pdf";
                    bckupDirFull = urlBaseDestino + NombreExpediente;

                    PdfFileEditor pdfEditor = new PdfFileEditor();

                    // Aquí ya estamos seguros de que los PDFs son válidos
                    pdfEditor.Concatenate(dire, bckupDirFull);

                    // Borrar PDFs individuales
                    foreach (var despa in listaDespachos)
                    {
                        doc_venta = despa.DocumentoVenta;
                        fileName = rut + "_" + doc_venta + ".pdf";
                        backupDir = urlBaseDestino + fileName;

                        if (File.Exists(backupDir))
                        {
                            File.Delete(backupDir);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return "... creando expediente de factura " + factura +
                       ", ocurrió un error en la concatenación: " + e.Message;
            }

            return "OK";
        }
        public string crearExpedienteSINVALIDARPDF(string rut, string factura)
        {
            var ambiente = ConfigurationManager.AppSettings["Ambiente"].ToString();

            var urlBaseCedibles = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseCedibles"].ToString() : ConfigurationManager.AppSettings["urlBaseCedibles_TEST"].ToString();
            var urlBaseDestino = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseIOExpedientes"].ToString() : ConfigurationManager.AppSettings["urlBaseIOExpedientes_TEST"].ToString();

            List<Movimiento> lista = new List<Movimiento>();

            string backupDir, sourceDir, /*sourceDirURI,*/ bckupDirFull, doc_venta, fileName, NombreExpediente;
            Despachos despacho = new Despachos();

            var listaDespachos = despacho.getDespachosByFactura(rut, factura, true);
            string[] dire = new string[listaDespachos.Count];
            int ind = 0;
            String res = "";
            foreach (var despa in listaDespachos)
            {
                doc_venta = despa.DocumentoVenta;

                fileName = rut + "_" + doc_venta + ".pdf";

                backupDir = urlBaseDestino + fileName;

                sourceDir = @urlBaseCedibles + rut + "/procesados/" + doc_venta + ".pdf";
                //sourceDirURI = urlBaseCedibles + rut + "/procesados/" + doc_venta + ".pdf";

                dire[ind] = backupDir;
                ind = ind + 1;

                byte[] fileData;
                try
                {

                    using (WebClient client = new WebClient())
                    {

                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        fileData = client.DownloadData(sourceDir);

                    }

                }
                catch (WebException e)
                {

                    if (e.Status == WebExceptionStatus.SendFailure)
                    {
                        res = "error WebExceptionStatus.SendFailure -> source dir:" + sourceDir;
                    }
                    else
                    {
                        if (e.Status == WebExceptionStatus.Timeout)
                        {
                            res = "error WebExceptionStatus.Timeout";
                        }
                        else if (e.Status == WebExceptionStatus.ProtocolError)
                        {
                            res = "Error de protocolo: {" + ((HttpWebResponse)e.Response).StatusCode + "} " + ((HttpWebResponse)e.Response).StatusDescription;
                        }
                    }
                    ind = 0;
                    return "... creando expediente de factura " + factura + ", Ocurrio un error: " + res + ", detalle: " + e.Message;
                }


                try
                {
                    if (ind > 0)
                    {
                        // using (WebClient client = new WebClient()) {  fileData = client.DownloadData(sourceDir); }
                        using (FileStream fileStream = new FileStream(backupDir, FileMode.Create))
                        {
                            for (int i = 0; i < fileData.Length; i++)
                            {
                                fileStream.WriteByte(fileData[i]);
                            }
                            fileStream.Flush();
                            fileStream.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    ind = 0;

                    return "... creando expediente de factura " + factura + ", Ocurrio un error: " + "FileStream(" + backupDir + ", FileMode.Create)), detalle: " + e.Message;
                }

                res = "OK";

            }

            try
            {
                if (ind > 0)
                {
                    NombreExpediente = "";
                    if (listaDespachos.Count > 0)
                    {
                        NombreExpediente = rut + "_" + factura + "_FULL.pdf";
                        bckupDirFull = urlBaseDestino + NombreExpediente;
                        PdfFileEditor pdfEditor = new PdfFileEditor();
                        pdfEditor.Concatenate(dire, bckupDirFull);
                    }
                    /* BORRAR DOCUMENTOS DESCARGADOS Y DEJAR SOLO EL EXPEDIENTE */
                    foreach (var despa in listaDespachos)
                    {
                        doc_venta = despa.DocumentoVenta;
                        fileName = rut + "_" + doc_venta + ".pdf";
                        backupDir = urlBaseDestino + fileName;
                        if (System.IO.File.Exists(backupDir))
                        {
                            System.IO.File.Delete(backupDir);
                        }
                    }
                    res = "OK";
                }
            }
            catch (Exception e)
            {
                ind = 0;
                res = e.Message;
                return "... creando expediente de factura " + factura + ", ocurrio un error: " + res;

            }
            return "OK";
        }

        public int porcentajeCargaExpediente(string rut, string factura)
        {

            //            var urlBaseCedibles = ConfigurationManager.AppSettings["urlBaseCedibles"].ToString();
            var urlBaseCedibles = "\\\\10.8.0.37\\CargaSapPro\\"; //ConfigurationManager.AppSettings["urlBaseCedibles"].ToString();


            List<Movimiento> lista = new List<Movimiento>();

            string sourceDir, doc_venta, fileName;
            Despachos despacho = new Despachos();

            var listaDespachos = despacho.getDespachosByFactura(rut, factura, true);
            string[] dire = new string[listaDespachos.Count];

            int totalDocumentos = listaDespachos.Count;
            int nDocumentosExisten = 0;
            int nDocumentosNOExisten = 0;
            int nFallos = 0;

            foreach (var despa in listaDespachos)
            {
                doc_venta = despa.DocumentoVenta;

                fileName = rut + "_" + doc_venta + ".pdf";

                //                sourceDir = @urlBaseCedibles + rut + "/procesados/" + doc_venta + ".pdf";
                sourceDir = @urlBaseCedibles + rut + "\\procesados\\" + doc_venta + ".pdf";

                try
                {

                    using (WebClient client = new WebClient())
                    {

                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        if (System.IO.File.Exists(sourceDir))
                        {
                            nDocumentosExisten++;
                        }
                        else
                        {
                            nDocumentosNOExisten++;
                        }

                    }

                }
                catch (WebException)
                {
                    nFallos++;
                }

            }

            return (totalDocumentos == 0) ? 0 : ((nDocumentosExisten * 100) / totalDocumentos);

        }


        public int existeExpediente(string rut, string factura, Boolean full)
        {
            string ambiente = ConfigurationManager.AppSettings["Ambiente"];
            try
            {
                //return 1; 
                //                var urlBase = ConfigurationManager.AppSettings["urlBaseIOExpedientes"].ToString();
                var urlBase = (ambiente=="Production")? ConfigurationManager.AppSettings["urlBaseIOExpedientes"].ToString(): ConfigurationManager.AppSettings["urlBaseIOExpedientes_TEST"].ToString();
                var nombreExpediente = (full) ? rut + "_" + factura + "_FULL.pdf" : rut + "_" + factura + ".pdf";
                var archivoFull = urlBase + nombreExpediente;
                if (System.IO.File.Exists(archivoFull)) return 1; else return 0;
            }
            catch
            {
                return 0;
            }

        }
        public int existeFacturaComision(string rut, string factura)
        {
            string ambiente = ConfigurationManager.AppSettings["Ambiente"];
            try
            {
                var urlBase = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseIOFacturaComision"].ToString() : ConfigurationManager.AppSettings["urlBaseIOFacturaComision_TEST"].ToString();
                var prefijo = ConfigurationManager.AppSettings["prefijoNombreComision"].ToString();
                var nombreFactura = prefijo + rut + "_" + factura + ".pdf";
                var archivo = urlBase + nombreFactura;
                if (System.IO.File.Exists(archivo)) return 1; else return 0;
            }
            catch
            {
                return 0;
            }
        }
        public int ExisteDocumentoAdjunto(string rut, string factura)
        {
            // Retorna 1 si existe al menos un PDF que matchee con la factura; 0 en caso contrario.
            // Si hay un error controlado (p.ej. carpeta no existe) retorna 0.

            // Sanitiza RUT: deja solo números (si necesitas conservar DV, ajusta aquí).
            string rutSanitizado = new string((rut ?? string.Empty).Where(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(rutSanitizado) || string.IsNullOrWhiteSpace(factura))
                return 0;

            // Lee ambiente y paths
            string ambiente = ConfigurationManager.AppSettings["Ambiente"];
            string basePath =
                string.Equals(ambiente, "Production", StringComparison.OrdinalIgnoreCase)
                ? ConfigurationManager.AppSettings["urlBaseIODocumentos"]       // debería ser una RUTA local/UNC
                : ConfigurationManager.AppSettings["urlBaseIODocumentos_TEST"]; // idem

            if (string.IsNullOrWhiteSpace(basePath))
                return 0;

            try
            {
                // Normaliza la carpeta del proveedor: <base>/<rutSanitizado>/
                string proveedorFolder = Path.Combine(basePath, rutSanitizado);

                // Si la carpeta no existe, no hay archivos
                if (!Directory.Exists(proveedorFolder))
                    return 0;

                // Patrón de búsqueda: PDFs que contengan el número de factura
                // Si tu convención es distinta (ej. "NC_{factura}_*.pdf" o "factura_comision_{rut}_{factura}.pdf"),
                // ajusta el patrón en consecuencia:
                string searchPattern = $"*{factura}*.pdf";

                // Enumeración eficiente (no carga todo a memoria)
                var existe = Directory.EnumerateFiles(proveedorFolder, searchPattern, SearchOption.TopDirectoryOnly)
                                      .Any();

                return existe ? 1 : 0;
            }
            catch (UnauthorizedAccessException)
            {
                // Sin permisos para leer la carpeta → lo tratamos como "no existe" (o loguea si quieres)
                return 0;
            }
        }


        //public int existeDocumentoAdjunto(string rut, string factura)
        //{
        //    string ambiente = ConfigurationManager.AppSettings["Ambiente"];
        //    try
        //    {
        //        var urlBase = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseIODocumentos"].ToString() : ConfigurationManager.AppSettings["urlBaseIODocumentos_TEST"].ToString();
        //        //var prefijo = ConfigurationManager.AppSettings["prefijoNombreComision"].ToString();
        //        string folder = urlBase + int.Parse(rut) + "/";
        //        //var archivo = urlBase + nombreFactura;

        //        string fileFind = "*.pdf";

        //        //linkCedible = folder + file;

        //        var archivosLista = Directory.GetFiles(folder, fileFind)
        //            .Select(Path.GetFileName)
        //            .OrderBy(f => f)
        //            .Select(f => new Archivo { Nombre = f })
        //            .ToList();


        //        if (System.IO.File.Exists(archivo)) return 1; else return 0;
        //    }
        //    catch
        //    {
        //        return 0;
        //    }

        //}

        public List<Movimiento> getExpediente(string rut, string factura)
        {

            //var defaultDir = "~/App_Data/";
            //defaultDir = "~/Content/Expedientes/";

            var defaultDir = "//10.8.0.60/inetpub/Archivos/PNACPACAM/Expedientes/";
            //            defaultDir = "~/Expedientes/";

            List<Movimiento> lista = new List<Movimiento>();

            string backupDir, sourceDir, bckupDirFull, doc_venta, fileName, NombreExpediente;
            Despachos despacho = new Despachos();

            var listaDespachos = despacho.getDespachosByFactura(rut, factura, true);
            string[] dire = new string[listaDespachos.Count];
            int ind = 0;
            try
            {
                foreach (var despa in listaDespachos)
                {
                    doc_venta = despa.DocumentoVenta;

                    fileName = rut + "_" + doc_venta + ".pdf";
                    backupDir = System.Web.HttpContext.Current.Server.MapPath(defaultDir) + fileName;

                    backupDir = defaultDir + fileName;

                    sourceDir = @"https://testaplicacionesweb.cenabast.cl:7001/archivoscedibles/" + rut + "/procesados/" + doc_venta + ".pdf";

                    //backupDir = System.Web.HttpContext.Current.Server.MapPath(defaultDir) + fileName;
                    dire[ind] = backupDir;
                    ind = ind + 1;

                    byte[] fileData;
                    using (WebClient client = new WebClient()) { fileData = client.DownloadData(sourceDir); }
                    using (FileStream fileStream = new FileStream(backupDir, FileMode.Create))
                    {
                        for (int i = 0; i < fileData.Length; i++)
                        {
                            fileStream.WriteByte(fileData[i]);
                        }
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }


            NombreExpediente = "";
            if (listaDespachos.Count > 0)
            {
                NombreExpediente = rut + "_" + factura + "_FULL.pdf";
                //                bckupDirFull = System.Web.HttpContext.Current.Server.MapPath(defaultDir) + NombreExpediente;
                bckupDirFull = defaultDir + NombreExpediente;
                PdfFileEditor pdfEditor = new PdfFileEditor();
                pdfEditor.Concatenate(dire, bckupDirFull);
            }
            /* BORRAR DOCUMENTOS DESCARGADOS Y DEJAR SOLO EL EXPEDIENTE */
            foreach (var despa in listaDespachos)
            {
                doc_venta = despa.DocumentoVenta;
                fileName = rut + "_" + doc_venta + ".pdf";
                //                backupDir = System.Web.HttpContext.Current.Server.MapPath(defaultDir) + fileName;
                backupDir = defaultDir + fileName;
                if (System.IO.File.Exists(backupDir))
                {
                    System.IO.File.Delete(backupDir);
                }
            }
            return lista;
        }


        public int _getCountSAPTable_VBPA()
        {
            int nregistros = 0;
            string sql = "SELECT count(*) cuenta FROM [PZ_SISTEMA_NUEVOMODELO].[dbo].[VBPA] ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<Estado> lista = new List<Estado>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        nregistros = int.Parse(reader["Cuenta"].ToString());
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
            return nregistros;

        }
        public string PutExpediente(string rut, string nfactura, string lista)
        {
            string res = "";
            int i = 0;
            string sql = "INSERT INTO dbo.LogExpedientes ( RutProveedor, NFactura, ListaDocumentosExpediente) VALUES ( @rut, @factura, @lista )";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["secondary"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            
            cmd.Parameters.AddWithValue("@rut", rut);
            cmd.Parameters.AddWithValue("@nombre", nfactura);
            cmd.Parameters.AddWithValue("@apellido", lista);

            con.Open();
            try
            {
                i = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {

                Console.WriteLine("Error: " + ex.ToString());

                if (ex.Number == 2627)
                {
                    res = "Funcionario Ya Existe";
                }
                else
                {
                    res = "Ha ocurrido un error : SqlNumber " + ex.Number;
                }
                i = 0;

            }
            finally
            {
                con.Close();
            }

            return res;

        }
    }
}


