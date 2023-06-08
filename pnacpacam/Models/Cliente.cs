using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace pnacpacam.Models
{
    public class Cliente
    {
        public string id_usuario { get; set; }
        public string rut { get; set; }
        public string password { get; set; }
        public string idPerfil { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string afiliado { get; set; }
        public string activo { get; set; }
        public string fechaIngreso { get; set; }
        public string nombreUsuario { get; set; }
        public string email { get; set; }
        public string nombreCompleto { get; set; }
        public Cliente getCliente(string rut)
        {
            string sql = "SELECT [id_usuario],[rut],[password],[idPerfil],[nombre],[apellido],[afiliado],[activo],[fechaIngreso],[nombreUsuario],[email], [nombre]+' '+[apellido] as nombreCompleto FROM [dbo].[usuario] where [rut] = @rut";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@rut", rut);

            SqlDataReader reader;
            Cliente cli = null;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    cli = new Cliente();
                    cli.id_usuario = reader["id_usuario"].ToString();
                    cli.rut = reader["rut"].ToString();
                    cli.password = reader["password"].ToString();
                    cli.idPerfil = reader["idPerfil"].ToString();
                    cli.nombre = reader["nombre"].ToString();
                    cli.apellido = reader["apellido"].ToString();
                    cli.afiliado = reader["afiliado"].ToString();
                    cli.activo = reader["activo"].ToString();
                    cli.fechaIngreso = reader["fechaIngreso"].ToString();
                    cli.nombreUsuario = reader["nombreUsuario"].ToString();
                    cli.email = reader["email"].ToString();
                    cli.nombreCompleto = reader["nombreCompleto"].ToString();
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
            return cli;
        }

        public List<Cliente> getListCliente()
        {
            string sql = "SELECT [id_usuario],[rut],[password],[idPerfil],[nombre],[apellido],[afiliado],[activo],[fechaIngreso],[nombreUsuario],[email], [nombre]+' '+[apellido] as nombreCompleto FROM [dbo].[usuario]";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventario"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            Cliente cli = null;
            List<Cliente> listaClientes = new List<Cliente>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        cli = new Cliente();
                        cli.id_usuario = reader["id_usuario"].ToString();
                        cli.rut = reader["rut"].ToString();
                        cli.password = reader["password"].ToString();
                        cli.idPerfil = reader["idPerfil"].ToString();
                        cli.nombre = reader["nombre"].ToString();
                        cli.apellido = reader["apellido"].ToString();
                        cli.afiliado = reader["afiliado"].ToString();
                        cli.activo = reader["activo"].ToString();
                        cli.fechaIngreso = reader["fechaIngreso"].ToString();
                        cli.nombreUsuario = reader["nombreUsuario"].ToString();
                        cli.email = reader["email"].ToString();
                        cli.nombreCompleto = reader["nombreCompleto"].ToString();
                        listaClientes.Add(cli);
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
            return listaClientes;
        }

    }
}