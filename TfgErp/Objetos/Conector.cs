using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfgErp.Login
{
    internal class Conector
    {
        public static string strProvider = @"Server=mysql-95908-0.cloudclusters.net; Database=auth; Uid=vrbadmin; Pwd=stigmadin125; Port=10034";
        String usuario, puntos, motores, reprimendas;
        MySqlConnection connection = new MySqlConnection(strProvider);

        public Conector()
        {

        }


        public void openConnection()
        {
            connection.Open();
        }

        public void closeConnection()
        {
            connection.Close();
        }

        public MySqlConnection GetConnection()
        {
            return connection;
        }
    }
}
