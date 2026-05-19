using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


using System.Security.Cryptography;
using System.Text;

namespace pnacpacam.Models
{
    public class Usuario
    {
        public string Rut { get; set; }
        public bool Estado { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Organismo { get; set; }
        public String FechaCreacion { get; set; }
        public String email { get; set; }
        public string Rol { get; set; }
        public String FechaActualizacion { get; set; }
        public string QryUsuario = "SELECT rut, Pwd clave, nombre, apellido, email, estado, rol FROM usuarios ";
        public string QryUsuarios = "SELECT rut, Pwd clave, nombre, apellido, email, estado, rol FROM usuarios where rut = @Rut;";
        public string QryAuthorization = "SELECT rut FROM usuarios where rut = @Rut and estado=1 and rut exist (SELECT [Rut] FROM [PNACPACAM].[dbo].[UsuarioRol] where rol = @rol) ;";
        public string QryAuthenticate = "SELECT rut from usuarios where rut = @Rut and @Pass =  Pwd ;";

        public Usuario GetUsuario(string Rut, string programa)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand("proc_Usuarios_Obtener_new @rut, @programa", con);
            cmd.Parameters.AddWithValue("@rut", Rut);
            cmd.Parameters.AddWithValue("@programa", programa);
            SqlDataReader reader;
            Usuario user = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    user = new Usuario();
                    user = _set(reader);
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

            return user;
        }
        public Usuario GetUsuario2(string Rut) //aqui se solicita modificar al usuario
        { 

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand("proc_Usuarios_Obtener @rut", con);
            cmd.Parameters.AddWithValue("@rut", Rut);
            SqlDataReader reader;
            Usuario user = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    user = new Usuario();
                    user = _set(reader);
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

            return user;
        }
        public List<Usuario> GeUsuarios()
        { 

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand("proc_Usuarios_Obtener", con);
            SqlDataReader reader;
            List<Usuario> lista = new List<Usuario>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Usuario user = new Usuario();
                    user = _set(reader);
                    lista.Add(user);
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

        public string Ingresar(Usuario user)
        {
            int i = 0;

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            // SqlCommand cmd = new SqlCommand("proc_Usuarios_Agregar @Rut, @Estado, @Rol, @Nombre, @Apellido, @Clave, @Organismo ", con);
            SqlCommand cmd = new SqlCommand("proc_Usuarios_Agregar @Rut, @Estado, @Nombre, @Apellido, @Organismo, @Clave ", con);
            cmd.Parameters.AddWithValue("@Rut", user.Rut);
            cmd.Parameters.AddWithValue("@Estado", user.Estado);
            //cmd.Parameters.AddWithValue("@Rol", user.Rol);
            cmd.Parameters.AddWithValue("@nombre", user.Nombre);
            cmd.Parameters.AddWithValue("@apellido", user.Apellido);
            cmd.Parameters.AddWithValue("@organismo", user.Organismo);

            var encryptedBase64 = CryptoHelper.EncryptToBase64(user.Clave);
            cmd.Parameters.AddWithValue("@Clave", encryptedBase64);

            con.Open();
            try
            {
                i = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message.ToString();
            }
            finally
            {
                con.Close();
            }

            return "";
        }
        public int Actualizar(Usuario user)
        {
            string sql;
            int i = 0;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString))
            {

                using (var cmd = new SqlCommand("proc_Usuarios_Actualizar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Rut", SqlDbType.NVarChar, 12).Value = user.Rut ?? (object)DBNull.Value;

                    if (!string.IsNullOrWhiteSpace(user.Clave))
                    {
                        var encryptedBase64 = CryptoHelper.EncryptToBase64(user.Clave);
                        cmd.Parameters.Add("@Clave", SqlDbType.NVarChar, 256).Value = encryptedBase64;
                    }

                    cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value =
                        string.IsNullOrWhiteSpace(user.Nombre) ? (object)DBNull.Value : user.Nombre;

                    cmd.Parameters.Add("@Apellido", SqlDbType.NVarChar, 100).Value =
                        string.IsNullOrWhiteSpace(user.Apellido) ? (object)DBNull.Value : user.Apellido;

                    cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value =
                        user.Estado == null ? (object)DBNull.Value : (user.Estado ? true : false);

                    con.Open();
                    i = cmd.ExecuteNonQuery();
                }

            }
            return i;
        }


        public int toggleUsuario(Usuario usuario)
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString))
            using (var cmd = new SqlCommand("proc_Usuarios_ActivarDesactivar", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Validaciones básicas antes de ir a SQL
                var rut = (usuario?.Rut ?? "").Trim();
                if (string.IsNullOrEmpty(rut))
                    throw new ArgumentException("El Rut del usuario es obligatorio.");

                // Si Estado es bool? (nullable), controla nulo
                int estado = (usuario?.Estado == true) ? 1 : 0;

                cmd.Parameters.Add("@Rut", SqlDbType.VarChar, 30).Value = rut;
                cmd.Parameters.Add("@Estado", SqlDbType.Int).Value = estado;

                con.Open();
                return cmd.ExecuteNonQuery(); // filas afectadas
            }
        }


        public int toggleUsuarioalternativa1(Usuario usuario)
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString))
            using (var cmd = new SqlCommand("proc_Usuarios_ActivarDesactivar", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Validaciones básicas antes de ir a SQL
                var rut = (usuario?.Rut ?? "").Trim();
                if (string.IsNullOrEmpty(rut))
                    throw new ArgumentException("El Rut del usuario es obligatorio.");

                // Si Estado es bool? (nullable), controla nulo
                int estado = (usuario?.Estado == true) ? 1 : 0;

                cmd.Parameters.Add("@Rut", SqlDbType.VarChar, 30).Value = rut;
                cmd.Parameters.Add("@Estado", SqlDbType.Int).Value = estado;

                con.Open();
                return cmd.ExecuteNonQuery(); // filas afectadas
            }
        }


        //public int toggleUsuario(Usuario usuario)
        //{
        //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);

        //    SqlCommand cmd = new SqlCommand("proc_Usuarios_ActivarDesactivar @RutProveedor, @estado", con);

        //    cmd.Parameters.AddWithValue("@rutProveedor", usuario.Rut);
        //    cmd.Parameters.AddWithValue("@estado", (usuario.Estado) ? 1:0);

        //    SqlDataReader reader;
        //    int i = 0;
        //     con.Open();
        //    try
        //    {
        //        i = cmd.ExecuteNonQuery();

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error: " + ex.ToString());
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }
        //    return i;
        //}
        public int Eliminar(string rut, int estado)
        {
            int i = 0;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("proc_Usuarios_Eliminar @Rut, @Estado", con))
                {
                    cmd.Parameters.AddWithValue("@Rut", rut);

                    cmd.Parameters.AddWithValue("@Estado", estado);
                    con.Open();
                    i = cmd.ExecuteNonQuery();
                }
            }
            return i;
        }
        public bool AuthorizationOLD(string rut, string rol)
        { 
            bool status = false;
            string sql = QryAuthorization;
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@rut", rut);
            cmd.Parameters.AddWithValue("@rol", rol);
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
                con.Close();
            }

            return status;
        }
        public bool Authorization(string rut, string programa)
        {

            bool autorizado;

            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("proc_Usuario_Authorization", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros de entrada
                    cmd.Parameters.AddWithValue("@Rut", rut);
                    cmd.Parameters.AddWithValue("@Programa", programa);

                    // Parámetro OUTPUT
                    SqlParameter pAutorizado = new SqlParameter("@Autorizado", SqlDbType.Bit);
                    pAutorizado.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(pAutorizado);

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    autorizado = Convert.ToBoolean(pAutorizado.Value);
                }
            }
            return autorizado;

        }
        public bool Authenticate(string Rut, string Pass)
        {

            bool autenticado;

            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["pnacpacam"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("proc_Usuario_Authenticate", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros de entrada
                    cmd.Parameters.AddWithValue("@Rut", Rut);
                    var encryptedBase64 = CryptoHelper.EncryptToBase64(Pass);
                    cmd.Parameters.AddWithValue("@Clave", encryptedBase64);

                    // Parámetro OUTPUT
                    SqlParameter pAutenticado = new SqlParameter("@Autenticado", SqlDbType.Bit);
                    pAutenticado.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(pAutenticado);

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    autenticado = Convert.ToBoolean(pAutenticado.Value);
                }
            }
            return autenticado;

        }

        private Usuario _set(SqlDataReader rd)
        {
            Usuario user = new Usuario();
            user.Rut = rd["Rut"].ToString();
            user.Estado = bool.Parse(rd["Estado"].ToString());
            user.Nombre = rd["Nombre"].ToString();
            user.Apellido = rd["Apellido"].ToString();
            user.Rol = rd["Rol"].ToString();
            user.Organismo = rd["Organismo"].ToString();
            return user;
        }

    }


    public class UsuarioViewModel
    {
        public string nombre { get; set; }
        public string rut { get; set; }

        public List<UsuarioViewModel> GetName(string name)
        {//sbritousuarios se activa mientras escribo el nombre del usuario
            string sql = "select rut, nombre+' '+apellido as nombre from funcionario where nombre like @name and estado = 1 ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["funcionario"].ConnectionString);
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
                con.Close();
            }

            return lista;
        }

    }


}