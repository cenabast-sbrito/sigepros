// NO EXISTEN REFERENCIAS A ESTE MODELO
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace pnacpacam.Models
{
    public class UsuarioInsumo
    {
        public int Id_funcionario { get; set; }
        [Required]
        public string Rut { get; set; }       ///[varchar](10) 
		[Required]
        public string Password { get; set; }       ///[varchar](50) 
		public string Nombre { get; set; }      ///[varchar](100)
		public string Apellido { get; set; }       ///[varchar](100)
		public int Anexo { get; set; }
        public string Email { get; set; }       ///[varchar](50)
		public string NombreUsuario { get; set; }       ///[varchar](50)
		public int CodigoDpto { get; set; }
        public int CodigoSubDpto { get; set; }
        public int CodigoUnidad { get; set; }
        public int IdCargo { get; set; }
        public int Estado { get; set; }


        public int IngresarUsuarioInsumo(UsuarioInsumo user)
        {
            int i = 0;
            string sql = "INSERT INTO dbo.funcionario ( rut, password, nombre, apellido, anexo, email, nombreUsuario, codigoDpto, codigoSubDpto, codigoUnidad, idCargo, estado) VALUES ( @rut, @password, @nombre, @apellido, @anexo, @email, @nombreUsuario, @codigoDpto, @codigoSubDpto, @codigoUnidad, @idCargo, @estado )";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["secondary"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);


//            cmd.Parameters.AddWithValue("@id_funcionario", user.Id_funcionario); 
            cmd.Parameters.AddWithValue("@rut", user.Rut);
            cmd.Parameters.AddWithValue("@password", user.Password);
            cmd.Parameters.AddWithValue("@nombre", user.Nombre);
            cmd.Parameters.AddWithValue("@apellido", user.Apellido);
            cmd.Parameters.AddWithValue("@anexo", user.Anexo);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@nombreUsuario", user.NombreUsuario);
            cmd.Parameters.AddWithValue("@codigoDpto", user.CodigoDpto);
            cmd.Parameters.AddWithValue("@codigoSubDpto", user.CodigoSubDpto);
            cmd.Parameters.AddWithValue("@codigoUnidad",  user.CodigoUnidad);
            cmd.Parameters.AddWithValue("@idCargo", user.IdCargo);
            cmd.Parameters.AddWithValue("@estado", user.Estado);
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
        public UsuarioInsumo GetUsuarioInsumo(string Rut)
        {
            string sql = "select id_funcionario, rut, password, nombre, apellido, anexo, email, nombreUsuario, codigoDpto, codigoSubDpto, codigoUnidad, idCargo, estado from dbo.funcionario where rut = @Rut";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["secondary"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Rut", Rut);
            SqlDataReader reader;
            UsuarioInsumo user = null;

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    user = new UsuarioInsumo();

                    user.Id_funcionario = int.Parse(reader["id_funcionario"].ToString());
                    user.Rut = reader["rut"].ToString();
                    user.Password = reader["password"].ToString();
                    user.Nombre = reader["nombre"].ToString();
                    user.Apellido = reader["apellido"].ToString();
                    user.Anexo = int.Parse(reader["anexo"].ToString());
                    user.Email = reader["email"].ToString();
                    user.NombreUsuario = reader["nombreUsuario"].ToString();
                    user.CodigoDpto = int.Parse(reader["codigoDpto"].ToString());
                    user.CodigoSubDpto = int.Parse(reader["codigoSubDpto"].ToString());
                    user.CodigoUnidad = int.Parse(reader["codigoUnidad"].ToString());
                    user.IdCargo = int.Parse(reader["idCargo"].ToString());
                    user.Estado = int.Parse(reader["estado"].ToString());

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
        public List<UsuarioInsumo> ObtenerUsuariosInsumo()
        {
            string sql = "select id_funcionario, rut, password, nombre, apellido, anexo, email, nombreUsuario, codigoDpto, codigoSubDpto, codigoUnidad, idCargo, estado from dbo.funcionario";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["secondary"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            List<UsuarioInsumo> lista = new List<UsuarioInsumo>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    UsuarioInsumo user = new UsuarioInsumo();

                    user.Id_funcionario = int.Parse(reader["id_funcionario"].ToString());
                    user.Rut = reader["rut"].ToString();
                    user.Password = reader["password"].ToString();
                    user.Nombre = reader["nombre"].ToString();
                    user.Apellido = reader["apellido"].ToString();
                    user.Anexo = int.Parse(reader["anexo"].ToString());
                    user.Email = reader["email"].ToString();
                    user.NombreUsuario = reader["nombreUsuario"].ToString();
                    user.CodigoDpto = int.Parse(reader["codigoDpto"].ToString());
                    user.CodigoSubDpto = int.Parse(reader["codigoSubDpto"].ToString());
                    user.CodigoUnidad = int.Parse(reader["codigoUnidad"].ToString());
                    user.IdCargo = int.Parse(reader["idCargo"].ToString());
                    user.Estado = int.Parse(reader["estado"].ToString());
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
        public int ActualizarUsuarioInsumo(UsuarioInsumo user)
        {
            int i = 0;
            string sql = "update dbo.funcionario set " +
                "id_funcionario = @id_funcionario, rut = @rut, password = @password, nombre = @nombre, apellido = @apellido, anexo = @anexo, email = @email, nombreUsuario = @nombreUsuario, codigoDpto = @codigoDpto, codigoSubDpto = @codigoSubDpto, codigoUnidad = @codigoUnidad, idCargo = @idCargo, estado = @estado" +
                " where id_funcionario = @id_funcionario and rut = @rut ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["secondary"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@id_funcionario", user.Id_funcionario);
            cmd.Parameters.AddWithValue("@rut", user.Rut);
            cmd.Parameters.AddWithValue("@password", user.Password);
            cmd.Parameters.AddWithValue("@nombre", user.Nombre);
            cmd.Parameters.AddWithValue("@apellido", user.Apellido);
            cmd.Parameters.AddWithValue("@anexo", user.Anexo);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@nombreUsuario", user.NombreUsuario);
            cmd.Parameters.AddWithValue("@codigoDpto", user.CodigoDpto);
            cmd.Parameters.AddWithValue("@codigoSubDpto", user.CodigoSubDpto);
            cmd.Parameters.AddWithValue("@codigoUnidad", user.CodigoUnidad);
            cmd.Parameters.AddWithValue("@idCargo", user.IdCargo);
            cmd.Parameters.AddWithValue("@estado", user.Estado);

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



    }
}
