using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace pnacpacam.Models
{
    public class ventaDetalle
    {
        public string folioVenta { get; set; }
        public string codProducto { get; set; }
        public string serie { get; set; }
        public string precio { get; set; }
        public string cantidad { get; set; }
        public string precioCosto { get; set; }
        public int addVentaDetalle(List<ventaDetalle> ventaDetalle, int folio)
        {
            int i = 0;


            string sql = "insert into [inventarioVentaDetalle] ( folioVenta,codProducto,serie,precio,cantidad,precioCosto ) " +
                                               " values ( @folioVenta, @codProducto, @serie, @precio, @cantidad, @precioCosto )";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@folioVenta", folio);
            cmd.Parameters.AddWithValue("@codProducto", ventaDetalle[0].codProducto);
            cmd.Parameters.AddWithValue("@serie", ventaDetalle[0].serie);
            cmd.Parameters.AddWithValue("@precio", Double.Parse(ventaDetalle[0].precio));
            cmd.Parameters.AddWithValue("@cantidad", int.Parse(ventaDetalle[0].cantidad));
            cmd.Parameters.AddWithValue("@precioCosto", Double.Parse(ventaDetalle[0].precioCosto));

            /*
            if (string.IsNullOrEmpty(distribucion.Fecha_Fac)) cmd.Parameters.AddWithValue("@Fecha_Fac", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Fecha_Fac", distribucion.Fecha_Fac);
            */

            con.Open();
            try
            {
                i = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                i = 0;
            }
            finally
            {
                // Cierro la Conexión.
                con.Close();
            }
            return i;

        }

        public static List<ventaDetalle> getVentaDetalle(int folioVenta)
        {
            string sql = "SELECT folioVenta,codProducto,serie,precio,cantidad,precioCosto " +
                               " FROM [dbo].[inventarioVentaDetalle]  "; //esta condicion debe ser evaluada, pues lo que indica es la precision de mostrar solo compras en piloto no todo tiene serie

            ///debo cambiar al procedimiento sp_inventario_cierreMensual
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<ventaDetalle> lista = new List<ventaDetalle>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    ventaDetalle ventaDetalle = new ventaDetalle();

                    ventaDetalle.folioVenta   = reader["folioVenta"].ToString();
                    ventaDetalle.codProducto  = reader["codProducto"].ToString();
                    ventaDetalle.serie        = reader["serie"].ToString();
                    ventaDetalle.precio       = reader["precio"].ToString();
                    ventaDetalle.cantidad     = reader["cantidad"].ToString();
                    ventaDetalle.precioCosto  = reader["precioCosto"].ToString();

                    lista.Add(ventaDetalle);

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