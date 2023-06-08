using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace pnacpacam.Models
{
    public class Venta
    {
        public int identificador { get; set; }
        public int folioVenta{ get; set; }
        public string rutCliente { get; set; }
        public string fechaVenta { get; set; }
        public string estadoAfiliacion { get; set; }
        public string ventaTercero { get; set; }
        public string tipoPago { get; set; }
        public string fecha { get; set; }
        public string rutResponsable { get; set; }
        public string estado { get; set; }
        public double totalVenta { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaActualizacion { get; set; }
        public List<ventaDetalle> detalle { get; set; }
        public int addVenta(Venta venta)
        {
            
            int logId = -1; int res = 0;
            
            string sql = " insert into [inventarioVenta] ( " +
                         "             rutCliente, estadoAfiliacion, ventaTercero, tipoPago, totalVenta, fechaVenta, rutResponsable, estado, fechaCreacion, fechaActualizacion ) " +
                         "   values ( @rutCliente, @estadoAfiliacion, @ventaTercero, @tipoPago, @totalVenta,CONVERT(varchar,GETDATE(),126), @rutResponsable, @estado,CONVERT(varchar,GETDATE(),126),CONVERT(varchar,GETDATE(),126))";
            
            string sqlId = " SELECT @@IDENTITY AS 'Identity' ";

            SqlDataReader reader;
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlCommand cmdId = new SqlCommand(sqlId, con);
            // cmd.Parameters.AddWithValue("@Ip", HttpContext.Current.Request.UserHostAddress);

            cmd.Parameters.AddWithValue("@rutCliente", venta.rutCliente);
            cmd.Parameters.AddWithValue("@fechaVenta", venta.fechaVenta);
            cmd.Parameters.AddWithValue("@estadoAfiliacion", venta.estadoAfiliacion);
            cmd.Parameters.AddWithValue("@tipoPago", venta.tipoPago);
            cmd.Parameters.AddWithValue("@ventaTercero", venta.ventaTercero);
            cmd.Parameters.AddWithValue("@totalVenta", venta.totalVenta);
            cmd.Parameters.AddWithValue("@rutResponsable", venta.rutResponsable);
            cmd.Parameters.AddWithValue("@estado", venta.estado);

            /* if (string.IsNullOrEmpty(distribucion.Fecha_Fac)) cmd.Parameters.AddWithValue("@Fecha_Fac", DBNull.Value); else cmd.Parameters.AddWithValue("@Fecha_Fac", distribucion.Fecha_Fac); */
            con.Open();
            try
            {
                res = cmd.ExecuteNonQuery();
                reader = cmdId.ExecuteReader();
                if (reader.Read())
                {
                    logId = int.Parse(reader["Identity"].ToString());
                    venta.identificador = logId;
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

        public List<Venta> getVentas()
        {

            string sql = "SELECT folioVenta, rutCliente, estadoAfiliacion, ventaTercero, tipoPago, totalVenta, fechaVenta, rutResponsable, estado, fechaCreacion, fechaActualizacion " +
                               " FROM [dbo].[inventarioVenta] order by fechaCreacion desc "; //esta condicion debe ser evaluada, pues lo que indica es la precision de mostrar solo compras en piloto no todo tiene serie

            ///debo cambiar al procedimiento sp_inventario_cierreMensual
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<Venta> lista = new List<Venta>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    Venta venta = new Venta();

                    venta.folioVenta         = int.Parse(reader["folioVenta"].ToString());
                    venta.rutCliente         = reader["rutCliente"].ToString();
                    venta.fechaVenta         = reader["fechaVenta"].ToString();
                    venta.estadoAfiliacion   = reader["estadoAfiliacion"].ToString();
                    venta.tipoPago           = reader["tipoPago"].ToString();
                    venta.ventaTercero       = reader["ventaTercero"].ToString();
                    venta.totalVenta         = Double.Parse(reader["totalVenta"].ToString());
                    venta.rutResponsable     = reader["rutResponsable"].ToString();
                    venta.estado             = reader["estado"].ToString();
                    venta.fechaCreacion      = reader["fechaCreacion"].ToString();
                    venta.fechaActualizacion = reader["fechaActualizacion"].ToString();
                    venta.detalle = ventaDetalle.getVentaDetalle(venta.folioVenta);
                    lista.Add(venta);

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
}








