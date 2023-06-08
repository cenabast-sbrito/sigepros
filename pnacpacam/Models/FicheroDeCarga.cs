using Aspose.Cells;
using Aspose.Cells.Drawing;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WebApi.Areas.HelpPage.ModelDescriptions;

namespace pnacpacam.Models
{
    public class FicheroDeCarga
    {
        public string idInventario { get; set; }
        public string codigoProducto { get; set; }
        public string denominacion { get; set; }
        public string cantidad { get; set; }
        public string serie { get; set; }
        public string idSerie { get; set; }
        public string fecha { get; set; }
        public string laboratorio { get; set; }
        public List<InventarioDetalle> importarInventario(string fichero)
        {
            // Existe sólo una hoja en el fichero de entrada
            int filaEncabezado = 0; int columnaLote = 0; int columnaSaldo = 0; int columnaFecha = 0; int columnaLaboratorio = 0; object val = "";

            List<InventarioDetalle> listaInventario = new List<InventarioDetalle>();
            InventarioDetalle inventario = new InventarioDetalle();

            // Cargar archivo de Excel
            //        Workbook wb = new Workbook("C:\\DESARROLLOS_2022\\pruebainventario.xlsx");
            string nombreCompleto = "C:\\DESARROLLOS_2022\\FARMACIA\\InventarioFicheroDeCarga\\" + fichero;
            Workbook wb = new Workbook(nombreCompleto);

            // Obtener todas las hojas de trabajo
            WorksheetCollection collection = wb.Worksheets;
            Worksheet worksheet = collection[0];

            // Imprimir el nombre de la hoja de trabajo
            Console.WriteLine("Worksheet: " + worksheet.Name);

            // Obtener el número de filas y columnas
            int rows = worksheet.Cells.MaxDataRow;
            int cols = worksheet.Cells.MaxDataColumn;
            int colbus = 0;
            /*
            for (int row = 0; row < rows; row++)
            {
                val = leeValorCelda(worksheet, row, 0);
                if (val.ToString() == "Codigo") { filaEncabezado = row; row = rows + 1; }
            }
            */

            filaEncabezado = localizaFila(worksheet, "Codigo", rows); // ubicar la fila del encabezado, y que obligatoriamente debe estar la palabra codigo en la columna cero
            /* columnaSaldo = localizaColumna(worksheet, "Saldo", filaEncabezado, cols, colbus);
             columnaLote = localizaColumna(worksheet, "Lote", filaEncabezado, cols, colbus);
             columnaFecha = localizaColumna(worksheet, "Fecha", filaEncabezado, cols, colbus);
             columnaLaboratorio = localizaColumna(worksheet, "Laboratorio", filaEncabezado, cols, colbus);*/

            // Bucle a través de filas omitiendo encabezado - empieza desde 1 y no desde cero que es la fila del encabezado
            for (int fil = filaEncabezado + 1; fil <= rows; fil++)
            {
                columnaLote = 0;
                columnaSaldo = 0;
                columnaFecha = 0;
                columnaLaboratorio = 0;
                columnaSaldo = localizaColumna(worksheet, "Saldo", filaEncabezado, cols, colbus);
                columnaLote = localizaColumna(worksheet, "Lote", filaEncabezado, cols, colbus);
                columnaFecha = localizaColumna(worksheet, "Fecha", filaEncabezado, cols, colbus);
                columnaLaboratorio = localizaColumna(worksheet, "Laboratorio", filaEncabezado, cols, colbus);

                for (int col = columnaLote; col <= cols; col++)
                {
                    inventario = new InventarioDetalle();
                    inventario.codigoProducto = leeValorCelda(worksheet, fil, 0);
                    inventario.denominacion = leeValorCelda(worksheet, fil, 1);
                    inventario.serie = leeValorCelda(worksheet, fil, columnaLote);
                    inventario.cantidad = leeValorCelda(worksheet, fil, columnaSaldo);
                    inventario.fecha = leeValorCelda(worksheet, fil, columnaFecha);
                    inventario.laboratorio = leeValorCelda(worksheet, fil, columnaLaboratorio);
                    if (inventario.cantidad != "") listaInventario.Add(inventario);
                    colbus = columnaLote + 1;
                    columnaLote = localizaColumna(worksheet, "Lote", filaEncabezado, cols, colbus);
                    columnaSaldo = localizaColumna(worksheet, "Saldo", filaEncabezado, cols, colbus);
                    columnaFecha = localizaColumna(worksheet, "Fecha", filaEncabezado, cols, colbus);
                    if (columnaLote < 0) { col = cols++; }
                }
            }
            return listaInventario;
        }
        public List<InventarioDetalle> asignarInventario(string fichero, int idInventario)
        {
            // Existe sólo una hoja en el fichero de entrada
            int filaEncabezado = 0; int columnaLote = 0; int columnaSaldo = 0; int columnaFecha = 0; int columnaLaboratorio = 0; object val = "";

            List<InventarioDetalle> listaInventario = new List<InventarioDetalle>();
            InventarioDetalle inventario = new InventarioDetalle();

            // Cargar archivo de Excel
            //        Workbook wb = new Workbook("C:\\DESARROLLOS_2022\\pruebainventario.xlsx");
            string nombreCompleto = "C:\\DESARROLLOS_2022\\FARMACIA\\InventarioFicheroDeCarga\\" + fichero;
            Workbook wb = new Workbook(nombreCompleto);

            // Obtener todas las hojas de trabajo
            WorksheetCollection collection = wb.Worksheets;
            Worksheet worksheet = collection[0];

            // Imprimir el nombre de la hoja de trabajo
            Console.WriteLine("Worksheet: " + worksheet.Name);

            // Obtener el número de filas y columnas
            int rows = worksheet.Cells.MaxDataRow;
            int cols = worksheet.Cells.MaxDataColumn;
            int colbus = 0;
            /*
            for (int row = 0; row < rows; row++)
            {
                val = leeValorCelda(worksheet, row, 0);
                if (val.ToString() == "Codigo") { filaEncabezado = row; row = rows + 1; }
            }
            */

            filaEncabezado = localizaFila(worksheet, "Codigo", rows); // ubicar la fila del encabezado, y que obligatoriamente debe estar la palabra codigo en la columna cero
            /* columnaSaldo = localizaColumna(worksheet, "Saldo", filaEncabezado, cols, colbus);
             columnaLote = localizaColumna(worksheet, "Lote", filaEncabezado, cols, colbus);
             columnaFecha = localizaColumna(worksheet, "Fecha", filaEncabezado, cols, colbus);
             columnaLaboratorio = localizaColumna(worksheet, "Laboratorio", filaEncabezado, cols, colbus);*/

            // Bucle a través de filas omitiendo encabezado - empieza desde 1 y no desde cero que es la fila del encabezado
            for (int fil = filaEncabezado + 1; fil <= rows; fil++)
            {
                columnaLote = 0;
                columnaSaldo = 0;
                columnaFecha = 0;
                columnaLaboratorio = 0;
                columnaSaldo = localizaColumna(worksheet, "Saldo", filaEncabezado, cols, colbus);
                columnaLote = localizaColumna(worksheet, "Lote", filaEncabezado, cols, colbus);
                columnaFecha = localizaColumna(worksheet, "Fecha", filaEncabezado, cols, colbus);
                columnaLaboratorio = localizaColumna(worksheet, "Laboratorio", filaEncabezado, cols, colbus);

                for (int col = columnaLote; col <= cols; col++)
                {
                    inventario = new InventarioDetalle();
                    inventario.idInventario = idInventario;
                    inventario.codigoProducto = leeValorCelda(worksheet, fil, 0);
                    inventario.denominacion = leeValorCelda(worksheet, fil, 1);
                    inventario.serie = leeValorCelda(worksheet, fil, columnaLote);
                    inventario.cantidad = leeValorCelda(worksheet, fil, columnaSaldo);
                    inventario.fecha = leeValorCelda(worksheet, fil, columnaFecha);
                    inventario.laboratorio = leeValorCelda(worksheet, fil, columnaLaboratorio);
                    if (inventario.cantidad != "")
                    {
                        listaInventario.Add(inventario);

                        if (inventario.setInventarioDetalle(inventario) <= 0) 
                            if (inventario.putInventarioDetalle(inventario) <=0) return null;

                    }

                    colbus = columnaLote + 1;
                    columnaLote = localizaColumna(worksheet, "Lote", filaEncabezado, cols, colbus);
                    columnaSaldo = localizaColumna(worksheet, "Saldo", filaEncabezado, cols, colbus);
                    columnaFecha = localizaColumna(worksheet, "Fecha", filaEncabezado, cols, colbus);
                    if (columnaLote < 0) { col = cols++; }

                }
            }
            return listaInventario;
        }

        public int setInventarioDetalle(modeloInventario inventario)
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

        private string leeValorCelda(Worksheet worksheet, int row, int col)
        {
            object val;
            try
            {
                val = worksheet.Cells[row, col].Value;
                if (val != null)
                    return val.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
            return "";
        }
        private int localizaColumna(Worksheet worksheet, string descripcion, int filaEncabezado, int fin, int desde)
        {
            object val = "";
            for (int col = desde; col < fin; col++)
            {
                val = leeValorCelda(worksheet, filaEncabezado, col);
                if (val.ToString() == descripcion) return col;
            }
            return -1;
        }
        private int localizaFila(Worksheet worksheet, string descripcion, int fin)
        {
            object val = "";
            for (int row = 0; row < fin; row++)
            {
                val = leeValorCelda(worksheet, row, 0);
                if (val.ToString() == descripcion) { return row; }
            }
            return -1;
        }


    }
}