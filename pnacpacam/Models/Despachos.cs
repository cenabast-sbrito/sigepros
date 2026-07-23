using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
namespace PNacPacam
{
    public class Despachos
    {
        public string LIFNR { get; set; }
        public string NFactura { get; set; }
        public string DocumentoVenta { get; set; }
        public string PedidoCompra { get; set; }
        public string GrupoArticulos { get; set; }
        public string Canal { get; set; }
        public string DescripcionCanal { get; set; }
        public string GuiaDespacho { get; set; }
        public string CodigoMaterial { get; set; }
        public string ValorNeto { get; set; }
        public string Denominacion { get; set; }
        public string Lote { get; set; }
        public string CantidadDocumento { get; set; }
        public string CantidadInformada { get; set; }  
        public string ZZIDPRO_E { get; set; }
        public string ZZIDMER_E { get; set; }
        public string RutEstablecimiento { get; set; }
        public string Destinatario { get; set; }
        public string ServicioSalud { get; set; }
        public string linkCedible { get; set; }
        public List<Despachos> getDespachosByFactura(string rut, string nFactura, Boolean incluye)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);

            string procGetGuias = "[dbo].[proc_Guias_Obtener]";

            SqlCommand cmd = new SqlCommand(procGetGuias, con);
            cmd.Parameters.AddWithValue("@rutProveedor", rut);
            cmd.Parameters.AddWithValue("@nFactura", nFactura);
            cmd.Parameters.AddWithValue("@excluyeSS", incluye ? 1 : 0); // excluir el servicio de salud mejora el tiempo de ejecución de la query Guías
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = 1; prueba de agotamiento en el tiempo de espera valor en 1  segundo

            SqlDataReader reader;
            List<Despachos> lista = new List<Despachos>();
            Despachos despacho = null;
            var urlBaseCedibles = ConfigurationManager.AppSettings["urlBaseCedibles"].ToString();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    despacho = new Despachos();
                    despacho.LIFNR = reader["LIFNR"].ToString();
                    despacho.NFactura = reader["NFactura"].ToString();
                    despacho.DocumentoVenta = reader["DocumentoVenta"].ToString();
                    despacho.PedidoCompra = reader["PedidoCompra"].ToString();
                    despacho.GrupoArticulos = reader["GrupoArticulos"].ToString();
                    despacho.Canal = reader["Canal"].ToString();
                    despacho.DescripcionCanal = reader["DescripcionCanal"].ToString();
                    despacho.GuiaDespacho = reader["GuiaDespacho"].ToString();
                    despacho.CodigoMaterial = reader["CodigoMaterial"].ToString();
                    despacho.Denominacion = reader["Denominacion"].ToString();
                    despacho.Lote = reader["Lote"].ToString();
                    despacho.CantidadDocumento = reader["CantidadDocumento"].ToString();
                    despacho.CantidadInformada = reader["CantidadInformada"].ToString();
                    despacho.ValorNeto = reader["ValorNeto"].ToString();
                    despacho.ZZIDPRO_E = reader["ZZIDPRO_E"].ToString();
                    despacho.ZZIDMER_E = reader["ZZIDMER_E"].ToString();
                    despacho.RutEstablecimiento = reader["RutEstablecimiento"].ToString();
                    despacho.Destinatario = reader["Destinatario"].ToString();
                    despacho.ServicioSalud = reader["ServicioSalud"].ToString();

                    
                   // despacho.linkCedible = urlBaseCedibles + int.Parse(despacho.LIFNR) + "/procesados/" + despacho.DocumentoVenta + ".pdf";
                    if (despacho != null &&
                        !string.IsNullOrWhiteSpace(urlBaseCedibles) &&
                        !string.IsNullOrWhiteSpace(despacho.LIFNR) &&
                        int.TryParse(despacho.LIFNR, out var lifnr) &&
                        !string.IsNullOrWhiteSpace(despacho.DocumentoVenta))
                    {
                        despacho.linkCedible = urlBaseCedibles + int.Parse(despacho.LIFNR) + "/procesados/" + despacho.DocumentoVenta + ".pdf";
                        //despacho.linkCedible = $"{urlBaseCedibles}{lifnr}/procesados/{despacho.DocumentoVenta}.pdf";
                    }
                    else
                    {
                        despacho.linkCedible = "no url"; // o string.Empty
                    }



                    lista.Add(despacho);
                }
            }
            catch (SqlException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                con.Close();
            }
            return lista;
        }
        public class Factura
        {
            public string DocumentoVenta { get; set; }
            public string CodigoMaterial { get; set; }
            public string Denominacion { get; set; }
            public string LIFNR { get; set; }
            public string NFactura { get; set; }
            public string Expediente { get; set; }
            public string TObservacion { get; set; }
            public string CEstado { get; set; }
            public string TEstado { get; set; }
            public string TEstadoTiempoPresente { get; set; }
            public string Estado_OK { get; set; }
            public string CEstadoOK_Observaciones { get; set; }
            public string Estado_NOOK { get; set; }
            public string TEstadoOK { get; set; }
            public string TEstadoNOOK { get; set; }
            public string TEstadoOK_Observaciones { get; set; }
            public string PuedeEjecutarOK { get; set; }
            public string PuedeEjecutarNOOK { get; set; }
            public string PuedeVerOK { get; set; }
            public string PuedeVerNOOK { get; set; }
            public string NAME1 { get; set; }
            public string ano { get; set; }
            public string linkExpedientes { get; set; }
            public bool ExisteExpediente { get; set; }

            public string linkComision { get; set; }
            public bool ExisteComision { get; set; }

            public string linkDocumentoAdjunto { get; set; }
            public bool ExisteDocumentoAdjunto { get; set; }

            //public string botoneraHTML { get; set; }

            private string sqlFiltroFacturas = " SELECT distinct pc.[LIFNR], [NFactura], [NAME1] " +
                                   "   FROM [PNACPACAM].[dbo].[Despachos] d inner join [PZ_SISTEMA_NUEVOMODELO].[dbo].[EKKO_OC2] pc on (EBELN=[Pedido de Compra]) inner join [PZ_SISTEMA_NUEVOMODELO].[dbo].[LFA1] lfa on (lfa.LIFNR=pc.LIFNR)  " +
                                   "  where ([NFactura] is not null) and (pc.[LIFNR]=@rutProveedor or @rutProveedor='' ) and (([NFactura] = @nFactura) or (@nFactura ='') )  ";

            public List<Factura> _getFacturas(string rutProveedor, string nFactura, string anio, string nestado, string rol)
            {
                string ambiente = ConfigurationManager.AppSettings["Ambiente"];

                var urlBaseDestino = (ambiente == "Production")
                    ? ConfigurationManager.AppSettings["urlBaseLinkExpedientes"].ToString()
                    : ConfigurationManager.AppSettings["urlBaseLinkExpedientes_TEST"].ToString();

                var rutaBaseFisicaExpedientes = (ambiente == "Production")
                    ? ConfigurationManager.AppSettings["urlBaseIOExpedientes"].ToString()
                    : ConfigurationManager.AppSettings["urlBaseIOExpedientes_TEST"].ToString();

                var rutaBaseFisicaComision = (ambiente == "Production")
                    ? ConfigurationManager.AppSettings["urlBaseIOFacturaComision"].ToString()
                    : ConfigurationManager.AppSettings["urlBaseIOFacturaComision_TEST"].ToString();

                // ✅ Base SIN rut
                var rutaBaseFisicaDocumentoAdjuntoBase = (ambiente == "Production")
                    ? ConfigurationManager.AppSettings["urlBaseIODocumentos"].ToString()
                    : ConfigurationManager.AppSettings["urlBaseIODocumentos_TEST"].ToString();

                var urlBaseDestinoDocumentoAdjuntoBase = (ambiente == "Production")
                    ? ConfigurationManager.AppSettings["urlBaseIODocumentos"].ToString()
                    : ConfigurationManager.AppSettings["urlBaseIODocumentos_TEST"].ToString();

                var urlBaseDestinoComision = (ambiente == "Production")
                    ? ConfigurationManager.AppSettings["urlBaseLinkComision"].ToString()
                    : ConfigurationManager.AppSettings["urlBaseLinkComision_TEST"].ToString();

                var prefijo = ConfigurationManager.AppSettings["prefijoNombreComision"].ToString();

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
                SqlCommand cmd = new SqlCommand("proc_Facturas_Obtener", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@rol", rol);
                cmd.Parameters.AddWithValue("@rutProveedor", rutProveedor);
                cmd.Parameters.AddWithValue("@NFactura", nFactura);
                cmd.Parameters.AddWithValue("@Ano", anio);
                cmd.Parameters.AddWithValue("@CEstado", nestado);
                cmd.Parameters.AddWithValue("@esHistorico", false);

                SqlDataReader reader;
                List<Factura> lista = new List<Factura>();

                con.Open();
                try
                {
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Factura factura = new Factura();

                        factura.LIFNR = _set(reader, "RutProveedor");
                        factura.CodigoMaterial = _set(reader, "CMaterial");
                        factura.Denominacion = _set(reader, "Denominacion");
                        factura.NFactura = _set(reader, "NFactura");
                        factura.Expediente = _set(reader, "Expediente");
                        factura.CEstado = _set(reader, "CEstado");
                        factura.TEstado = _set(reader, "TEstado");
                        factura.TEstadoTiempoPresente = _set(reader, "TEstadoTiempoPresente");
                        factura.Estado_OK = _set(reader, "Estado_OK");
                        factura.CEstadoOK_Observaciones = _set(reader, "CEstadoOK_Observaciones");
                        factura.Estado_NOOK = _set(reader, "Estado_NOOK");
                        factura.TEstadoOK = _set(reader, "TEstadoOK");
                        factura.TEstadoOK_Observaciones = _set(reader, "TEstadoOK_Observaciones");
                        factura.TEstadoNOOK = _set(reader, "TEstadoNOOK");
                        factura.PuedeEjecutarOK = _set(reader, "PuedeEjecutarOK");
                        factura.PuedeEjecutarNOOK = _set(reader, "PuedeEjecutarNOOK");
                        factura.PuedeVerOK = _set(reader, "PuedeVerOK");
                        factura.PuedeVerNOOK = _set(reader, "PuedeVerNOOK");
                        factura.NAME1 = _set(reader, "NombreProveedor");
                        factura.TObservacion = _set(reader, "TObservacion");
                        factura.ano = _set(reader, "ano");

                        // ===================== EXPEDIENTE =====================
                        string nombreExpediente = $"{factura.LIFNR}_{factura.NFactura}_FULL.pdf";
                        string rutaExpediente = Path.Combine(rutaBaseFisicaExpedientes, nombreExpediente);

                        factura.ExisteExpediente = File.Exists(rutaExpediente);
                        factura.linkExpedientes = factura.ExisteExpediente
                            ? urlBaseDestino + nombreExpediente
                            : null;

                        // ===================== COMISION =====================
                        string nombreComision = $"{prefijo}{factura.LIFNR}_{factura.NFactura}.pdf";
                        string rutaComision = Path.Combine(rutaBaseFisicaComision, nombreComision);

                        factura.ExisteComision = File.Exists(rutaComision);
                        factura.linkComision = factura.ExisteComision
                            ? urlBaseDestinoComision + nombreComision
                            : null;

                        // ===================== DOCUMENTO ADJUNTO (FIX ✅) =====================
                        factura.ExisteDocumentoAdjunto = false;
                        factura.linkDocumentoAdjunto = null;

                        // ✅ usar el rut REAL desde la BD
                        var rutFactura = quitarEspaciosYCerosIzquierda(factura.LIFNR, "");

                        var rutaFisicaAdjunto = Path.Combine(rutaBaseFisicaDocumentoAdjuntoBase, rutFactura);
                        var urlAdjunto = urlBaseDestinoDocumentoAdjuntoBase + rutFactura + "/";

                        if (!string.IsNullOrWhiteSpace(rutFactura) &&
                            Directory.Exists(rutaFisicaAdjunto))
                        {
                            var archivoAdjunto = Directory
                                .EnumerateFiles(
                                    rutaFisicaAdjunto,
                                    $"*_{factura.NFactura}_*.pdf"
                                )
                                .Select(Path.GetFileName)
                                .FirstOrDefault();

                            factura.ExisteDocumentoAdjunto = archivoAdjunto != null;
                            factura.linkDocumentoAdjunto = factura.ExisteDocumentoAdjunto
                                ? urlAdjunto + archivoAdjunto
                                : null;
                        }

                        lista.Add(factura);
                    }
                }
                catch (Exception ex)
                {
                    lista = null;
                    Console.WriteLine("Error: " + ex.ToString());
                }
                finally
                {
                    con.Close();
                }

                return lista;
            }
            public List<Factura> _getFacturas_OLD(string rutProveedor, string nFactura, string anio, string nestado, string rol)
            {
                string ambiente = ConfigurationManager.AppSettings["Ambiente"];

                var urlBaseDestino = (ambiente=="Production")? ConfigurationManager.AppSettings["urlBaseLinkExpedientes"].ToString() :
                    ConfigurationManager.AppSettings["urlBaseLinkExpedientes_TEST"].ToString(); //"https://testaplicacionesweb.cenabast.cl:7001/Archivos/PNACPACAM/Expedientes/" 

                var rutaBaseFisicaExpedientes = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseIOExpedientes"].ToString() :
                                                                             ConfigurationManager.AppSettings["urlBaseIOExpedientes_TEST"].ToString();

                var rutaBaseFisicaComision = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseIOFacturaComision"].ToString() :
                                                                          ConfigurationManager.AppSettings["urlBaseIOFacturaComision_TEST"].ToString();

                var rutaBaseFisicaDocumentoAdjunto = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseIODocumentos"].ToString() + rutProveedor + "/":
                                                                                  ConfigurationManager.AppSettings["urlBaseIODocumentos_TEST"].ToString() + rutProveedor + "/"; ;

                var urlBaseDestinoComision = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseLinkComision"].ToString() :
                    ConfigurationManager.AppSettings["urlBaseLinkComision_TEST"].ToString(); 
                
                var urlBaseDestinoDocumentoAdjunto = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseIODocumentos"].ToString()+rutProveedor+"/":
                    ConfigurationManager.AppSettings["urlBaseIODocumentos_TEST"].ToString() + rutProveedor + "/"; 
               
                var prefijo = ConfigurationManager.AppSettings["prefijoNombreComision"].ToString();

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
                //SqlCommand cmd = new SqlCommand(sqlFacturas, con);
                SqlCommand cmd = new SqlCommand("[dbo].[proc_Facturas_Obtener]  @rol , @rutProveedor, @NFactura, @Ano, @CEstado, @esHistorico ", con);

                rutProveedor = quitarEspaciosYCerosIzquierda(rutProveedor, "");
                nFactura = quitarEspaciosYCerosIzquierda(nFactura, "");
                anio = quitarEspaciosYCerosIzquierda(anio, "");

                cmd.Parameters.AddWithValue("@rol",          rol);
                cmd.Parameters.AddWithValue("@rutProveedor", rutProveedor);
                cmd.Parameters.AddWithValue("@NFactura",     nFactura);
                cmd.Parameters.AddWithValue("@Ano",          anio);
                cmd.Parameters.AddWithValue("@CEstado",      nestado);
                cmd.Parameters.AddWithValue("@esHistorico", false);

                SqlDataReader reader;
                List<Factura> lista = new List<Factura>();
                con.Open();
                try
                {

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        Factura factura = new Factura();

                        factura.LIFNR = _set(reader, "RutProveedor");
                        factura.CodigoMaterial = _set(reader, "CMaterial");
                        factura.Denominacion = _set(reader, "Denominacion");
                        factura.NFactura = _set(reader, "NFactura");
                        factura.Expediente = _set(reader, "Expediente");
                        factura.CEstado = _set(reader, "CEstado");
                        factura.TEstado = _set(reader, "TEstado");
                        factura.TEstadoTiempoPresente = _set(reader, "TEstadoTiempoPresente");
                        factura.Estado_OK = _set(reader, "Estado_OK");
                        factura.CEstadoOK_Observaciones = _set(reader, "CEstadoOK_Observaciones");
                        factura.Estado_NOOK = _set(reader, "Estado_NOOK");
                        factura.TEstadoOK = _set(reader, "TEstadoOK");
                        factura.TEstadoOK_Observaciones = _set(reader, "TEstadoOK_Observaciones");
                        factura.TEstadoNOOK = _set(reader, "TEstadoNOOK");
                        factura.PuedeEjecutarOK = _set(reader, "PuedeEjecutarOK");
                        factura.PuedeEjecutarNOOK = _set(reader, "PuedeEjecutarNOOK");
                        factura.PuedeVerOK = _set(reader, "PuedeVerOK");
                        factura.PuedeVerNOOK = _set(reader, "PuedeVerNOOK");
                        factura.NAME1 = _set(reader, "NombreProveedor");
                        factura.TObservacion = _set(reader, "TObservacion");
                        factura.ano = _set(reader, "ano");

                        factura.linkExpedientes      = @urlBaseDestino + factura.LIFNR + "_" + factura.NFactura + "_FULL.pdf";
                        factura.linkComision         = urlBaseDestinoComision + prefijo + factura.LIFNR + "_" + factura.NFactura + ".pdf";
                        factura.linkDocumentoAdjunto = urlBaseDestinoDocumentoAdjunto + "NC_" + factura.NFactura + "_" + "*.pdf";//TENGO QUE DEFINIR 

                        // AQUI DECIDO SI MOSTRAR O NO EL ICONO DE LOS DOCUMENTOS RELACIONADOS: EXPEDIENTE, COMISION y OTROS

                        // ===================== EXPEDIENTE =====================
                        string nombreExpediente = $"{factura.LIFNR}_{factura.NFactura}_FULL.pdf";
                        string rutaExpediente = Path.Combine(rutaBaseFisicaExpedientes, nombreExpediente);

                        factura.ExisteExpediente = File.Exists(rutaExpediente);
                        factura.linkExpedientes = factura.ExisteExpediente
                            ? urlBaseDestino + nombreExpediente
                            : null;

                        // ===================== COMISION =====================
                        string nombreComision = $"{prefijo}{factura.LIFNR}_{factura.NFactura}.pdf";
                        string rutaComision = Path.Combine(rutaBaseFisicaComision, nombreComision);

                        factura.ExisteComision = File.Exists(rutaComision);
                        factura.linkComision = factura.ExisteComision
                            ? urlBaseDestinoComision + nombreComision
                            : null;

                        // ===================== DOCUMENTO ADJUNTO =====================

                        factura.ExisteDocumentoAdjunto = false;
                        factura.linkDocumentoAdjunto = null;

                        if (!string.IsNullOrWhiteSpace(rutProveedor)
                            && Directory.Exists(rutaBaseFisicaDocumentoAdjunto))
                        {
                            var archivoAdjunto = Directory
                                .EnumerateFiles(
                                    rutaBaseFisicaDocumentoAdjunto,
                                    $"*_{factura.NFactura}_*.pdf"
                                )
                                .Select(Path.GetFileName)
                                .FirstOrDefault();

                            factura.ExisteDocumentoAdjunto = archivoAdjunto != null;
                            factura.linkDocumentoAdjunto = factura.ExisteDocumentoAdjunto
                                ? urlBaseDestinoDocumentoAdjunto + archivoAdjunto
                                : null;
                        }

                        //factura.botoneraHTML = reader["BotoneraHTML"].ToString();
                        lista.Add(factura);

                    }

                }
                catch (Exception ex)
                {
                    lista = null;
                    Console.WriteLine("Error: " + ex.ToString());
                }
                finally
                {
                    con.Close();
                }
                return lista;
            }
            public List<Factura> _getFacturasHistoricas(string rutProveedor, string nFactura, string anio, string estado, string rol)
            {

                string ambiente = ConfigurationManager.AppSettings["Ambiente"];

                var urlBaseDestino         = (ambiente=="Production")? ConfigurationManager.AppSettings["urlBaseLinkExpedientes"].ToString() : ConfigurationManager.AppSettings["urlBaseLinkExpedientes_TEST"].ToString(); //"https://testaplicacionesweb.cenabast.cl:7001/Archivos/PNACPACAM/Expedientes/" 
                var urlBaseDestinoComision = (ambiente=="Production")? ConfigurationManager.AppSettings["urlBaseLinkComision"].ToString()    : ConfigurationManager.AppSettings["urlBaseLinkComision_TEST"].ToString()   ; //"https://testaplicacionesweb.cenabast.cl:7001/Archivos/PNACPACAM/FacturasComision/factura_comision_"
                var prefijo                = (ambiente=="Production")? ConfigurationManager.AppSettings["prefijoNombreComision"].ToString()  : ConfigurationManager.AppSettings["prefijoNombreComision_TEST"].ToString() ;

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
                //SqlCommand cmd = new SqlCommand(sqlFacturas, con);
                SqlCommand cmd = new SqlCommand("[dbo].[proc_FacturasHistoricas_Obtener]  @rol , @rutProveedor, @NFactura, @Ano, @CEstado, @esHistorico ", con); // a diferncia de su homologo getFacturas este proclo unico que hace es enviar el valor 1 para activar la variable esHistorico en el procedimiento empaquetado

                rutProveedor = quitarEspaciosYCerosIzquierda(rutProveedor, "");
                nFactura = quitarEspaciosYCerosIzquierda(nFactura, "");
                anio = quitarEspaciosYCerosIzquierda(anio, "");
                estado = quitarEspaciosYCerosIzquierda(estado, "");
                cmd.Parameters.AddWithValue("@rol", rol);
                cmd.Parameters.AddWithValue("@rutProveedor", rutProveedor);
                cmd.Parameters.AddWithValue("@NFactura", nFactura);
                cmd.Parameters.AddWithValue("@Ano", anio);
                cmd.Parameters.AddWithValue("@CEstado", estado);
                cmd.Parameters.AddWithValue("@esHistorico", true);  

                SqlDataReader reader;
                List<Factura> lista = new List<Factura>();
                con.Open();
                try
                {
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        Factura factura = new Factura();
                        
                        factura.LIFNR                   = _set(reader, "RutProveedor");
                        factura.CodigoMaterial          = _set(reader, "CMaterial");
                        factura.Denominacion            = _set(reader, "Denominacion");
                        factura.NFactura                = _set(reader, "NFactura");
                        factura.Expediente              = _set(reader, "Expediente");
                        factura.CEstado                 = _set(reader, "CEstado");
                        factura.TEstado                 = _set(reader, "TEstado");
                        factura.TEstadoTiempoPresente   = _set(reader, "TEstadoTiempoPresente");
                        factura.Estado_OK               = _set(reader, "Estado_OK");
                        factura.CEstadoOK_Observaciones = _set(reader, "CEstadoOK_Observaciones");
                        factura.Estado_NOOK             = _set(reader, "Estado_NOOK");
                        factura.TEstadoOK               = _set(reader, "TEstadoOK");
                        factura.TEstadoOK_Observaciones = _set(reader, "TEstadoOK_Observaciones");
                        factura.TEstadoNOOK             = _set(reader, "TEstadoNOOK");
                        factura.PuedeEjecutarOK         = _set(reader, "PuedeEjecutarOK");
                        factura.PuedeEjecutarNOOK       = _set(reader, "PuedeEjecutarNOOK");
                        factura.PuedeVerOK              = _set(reader, "PuedeVerOK");
                        factura.PuedeVerNOOK            = _set(reader, "PuedeVerNOOK");
                        factura.NAME1                   = _set(reader, "NombreProveedor");
                        factura.TObservacion            = _set(reader, "TObservacion");
                        factura.linkExpedientes         = @urlBaseDestino + factura.LIFNR + "_" + factura.NFactura + "_FULL.pdf";

                        factura.linkComision = urlBaseDestinoComision + prefijo + factura.LIFNR + "_" + factura.NFactura + ".pdf";

                        factura.ano = _set(reader, "ano");
                        //factura.botoneraHTML = reader["BotoneraHTML"].ToString();
                        lista.Add(factura);

                    }
                }
                catch (Exception ex)
                {
                    lista = null;
                    Console.WriteLine("Error: " + ex.ToString());
                }
                finally
                {
                    con.Close();
                }
                return lista;
            }
            public Factura _getFactura(string rutProveedor, string nFactura, string anio, string estado, string rol)
            {

                string ambiente = ConfigurationManager.AppSettings["Ambiente"];

                var urlBaseDestino         = (ambiente=="Production")? ConfigurationManager.AppSettings["urlBaseLinkExpedientes"].ToString(): ConfigurationManager.AppSettings["urlBaseLinkExpedientes_TEST"].ToString(); //"https://testaplicacionesweb.cenabast.cl:7001/Archivos/PNACPACAM/Expedientes/" 
                var urlBaseDestinoComision = (ambiente=="Production")? ConfigurationManager.AppSettings["urlBaseLinkComision"].ToString()   : ConfigurationManager.AppSettings["urlBaseLinkComision_TEST"].ToString()   ; //"https://testaplicacionesweb.cenabast.cl:7001/Archivos/PNACPACAM/FacturasComision/factura_comision_"
                var prefijo                = (ambiente=="Production")? ConfigurationManager.AppSettings["prefijoNombreComision"].ToString() : ConfigurationManager.AppSettings["prefijoNombreComision_TEST"].ToString() ;

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
                SqlCommand cmd = new SqlCommand("[dbo].[proc_Facturas_Obtener]  @rol , @rutProveedor, @NFactura, @Ano, @CEstado, 0 ", con);

                rutProveedor = quitarEspaciosYCerosIzquierda(rutProveedor, "");
                nFactura = quitarEspaciosYCerosIzquierda(nFactura, "");
                anio = quitarEspaciosYCerosIzquierda(anio, "");
                estado = quitarEspaciosYCerosIzquierda(estado, "");
                cmd.Parameters.AddWithValue("@rol", rol);
                cmd.Parameters.AddWithValue("@rutProveedor", rutProveedor);
                cmd.Parameters.AddWithValue("@NFactura", nFactura);
                cmd.Parameters.AddWithValue("@Ano", anio);
                cmd.Parameters.AddWithValue("@CEstado", estado);

                SqlDataReader reader;
                Factura factura = null;
                con.Open();
                try
                {
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {

                        factura = new Factura();

                        factura.LIFNR = _set(reader, "RutProveedor");
                        factura.CodigoMaterial = _set(reader, "CMaterial");
                        factura.Denominacion = _set(reader, "Denominacion");
                        factura.NFactura = _set(reader, "NFactura");
                        factura.Expediente = _set(reader, "Expediente");
                        factura.CEstado = _set(reader, "CEstado");
                        factura.TEstado = _set(reader, "TEstado");
                        factura.TEstadoTiempoPresente = _set(reader, "TEstadoTiempoPresente");
                        factura.Estado_OK = _set(reader, "Estado_OK");
                        factura.CEstadoOK_Observaciones = _set(reader, "CEstadoOK_Observaciones");
                        factura.Estado_NOOK = _set(reader, "Estado_NOOK");
                        factura.TEstadoOK = _set(reader, "TEstadoOK");
                        factura.TEstadoOK_Observaciones = _set(reader, "TEstadoOK_Observaciones");
                        factura.TEstadoNOOK = _set(reader, "TEstadoNOOK");
                        factura.PuedeEjecutarOK = _set(reader, "PuedeEjecutarOK");
                        factura.PuedeEjecutarNOOK = _set(reader, "PuedeEjecutarNOOK");
                        factura.PuedeVerOK = _set(reader, "PuedeVerOK");
                        factura.PuedeVerNOOK = _set(reader, "PuedeVerNOOK");
                        factura.NAME1 = _set(reader, "NombreProveedor");
                        factura.linkExpedientes = @urlBaseDestino + factura.LIFNR + "_" + factura.NFactura + "_FULL.pdf";

                        factura.linkComision = urlBaseDestinoComision + prefijo + factura.LIFNR + "_" + factura.NFactura + ".pdf";
                        //factura.botoneraHTML = reader["BotoneraHTML"].ToString();

                    }
                }
                catch (Exception ex)
                {
                    factura = null;
                    Console.WriteLine("Error: " + ex.ToString());
                }
                finally
                {
                    con.Close();
                }
                return factura;
            }
            public static int _existeFacturaProveedor(string rutproveedor, string nFactura)
            {

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
                SqlCommand cmd = new SqlCommand("[dbo].[proc_ExisteFacturaProveedor]  @rutProveedor, @NFactura ", con);

                //rutProveedor = quitarEspaciosYCerosIzquierda(rutProveedor, "");
                //nFactura = quitarEspaciosYCerosIzquierda(nFactura, "");
                //cmd.Parameters.AddWithValue("@rutProveedor", rutProveedor);
                cmd.Parameters.Add("@rutProveedor", SqlDbType.Char, 10)
  .Value = rutproveedor.Trim().PadLeft(10, '0');
                cmd.Parameters.AddWithValue("@NFactura", nFactura);

                SqlDataReader reader;
                int existe = 0;
 
                con.Open();
                try
                {
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {

                         existe = int.Parse(reader["Existe"].ToString());

                    }
                }
                catch (Exception ex)
                {
                    existe = 0;
                    Console.WriteLine("Error: " + ex.ToString());
                }
                finally
                {
                    con.Close();
                }
                return existe;
            }
            private string quitarEspaciosYCerosIzquierda(string value, string defaultValue)
            {
                try
                {
                    /* permite quitar espacios y ceros a la izquierda */
                    Int32 ivalue = (string.IsNullOrEmpty(value)) ? 0 : Int32.Parse(value);
                    string fValue = (ivalue == 0) ? defaultValue : ivalue.ToString();
                    return fValue;
                }
                catch (Exception)
                {
                    return "Error: value " + value;
                }
            }
            private string _set(SqlDataReader r, string variable)
            {
                try
                {
                    if (string.IsNullOrEmpty(r[variable].ToString())) { return ""; } else { return r[variable].ToString(); }
                }
                catch (Exception)
                {
                    return "";
                }
            }
            public string getNombre()
            {

                if (NAME1.IsNullOrWhiteSpace()) NAME1 = "N/D";
                return NAME1;
            }
            public string getRut()
            {
                if (LIFNR.IsNullOrWhiteSpace()) LIFNR = "N/D";
                return LIFNR;
            }
            public string getFactura()
            {
                if (NFactura.IsNullOrWhiteSpace()) NFactura = "N/D";
                return NFactura;
            }

        }

    }
}