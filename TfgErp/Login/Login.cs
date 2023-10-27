using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace TfgErp.Login
{
    internal class Login
    {
        static Conector conector = new Conector();

        static MySqlConnection connection = conector.GetConnection();


        public static bool iniciarSesion(String UserNameTextBox, String PasswordTextBox)
        {
            try
            {
                if (UserNameTextBox != "" && PasswordTextBox != "")
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(@"SELECT * FROM credentials WHERE usuario='" + UserNameTextBox + "' AND pass='" + Seguridad.Encriptar(PasswordTextBox) + "'", connection);
                    MySqlDataReader lector = cmd.ExecuteReader();
                    if (lector.Read())
                    {

                        return true;

                    }
                    else
                    {
                        MessageBox.Show("Compruebe las credenciales", "VRB_CMS");
                        return false;
                    }
                    connection.Close();

                }
            }
            catch (MySqlException a)
            {
                MessageBox.Show("Comprueba los datos al logearte");
            }
            return false;

        }
    }
}
