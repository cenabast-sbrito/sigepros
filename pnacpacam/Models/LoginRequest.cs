using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace pnacpacam
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        //NO ESTA EN USO
        public bool GetUsuario(string rut, string password)
        {

            string sql = "select * from inventarioUsuarios u inner join aa_ControlInsumos.dbo.funcionario f on f.rut = u.Rut where f.password = @password and u.Rut = @rut and u.Estado = 1 ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@rut", rut);
            cmd.Parameters.AddWithValue("@password", password);
            SqlDataReader reader;
            bool isValid = false;

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    isValid = true;

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

            return isValid;
        }
        public bool GetProveedor(string rut, string password)
        {

            string sql = "select RutProveedor from ClaveRegistroProveedor where RutProveedor = @Rut_Proveedor and PasswordApi = @PasswordApi ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString1"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Rut_Proveedor", rut);
            cmd.Parameters.AddWithValue("@PasswordApi", password);
            SqlDataReader reader;
            bool isValid = false;

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    isValid = true;

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

            return isValid;
        }

        public bool GetAdministrador(string rut, string password)
        {

            string sql = "select Rut from DistribucionUsuario where Rut = @Rut and Password = @Password ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Rut", rut);
            cmd.Parameters.AddWithValue("@Password", password);
            SqlDataReader reader;
            bool isValid = false;

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    isValid = true;

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

            return isValid;

        }

        public bool GetFuncionarios(string rut, string password)
        {

            string sql = "select Rut from DistribucionUsuario where Rut = @Rut and Password = @Password ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Rut", rut);
            cmd.Parameters.AddWithValue("@Password", password);
            SqlDataReader reader;
            bool isValid = false;

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    isValid = true;

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

            return isValid;


        }

    }
    public class Session
    {
        public string Username { get; set; }
        public string Status { get; set; }
    }
}