using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace pnacpacam.Models
{
    public class FacturaLista
    {
        public string factura {  get; set; }
        public List<FacturaLista> getListFactura(string rutproveedor)
        {

            string sql = @"
             SELECT DISTINCT e.LIFNR, e.EBELN, d.NFactura NFactura
                    FROM Despachos AS d
                    INNER JOIN PZ_SISTEMA_NUEVOMODELO.dbo.EKKO_OC2 AS e
                        ON d.[Pedido de Compra] = e.EBELN
                    WHERE e.LIFNR = @rutProveedor
                    ;
            ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);

            SqlCommand cmd = new SqlCommand("proc_FacturasProveedor_Obtener @rutProveedor", con);
            
            //SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@rutProveedor", SqlDbType.Char, 10)
              .Value = rutproveedor.Trim().PadLeft(10, '0');

            SqlDataReader reader;

            List<FacturaLista> facturas = new List<FacturaLista>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                // if (reader.HasRows)  
                // {
                while (reader.Read())
                {
                    FacturaLista factura = new FacturaLista();
                    factura.factura = (reader["NFactura"].ToString());

                    facturas.Add(factura);
                }
            }
            catch (SqlException sex)
            {
                Console.WriteLine("Error: " + sex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return facturas;
        }
    }
}