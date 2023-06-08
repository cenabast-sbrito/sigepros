using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Web.Mvc;







//using Grpc.Core;
//using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
//using Grpc.Core;

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
        public string GuiaDespacho { get; set; }
        public string CodigoMaterial { get; set; }
        public string Lote { get; set; }
        public string CantidadDocumento { get; set; }
        public string CantidadInformada { get; set; }
        public string ValorNeto { get; set; }
        public string Observaciones { get; set; }
        public string DocumentoEntrega { get; set; }
        public string FechaVencimiento { get; set; }
        public string MotivoRechazo { get; set; }


        public Despachos getDespacho(int pedidoCompra)
        {
            string sql = "select  " +
                "[NFactura],[Documento de venta],[Pedido de Compra],[Grupo de articulos],[Canal],[Guía de despacho],[Código Material],[Lote],[Cantidad documento d],[Cantidad informada],[Valor neto],[Observaciones],[Documento de entrega],[Fecha vencimiento],[Motivo Rechazo]" +
                "FROM [PNACPACAM].[dbo].[Despachos] d inner join [PZ_SISTEMA_NUEVOMODELO].[dbo].[EKKO_OC2]  pc on (EBELN=[Pedido de Compra]) " +
                "where [NFactura] is not null and [Pedido de Compra] = @pedidoCompra ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@pedidocompra", pedidoCompra);
            SqlDataReader reader;
            Despachos despacho = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    despacho = new Despachos();

                    despacho.NFactura = reader["[NFactura]"].ToString();
                    despacho.DocumentoVenta = reader["[Documento de venta]"].ToString();
                    despacho.PedidoCompra = reader["[Pedido de Compra]"].ToString();
                    despacho.GrupoArticulos = reader["[Grupo de articulos]"].ToString();
                    despacho.Canal = reader["[Canal]"].ToString();
                    despacho.GuiaDespacho = reader["[Guía de despacho]"].ToString();
                    despacho.CodigoMaterial = reader["[Código Material]"].ToString();
                    despacho.Lote = reader["[Lote]"].ToString();
                    despacho.CantidadDocumento = reader["[Cantidad documento d]"].ToString();
                    despacho.CantidadInformada = reader["[Cantidad informada]"].ToString();
                    despacho.ValorNeto = reader["[Valor neto]"].ToString();
                    despacho.Observaciones = reader["[Observaciones]"].ToString();
                    despacho.DocumentoEntrega = reader["[Documento de entrega]"].ToString();
                    despacho.FechaVencimiento = reader["[Fecha vencimiento]"].ToString();
                    despacho.MotivoRechazo = reader["[Motivo Rechazo]"].ToString();

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

            return despacho;
        }

        public List<Despachos> getDespachos(string rutProveedor)
        {
            string sql = "select LIFNR, " +
                "[NFactura] NFactura, " +
                "[Documento de venta] DocumentoVenta, " +
                "[Pedido de Compra] PedidoCompra, " +
                "[Grupo de articulos]  GrupoArticulos, " +
                "[Canal] Canal, " +
                "[Guía de despacho] GuiaDespacho , " +
                "[Código Material] CodigoMaterial, " +
                "[Lote] Lote, " +
                "[Cantidad documento d] CantidadDocumento, " +
                "[Cantidad informada] CantidadInformada, " +
                "[Valor neto] ValorNeto, " +
                "[Observaciones] Observaciones, " +
                "[Documento de entrega] DocumentoEntrega, " +
                "[Fecha vencimiento] FechaVencimiento, " +
                "[Motivo Rechazo] MotivoRechazo " +
                "FROM [PNACPACAM].[dbo].[Despachos] d inner join [PZ_SISTEMA_NUEVOMODELO].[dbo].[EKKO_OC2] pc on (EBELN=[Pedido de Compra]) " +
                " where [NFactura] is not null and [LIFNR]=@rutProveedor ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@rutProveedor", rutProveedor);

            cmd.CommandType = CommandType.Text;
            SqlDataReader reader;
            List<Despachos> lista = new List<Despachos>();
            Despachos despacho = null;

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
                    despacho.GuiaDespacho = reader["GuiaDespacho"].ToString();
                    despacho.CodigoMaterial = reader["CodigoMaterial"].ToString();
                    despacho.Lote = reader["Lote"].ToString();
                    despacho.CantidadDocumento = reader["CantidadDocumento"].ToString();
                    despacho.CantidadInformada = reader["CantidadInformada"].ToString();
                    despacho.ValorNeto = reader["ValorNeto"].ToString();
                    despacho.Observaciones = reader["Observaciones"].ToString();
                    despacho.DocumentoEntrega = reader["DocumentoEntrega"].ToString();
                    despacho.FechaVencimiento = reader["FechaVencimiento"].ToString();
                    despacho.MotivoRechazo = reader["MotivoRechazo"].ToString();
                    lista.Add(despacho);
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

            return lista;
        }

        public int unirPDF(){

            var html_a = @"<p> [PDF_A] </p>
                   <p> [PDF_A] 1st Page </p>
                   <div style = 'page-break-after: always;' ></div>
                   <p> [PDF_A] 2nd Page</p>";

            var html_b = @"<p> [PDF_B] </p>
                   <p> [PDF_B] 1st Page </p>
                   <div style = 'page-break-after: always;' ></div>
                   <p> [PDF_B] 2nd Page</p>";

            var Renderer = new IronPdf.ChromePdfRenderer();
            var pdfdoc_a = Renderer.RenderHtmlAsPdf(html_a);
            var pdfdoc_b = Renderer.RenderHtmlAsPdf(html_b);
            var merged = IronPdf.PdfDocument.Merge(pdfdoc_a, pdfdoc_b);
            var ruta = "~/App_Data/Ejemplo.pdf";
 //           merged.SaveAs(ruta);
            

            return 1;

        }


    }
}