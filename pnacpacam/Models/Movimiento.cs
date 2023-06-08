using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace pnacpacam.Models
{
    public class Movimiento
    {
        public string codigoProducto { get; set; } //[varchar] (50) NOT NULL,
        public string serie { get; set; } //[varchar] (50) NOT NULL,
        public string fechaMovimiento { get; set; } // [datetime]
        public string BodegaOrigen { get; set; } // [varchar] (3) NOT NULL,
        public string BodegaDestino { get; set; } // [varchar] (3) NOT NULL,
        public string cantidad { get; set; } // [int] NOT NULL,
        public string observacion { get; set; } // [varchar] (50) NOT NULL,
        public string rutResponsable { get; set; } // [varchar] (10) NOT NULL,
        public string fechaCreacion { get; set; } // [datetime] NULL,

        public Movimiento getMovimiento(string codigoProducto, string serie, string fechaMovimiento)
        {
            string sql = " select codigoProducto    " +
                         "  	 ,serie             " +
                         "  	 ,fechaMovimiento   " +
                         "  	 ,BodegaOrigen      " +
                         "  	 ,BodegaDestino     " +
                         "  	 ,cantidad          " +
                         "  	 ,observacion       " +
                         "  	 ,rutResponsable    " +
                         "  	 ,fechaCreacion     " +
                         " FROM [dbo].[inventarioMovimiento] where codigoProducto = @codigoProducto and serie = @serie and fechaMovimiento=@fechamovimiento"; 

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@codigoProducto", codigoProducto);
            cmd.Parameters.AddWithValue("@serie", serie);
            cmd.Parameters.AddWithValue("@fechaMovimiento", fechaMovimiento);

            SqlDataReader reader;
            Movimiento mov = null;

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    mov = new Movimiento();

                    mov.codigoProducto  = reader["codigoProducto"].ToString();
                    mov.serie           = reader["serie"].ToString();
                    mov.fechaMovimiento = reader["fechaMovimiento"].ToString();
                    mov.BodegaOrigen    = reader["BodegaOrigen"].ToString();
                    mov.BodegaDestino   = reader["BodegaDestino"].ToString();
                    mov.cantidad        = reader["cantidad"].ToString();
                    mov.observacion     = reader["observacion"].ToString();
                    mov.rutResponsable  = reader["rutResponsable"].ToString();
                    mov.fechaCreacion   = reader["fechaCreacion"].ToString();
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
            return mov;
        }
        public List<Movimiento> getMovimientos()
        {
            string sql = "select codigoProducto    " +
                         " 		 ,serie             " +
                         " 		 ,fechaMovimiento   " +
                         " 		 ,BodegaOrigen      " +
                         " 		 ,BodegaDestino     " +
                         " 		 ,cantidad          " +
                         " 		 ,observacion       " +
                         " 		 ,rutResponsable    " +
                         " 		 ,fechaCreacion     " +
                         " FROM [dbo].[inventarioMovimiento] ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<Movimiento> listaMovimientos = new List<Movimiento>();
            Movimiento mov = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    mov = new Movimiento();

                    mov.codigoProducto  = reader["codigoProducto"].ToString();
                    mov.serie           = reader["serie"].ToString();
                    mov.fechaMovimiento = reader["fechaMovimiento"].ToString();
                    mov.BodegaOrigen    = reader["BodegaOrigen"].ToString();
                    mov.BodegaDestino   = reader["BodegaDestino"].ToString();
                    mov.cantidad        = reader["cantidad"].ToString();
                    mov.observacion     = reader["observacion"].ToString();
                    mov.rutResponsable  = reader["rutResponsable"].ToString();
                    mov.fechaCreacion   = reader["fechaCreacion"].ToString();

                    listaMovimientos.Add(mov);

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

            return listaMovimientos;
        }
        public int putMovimientoPENDIENTE(Movimiento movimiento)
        {

            /*
            codigoProducto
            serie
            fechaMovimiento
            BodegaOrigen
            BodegaDestino
            cantidad
            observacion
            rutResponsable
            fechaCreacion
            */


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
                cmd.Parameters.AddWithValue("@idInventario", movimiento.codigoProducto);
                cmd.Parameters.AddWithValue("@idInventario", movimiento.codigoProducto);
                cmd.Parameters.AddWithValue("@idInventario", movimiento.codigoProducto);
                cmd.Parameters.AddWithValue("@idInventario", movimiento.codigoProducto);
                cmd.Parameters.AddWithValue("@idInventario", movimiento.codigoProducto);
                cmd.Parameters.AddWithValue("@idInventario", movimiento.codigoProducto);

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
        public int setMovimiento(Movimiento movimiento)
        {

            int i = 0;

            string sql = "INSERT INTO [dbo].[inventarioMovimiento] " +
                         "       ( codigoProducto, serie, fechaMovimiento, BodegaOrigen, BodegaDestino, cantidad, observacion, rutResponsable, fechaCreacion ) VALUES           " +
                         "       ( @codigoProducto,@serie,@fechaMovimiento,@BodegaOrigen,@BodegaDestino,@cantidad,@observacion,@rutResponsable, CONVERT(varchar,GETDATE(),126) ) ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            try
            {

                cmd.Parameters.AddWithValue("@codigoProducto", movimiento.codigoProducto );

                cmd.Parameters.AddWithValue("@serie", movimiento.serie);
                cmd.Parameters.AddWithValue("@fechaMovimiento", DateTime.Parse(movimiento.fechaMovimiento));
                cmd.Parameters.AddWithValue("@BodegaOrigen", movimiento.BodegaOrigen);
                cmd.Parameters.AddWithValue("@BodegaDestino", movimiento.BodegaDestino);
                cmd.Parameters.AddWithValue("@cantidad", movimiento.cantidad);
                cmd.Parameters.AddWithValue("@observacion", movimiento.observacion);

                cmd.Parameters.AddWithValue("@rutResponsable", movimiento.rutResponsable);
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

    public class TipoMovimiento
    {
        public string id_movimiento { get; set; } //[int] NOT NULL,
        public string movimiento { get; set; } // [varchar] (50) NOT NULL,
        public string entradaSalida { get; set; } // [char](1) NOT NULL,

        public TipoMovimiento getTipoMovimiento(string id_movimiento)
        {
            string sql = " select id_movimiento      " +
                         "  	 ,movimiento         " +
                         "  	 ,entradaSalida      " +
                         " FROM [dbo].[inventarioTipoMovimiento] where id_movimiento = @id_movimiento ";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id_movimiento", id_movimiento);

            SqlDataReader reader;
            TipoMovimiento mov = null;

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();

                if (reader.Read())
                {

                    mov = new TipoMovimiento();

                    mov.id_movimiento = reader["id_movimiento"].ToString();
                    mov.movimiento = reader["movimiento"].ToString();
                    mov.entradaSalida = reader["entradaSalida"].ToString();


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
            return mov;
        }

        public List<TipoMovimiento> getTipoMovimientos()
        {
            string sql = " select id_movimiento      " +
                         "  	 ,movimiento         " +
                         "  	 ,entradaSalida      " +
                         " FROM [dbo].[inventarioTipoMovimiento] where id_movimiento = @id_movimiento ";


            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<TipoMovimiento> listaTipoMovimientos = new List<TipoMovimiento>();
            TipoMovimiento mov = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    mov = new TipoMovimiento();

                    mov.id_movimiento = reader["id_movimiento"].ToString();
                    mov.movimiento = reader["movimiento"].ToString();
                    mov.entradaSalida = reader["entradaSalida"].ToString();
                    listaTipoMovimientos.Add(mov);

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

            return listaTipoMovimientos;
        }
    
    }
    public class Bodega
    {
        public string codigoBodega { get; set; }
        public string BodegaDescripcion { get; set; }
        public string HaberDeber { get; set; }
                

        public List<Bodega> getBodegas()
        {
            string sql = " select [codigoBodega]" +
                         "  	 ,[BodegaDescripcion]" +
                         "  	 ,[HaberDeber]" +
                         " FROM [dbo].[inventarioBodega]";


            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<Bodega> listaBodegas = new List<Bodega>();
            Bodega mov = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader(); 
                while (reader.Read())
                {

                    mov = new Bodega();

                    mov.codigoBodega      = reader["codigoBodega"].ToString();
                    mov.BodegaDescripcion = reader["BodegaDescripcion"].ToString();
                    mov.HaberDeber        = reader["HaberDeber"].ToString();
                    listaBodegas.Add(mov);

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

            return listaBodegas;
        }

    }

}