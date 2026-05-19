using Aspose.Pdf.Operators;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Numerics;

namespace pnacpacam.Models
{
    public class Proveedor
    {
        public int RutProveedor { get; set; }
        public string Nombre                 { get; set; }
        public string Direccion              { get; set; }
        public int Estado { get; set; }
        public string FechaCreacion          { get; set; }
        public string FechaActualizacion { get; set; }
        public int CPrograma { get; set; }
        public string TPrograma { get; set; }

        public int Ingresar(Proveedor proveedor, out string mensaje)
        {
            mensaje = string.Empty;
            int i = 0;

            using (SqlConnection con = new SqlConnection(
                ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("proc_Proveedor_Agregar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@rutProveedor", proveedor.RutProveedor);
                    cmd.Parameters.AddWithValue("@nombre", proveedor.Nombre);
                    cmd.Parameters.AddWithValue("@direccion",
                        string.IsNullOrEmpty(proveedor.Direccion) ? "" : proveedor.Direccion);
                    cmd.Parameters.AddWithValue("@CPrograma", proveedor.CPrograma);

                    try
                    {
                        con.Open();
                        i = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        // Error controlado desde SQL (RAISERROR)
                        mensaje = ex.Message;
                        i = -1;
                    }
                    catch (Exception ex)
                    {
                        // Error inesperado
                        mensaje = "Ocurrió un error inesperado al ingresar el proveedor.";
                        i = -2;
                    }
                }
            }

            return i;
        }

        //public int Ingresar(Proveedor proveedor)
        //{
        //    int i = 0;

        //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
        //    SqlCommand cmd = new SqlCommand("proc_Proveedor_Agregar @rut, @nombre, @direccion, @programa", con);
        //    cmd.Parameters.AddWithValue("@rut", proveedor.RutProveedor);
        //    cmd.Parameters.AddWithValue("@nombre", proveedor.Nombre);
        //    cmd.Parameters.AddWithValue("@direccion", (string.IsNullOrEmpty(proveedor.Direccion)) ? "" : proveedor.Direccion);
        //    cmd.Parameters.AddWithValue("@programa", proveedor.CPrograma);
        //    con.Open();
        //    try
        //    {
        //        i = cmd.ExecuteNonQuery();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error: " + ex.ToString());
        //        i = 0;
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }

        //    return i;
        //}
        public int Actualizar(Proveedor proveedor)
        {
            int i = 0;

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand("proc_Proveedor_Actualizar @rut, @nombre, @direccion, @estado, @programa", con);
            cmd.Parameters.AddWithValue("@rut", proveedor.RutProveedor);
            cmd.Parameters.AddWithValue("@nombre", proveedor.Nombre);
            cmd.Parameters.AddWithValue("@direccion", (string.IsNullOrEmpty(proveedor.Direccion)) ? "" : proveedor.Direccion);
            cmd.Parameters.AddWithValue("@estado", proveedor.Estado == null ? 1 : proveedor.Estado);
            cmd.Parameters.AddWithValue("@programa", proveedor.CPrograma);

            con.Open();
            try
            {
                i = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return i;
        }

        public Proveedor getProveedor(int programa, string rut)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);

            SqlCommand cmd = new SqlCommand("proc_Proveedor_Obtener @RutProveedor, @tipoConsulta, @programa ", con);

            cmd.Parameters.AddWithValue("@tipoConsulta", DBNull.Value);
            cmd.Parameters.AddWithValue("@rutProveedor", rut);
            cmd.Parameters.AddWithValue("@programa", programa);

            SqlDataReader reader;
            Proveedor prov = null;

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    prov = new Proveedor();
                    prov.RutProveedor = int.Parse(reader["RutProveedor"].ToString());
                    prov.Nombre = reader["Nombre"].ToString();
                    prov.Direccion = reader["Direccion"].ToString().IsNullOrWhiteSpace() ? "" : reader["Direccion"].ToString();
                    prov.CPrograma = int.Parse(reader["RutProveedor"].ToString());
                    if (reader["Estado"].ToString().IsNullOrWhiteSpace()) { prov.Estado = 1; } else { prov.Estado = 0; }

                }
            }
            catch (SqlException sex)
            {
                Console.WriteLine("Error: " + sex.ToString());
            }
            finally
            {
                con.Close();
            }
            return prov;
        }
        public int delProveedor(string rut)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);

            SqlCommand cmd = new SqlCommand("proc_Proveedor_Eliminar @RutProveedor", con);

            cmd.Parameters.AddWithValue("@rutProveedor", rut);

            SqlDataReader reader;
            int i = 0;
            Proveedor prov = null;

            con.Open();
            try
            {
                i = cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return i;
        }
        public int addProveedor(string rut)
        {

            string sql = @"
             UPDATE dbo.Proveedor
                    SET 
                        Estado = 1,
                        FechaActualizacion = GETDATE()
                    WHERE convert(int,RutProveedor) = CONVERT(int, @RutProveedor);
            ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);

//            SqlCommand cmd = new SqlCommand("proc_Proveedor_Activar @RutProveedor", con);
            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@rutProveedor", rut);

            SqlDataReader reader;
            int i = 0;
            Proveedor prov = null;

            con.Open();
            try
            {
                i = cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return i;
        }
        public List<Proveedor> getListProveedorPnacPacam(string tipoConsulta, int programa)
        {
            /* tipoConsulta 
               A: estado = 1
               B: estado = 0
               C: estado = null
             */
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);

            SqlCommand cmd = new SqlCommand("proc_Proveedor_Obtener @RutProveedor, @tipoConsulta, @programa", con);

            // Parámetros
            cmd.Parameters.AddWithValue("@tipoConsulta", tipoConsulta);
            cmd.Parameters.AddWithValue("@RutProveedor", "");
            cmd.Parameters.AddWithValue("@programa", programa);

            SqlDataReader reader;

            Proveedor prov = null;
            List<Proveedor> listaproveedores = new List<Proveedor>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                // if (reader.HasRows)  
                // {
                while (reader.Read())
                {
                    prov = new Proveedor();
                    prov.RutProveedor = int.Parse(reader["RutProveedor"].ToString());
                    prov.Nombre = reader["Nombre"].ToString();
                    prov.Direccion = reader["Direccion"].ToString().IsNullOrWhiteSpace() ? "" : reader["Direccion"].ToString();
                    prov.CPrograma = int.Parse(reader["CPrograma"].ToString());
                    prov.TPrograma = reader["TPrograma"].ToString();
                    prov.Estado = reader.IsDBNull(reader.GetOrdinal("Estado"))
               ? 1   // o el valor por defecto que tú quieras
               : Convert.ToInt32(reader["Estado"]);
                    listaproveedores.Add(prov);
                }
                //  }
            }catch(SqlException sex) { 
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
            return listaproveedores;
        }

        public Proveedor getProveedorPedidoCompra(string pedidoCompra)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand("proc_ProveedorSAP_Obtener @pedidoCompra", con);
            cmd.Parameters.AddWithValue("@pedidoCompra", pedidoCompra);

            SqlDataReader reader;
            Proveedor prov = new Proveedor(); // <-- SIEMPRE instanciado

            prov.RutProveedor = 0;
            prov.Nombre = "";

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    prov.RutProveedor = int.Parse(reader["rutProveedor"].ToString());
                    prov.Nombre = reader["Nombre"].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                // prov sigue en 0,0 y "", correcto
            }
            finally
            {
                con.Close();
            }

            return prov;  // <-- nunca será null
        }

    }

}