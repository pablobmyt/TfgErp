using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfgErp.DataBase
{
    public class ConexionBD
    {
        // Tus parámetros de conexión
        private static string strProvider = @"Server=mysql-95908-0.cloudclusters.net; Database=auth; Uid=auth; Pwd=vrbadmin; Port=10034";
        private MySqlConnection connection;
        private MySqlDataAdapter adapter;

        public ConexionBD()
        {
            connection = new MySqlConnection(strProvider);
        }

        // Método para abrir la conexión
        public void AbrirConexion()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                    Console.WriteLine("Conexión establecida con éxito.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al conectar con la base de datos: " + ex.Message);
            }
        }

        // Método para cerrar la conexión
        public void CerrarConexion()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    Console.WriteLine("Conexión cerrada con éxito.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cerrar la conexión: " + ex.Message);
            }
        }

        // Puedes agregar más métodos según lo que necesites hacer con tu base de datos.
    }
}
