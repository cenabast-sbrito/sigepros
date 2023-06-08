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

namespace pnacpacam.Models
{
    public class modeloInventario
    {
        public string RutCreador { get; set; }
        public int IdInventario { get; set; }
        public string Descripcion { get; set; }
        public string Programado { get; set; }
        public string Estado { get; set; }
        public string RutResponsable { get; set; }
        public string FechaInventario { get; set; }
        public DateTime DFechaCreacion { get; set; }
        public DateTime DFechaModificacion { get; set; }
        public string FechaCreacion { get; set; }
        public string FechaModificacion { get; set; }
        // pestaña principal CDC lista de Convenios
        public List<modeloInventario> getListInventario()
        {
            string sql = "SELECT [idInventario],[fechaInventario],[descripcion],[programado],[estado],[rutResponsable],[fechaCreacion],[fechaModificacion] FROM [dbo].[inventario] where [estado]='ABIERTO'"; // where VigenciaConvenio=@periodo; ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            modeloInventario vInventario = null;
            List<modeloInventario> listaInventario = new List<modeloInventario>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        vInventario = new modeloInventario();
                        vInventario.IdInventario = int.Parse(reader["idInventario"].ToString());
                        vInventario.FechaInventario = reader["fechaInventario"].ToString(); //int.Parse(reader["fechaInventario"].ToString());
                        vInventario.Descripcion = reader["descripcion"].ToString();
                        vInventario.Programado = reader["programado"].ToString();
                        vInventario.Estado = reader["estado"].ToString();
                        vInventario.FechaCreacion = reader["fechaCreacion"].ToString(); //DateTime.Parse(reader["fechaCreacion"].ToString());
                        vInventario.FechaModificacion = reader["fechaModificacion"].ToString();// DateTime.Parse(reader["fechaModificacion"].ToString());
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

        public modeloInventario getInventario(int IdInventario)
        {
            string sql = "SELECT [idInventario],[fechaInventario],[descripcion],[programado],[estado],[rutResponsable],[fechaCreacion],[fechaModificacion] FROM [dbo].[inventario] where idInventario = @IdInventario; ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;

            modeloInventario inventario = null;
            con.Open();
            try
            {
                cmd.Parameters.AddWithValue("@IdInventario", IdInventario);
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        inventario = new modeloInventario();
                        inventario.IdInventario = int.Parse(reader["idInventario"].ToString());
                        inventario.FechaInventario = DateTime.Parse(reader["fechaInventario"].ToString()).ToString();
                        inventario.Descripcion = reader["descripcion"].ToString();
                        inventario.Programado = reader["programado"].ToString();
                        inventario.Estado = reader["estado"].ToString();
                        inventario.RutResponsable = reader["rutResponsable"].ToString();
                        inventario.FechaCreacion = reader["fechaCreacion"].ToString();
                        inventario.FechaModificacion = reader["fechaModificacion"].ToString();
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
            return inventario;
        }

        public List<modeloInventario> getReporteInventario()
        {
            string sql = "SELECT [id_producto],[codigo],[descripcion],[programado],[estado],[rutResponsable],[fechaCreacion],[fechaModificacion] FROM [dbo].[producto] "; // where VigenciaConvenio=@periodo; ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            modeloInventario vInventario = null;
            List<modeloInventario> listaInventario = new List<modeloInventario>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        vInventario = new modeloInventario();
                        vInventario.IdInventario = int.Parse(reader["idInventario"].ToString());
                        vInventario.FechaInventario = reader["fechaInventario"].ToString(); //int.Parse(reader["fechaInventario"].ToString());
                        vInventario.Descripcion = reader["descripcion"].ToString();
                        vInventario.Programado = reader["programado"].ToString();
                        vInventario.Estado = reader["estado"].ToString();
                        vInventario.FechaCreacion = reader["fechaCreacion"].ToString(); //DateTime.Parse(reader["fechaCreacion"].ToString());
                        vInventario.FechaModificacion = reader["fechaModificacion"].ToString();// DateTime.Parse(reader["fechaModificacion"].ToString());
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

        public int putInventario(modeloInventario inventario)
        {
            int i = 0;
            string sql = "UPDATE [dbo].[inventario] " +
                "SET [fechaInventario] = convert(date, @fechaInventario, 105), " +
                "    [descripcion] = @descripcion, " +
                "    [programado] = @programado, " +
                "    [estado] = @estado, [fechaModificacion] = GETDATE() " +
                " WHERE idInventario = @idInventario ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            try
            {
                cmd.Parameters.AddWithValue("@idInventario", inventario.IdInventario);
                cmd.Parameters.AddWithValue("@fechaInventario", inventario.FechaInventario);
                cmd.Parameters.AddWithValue("@descripcion", inventario.Descripcion);
                cmd.Parameters.AddWithValue("@programado", inventario.Programado);
                cmd.Parameters.AddWithValue("@estado", inventario.Estado);
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
        public int setInventario(modeloInventario inventario)
        {
            int i = 0;

            string sql = "INSERT INTO [dbo].[inventario]  ( [fechaInventario], [descripcion] , [programado] , [estado], fechaModificacion,  fechaCreacion, rutResponsable ) VALUES  " +
                         "                                ( @fechaInventario,  @descripcion,  @programado,  @estado, CONVERT(varchar,GETDATE(),126), CONVERT(varchar,GETDATE(),126), @rutResponsable ) ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            try
            {
                // cmd.Parameters.AddWithValue("@idInventario", inventario.IdInventario);
                cmd.Parameters.AddWithValue("@fechaInventario", inventario.FechaInventario);//, inventario.FechaInventario);
                cmd.Parameters.AddWithValue("@descripcion", inventario.Descripcion);
                cmd.Parameters.AddWithValue("@programado", inventario.Programado);
                cmd.Parameters.AddWithValue("@estado", inventario.Estado);
                cmd.Parameters.AddWithValue("@rutResponsable", inventario.RutCreador);
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


        public List<InventarioDetalle> importarInventario_SIRVIODEBASE( string fichero )
        {

            List<InventarioDetalle> listaInventario = new List<InventarioDetalle>();
            InventarioDetalle inventario = new InventarioDetalle();

            // Cargar archivo de Excel
            //        Workbook wb = new Workbook("C:\\DESARROLLOS_2022\\pruebainventario.xlsx");
            string nombreCompleto = "C:\\DESARROLLOS_2022\\" + fichero;
            Workbook wb = new Workbook(nombreCompleto);

            // Obtener todas las hojas de trabajo
            WorksheetCollection collection = wb.Worksheets;

            // Recorra todas las hojas de trabajo
            for (int worksheetIndex = 0; worksheetIndex < collection.Count; worksheetIndex++)
            {

                // Obtener hoja de trabajo usando su índice
                Worksheet worksheet = collection[worksheetIndex];

                // Imprimir el nombre de la hoja de trabajo
                Console.WriteLine("Worksheet: " + worksheet.Name);

                // Obtener el número de filas y columnas
                int rows = worksheet.Cells.MaxDataRow;
                int cols = worksheet.Cells.MaxDataColumn;

                // Bucle a través de filas omitiendo encabezado - empieza desde 1 y no desde cero que es la fila del encabezado
                for (int i = 1; i <= rows; i++)
                {

                    for (int j = 0; j < cols; j++)
                    {
                        Console.Write(worksheet.Cells[i, j].Value + " | ");
                    }

                    inventario.codigoProducto = ""+worksheet.Cells[i, 0].Value;
                    inventario.denominacion = (string)worksheet.Cells[i, 1].Value;
                    inventario.serie = "" + worksheet.Cells[i, 2].Value;
                    inventario.cantidad = "" + worksheet.Cells[i, 3].Value;
                    listaInventario.Add(inventario);

                }
            }
            return listaInventario;
        }


    }

    public class InventarioDetalle
    {
        public int idInventario { get; set; }
        public string codigoProducto { get; set; }
        public string denominacion { get; set; }
        public string cantidad { get; set; }
        public string serie { get; set; }
        public string idSerie { get; set; }
        public string fecha { get; set; }
        public string laboratorio { get; set; }



        /*
  	  select a.codigoProducto, prod.denominacion, a.serie, a.fechaVencimiento,  sum(ISNULL(a.cantidad,0)) as cantidad      
	  from ( SELECT d.idInventario, p.codigoProducto, d.cantidad, p.serie, p.fechaVencimiento               
	  FROM dbo.inventarioDetalle d right join (select distinct codigoProducto, serie, fechaVencimiento from dbo.Serie_producto) as p  on (d.codigoProducto=p.codigoProducto and d.serie=p.serie)               
	  WHERE d.idInventario = 1 or d.idInventario is null ) as a  inner join dbo.producto prod on (prod.codigo=a.codigoProducto) and codigoProducto=100004700
	  group by a.codigoProducto, prod.denominacion, a.serie, a.fechaVencimiento
         */

        public List<InventarioDetalle> getInventarioDetalleV2(int idInventario)
        {

            string sql = "	 select a.codigoProducto, prod.denominacion, a.serie, a.fechaVencimiento,  sum(ISNULL(a.cantidad,0)) as cantidad" +
                            " from(SELECT d.idInventario, p.codigoProducto, d.cantidad, p.serie, p.fechaVencimiento " +
                            " FROM dbo.inventarioDetalle d right join(select distinct codigoProducto, serie, fechaVencimiento from dbo.Serie_producto) as p  on(d.codigoProducto = p.codigoProducto and d.serie = p.serie) " +
                            " WHERE d.idInventario = @idInventario or d.idInventario is null) as a  inner join dbo.producto prod on(prod.codigo = a.codigoProducto) " +
                            " group by a.codigoProducto, prod.denominacion, a.serie, a.fechaVencimiento ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            InventarioDetalle inventarioDetalle = null;
            List<InventarioDetalle> listainventarioDetalle = new List<InventarioDetalle>();
            con.Open();
            try
            {
                cmd.Parameters.AddWithValue("@idInventario", idInventario);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    inventarioDetalle = new InventarioDetalle();
                    inventarioDetalle.idInventario = idInventario;
                    inventarioDetalle.codigoProducto = reader["codigoProducto"].ToString();
                    inventarioDetalle.denominacion = reader["denominacion"].ToString();
                    inventarioDetalle.cantidad = reader["cantidad"].ToString();
                    inventarioDetalle.serie = reader["serie"].ToString();
                    /*no se incluye el id serie puesto que se muestra el inventario sobre el global serie y no sus codigos personales*/
                    // inventarioDetalle.idSerie = reader["id_serie_producto"].ToString();
                    inventarioDetalle.fecha = reader["fechaVencimiento"].ToString().Substring(0, 10);
                    listainventarioDetalle.Add(inventarioDetalle);
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

            return listainventarioDetalle;

        }

        public List<InventarioDetalle> getInventarioDetalle(int idInventario)
        {

/*            string sql = " select a.idInventario, a.codigoProducto, prod.denominacion,  a.cantidad, a.serie, a.fechaVencimiento from ( SELECT d.idInventario, p.codigoProducto, d.cantidad, p.serie, p.fechaVencimiento FROM dbo.inventarioDetalle d right join dbo.Serie_producto p on (d.codigoProducto=p.codigoProducto and d.id_serie_producto=p.id_serie_producto)  WHERE d.idInventario = @idInventario or d.idInventario is null ) as a  inner join dbo.producto prod on (prod.codigo=a.codigoProducto)";
*/
            string sql = "	  select a.codigoProducto, prod.denominacion, a.serie, a.fechaVencimiento,  sum(ISNULL(a.cantidad,0)) as cantidad " +
                  "     from ( SELECT d.idInventario, p.codigoProducto, d.cantidad, p.serie, p.fechaVencimiento " +
                  "              FROM dbo.inventarioDetalle d right join dbo.Serie_producto p on (d.codigoProducto=p.codigoProducto and d.serie=p.serie)  " +
                  "             WHERE d.idInventario = @idInventario or d.idInventario is null ) as a  inner join dbo.producto prod on (prod.codigo=a.codigoProducto)" +
                  "	group by a.codigoProducto, prod.denominacion, a.serie, a.fechaVencimiento";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            InventarioDetalle inventarioDetalle = null;
            List<InventarioDetalle> listainventarioDetalle = new List<InventarioDetalle>();
            con.Open();
            try
            {
                cmd.Parameters.AddWithValue("@idInventario", idInventario);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    inventarioDetalle = new InventarioDetalle();
                    inventarioDetalle.idInventario = idInventario;
                    inventarioDetalle.codigoProducto = reader["codigoProducto"].ToString();
                    inventarioDetalle.denominacion = reader["denominacion"].ToString();
                    inventarioDetalle.cantidad = reader["cantidad"].ToString();
                    inventarioDetalle.serie = reader["serie"].ToString();
                    /*no se incluye el id serie puesto que se muestra el inventario sobre el global serie y no sus codigos personales*/
                   // inventarioDetalle.idSerie = reader["id_serie_producto"].ToString();
                    inventarioDetalle.fecha = reader["fechaVencimiento"].ToString().Substring(0,10);
                    listainventarioDetalle.Add(inventarioDetalle);
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

            return listainventarioDetalle;

        }
        public int setInventarioDetalle(InventarioDetalle inventarioDetalle)
        {
            int i = 0;

            string sql = "INSERT INTO [dbo].[inventarioDetalle]  ( [idInventario], [codigoProducto] ,  [serie] , [cantidad] ) VALUES  " +
                                                               " ( @idInventario,  @codigoProducto,  @serie,  @cantidad ) ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            try
            {
                cmd.Parameters.AddWithValue("@idInventario", inventarioDetalle.idInventario);
                cmd.Parameters.AddWithValue("@codigoProducto", inventarioDetalle.codigoProducto);
                cmd.Parameters.AddWithValue("@cantidad", inventarioDetalle.cantidad);
                /*cmd.Parameters.AddWithValue("@idSerie", inventarioDetalle.idSerie);  */
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

            string sql = " UPDATE [dbo].[inventarioDetalle] " +
                         "    SET cantidad = @cantidad " +
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









