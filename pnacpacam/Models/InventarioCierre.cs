using iTextSharp.text.pdf.codec.wmf;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Aspose.Cells;
using iTextSharp.text.pdf.parser;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Asn1.X509;
using iTextSharp.text;
using System.Drawing;
using System.Collections;

namespace pnacpacam.Models
{
   
    public class modeloCierre
    {
        public string miproducto { get; set; } //  INT;
        public string codigo { get; set; } //  INT;
        public string denominacion { get; set; } //  INT;
        public double SaldoSerieProducto { get; set; } //  DECIMAL(20,6);
        public double PrecioUnitario { get; set; } //  DECIMAL(20,6);
        public double PrecioPromedioPonderado { get; set; } //  DECIMAL(20,6);
        public double SaldoPeso { get; set; }
        public double PrecioCompra { get; set; } //  DECIMAL(20,6);
        public double CantCompra { get; set; } //  DECIMAL(20,6);
        public double CantEntrega { get; set; } //  DECIMAL(20,6);
        public double CostoActual { get; set; } //  DECIMAL(20,6);

        public double SaldoInicial { get; set; } //  DECIMAL(20,6);
        public double SaldoActual { get; set; } //  DECIMAL(20,6);
        public double cierreMensual { get; set; } //  DECIMAL(20,6);
        public double ventaProducto { get; set; } //  DECIMAL(20,6);

        public string mes { get; set; } // VARCHAR(2);
        public string anio { get; set; } //  VARCHAR(4);
        public string mesant { get; set; } //  VARCHAR(2);
        public string anioant { get; set; } //  VARCHAR(4);
        public DateTime fechaCierre { get; set; } //  DATE;

        public List<modeloCierre> getCierreDetalleSinProcedure()
        {

            string sqlSaldoInicial;
            string sqlSaldo;
            string sqlCompra;
            string sqlVentaProducto;
            string sqlPrecioUnitario;
            string sqlCantCompra;
            string sqlPrecioCompra;
            string sqlCantEntrega;
            string sqlCostoActual;

            modeloCierre cierreMensual = new modeloCierre();
            List<modeloCierre> cierre = new List<modeloCierre>();

            string mes = "10";
            string mesant = "09";
            string anio = "2022";
            string anioant = "2022";

            // recorreremos cada uno de los productos activos
            // string sql = "SELECT p.codigo as CODIGO FROM producto p where estado = 1 order by denominacion asc";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            SqlDataReader reader1;
            SqlDataReader reader2;
            SqlDataReader reader3;
            SqlDataReader reader4;
            SqlDataReader reader5;
            SqlDataReader reader6;
            SqlDataReader reader7;
            SqlDataReader reader8;

            List<modeloProducto> lista = new List<modeloProducto>();

            string sqlProductos = "SELECT [id_producto],[codigo],[id_unidadVenta],[saldo],[precioUnitario],[denominacion],[estado],[stockCritico] FROM [dbo].[producto]";
            con.Open();
            cmd = new SqlCommand(sqlProductos, con);
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    modeloProducto producto = new modeloProducto();
                    producto.id_producto = int.Parse(reader["id_producto"].ToString());
                    producto.denominacion = reader["denominacion"].ToString();
                    producto.codigo = reader["codigo"].ToString();
                    producto.id_unidadVenta = reader["id_unidadVenta"].ToString();
                    producto.saldo = reader["saldo"].ToString();
                    producto.precioUnitario = reader["precioUnitario"].ToString();
                    producto.estado = reader["estado"].ToString();
                    producto.stockCritico = reader["stockCritico"].ToString();
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

            try
            {
//                cmd.Parameters.AddWithValue("@idInventario", idInventario);
                foreach (modeloProducto p in lista)// while (ForEach(productos))
                {
                    cierreMensual = new modeloCierre();
                    cierreMensual.fechaCierre = DateTime.Parse("01-" + mes + "-" + anio);
                    cierreMensual.fechaCierre = cierreMensual.fechaCierre.AddMonths(1).AddDays(-1);
                    cierreMensual.codigo = p.codigo;
                    cierreMensual.denominacion = p.denominacion;

                    sqlSaldoInicial = "" +
                           " select saldo " +
                           "   from cierreMensual_producto cmp inner join cierreMensual cm on (cm.id_cierreMensual = cmp.id_cierreMensual)" +
                           "  where DATEPART(month, fecha) = " + mesant +
                           "    and DATEPART(year, fecha) = " + anioant  +
                           "    and cmp.codigo = " + p.codigo+" " +
                           "    and cmp.estado = 1 ";

                    sqlSaldo = "" +
                           " select saldo as cierremensual " +
                           "   from cierreMensual_producto cmp inner join cierreMensual cm on cm.id_cierreMensual = cmp.id_cierreMensual " +
                           "  where DATEPART(month, fecha) = " + mesant +
                           "    and DATEPART(year, fecha) = " + anioant +
                           "    and cmp.codigo = " + p.codigo + " " +
                           "    and cmp.estado = 1";

                    sqlVentaProducto = "" +
                           " select SUM(vp.cantidad) as ventaProducto " +
                           "   from venta v inner join venta_producto vp on v.folio = vp.folio " +
                           "  where DATEPART(month, v.fecha) = " + mes +
                           "    and DATEPART(year, v.fecha) = " + anio +
                           "    and v.estado = 1 " +
                           "    and vp.codProducto = " + p.codigo;

                    con.Open();
                    cmd = new SqlCommand(sqlSaldoInicial, con);
                    try
                    {
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            cierreMensual.cierreMensual = double.Parse(reader["saldo"].ToString());
                        }
                    }
                    catch (Exception) { cierreMensual.cierreMensual = 0; }
                    finally { con.Close(); }

                    sqlCompra = "" +
                                " select SUM(cantidadVenta) as CantCompra " +
                                "   from compra where DATEPART(month, fechaIngreso) = " + mes +
                                "    and DATEPART(year, fechaIngreso) = " + anio +
                                "    and codigoProducto = " + p.codigo;

                    con.Open();
                    cmd = new SqlCommand(sqlCompra, con);
                    try
                    {
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            cierreMensual.CantCompra = double.Parse(reader["CantCompra"].ToString());
                        }

                    }
                    catch (Exception) { cierreMensual.CantCompra = 0; }
                    finally { con.Close(); }


                    con.Open();
                    cmd = new SqlCommand(sqlVentaProducto, con);
                    try
                    {
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            cierreMensual.ventaProducto = double.Parse(reader["ventaProducto"].ToString());
                        }
                    }
                    catch (Exception) { cierreMensual.ventaProducto = 0; }
                    finally { con.Close(); }

                    cierreMensual.SaldoActual = cierreMensual.cierreMensual + cierreMensual.CantCompra - cierreMensual.ventaProducto;

                    sqlPrecioUnitario = "" +
                           " select AVG(precioUnitario) as PrecioUnitario " +
                           "   from producto " +
                           "  where codigo = " + p.codigo;


                    con.Open();
                    cmd = new SqlCommand(sqlPrecioUnitario, con);
                    try
                    {
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            cierreMensual.PrecioUnitario = double.Parse(reader["PrecioUnitario"].ToString());
                        }
                    }
                    catch (Exception) { cierreMensual.PrecioUnitario = 0; }
                    finally { con.Close(); }

                    // cierreMensual_producto.saldo + compra.cantidadVenta - venta_producto.cantidad + Mermas.cantidad

                    sqlPrecioCompra = "" +
                           " select costoBienestar as  PrecioCompra " +
                           "   from compra " +
                           "  where DATEPART(month, fechaIngreso) = " + mes +
                           "    and DATEPART(year, fechaIngreso) = " + anio +
                           "    and id_compra = (select MAX(id_compra) " +
                           "   from compra where codigoProducto = " + p.codigo + ") ";
                    con.Open();
                    cmd = new SqlCommand(sqlPrecioCompra, con);
                    try
                    {
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            cierreMensual.PrecioCompra = double.Parse(reader["PrecioCompra"].ToString());
                        }
                    }
                    catch (Exception) { cierreMensual.PrecioCompra = 0; }
                    finally { con.Close(); }


                    sqlCostoActual = "" +
                           " select SUM(vp.cantidad * vp.precioCosto)  as CostoActual " +
                           "   from venta v inner join venta_producto vp on v.folio = vp.folio " +
                           "  where DATEPART(month, v.fecha) = " + mes +
                           "    and DATEPART(year, v.fecha) = " + anio +
                           "    and v.estado = 1 " +
                           "    and codProducto = " + p.codigo;

                    con.Open();
                    cmd = new SqlCommand(sqlCostoActual, con);
                    try
                    {
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            cierreMensual.CostoActual = double.Parse(reader["CostoActual"].ToString());
                        }
                    }
                    catch (Exception) { cierreMensual.CostoActual = 0; }
                    finally { con.Close(); }


                    sqlCantEntrega = "" +
                           " select SUM(vp.cantidad) as CantEntrega " +
                           "   from venta v inner  join venta_producto vp on v.folio = vp.folio " +
                           "  where DATEPART(month, v.fecha) = " + mes +
                           "    and DATEPART(year, v.fecha) = " + anio +
                           "    and v.estado = 1 " +
                           "    and codProducto = " + p.codigo;
                    con.Open();
                    cmd = new SqlCommand(sqlCantEntrega, con);
                    try
                    {
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            cierreMensual.CantEntrega = double.Parse(reader["CantEntrega"].ToString());
                        }
                    }
                    catch (Exception) { cierreMensual.CantEntrega = 0; }
                    finally { con.Close(); }

                    cierreMensual.SaldoPeso = cierreMensual.PrecioUnitario * cierreMensual.SaldoActual;


                    cierre.Add(cierreMensual);

                   // PRINT concat('SALDO EN $ -------> ' , CAST((@PrecioUnitario * @SaldoActual) as decimal(20, 2)))


                    /*

                    select @SaldoSerieProducto = sum(cantidadComprada) from Serie_producto where codigoProducto = @miproducto

                    select @SaldoSerieProducto = sum(cantidadComprada) from Serie_producto where codigoProducto = @miproducto

                    select top 1 @PrecioUnitario = precioPromedioPonderado FROM compra where codigoProducto = @miproducto and fecha<@fechaCierre order by fecha desc
                                                               







                                                                           PRINT('VALORIZADO A PRECIO PROMEDIO')

            PRINT('')

            PRINT concat('PRODUCTO: ','[', @miproducto,']')  
			PRINT concat('Saldo Inicial: ' , CAST(@SaldoInicial as decimal(20, 6)))
			PRINT concat('SALDO ACTUAL: ' , CAST(@SaldoActual as decimal(20, 6)))
			PRINT concat('PRECIO UNITARIO --> ' , CAST(@PrecioUnitario as decimal(20, 6)))
			
			PRINT concat('CANT. COMPRA -----> ' , CAST(@CantCompra as decimal(20, 6)))
			PRINT concat('PRECIO COMPRA ----> ' , CAST(@PrecioCompra as decimal(20, 6)))
			PRINT concat('CANT. ENTREGA ----> ' , CAST(@CantEntrega as decimal(20, 6)))
			PRINT concat('COSTO ACTUAL -----> ' , CAST(@CostoActual as decimal(20, 6)))
			PRINT('')

            PRINT('DIFERENCIAS ')

            PRINT('')

            PRINT concat('SALDO ACTUAL VALORIZADO ----> ' , CAST(@SaldoActual as decimal(20, 6)))
			PRINT concat('SALDO SERIE PRODUCTO -------> ' , CAST(@SaldoSerieProducto as decimal(20, 6)))
			PRINT concat('DIFERENCIA -----------------> ' , CAST((@SaldoActual - @SaldoSerieProducto) as decimal(20, 6)))




                    */

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

            return cierre;

        }

        public List<modeloCierre> getCierreDetalle(string mes, string ano)
        {

            modeloCierre cierreMensual = new modeloCierre();
            List<modeloCierre> cierre = new List<modeloCierre>();

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

            List<modeloCierre> lista = new List<modeloCierre>();

            // calculamos el mes y año previo al cierre
            if (vmes=="01") { vmesant = "12"; aux = int.Parse(vano) - 1; vanoant = aux.ToString(); }
            else {
                vanoant = vano; 
                aux = int.Parse(vmes) - 1; 
                vmesant = aux.ToString(fmt);
            }

            string sqlProductos = "[dbo].[sp_cierreMensualNew] "+vmes+","+vano+ ","+vmesant+ ","+vanoant+"";
            con.Open();
            cmd = new SqlCommand(sqlProductos, con);
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    modeloCierre producto = new modeloCierre();

                    producto.codigo = reader["codigo"].ToString();

                    producto.denominacion = reader["denominacion"].ToString();

                    if (reader["SaldoInicial"].ToString().IsNullOrWhiteSpace()) producto.SaldoInicial = 0;
                    else producto.SaldoInicial = double.Parse(reader["SaldoInicial"].ToString());

                    if (reader["PrecioUnitario"].ToString().IsNullOrWhiteSpace()) producto.PrecioUnitario = 0;
                    else producto.PrecioUnitario = double.Parse(reader["PrecioUnitario"].ToString());

                    if (reader["PrecioPromedioPonderado"].ToString().IsNullOrWhiteSpace()) producto.PrecioPromedioPonderado = 0;
                    else producto.PrecioPromedioPonderado = double.Parse(reader["PrecioPromedioPonderado"].ToString());

                    if (reader["CantCompra"].ToString().IsNullOrWhiteSpace()) producto.CantCompra = 0;
                    else producto.CantCompra = double.Parse(reader["CantCompra"].ToString());

                    if (reader["PrecioCompra"].ToString().IsNullOrWhiteSpace()) producto.PrecioCompra = 0;
                    else producto.PrecioCompra = double.Parse(reader["PrecioCompra"].ToString());

                    if (reader["CostoActual"].ToString().IsNullOrWhiteSpace()) producto.CostoActual = 0;
                    else producto.CostoActual = double.Parse(reader["CostoActual"].ToString());

                    // variables derivadas
                    producto.SaldoActual = producto.SaldoInicial + producto.CantCompra - producto.ventaProducto;
                    producto.CantEntrega = producto.ventaProducto;
                    producto.SaldoPeso = producto.PrecioUnitario * producto.SaldoActual;
 
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
        public List<modeloCierre> getCierre(string mes, string ano)
        {

            modeloCierre cierreMensual = new modeloCierre();
            List<modeloCierre> cierre = new List<modeloCierre>();

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

            List<modeloCierre> lista = new List<modeloCierre>();

            // calculamos el mes y año previo al cierre
            if (vmes == "01") { vmesant = "12"; aux = int.Parse(vano) - 1; vanoant = aux.ToString(); }
            else
            {
                vanoant = vano;
                aux = int.Parse(vmes) - 1;
                vmesant = aux.ToString(fmt);
            }

            string sqlProductos = "[dbo].[sp_cierreMensualNew] " + vmes + "," + vano + "," + vmesant + "," + vanoant + "";
            con.Open();
            cmd = new SqlCommand(sqlProductos, con);
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    modeloCierre producto = new modeloCierre();

                    producto.codigo = reader["codigo"].ToString();

                    producto.denominacion = reader["denominacion"].ToString();

                    if (reader["SaldoInicial"].ToString().IsNullOrWhiteSpace()) producto.SaldoInicial = 0;
                    else producto.SaldoInicial = double.Parse(reader["SaldoInicial"].ToString());

                    if (reader["PrecioUnitario"].ToString().IsNullOrWhiteSpace()) producto.PrecioUnitario = 0;
                    else producto.PrecioUnitario = double.Parse(reader["PrecioUnitario"].ToString());

                    if (reader["PrecioPromedioPonderado"].ToString().IsNullOrWhiteSpace()) producto.PrecioPromedioPonderado = 0;
                    else producto.PrecioPromedioPonderado = double.Parse(reader["PrecioPromedioPonderado"].ToString());

                    if (reader["CantCompra"].ToString().IsNullOrWhiteSpace()) producto.CantCompra = 0;
                    else producto.CantCompra = double.Parse(reader["CantCompra"].ToString());

                    if (reader["PrecioCompra"].ToString().IsNullOrWhiteSpace()) producto.PrecioCompra = 0;
                    else producto.PrecioCompra = double.Parse(reader["PrecioCompra"].ToString());

                    if (reader["CostoActual"].ToString().IsNullOrWhiteSpace()) producto.CostoActual = 0;
                    else producto.CostoActual = double.Parse(reader["CostoActual"].ToString());

                    // variables derivadas
                    producto.SaldoActual = producto.SaldoInicial + producto.CantCompra - producto.ventaProducto;
                    producto.CantEntrega = producto.ventaProducto;
                    producto.SaldoPeso = producto.PrecioUnitario * producto.SaldoActual;

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
        public int setInventarioDetalle(InventarioDetalle inventarioDetalle)
        {
            int i = 0;

            string sql = "INSERT INTO [dbo].[inventarioDetalle]  ( [idInventario], [codigoProducto] , [cantidad],  [serie]  ) VALUES  " +
                                                               " ( @idInventario,  @codigoProducto,  @cantidad,  @serie ) ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();

            try
            {
                cmd.Parameters.AddWithValue("@idInventario", inventarioDetalle.idInventario);
                cmd.Parameters.AddWithValue("@codigoProducto", inventarioDetalle.codigoProducto);
                cmd.Parameters.AddWithValue("@cantidad", inventarioDetalle.cantidad);
                cmd.Parameters.AddWithValue("@idSerie", inventarioDetalle.idSerie);
                cmd.Parameters.AddWithValue("@serie", inventarioDetalle.serie);
                i = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                i = 0;
            }
            finally
            {
                con.Close();
            }
            return i;

        }
        public int putInventarioDetalle(InventarioDetalle inventarioDetalle)
        {
            int i = 0;

            string sql = " UPDATE [dbo].[inventarioDetalle] SET  [cantidad] = @cantidad " +
                         "  WHERE idInventario = @idInventario " +
                         "    AND codigoProducto = @codigoProducto " +
                         "    AND serie=@Serie ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            try
            {
                cmd.Parameters.AddWithValue("@idInventario", inventarioDetalle.idInventario);
                cmd.Parameters.AddWithValue("@codigoProducto", inventarioDetalle.codigoProducto);
                cmd.Parameters.AddWithValue("@cantidad", inventarioDetalle.cantidad);
                cmd.Parameters.AddWithValue("@Serie", inventarioDetalle.serie);
                i = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                i = 0;
            }
            finally
            {
                con.Close();
            }
            return i;
        }
    }

}









