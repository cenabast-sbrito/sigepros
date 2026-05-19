using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

using System.Linq;

namespace pnacpacam.Models
{
    public class Funcionario
    {
        public int Id_funcionario { get; set; }
        public string Rut { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Anexo { get; set; }
        public string Email { get; set; }
        public string NombreUsuario { get; set; }
        public int CodigoDpto { get; set; }
        public int CodigoSubDpto { get; set; }
        public int CodigoUnidad { get; set; }
        public int IdCargo { get; set; }
        public int Estado { get; set; }

        public Funcionario() { }

        public Funcionario GetFuncionario(string Rut)
        {
            Funcionario funcionario = null;
            string sql = "SELECT id_funcionario, rut, password, nombre, apellido, anexo, email, nombreUsuario, codigoDpto, codigoSubDpto, codigoUnidad, idCargo, estado FROM funcionario where rut = @Rut;";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["funcionario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@Rut", Rut);
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    funcionario = new Funcionario();
                    // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                    funcionario.Id_funcionario = int.Parse(reader["id_funcionario"].ToString());
                    funcionario.Rut = reader["rut"].ToString();
                    funcionario.Password = reader["password"].ToString();
                    funcionario.Nombre = reader["nombre"].ToString();
                    funcionario.Apellido = reader["apellido"].ToString();
                    funcionario.Email = reader["email"].ToString();
                    funcionario.NombreUsuario = reader["nombreUsuario"].ToString();
                    funcionario.CodigoDpto = int.Parse(reader["codigoDpto"].ToString());
                    funcionario.CodigoSubDpto = int.Parse(reader["codigoSubDpto"].ToString());
                    funcionario.CodigoUnidad = int.Parse(reader["codigoUnidad"].ToString());
                    funcionario.IdCargo = int.Parse(reader["idCargo"].ToString());
                    funcionario.Estado = int.Parse(reader["estado"].ToString());
                    //funcionario.Anexo = int.Parse(reader["anexo"].ToString());
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
            return funcionario;
        }

        public List<Funcionario> GetFuncionarioByCodigo(int Codigo)
        {
            // se agrega una fila 'OTRO FUNCIONARIO' la cual permitira al gestopr definir un usuario que no se encuentra en la lista oficial
            Funcionario funcionario = null;
            List<Funcionario> funcionarios = new List<Funcionario>();
            string sql = "SELECT id_funcionario, rut, password, nombre, apellido, anexo, email, nombreUsuario, codigoDpto, codigoSubDpto, codigoUnidad, idCargo, estado FROM funcionario where codigoDpto = @Codigo or codigoSubDpto = @Codigo or codigoUnidad = @Codigo " +
                " union " +
                " select 9999 as id_funcionario, 9999 as rut, '' as password, 'OTRO FUNCIONARIO' as nombre, '' as apellido, 0 as anexo, '' as email, '' as nombreUsuario, 9999 as codigoDpto, 9999 as codigoSubDpto, 9999 as codigoUnidad, 9999 as idCargo, 1 as estado" +
                ";";


            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["secondary"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@Codigo", Codigo);
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        funcionario = new Funcionario();
                        // Defino los Estados para los elementos de mi Formulario cuando hago click en el botón Buscar.
                        funcionario.Id_funcionario = int.Parse(reader["id_funcionario"].ToString());
                        funcionario.Rut = reader["rut"].ToString();
                        funcionario.Password = reader["password"].ToString();
                        funcionario.Nombre = reader["nombre"].ToString();
                        funcionario.Apellido = reader["apellido"].ToString();
                        funcionario.Anexo = reader["anexo"] == DBNull.Value ? 0 : int.Parse(reader["anexo"].ToString());
                        funcionario.Email = reader["email"].ToString();
                        funcionario.NombreUsuario = reader["nombreUsuario"] == DBNull.Value ? "NULL" : reader["nombreUsuario"].ToString();
                        funcionario.CodigoDpto = int.Parse(reader["codigoDpto"].ToString());
                        funcionario.CodigoSubDpto = int.Parse(reader["codigoSubDpto"].ToString());
                        funcionario.CodigoUnidad = int.Parse(reader["codigoUnidad"].ToString());
                        funcionario.IdCargo = int.Parse(reader["idCargo"].ToString());
                        funcionario.Estado = int.Parse(reader["estado"].ToString());
                        funcionarios.Add(funcionario);
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
            return funcionarios;
        }

        public List<string> GetEmailFuncionario(List<string> listaNotificacion)
        {
            List<string> listaEmail = new List<string>();
            List<string> aux = listaNotificacion.Distinct().ToList();
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["secondary"].ConnectionString);

            var parameters = new string[aux.Count];
            SqlCommand cmd = new SqlCommand();

            for (int i = 0; i < aux.Count; i++)
            {
                parameters[i] = string.Format("@Rut{0}", i);
                cmd.Parameters.AddWithValue(parameters[i], aux[i]);
            }
            cmd.CommandText = string.Format("SELECT email FROM funcionario WHERE rut IN ({0})", string.Join(", ", parameters));
            cmd.Connection = con;
            con.Open();
            SqlDataReader reader;

            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    listaEmail = new List<string>();
                    while (reader.Read())
                    {
                        listaEmail.Add(reader["email"].ToString().Trim());
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

            return listaEmail;

        }

        public bool Authenticate(string Rut, string Pass)
        {

            bool status = false;
            string sql = "SELECT id_funcionario from funcionario where rut = @Rut and password = @Pass;";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["funcionario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@Rut", Rut);
            cmd.Parameters.AddWithValue("@Pass", Pass);
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
        public static string GetNombreCompleto(string Rut)
        {
            string sql = "SELECT nombre+' '+apellido as NombreCompleto FROM funcionario where rut = @Rut;";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["secondary"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            string nombreCompleto = " ";
            try
            {
                con.Open();
                cmd.Parameters.AddWithValue("@Rut", Rut);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    nombreCompleto = reader["NombreCompleto"].ToString();
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
            return nombreCompleto;
        }
        public static string GetCorreo(string Rut)
        {
            string sql = "SELECT email as email FROM funcionario where rut = @Rut;";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["secondary"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            string email = " ";
            try
            {
                con.Open();
                cmd.Parameters.AddWithValue("@Rut", Rut);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    email = reader["email"].ToString();
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
            return email;
        }
        public string PutFuncionario(Funcionario funcionario)
        {
            string res = "";
            int i = 0;
            string sql = "INSERT INTO dbo.funcionario ( rut, password, nombre, apellido, anexo, email, nombreUsuario, codigoDpto, codigoSubDpto, codigoUnidad, idCargo, estado) VALUES ( @rut, @password, @nombre, @apellido, @anexo, @email, @nombreUsuario, @codigoDpto, @codigoSubDpto, @codigoUnidad, @idCargo, @estado )";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["secondary"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);

            string emailFuncionario = funcionario.Email + "@cenabast.cl";

            cmd.Parameters.AddWithValue("@rut", funcionario.Rut);
            cmd.Parameters.AddWithValue("@nombre", funcionario.Nombre);
            cmd.Parameters.AddWithValue("@apellido", funcionario.Apellido);
            cmd.Parameters.AddWithValue("@email", emailFuncionario);

            cmd.Parameters.AddWithValue("@password", DBNull.Value);//, funcionario.Password);
            cmd.Parameters.AddWithValue("@anexo", DBNull.Value); //, funcionario.Anexo);
            cmd.Parameters.AddWithValue("@nombreUsuario", DBNull.Value); //, funcionario.NombreUsuario);
            cmd.Parameters.AddWithValue("@codigoDpto", DBNull.Value); //, funcionario.CodigoDpto);
            cmd.Parameters.AddWithValue("@codigoSubDpto", DBNull.Value); //, funcionario.CodigoSubDpto);
            cmd.Parameters.AddWithValue("@codigoUnidad", DBNull.Value); //, funcionario.CodigoUnidad);
            cmd.Parameters.AddWithValue("@idCargo", DBNull.Value); //, funcionario.IdCargo);

            cmd.Parameters.AddWithValue("@estado", 1);
            con.Open();
            try
            {
                i = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {

                Console.WriteLine("Error: " + ex.ToString());

                if (ex.Number == 2627)
                {
                    res = "Funcionario Ya Existe";
                }
                else
                {
                    res = "Ha ocurrido un error : SqlNumber " + ex.Number;
                }
                i = 0;

            }
            finally
            {
                con.Close();
            }

            return res;

        }
    }



}