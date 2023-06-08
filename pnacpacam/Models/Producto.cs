using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using pnacpacam.Models;

namespace pnacpacam
{
    /// <summary>
    /// Esta es la clase base de dsitribucion que representa la informacion logistica necesaria para tener claridad de la distribucion
    /// </summary> 
    public class modeloProducto
    {
        public int id_producto { get; set; } //[int] IDENTITY(1,1) NOT NULL,
        public string codigo { get; set; } //[varchar] (50) NULL,
        public string serie { get;set; }
        public string id_unidadVenta { get; set; } // [int] NULL,
        public string unidadVenta { get; set; } // [int] NULL,
        public string um_estado { get; set; } // [int] NULL,
        public int um_cantidad { get; set; } // [int] NULL,
        public string cantidad { get; set; }  //[int] NULL,
        public string saldo { get; set; }  //[int] NULL,
        public string precioUnitario { get; set; }   //[float] NULL,
        public string denominacion { get; set; }  //      [varchar]        (max) NULL,
        public string estado { get; set; }       // [int] NULL,
        public string stockCritico { get; set; }  //        [int] NULL,
        public string error { get; set; }
        public int Ingresar(modeloProducto producto)
        {
            int i = 0;

            string sql = "insert into producto (Rut_Proveedor, Doc_Cenabast, Factura, Fecha_Fac, Guia, Fecha_Gui, O_Trans, FechaCreacion, FechaActualizacion) values ( @Rut_Proveedor , @Doc_Cenabast, @Factura, @Fecha_Fac, @Guia, @Fecha_Gui, @O_Trans, CONVERT(varchar,GETDATE(),126), CONVERT(varchar,GETDATE(),126))";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Rut_Proveedor", producto.id_producto);

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
                con.Close();
            }

            return i;
        }
        public modeloProducto getProducto(string codigo)
        {
            string sql = "SELECT p.[id_producto]    id_producto, " +
                               " p.[codigo]         codigo, " +
                               " p.[id_unidadVenta] id_unidadVenta, " +
                               " u.nombre nombre, " +
                               " u.estado um_estado, " +
                               " u.cantidad um_cantidad, " +
                               " p.[saldo] saldo, " +
                               " p.[precioUnitario] precioUnitario, " +
                               " p.[denominacion] denominacion, " +
                               " p.[estado] estado, " +
                               " p.[stockCritico] stockCritico " +
                               " FROM [dbo].[producto] p inner join [dbo].[unidadMedida] u on (p.id_unidadVenta=u.id_unidadMedida)";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@producto", codigo);
            SqlDataReader reader;
            modeloProducto producto = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    producto = new modeloProducto();
                    producto.id_producto = int.Parse(reader["id_producto"].ToString());
                    producto.denominacion = reader["denominacion"].ToString();
                    producto.codigo = reader["codigo"].ToString();
                    producto.id_unidadVenta = reader["id_unidadVenta"].ToString();
                    producto.unidadVenta = reader["nombre"].ToString();
                    producto.saldo = reader["saldo"].ToString();
                    producto.um_estado = reader["um_estado"].ToString();
                    producto.um_cantidad = int.Parse(reader["um_cantidad"].ToString());
                    producto.precioUnitario = reader["precioUnitario"].ToString();
                    producto.estado = reader["estado"].ToString();
                    producto.stockCritico = reader["stockCritico"].ToString();
                }
            }
            catch (Exception ex)
            {
                producto.error = ex.ToString();
                Console.WriteLine("Error: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }

            return producto;
        }
        public modeloProducto getProductoSerie(string codigo)
        {
            //string sql = "SELECT [id_producto],[codigo],[id_unidadVenta],[saldo],[precioUnitario],[denominacion],[estado],[stockCritico]  FROM [dbo].[producto] where [codigo] =  @producto";
            string sql = "SELECT p.[id_producto] id_producto, " +
                               " p.[codigo] codigo, " +
                               " i.[serie], i.[cantidad], " +
                               " p.[id_unidadVenta] id_unidadVenta, " +
                               " u.nombre nombre, " +
                               " p.[saldo] saldo, " +
                               " p.[precioUnitario] precioUnitario, " +
                               " p.[denominacion] denominacion, " +
                               " p.[estado] estado, " +
                               " p.[stockCritico] stockCritico " +
                               " FROM [dbo].[producto] p inner join [dbo].[unidadMedida]               u on (p.id_unidadVenta=u.id_unidadMedida)" +
                               "                         left  join [dbo].[inventarioDocumentoDetalle] i on (p.codigo=i.codigoProducto)";


            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@producto", codigo);
            SqlDataReader reader;
            modeloProducto producto = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    producto = new modeloProducto();
                    producto.id_producto = int.Parse(reader["id_producto"].ToString());
                    producto.denominacion = reader["denominacion"].ToString();
                    producto.codigo = reader["codigo"].ToString();
                    producto.serie = reader["serie"].ToString();
                    producto.cantidad = reader["cantidad"].ToString();
                    producto.id_unidadVenta = reader["id_unidadVenta"].ToString();
                    producto.unidadVenta = reader["nombre"].ToString();
                    producto.saldo = reader["saldo"].ToString();
                    producto.cantidad = reader["cantidad"].ToString();
                    producto.precioUnitario = reader["precioUnitario"].ToString();
                    producto.estado = reader["estado"].ToString();
                    producto.stockCritico = reader["stockCritico"].ToString();
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

            return producto;
        }
        public List<modeloProducto> getProductos() //se accede al maestro de productos sin serie para las listas de ingreso de documentos
        {
            string sql = "SELECT p.[id_producto] id_producto, " +
                               " p.[codigo] codigo, " +
                               " p.[id_unidadVenta] id_unidadVenta, " +
                               " u.nombre nombre, " +
                               " p.[saldo] saldo, " +
                               " p.[precioUnitario] precioUnitario, " +
                               " p.[denominacion] denominacion, " +
                               " p.[estado] estado, " +
                               " p.[stockCritico] stockCritico " +
                               " FROM [dbo].[producto] p inner join [dbo].[unidadMedida] u on (p.id_unidadVenta=u.id_unidadMedida)";
            ///debo cambiar al procedimiento sp_inventario_cierreMensual
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<modeloProducto> lista = new List<modeloProducto>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    modeloProducto producto = new modeloProducto();
                    producto.id_producto = int.Parse(reader["id_producto"].ToString());
                    producto.denominacion = reader["denominacion"].ToString();
                    producto.codigo = reader["codigo"].ToString();
                    //producto.serie= reader["serie"].ToString();
                    producto.id_unidadVenta = reader["id_unidadVenta"].ToString();
                    producto.unidadVenta = reader["nombre"].ToString();
                    producto.saldo = reader["saldo"].ToString();
                    //producto.cantidad = reader["cantidad"].ToString();
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
            return lista;
        }
        public List<modeloProducto> getProductosSerie()
        {
            string sql = "SELECT p.[id_producto] id_producto, " +
                               " p.[codigo] codigo, " +
                               " i.[serie], i.[cantidad]," +
                               " p.[id_unidadVenta] id_unidadVenta, " +
                               " u.nombre nombre, " +
                               " p.[saldo] saldo, " +
                               " p.[precioUnitario] precioUnitario, " +
                               " p.[denominacion] denominacion, " +
                               " p.[estado] estado, " +
                               " p.[stockCritico] stockCritico " +
                               " FROM [dbo].[producto] p inner join [dbo].[unidadMedida]               u on (p.id_unidadVenta=u.id_unidadMedida)" +
                               "                         left  join [dbo].[inventarioDocumentoDetalle] i on (p.codigo=i.codigoProducto) " +
                               " where serie is not null "; //esta condicion debe ser evaluada, pues lo que indica es la precision de mostrar solo compras en piloto no todo tiene serie

            ///debo cambiar al procedimiento sp_inventario_cierreMensual
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<modeloProducto> lista = new List<modeloProducto>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    modeloProducto producto = new modeloProducto();
                    producto.id_producto = int.Parse(reader["id_producto"].ToString());
                    producto.denominacion = reader["denominacion"].ToString();
                    producto.codigo = reader["codigo"].ToString();
                    producto.serie = reader["serie"].ToString();
                    producto.id_unidadVenta = reader["id_unidadVenta"].ToString();
                    producto.unidadVenta = reader["nombre"].ToString();
                    producto.saldo = reader["saldo"].ToString();
                    producto.cantidad = reader["cantidad"].ToString();
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
            return lista;
        }
        public class unidadMedida
        {
            public string id_unidadVenta { get; set; } // [int] NULL,
            public string unidadVenta { get; set; } // [int] NULL,
            public unidadMedida getUnidadVenta(string codigoProducto)
            {
                string sql = "SELECT id_unidadMedida,nombre  FROM [dbo].[unidadMedida] where [id_unidadMedida] = (select id_unidadVenta from producto where codigo=@codigoProducto)";
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@codigoProducto", codigoProducto);
                SqlDataReader reader;
                unidadMedida um = new unidadMedida();
                con.Open();
                try
                {
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        um.id_unidadVenta = reader["id_unidadMedida"].ToString();
                        um.unidadVenta = reader["nombre"].ToString();
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
                return um;
            }
        }
    
        public class productoSeries
        {
            public string codigoProducto { get; set; }
            public string serie { get; set; }
            public int cantidad { get; set; }
            public DateTime fechaIngreso { get; set; }  
            public List<productoSeries> getSeriesProductos(string codigoProducto)
            {
                /*
                                                    " cantidad," +
                                                   " fechaIngreso " +
                */

                string sql = "SELECT distinct serie " +
                                   " FROM [dbo].[inventarioDocumentoDetalle] where codigoProducto = @codigoProducto";
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@codigoProducto", codigoProducto);

                SqlDataReader reader;
                List<productoSeries> lista = new List<productoSeries>();
                con.Open();
                try
                {
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        productoSeries producto = new productoSeries();
                        producto.codigoProducto = codigoProducto;
                        producto.serie = reader["serie"].ToString();
                        //producto.cantidad = int.Parse(reader["cantidad"].ToString());
                        //producto.fechaIngreso = reader["fechaIngreso"].ToString();
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
    
        public class productoBodega
        {
            public string bodega { get; set; }
            public List<productoBodega> getBodegasProducto(string codigoProducto, string serie)
            {
                string sql = " SELECT [BodegaDestino] FROM [dbo].[inventarioMovimiento] " +
                             "  where codigoProducto = @codigoProducto " +
                             " order by fechaMovimiento desc ";
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@codigoProducto", codigoProducto);
                //cmd.Parameters.AddWithValue("@serie", serie);

                SqlDataReader reader;
                productoBodega productoBodega = null;
                List<productoBodega> lista = new List<productoBodega>();
                con.Open();
                try
                {
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        productoBodega = new productoBodega();
                        productoBodega.bodega = reader["BodegaDestino"].ToString();
                        lista.Add(productoBodega);
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
            public List<productoBodega> getBodegas()
            {
                string sql = " SELECT [codigoBodega] FROM [dbo].[inventarioBodega] ";
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
                SqlCommand cmd = new SqlCommand(sql, con);
                //cmd.Parameters.AddWithValue("@serie", serie);

                SqlDataReader reader;
                productoBodega productoBodega = null;
                List<productoBodega> lista = new List<productoBodega>();
                con.Open();
                try
                {
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        productoBodega = new productoBodega();
                        productoBodega.bodega = reader["codigoBodega"].ToString();
                        lista.Add(productoBodega);
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
}