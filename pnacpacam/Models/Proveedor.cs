using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace pnacpacam.Models
{
    public class Proveedor
    {
        public string rut { get; set; }
        public string nombre { get; set; }
        public Proveedor getProveedor(string rut)
        {

/*
  string sql = "SELECT [LIFNR] rutProveedor, [NAME1] nombre " +
                         "  FROM [PZ_SISTEMA_NUEVOMODELO].[dbo].[LFA1] " +
                         " where rutProveedor = @rut WHERE LIFNR IN (SELECT DISTINCT )";*/

            string sql = "SELECT distinct c.LIFNR, NAME1, STRAS FROM " +
                     " (SELECT pc.LIFNR FROM [PNACPACAM].[dbo].[Despachos] d inner join " +
                     "                       [PZ_SISTEMA_NUEVOMODELO].[dbo].[EKKO_OC2] pc on (EBELN=[Pedido de Compra]) ) as c " +
                     " INNER JOIN [PZ_SISTEMA_NUEVOMODELO].[dbo].LFA1 lf ON (lf.LIFNR=c.LIFNR) where c.LIFNR = @rut";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["nuevoModelo"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@rut", rut);

            SqlDataReader reader;
            Proveedor prov = null;

            con.Open();
            try
            {
                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    prov = new Proveedor();
                    prov.rut = reader["rutProveedor"].ToString();
                    prov.nombre = reader["nombre"].ToString();
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
            return prov;
        }

        public List<Proveedor> getListProveedor()
        {
            /*
            string sql = "SELECT [LIFNR] rutProveedor, [NAME1] nombre " +
                         "  FROM [PZ_SISTEMA_NUEVOMODELO].[dbo].[LFA1]";
            */
            string sql = "SELECT distinct c.LIFNR rutProveedor, NAME1 nombre, STRAS direccion FROM " +
                         " (SELECT pc.LIFNR FROM [PNACPACAM].[dbo].[Despachos] d inner join " +
                         "                       [PZ_SISTEMA_NUEVOMODELO].[dbo].[EKKO_OC2] pc on (EBELN=[Pedido de Compra]) ) as c " +
                         " INNER JOIN [PZ_SISTEMA_NUEVOMODELO].[dbo].LFA1 lf ON (lf.LIFNR=c.LIFNR)";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["nuevoModelo"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader;
            Proveedor prov = null;
            List<Proveedor> listaproveedores = new List<Proveedor>();
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        prov = new Proveedor();
                        prov.rut = reader["rutProveedor"].ToString();
                        prov.nombre = reader["nombre"].ToString();

                        listaproveedores.Add(prov);
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
            return listaproveedores;
        }


      

    }

}