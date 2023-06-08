using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

using System.Linq;
using System.Web;

namespace pnacpacam.Models
{
    public class Reportes
    {
        public int Codigo { get; set; }
        public string Denominacion { get; set; }
        public string Serie { get; set; }
        public string FechaVencimiento { get; set; }
        public string PrecioUnitario { get; set; }
        public string Saldo { get; set; }
        public string UnidadMedida { get; set; }
        public int StockCritico { get; set; }
        public List<Reportes> getReporteInventario(int queMostrar)
        {
            string sql = " 	 SELECT p.codigo, p.denominacion, s.serie, s.fechaVencimiento, um.nombre, AVG(p.precioUnitario) as precioUnitario, SUM(s.cantidadComprada) as cantidadComprada, SUM(p.stockCritico) as stockCritico " +
                         "        FROM producto p inner join Serie_producto s on (p.codigo= s.codigoProducto) inner join unidadMedida um on ( p.id_unidadVenta = um.id_unidadMedida ) " +
                         "       WHERE p.estado = 1 and um.estado = 1 ";
            if (queMostrar == 0) // "todos")
                sql = sql + "";
            if (queMostrar == 1) // "vencidos")
                sql = sql + " and s.fechaVencimiento<=getdate()";
            if (queMostrar == 2) // "porvencer")
                sql = sql + " and ( s.fechaVencimiento>=getdate() and s.fechaVencimiento<(getdate()+90)) ";
            if (queMostrar == 3) // "stockCritico")
                sql = sql + " and ( s.cantidadComprada <= p.stockCritico ) ";

            sql = sql + "    GROUP By p.codigo, p.denominacion, s.serie, s.fechaVencimiento, um.nombre ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            Reportes vInventario = null;
            List<Reportes> listaInventario = new List<Reportes>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        vInventario = new Reportes();
                        vInventario.Codigo = int.Parse(reader["codigo"].ToString());
                        vInventario.Denominacion = reader["denominacion"].ToString(); //int.Parse(reader["fechaInventario"].ToString());
                        vInventario.Serie = reader["serie"].ToString();
                        vInventario.FechaVencimiento = reader["fechaVencimiento"].ToString().Substring(0,10);
                        vInventario.PrecioUnitario = reader["precioUnitario"].ToString();
                        vInventario.Saldo = reader["cantidadComprada"].ToString(); 
                        vInventario.UnidadMedida = reader["nombre"].ToString();
                        vInventario.StockCritico = int.Parse(reader["stockCritico"].ToString());
                        listaInventario.Add(vInventario);
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
            return listaInventario;
        }
    }
    public class sp_cierreMensual_From_Reporte
    {
        /* 
        [dbo].[sp_cierreMensual_From_Reporte]:
                CODIGO 
                DENOMINACION
                SERIE
                [SALDO INICIAL]
                [SALDO ACTUAL] 
                [PRECIO UNITARIO] 
                [SALDO EN $]
                [CANT. COMPRA]
                [PRECIO COMPRA]
                [CANT. ENTREGA]
                [COSTO ACTUAL] 
        */
        public string CODIGO  { get; set; }
        public string DENOMINACION { get; set; }
        public string IDSERIE { get; set; }
        public string SERIE { get; set; }
        public double SALDO_INICIAL { get; set; }
        public double SALDO_ACTUAL  { get; set; }
        public double PRECIO_UNITARIO { get; set; }
        public double SALDO_EN_PESO { get; set; }
        public double CANT_COMPRA { get; set; }
        public double PRECIO_COMPRA { get; set; }
        public double CANT_ENTREGA { get; set; }
        public double COSTO_ACTUAL { get; set; }
        public double SALDO_FISICO { get; set; }
        public List<sp_cierreMensual_From_Reporte> getPreviewCierre(string mes, string ano)
        {

            sp_cierreMensual_From_Reporte cierreMensual = new sp_cierreMensual_From_Reporte();
            List<sp_cierreMensual_From_Reporte> cierre = new List<sp_cierreMensual_From_Reporte>();

            string vmes = mes;
            string vmesant = "";
            string vano = ano;
            string vanoant = "";
            int aux = 0;
            string fmt = "00";

            // recorreremos cada uno de los productos activos
            // string sql = "SELECT p.codigo as CODIGO FROM producto p where estado = 1 order by denominacion asc";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            List<sp_cierreMensual_From_Reporte> lista = new List<sp_cierreMensual_From_Reporte>();

            // calculamos el mes y año previo al cierre
            if (vmes == "01") { vmesant = "12"; aux = int.Parse(vano) - 1; vanoant = aux.ToString(); }
            else
            {
                vanoant = vano;
                aux = int.Parse(vmes) - 1;
                vmesant = aux.ToString(fmt);
            }

//            string sqlProductos = "[dbo].[sp_cierreMensual_From_Reporte] " + vmes + "," + vano + "," + vmesant + "," + vanoant + "";
            string sqlProductos = "[dbo].[sp_inventario_cierreMensual] " + vmes + "," + vano + "," + vmesant + "," + vanoant + "";
            
            con.Open();
            cmd = new SqlCommand(sqlProductos, con);
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    sp_cierreMensual_From_Reporte producto = new sp_cierreMensual_From_Reporte();

                    producto.CODIGO = reader["CODIGO"].ToString();

                    producto.DENOMINACION = reader["DENOMINACION"].ToString();

                    producto.SERIE = reader["SERIE"].ToString();

                    if (reader["SALDO_INICIAL"].ToString().IsNullOrWhiteSpace()) producto.SALDO_INICIAL = 0;
                    else producto.SALDO_INICIAL = double.Parse(reader["SALDO_INICIAL"].ToString());

                    if (reader["SALDO_ACTUAL"].ToString().IsNullOrWhiteSpace()) producto.SALDO_ACTUAL = 0;
                    else producto.SALDO_ACTUAL = double.Parse(reader["SALDO_ACTUAL"].ToString());

                    if (reader["PRECIO_UNITARIO"].ToString().IsNullOrWhiteSpace()) producto.PRECIO_UNITARIO = 0;
                    else producto.PRECIO_UNITARIO = double.Parse(reader["PRECIO_UNITARIO"].ToString());

                    if (reader["SALDO_EN_PESO"].ToString().IsNullOrWhiteSpace()) producto.SALDO_EN_PESO = 0;
                    else producto.SALDO_EN_PESO = double.Parse(reader["SALDO_EN_PESO"].ToString());

                    if (reader["CANT_COMPRA"].ToString().IsNullOrWhiteSpace()) producto.CANT_COMPRA = 0;
                    else producto.CANT_COMPRA = double.Parse(reader["CANT_COMPRA"].ToString());

                    if (reader["PRECIO_COMPRA"].ToString().IsNullOrWhiteSpace()) producto.PRECIO_COMPRA = 0;
                    else producto.PRECIO_COMPRA = double.Parse(reader["PRECIO_COMPRA"].ToString());

                    if (reader["CANT_ENTREGA"].ToString().IsNullOrWhiteSpace()) producto.CANT_ENTREGA = 0;
                    else producto.CANT_ENTREGA = double.Parse(reader["CANT_ENTREGA"].ToString());

                    if (reader["COSTO_ACTUAL"].ToString().IsNullOrWhiteSpace()) producto.COSTO_ACTUAL = 0;
                    else producto.COSTO_ACTUAL = double.Parse(reader["COSTO_ACTUAL"].ToString());

                    if (reader["SALDO_FISICO"].ToString().IsNullOrWhiteSpace()) producto.SALDO_FISICO = 0;
                    else producto.SALDO_FISICO = double.Parse(reader["SALDO_FISICO"].ToString());

                    // variables derivadas
                    /*
                    producto.SaldoActual = producto.SaldoInicial + producto.CantCompra - producto.ventaProducto;
                    producto.CantEntrega = producto.ventaProducto;
                    producto.SaldoPeso = producto.PrecioUnitario * producto.SaldoActual;
                    */

                    lista.Add(producto);

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




