using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace pnacpacam.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        [Required]
        public string Rut { get; set; }
        public bool Estado { get; set; }
        [Required]
        public string IdPerfil { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaActualizacion { get; set; }


        /// <summary>
        /// Ingresa un Objeto Distribucion a BBDD
        /// </summary> 
        public int Ingresar(Usuario user)
        {
            int i = 0;
            string sql = "insert into inventarioUsuarios (Rut, Estado, IdPerfil, FechaCreacion, FechaActualizacion) values ( @Rut , @Estado, @Perfil, CONVERT(varchar,GETDATE(),126), CONVERT(varchar,GETDATE(),126))";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Rut", user.Rut);
            cmd.Parameters.AddWithValue("@Estado", user.Estado);
            cmd.Parameters.AddWithValue("@Perfil", user.IdPerfil);
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
                // Cierro la Conexión.
                con.Close();
            }

            return i;
        }

        /// <summary>
        /// MODELO ACTIVO, esta siendo usado por la APP
        /// Obtiene un Objeto Usuario
        /// </summary> 
        public Usuario GetUsuario(string Rut)
        {
            //string sql = "select Id_usuario, Rut, activo, IdPerfil, FechaIngreso from Usuario where Rut = @Rut";
            string sqlNEW = "select Id_usuario, Rut, activo, IdPerfil, FechaIngreso from inventarioUsuarios where activo='true' and substring(rut,1,len(rut)-2) = @rut"; ///and substring(rut,1,CHARINDEX('-',rut)-1) = '@rut'";
            string sql = "select Idusuario, Rut, estado, IdPerfil from inventarioUsuarios where Rut = @Rut";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Rut", Rut);
            SqlDataReader reader;
            Usuario user = null;

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    user = new Usuario();
                    //user.IdUsuario = int.Parse(reader["Id_usuario"].ToString());
                    user.IdUsuario = int.Parse(reader["Idusuario"].ToString());
                    user.Rut = reader["Rut"].ToString();
                    //user.Estado = bool.Parse(reader["activo"].ToString());
                    user.Estado = bool.Parse(reader["estado"].ToString());
                    user.IdPerfil = reader["IdPerfil"].ToString();
                    //user.FechaCreacion = DateTime.Parse(reader["FechaIngreso"].ToString());
//                    user.FechaActualizacion = DateTime.Parse(reader["FechaActualizacion"].ToString());
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
            return user;
        }

        /// <summary>
        /// Obtiene una lista de Objetos Distribucion a BBDD
        /// </summary> 
        public List<Usuario> ObtenerUsuarios()
        {
            string sql = "select IdUsuario, Rut, Estado, IdPerfil, FechaCreacion, FechaActualizacion from inventarioUsuarios ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<Usuario> lista = new List<Usuario>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    Usuario user = new Usuario();
                    user.IdUsuario = int.Parse(reader["IdUsuario"].ToString());
                    user.Rut = reader["Rut"].ToString();
                    user.Estado = bool.Parse(reader["Estado"].ToString());
                    user.IdPerfil = reader["IdPerfil"].ToString();
                    user.FechaCreacion = DateTime.Parse(reader["FechaCreacion"].ToString());
                    user.FechaActualizacion = DateTime.Parse(reader["FechaActualizacion"].ToString());
                    lista.Add(user);
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
        /// <summary>
        /// Actualiza un Objeto Distribucion a BBDD
        /// </summary> 
        public int Actualizar(Usuario user )
        {

            int i = 0;
            string sql = "update inventarioUsuarios set Estado = @Estado, IdPerfil = @IdPerfil, FechaActualizacion = GETDATE() where IdUsuario = @IdUsuario and Rut = @Rut ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Estado", user.Estado);
            cmd.Parameters.AddWithValue("IdPerfil", user.IdPerfil);
            cmd.Parameters.AddWithValue("@IdUsuario", user.IdUsuario);
            cmd.Parameters.AddWithValue("@Rut", user.Rut);

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
        public int siExiste(string Rut)
        {

            int i = 0;
            string sql = "select Rut from inventarioUsuarios where Rut = @Rut";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Rut", Rut);
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
                // Cierro la Conexión.
                con.Close();
            }

            return i;
        }

        public bool Authorization(string rut)
        {
            return true;
            bool status = false;
            string sql = "select rut from inventarioUsuarios where activo='true' and substring(rut,1,len(rut)-2) = @rut"; ///and substring(rut,1,CHARINDEX('-',rut)-1) = '@rut'";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@rut", rut);
            SqlDataReader reader;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    status = true;
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

            return status;
        }

    }

    public class UsuarioViewModel
    {
        public string nombre { get; set; }
        public string rut { get; set; }

        /// <summary>
        /// Obtiene una lista de Objeto UsuarioViewModel
        /// </summary> 
        public List<UsuarioViewModel> GetName(string name)
        {
           string sql = "select rut, nombre+' '+apellido as nombre from funcionario where nombre like @name and estado = 1 ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["secondary"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@name", name + "%");
            SqlDataReader reader;
            List<UsuarioViewModel> lista = new List<UsuarioViewModel>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    UsuarioViewModel user = new UsuarioViewModel();
                    user.rut = reader["rut"].ToString();
                    user.nombre = reader["nombre"].ToString();
                    lista.Add(user);
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

    public class UsuarioViewModelModify
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Rut { get; set; }
        public bool Estado { get; set; }
        public string Email { get; set; }
        public string IdPerfil { get; set; }

        public string NombrePerfil { get; set; }
        /// <summary>
        /// Obtiene una lista de Objetos Distribucion a BBDD
        /// </summary> 
        public List<UsuarioViewModelModify> GeUsuarios()
        {
            string sql = "select u.IdUsuario, u.Rut, (f.nombre+' '+f.apellido) as Funcionario, f.email, u.IdPerfil, (Select NombrePerfil from Perfil where IdPerfil = u.IdPerfil) as NombrePerfil, u.Estado  from aa_ControlInsumos.dbo.funcionario f inner join inventarioUsuarios u on f.rut = u.Rut";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<UsuarioViewModelModify> lista = new List<UsuarioViewModelModify>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    UsuarioViewModelModify user = new UsuarioViewModelModify();
                    user.Id = int.Parse(reader["IdUsuario"].ToString());
                    user.Rut = reader["Rut"].ToString();
                    user.Nombre = reader["Funcionario"].ToString();
                    user.Email = reader["email"].ToString();
                    user.IdPerfil = reader["IdPerfil"].ToString();
                    user.NombrePerfil = reader["NombrePerfil"].ToString();
                    user.Estado = bool.Parse(reader["Estado"].ToString());
                    lista.Add(user);
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

        public UsuarioViewModelModify GetUsuario(string Rut, string stringConexion)
        {
            string sql = "select u.IdUsuario, u.Rut, (f.nombre+' '+f.apellido) as Funcionario, f.email, u.IdPerfil, (Select NombrePerfil from Perfil where IdPerfil = u.IdPerfil) as NombrePerfil, u.Estado  from aa_controlinsumos.dbo.funcionario f inner join inventarioUsuarios u on f.rut = u.Rut where f.estado = 1 and u.Rut = @Rut";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Rut", Rut);
            SqlDataReader reader;
            UsuarioViewModelModify user = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
  
                    user = new UsuarioViewModelModify();
                    user.Id = int.Parse(reader["IdUsuario"].ToString());
                    user.Rut = reader["Rut"].ToString();
                    user.Nombre = reader["Funcionario"].ToString();
                    user.Email = reader["email"].ToString();
                    user.IdPerfil = reader["IdPerfil"].ToString();
                    user.NombrePerfil = reader["NombrePerfil"].ToString();
                    user.Estado = bool.Parse(reader["Estado"].ToString());
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

            return user;
        }
    }
}