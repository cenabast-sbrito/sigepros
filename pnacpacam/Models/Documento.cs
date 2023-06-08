using pnacpacam.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static Antlr.Runtime.Tree.TreeWizard;

namespace pnacpacam.Models
{

    public class Documento
    {
        public string rutProveedor { get; set; }
        public string nombreProveedor { get; set; }
        public string numeroDocumento { get; set; }
        public string tipoDocumento { get; set; }
        public string fechaDocumento { get; set; }
        public string horaDocumento { get; set; }
        public string numeroDocumentoRef { get; set; }
        public string tipoDocumentoRef { get; set; }
        public string sel_Producto { get; set; }
        public string denominacion { get; set; }
        public string serie { get; set; }
        public string vencimiento { get; set; }
        public string dsp_codigoUnidad { get; set; }
        public string dsp_unidadVenta { get; set; }
        public string cantidad { get; set; }
        public string costoCenabast { get; set; }
        public string costoBienestar { get; set; }
        public string precioPromedioPonderado { get; set; }
        public List<Documento> getDocumentos()
        {
            string sql = "select  m.rutProveedor,  r.nombre,        " +
                         " 		  m.idTipoDocumento,        " +
                         " 		  m.numeroDocumento,        " +
                         " 		  fechaDocumento,           " +
                         " 		  idTipoDocumentoRef,       " +
                         " 		  numeroDocumentoRef,       " +
                         " 		  totalDocumento,           " +
                         " 		  codigoProducto, p.denominacion," +
                         " 		  serie,                    " +
                         " 		  fechaVencimiento,         " +
                         " 		  cantidad,                 " +
                         " 		  d.id_unidadVenta,           " +
                         " 		  costoCenabast,            " +
                         " 		  costoBienestar,           " +
                         " 		  precioPromedioPonderado,  " +
                         " 		  m.rutResponsable,           " +
                         " 		  fechaIngreso              " +
                         " FROM   [dbo].[inventarioDocumento] m " +
                         "                   inner join[dbo].[inventarioDocumentoDetalle] d " +
                         " on(m.rutProveedor = d.rutProveedor " +
                         " and m.idTIpoDocumento = d.idTipoDocumento " +
                         " and m.numeroDocumento = d.numeroDocumento)  " +
                         " inner join [dbo].[producto] p on (d.codigoProducto=p.codigo) " +
                         " inner join [dbo].[proveedor] r on (m.rutProveedor = r.rutProveedor)";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<Documento> listaDocumentos = new List<Documento>();
            Documento doc = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    doc = new Documento();

                    doc.rutProveedor = reader["rutProveedor"].ToString();
                    doc.nombreProveedor = reader["nombre"].ToString();
                    doc.numeroDocumento = reader["numeroDocumento"].ToString();
                    doc.tipoDocumento = reader["idTipoDocumento"].ToString();

                    doc.fechaDocumento = reader["fechaDocumento"].ToString();
                    doc.horaDocumento = reader["fechaDocumento"].ToString();

                    doc.numeroDocumentoRef = reader["numeroDocumentoRef"].ToString();
                    doc.tipoDocumentoRef = reader["idTipoDocumentoRef"].ToString();

                    doc.sel_Producto = reader["codigoProducto"].ToString();
                    doc.denominacion = reader["denominacion"].ToString();

                    doc.serie = reader["serie"].ToString();
                    doc.vencimiento = reader["fechaVencimiento"].ToString();
                    doc.dsp_codigoUnidad = reader["id_unidadVenta"].ToString();
                    doc.dsp_unidadVenta = reader["id_unidadVenta"].ToString();
                    doc.cantidad = reader["cantidad"].ToString();
                    doc.costoCenabast = reader["costoCenabast"].ToString();
                    doc.costoBienestar = reader["costoBienestar"].ToString();
                    doc.precioPromedioPonderado = reader["precioPromedioPonderado"].ToString();

                    listaDocumentos.Add(doc);

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

            return listaDocumentos;
        }
    }
}
