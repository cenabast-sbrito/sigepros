using Aspose.Pdf.Plugins;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace pnacpacam.Models
{

    public class Rol
    {
        public string CRol { get; set; }
        public string TRol { get; set; }
        public string CPrograma { get; set; }
        public string Organismo { get; set; }
        public List<Rol> GetRoles(string organismo, string rut)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(
                @"
SELECT r.CRol,
       r.TRol,
       r.Organismo,
       r.CPrograma,
       r.Permiso
FROM Rol r
WHERE r.Organismo = @organismo
  -- Excluir roles ya asignados
  AND NOT EXISTS (
      SELECT 1
      FROM UsuarioRol ur
      WHERE ur.Rut = @rut
        AND ur.CRol = r.CRol
  )
  -- Regla de exclusión mutua de permisos
  AND NOT (
      /* Caso 1: quiere Consulta pero ya tiene Administra */
      (r.Permiso = 'Consulta'
       AND EXISTS (
           SELECT 1
           FROM UsuarioRol ur1
           INNER JOIN Rol r1 ON r1.CRol = ur1.CRol
           WHERE ur1.Rut = @rut
             AND r1.Permiso = 'Administra'
       ))
      OR
      /* Caso 2: quiere Administra pero ya tiene Consulta */
      (r.Permiso = 'Administra'
       AND EXISTS (
           SELECT 1
           FROM UsuarioRol ur2
           INNER JOIN Rol r2 ON r2.CRol = ur2.CRol
           WHERE ur2.Rut = @rut
             AND r2.Permiso = 'Consulta'
       ))
  );
"
                , con);
            SqlDataReader reader;
            List<Rol> lista = new List<Rol>();
            cmd.Parameters.AddWithValue("@organismo", organismo);
            cmd.Parameters.AddWithValue("@rut", rut);

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Rol rol = new Rol();
                    rol.CRol = reader["CRol"].ToString();
                    rol.TRol = reader["TRol"].ToString();
                    rol.CPrograma = reader["CPrograma"].ToString();
                    rol.Organismo = reader["Organismo"].ToString();

                    lista.Add(rol);
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


        public string setRolUsuario(string rut, string cRol)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            try
            {
                using (var cmd = new SqlCommand("dbo.proc_UsuarioRol_Agregar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Rut", SqlDbType.VarChar, 30).Value = rut ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@CRol", SqlDbType.Char, 3).Value = cRol ?? (object)DBNull.Value;

                    if (con.State != ConnectionState.Open) con.Open();
                    cmd.ExecuteNonQuery();

                    return "OK";
                }
            }
            catch (SqlException sqlEx)
            {
                return SqlExceptionHandler.GetMessage(sqlEx);
            }
            catch (Exception ex)
            {
                return SqlExceptionHandler.GetMessage(ex);
            }

            //catch (SqlException sqlEx)
            //{
            //    if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
            //        return "El usuario ya posee ese rol (duplicado)." + sqlEx.Number;

            //    switch (sqlEx.Number)
            //    {
            //        case 50100: return "El Rut es obligatorio." + sqlEx.Number;
            //        case 50101: return "El código de rol es obligatorio." + sqlEx.Number;
            //        case 50102: return "El rol indicado no existe." + sqlEx.Number;
            //        case 50103: return "El usuario ya posee ese rol." + sqlEx.Number;
            //        case 50104: return "No se permite mezclar organismos." + sqlEx.Number;
            //        case 50105: return "No puede asignar Admin CENABAST si tiene Gestor CENABAST." + sqlEx.Number;
            //        case 50106: return "No puede asignar Gestor CENABAST si tiene Admin CENABAST." + sqlEx.Number;
            //        default:    return $"Error SQL ({sqlEx.Number})." + sqlEx.Number;
            //    }
            //}
            //catch (Exception)
            //{
            //    return "Error inesperado.";
            //}
        }


        public string setRolUsuarioOLD(string rut, string rol )
        {
            int i = 0;
            string respuesta = null;
            
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand("proc_UsuarioRol_Agregar @Rut, @CRol ", con);
            SqlDataReader reader;

            cmd.Parameters.AddWithValue("@Rut", rut);
            cmd.Parameters.AddWithValue("@CRol", rol);

            con.Open();
            try
            {
                i = cmd.ExecuteNonQuery();
                return "";

            }
            catch (SqlException sqlEx)
            {
                // 2627: Violation of PRIMARY KEY or UNIQUE constraint
                // 2601: Cannot insert duplicate key row in object with UNIQUE index
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    // Manejo específico: el registro ya existe
                    // Opciones: devolver código, lanzar excepción propia, loguear, etc.
                    Console.WriteLine("Registro duplicado (PK/UNIQUE). Detalle: " + sqlEx.Message);
                    // Ejemplo: retorno semántico -1 indica duplicado
                    return "Registro duplicado (PK/UNIQUE). Detalle: " + sqlEx.Message;
                    //return -1;
                }
                else
                {
                    return "SqlException ({sqlEx.Number}): {sqlEx.Message}";
                    // Otros errores SQL: re-lanzar o manejar según corresponda
                    //Console.Error.WriteLine($"SqlException ({sqlEx.Number}): {sqlEx.Message}");
                    //throw; // Re-lanzamos para que capas superiores decidan
                }
            }
            catch (Exception ex)
            {
                // Excepciones no-SQL (timeouts, null ref, etc.)
                return "Error inesperado: " + ex.Message;
                Console.Error.WriteLine("Error inesperado: " + ex);
                throw;
            }
            finally
            {
                if (con.State != ConnectionState.Closed)
                    con.Close();
            }

            return "";
        }
    }
    public class Privilegios
    {
        public string CRol { get; set; }
        public string NEstado { get; set; }
        public string Ejecuta { get; set; }
        public string Visualiza { get; set; }
        public List<Privilegios> _getPrivilegios(string Rol)
        {
            string sql = "SELECT [CRol],[NEstado],[Ejecuta],[Visualiza] FROM [dbo].[Privilegios] where [Rol] = '@Rol' ";
            List<Privilegios> privilegios = new List<Privilegios>();

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@Rol", Rol);
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Privilegios privilegio = new Privilegios();
                    privilegio.CRol        = reader["CRol"].ToString();
                    privilegio.NEstado     = reader["NEstado"].ToString();
                    privilegio.Visualiza   = reader["Visualiza"].ToString();
                    privilegio.Ejecuta     = reader["Ejecuta"].ToString();
                    privilegios.Add(privilegio);

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
            return privilegios;
        }


    }

    public class RolesUsuarios
    {
        public string Rut { get; set; }
        public string CRol { get; set; }
        public string TRol { get; set; }
        public List<RolesUsuarios> GetRolesUsuario(string rut)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT up.Rut, up.CRol, (select trol from [Rol] where crol = up.crol) TRol FROM UsuarioRol up WHERE up.Rut = @rut ", con);
            SqlDataReader reader;
            List<RolesUsuarios> lista = new List<RolesUsuarios>();
            cmd.Parameters.AddWithValue("@rut", rut);

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RolesUsuarios rol = new RolesUsuarios();
                    rol.Rut = reader["Rut"].ToString();
                    rol.CRol = reader["CRol"].ToString();
                    rol.TRol = reader["TRol"].ToString();

                    lista.Add(rol);
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
        public string DelRolesUsuario(string rut)
        {
            string respuesta = "";
            int i;

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand("DELETE FROM UsuarioRol WHERE Rut = @rut ", con);
            cmd.Parameters.AddWithValue("@rut", rut);

            con.Open();
            try
            {
                i = cmd.ExecuteNonQuery();
                if (i>0) respuesta = "OK";
            }
            catch (SqlException ex)
            {

                Console.WriteLine("Error: " + ex.ToString());
                respuesta = "Ha ocurrido un error : " + ex.Message;

            }
            catch (Exception ex)
            {

                Console.WriteLine("Error: " + ex.ToString());
                respuesta = "Ha ocurrido un error : " + ex.Message;

            }
            finally
            {
                con.Close();
            }

            return respuesta;
        }
    }
}