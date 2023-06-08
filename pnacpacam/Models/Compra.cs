using Microsoft.Ajax.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
//using static Inventario.Models.Compra.SubDepartamentos;

namespace pnacpacam.Models
{
    public class Compra
    {
        public string id_compra { get; set; }
        public string fechaFactura { get; set; }
        public string fechaIngreso { get; set; }
        public string guia { get; set; }
        public string codigoProducto { get; set; }
        public string denominacion { get; set; }
        public string id_unidadVenta { get; set; }
        public string unidadVenta { get; set; }


        public string rutProveedor { get; set; }
        public string cantidadVenta { get; set; }
        public string costoCenabast { get; set; }
        public string costoBienestar { get; set; }
        public string precioPromedioPonderado { get; set; }
        public string totalFactura { get; set; }
        public string rutResponsable { get; set; }


//        public string rutProveedor { get; set; }
        public string nombreProveedor { get; set; }
        public string numeroDocumento { get; set; }
        public string tipoDocumento { get; set; }
        public string fechaDocumento { get; set; }
        public string horaDocumento { get; set; }
        public string numeroDocumentoRef { get; set; }
        public string tipoDocumentoRef { get; set; }
 //       public string codigoProducto { get; set; }
        public string sel_Producto { get; set; }
        public string serie { get; set; }
        public string fechaVencimiento { get; set; }
        public string dsp_unidadVenta { get; set; }
        public string cantidad { get; set; }
        public string totalDocumento { get; set; }


        public string id_serie_producto { get; set; }
        public string cantidadComprada { get; set; }

        public int setCompra(Compra compra)
        {

            int i = 0;
            string sql = " INSERT INTO[dbo].[inventarioDocumento] " +
                         "           ([rutProveedor]        " +
                         "           ,[idTipoDocumento]     " +
                         "           ,[numeroDocumento]     " +
                         "           ,[fechaDocumento]      " +
                         "           ,[idTipoDocumentoRef]  " +
                         "           ,[numeroDocumentoRef]  " +
                         "           ,[totalDocumento])     " +
                         "     VALUES                       " +
                         "           (                      " +
                         "             @rutProveedor        " +
                         "           , @idTipoDocumento     " +
                         "           , @numeroDocumento     " +
                         "           , @fechaDocumento      " +
                         "           , @idTipoDocumentoRef  " +
                         "           , @numeroDocumentoRef  " +
                         "           , @totalDocumento      " +
                         "           )                      ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                try
                {
                cmd.Parameters.AddWithValue("@rutProveedor", compra.rutProveedor);
                //cmd.Parameters.AddWithValue("@sel_Proveedor", compra.sel_Proveedor);
                cmd.Parameters.AddWithValue("@idTipoDocumento", compra.tipoDocumento);
                cmd.Parameters.AddWithValue("@numeroDocumento", compra.numeroDocumento);
                cmd.Parameters.AddWithValue("@fechaDocumento", DateTime.Parse(compra.fechaDocumento)); // + compra.horaDocumento);

                if (compra.tipoDocumentoRef.IsNullOrWhiteSpace())
                {
                    cmd.Parameters.AddWithValue("@idTipoDocumentoRef", DBNull.Value);
                    cmd.Parameters.AddWithValue("@numeroDocumentoRef", DBNull.Value);
                }
                else {
                    cmd.Parameters.AddWithValue("@idTipoDocumentoRef", compra.tipoDocumentoRef);
                    cmd.Parameters.AddWithValue("@numeroDocumentoRef", compra.numeroDocumentoRef);
                }

                cmd.Parameters.AddWithValue("@totalDocumento", compra.totalDocumento);

/*
    cantidad: "150000"
    codigoProducto: undefined
    costoBienestar: undefined
    costoCenabast: "23123"
    dsp_unidadVenta: "Comprimido"
    fechaDocumento: "2023-05-11"
    fechaVencimiento: "2023-05-11"
    horaDocumento: "11:52:00"
    id_unidadVenta: ""
    numeroDocumento: "123"
    numeroDocumentoRef: ""
    precioPromedioPonderado: undefined
    rutProveedor: "59043540-6"
    sel_Producto: undefined
    sel_Proveedor: "59043540-6"
    serie: "AX10123"
    tipoDocumento: "30"
    tipoDocumentoRef: "32"
    totalDocumento: "$ 4.127.455.500"
*/



                //                resultado.FechaCreacion = DateTime.Parse(reader["FechaCreacion"].ToString());

                /*
                                cmd.Parameters.AddWithValue("@codigo", compra.codigoProducto);
                                cmd.Parameters.AddWithValue("@sel_Producto", compra.sel_Producto);
                                cmd.Parameters.AddWithValue("@serie", compra.serie);
                                cmd.Parameters.AddWithValue("@vencimiento", compra.vencimiento);
                                cmd.Parameters.AddWithValue("@dsp_codigoUnidad", compra.dsp_codigoUnidad);
                                cmd.Parameters.AddWithValue("@dsp_unidadVenta", compra.dsp_unidadVenta);
                                cmd.Parameters.AddWithValue("@cantidad", compra.cantidad);
                                cmd.Parameters.AddWithValue("@costoCenabast", compra.costoCenabast);
                                cmd.Parameters.AddWithValue("@costoBienestar", compra.costoBienestar);
                                cmd.Parameters.AddWithValue("@precioPromedioPonderado", compra.precioPromedioPonderado);
                */
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
        public List<Compra> getCompras()
        {
            string sql = "Select c.[id_compra],c.[fecha],c.[guia],c.[rutProveedor],c.[codigoProducto],p.denominacion,c.[id_unidadVenta],u.nombre,c.[cantidadVenta],c.[costoCenabast],c.[costoBienestar],c.[precioPromedioPonderado],c.[totalFactura],c.[rutResponsable],c.[fechaIngreso] FROM [dbo].[compra] c inner join [dbo].[unidadMedida] u on (c.id_unidadVenta=u.id_unidadMedida) inner join [dbo].[producto] p on (c.codigoProducto=p.codigo)";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<Compra> lista = new List<Compra>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Compra compra = new Compra();

                    compra.id_compra = reader["id_compra"].ToString();
                    compra.fechaFactura = reader["fecha"].ToString().Substring(0, 10); // DateTime.Parse(....);// convert(varchar, getdate(), 111)
                    compra.fechaIngreso = reader["fechaIngreso"].ToString().Substring(0, 10);
                    compra.guia = reader["guia"].ToString();
                    compra.rutProveedor = reader["rutProveedor"].ToString();
                    compra.codigoProducto = reader["codigoProducto"].ToString();
                    compra.denominacion = reader["denominacion"].ToString();
                    compra.id_unidadVenta = reader["id_unidadVenta"].ToString();
                    compra.unidadVenta = reader["nombre"].ToString();
                    compra.cantidadVenta = reader["cantidadVenta"].ToString();
                    compra.costoCenabast = reader["costoCenabast"].ToString();
                    compra.costoBienestar = reader["costoBienestar"].ToString();
                    compra.precioPromedioPonderado = reader["precioPromedioPonderado"].ToString();

                    compra.totalFactura = reader["totalFactura"].ToString();
                    compra.rutResponsable = reader["rutResponsable"].ToString();

                    lista.Add(compra);
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
        public int setCompraDetalle(Compra compra)
        {

            int i = 0;
            string sql = " INSERT INTO [dbo].[inventarioDocumentoDetalle] " +
                         "            ([rutProveedor]                     " +
                         "            ,[idTipoDocumento]                  " +
                         "            ,[numeroDocumento]                  " +
                         "            ,[codigoProducto]                   " +
                         "            ,[serie]                            " +
                         "            ,[fechaVencimiento]                 " +
                         "            ,[cantidad]                         " +
                         "            ,[id_unidadVenta]                   " +
                         "            ,[costoCenabast]                    " +
                         "            ,[costoBienestar]                   " +
                         "            ,[precioPromedioPonderado]          " +
                         "            ,[rutResponsable]                   " +
                         "            ,[fechaIngreso])                    " +
                         "      VALUES                                    " +
                         "            (                                   " +
                         " 		       @rutProveedor                     " +
                         "            ,@idTipoDocumento                  " +
                         "            ,@numeroDocumento                  " +
                         "            ,@codigoProducto                   " +
                         "            ,@serie                            " +
                         "            ,@fechaVencimiento                 " +
                         "            ,@cantidad                         " +
                         "            ,@id_unidadVenta                   " +
                         "            ,@costoCenabast                    " +
                         "            ,@costoBienestar                   " +
                         "            ,@precioPromedioPonderado          " +
                         "            ,@rutResponsable                   " +
                         "            ,CONVERT(varchar,GETDATE(),126)    " +
                         " 		   )                                     ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            try
            {
                cmd.Parameters.AddWithValue("@rutProveedor", compra.rutProveedor);
                cmd.Parameters.AddWithValue("@idTipoDocumento", compra.tipoDocumento);
                cmd.Parameters.AddWithValue("@numeroDocumento", compra.numeroDocumento);
                cmd.Parameters.AddWithValue("@codigoProducto", compra.codigoProducto); /*FALTA*/
                cmd.Parameters.AddWithValue("@fechaDocumento", DateTime.Parse(compra.fechaDocumento)); // + compra.horaDocumento);
                cmd.Parameters.AddWithValue("@serie", compra.serie);
                cmd.Parameters.AddWithValue("@fechaVencimiento", compra.fechaVencimiento);
                cmd.Parameters.AddWithValue("@cantidad", compra.cantidad);
                cmd.Parameters.AddWithValue("@id_unidadVenta", compra.id_unidadVenta); /*FALTA*/
                cmd.Parameters.AddWithValue("@costoCenabast", compra.costoCenabast);
                cmd.Parameters.AddWithValue("@costoBienestar", compra.totalDocumento);// DEBO CALCULAR ESTE VALOR costoBienestar);
                cmd.Parameters.AddWithValue("@precioPromedioPonderado", 1);// DEBO CALCULAR ESTE VALOR compra.precioPromedioPonderado);
                cmd.Parameters.AddWithValue("@rutResponsable", compra.rutResponsable);
                // cmd.Parameters.AddWithValue("@fechaIngreso", );

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

        public class compraDetalle
        {

            public string id_serie_producto { get; set; }
            public string codigoProducto { get; set; }
            public string serie { get; set; }
            public string fechaVencimiento { get; set; }
            public string cantidadComprada { get; set; }
            public string id_compra { get; set; }
            public int setCompraDetalle(Compra compra)
            {


                int i = 0;
                string sql = " INSERT INTO [dbo].[inventarioDocumentoDetalle] " +
                             "            ([rutProveedor]                     " +
                             "            ,[idTipoDocumento]                  " +
                             "            ,[numeroDocumento]                  " +
                             "            ,[codigoProducto]                   " +
                             "            ,[serie]                            " +
                             "            ,[fechaVencimiento]                 " +
                             "            ,[cantidad]                         " +
                             "            ,[id_unidadVenta]                   " +
                             "            ,[costoCenabast]                    " +
                             "            ,[costoBienestar]                   " +
                             "            ,[precioPromedioPonderado]          " +
                             "            ,[rutResponsable]                   " +
                             "            ,[fechaIngreso])                    " +
                             "      VALUES                                    " +
                             "            (                                   " +
                             " 		       @rutProveedor,                     " +
                             "            ,@idTipoDocumento,                  " +
                             "            ,@numeroDocumento,                  " +
                             "            ,@codigoProducto,                   " +
                             "            ,@serie,                            " +
                             "            ,@fechaVencimiento,                 " +
                             "            ,@cantidad,                         " +
                             "            ,@id_unidadVenta,                   " +
                             "            ,@costoCenabast,                    " +
                             "            ,@costoBienestar,                   " +
                             "            ,@precioPromedioPonderado,          " +
                             "            ,@rutResponsable,                   " +
                             "            ,@fechaIngreso                      " +
                             " 		   )                                     ";

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                try
                {
                    cmd.Parameters.AddWithValue("@rutProveedor", compra.rutProveedor);
                    cmd.Parameters.AddWithValue("@idTipoDocumento", compra.tipoDocumento);
                    cmd.Parameters.AddWithValue("@numeroDocumento", compra.numeroDocumento);

                    cmd.Parameters.AddWithValue("@codigoProducto", compra.codigoProducto);


                    cmd.Parameters.AddWithValue("@fechaDocumento", DateTime.Parse(compra.fechaDocumento)); // + compra.horaDocumento);

                    cmd.Parameters.AddWithValue("@idTipoDocumentoRef", compra.tipoDocumentoRef);
                    cmd.Parameters.AddWithValue("@numeroDocumentoRef", compra.numeroDocumentoRef);

                    cmd.Parameters.AddWithValue("@totalDocumento", compra.totalDocumento);
                    cmd.Parameters.AddWithValue("@serie", compra.serie);
                    cmd.Parameters.AddWithValue("@fechaVencimiento", compra.fechaVencimiento);
                    cmd.Parameters.AddWithValue("@cantidad", compra.cantidad);
                    cmd.Parameters.AddWithValue("@id_unidadVenta", compra.id_unidadVenta);
                    cmd.Parameters.AddWithValue("@costoCenabast", compra.costoCenabast);
                    cmd.Parameters.AddWithValue("@costoBienestar", compra.costoBienestar);
                    cmd.Parameters.AddWithValue("@precioPromedioPonderado", compra.precioPromedioPonderado);
                    cmd.Parameters.AddWithValue("@costoCenabast", compra.costoCenabast);
                    cmd.Parameters.AddWithValue("@costoBienestar", compra.costoBienestar);
                    cmd.Parameters.AddWithValue("@rutResponsable", compra.rutResponsable);
                   // cmd.Parameters.AddWithValue("@fechaIngreso", );

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
            public List<compraDetalle> getDetalleCompra()
            {
                string sql = "SELECT [id_serie_producto],[codigoProducto],[serie],[fechaVencimiento],[cantidadComprada],[id_compra] FROM [dbo].[Serie_producto]";

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader reader;
                List<compraDetalle> lista = new List<compraDetalle>();
                con.Open();
                try
                {
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        compraDetalle detallecompra = new compraDetalle();

                        detallecompra.id_serie_producto = reader["id_serie_producto"].ToString();
                        detallecompra.codigoProducto = reader["codigoProducto"].ToString();
                        detallecompra.serie = reader["serie"].ToString();
                        detallecompra.fechaVencimiento = reader["fechaVencimiento"].ToString();
                        detallecompra.cantidadComprada = reader["cantidadComprada"].ToString();
                        detallecompra.id_compra = reader["id_compra"].ToString();

                        lista.Add(detallecompra);
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

        
        
        
        }

    }
}






