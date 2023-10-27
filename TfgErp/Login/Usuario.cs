using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfgErp.Login
{
    public class Usuario
    {
        public string NombreUsuario { get; set; }
        public string Contraseña { get; set; }
        public string Email { get; set; }
    }

    public class InicioSesion
    {
        private List<Usuario> usuariosRegistrados;
        private string sendGridApiKey = "TU_CLAVE_API_SENDGRID"; // Reemplaza con tu clave API de SendGrid

        public InicioSesion()
        {
            // Inicializamos una lista de usuarios registrados (puedes cargarla desde la base de datos si es necesario)
            usuariosRegistrados = new List<Usuario>();
        }

        public bool RegistrarUsuario(string nombreUsuario, string contraseña, string email)
        {
            // Verificar si el usuario ya está registrado por su nombre de usuario o email
            if (usuariosRegistrados.Any(u => u.NombreUsuario == nombreUsuario) || usuariosRegistrados.Any(u => u.Email == email))
            {
                return false; // El usuario ya existe
            }

            // Si el usuario no existe, lo registramos
            var nuevoUsuario = new Usuario
            {
                NombreUsuario = nombreUsuario,
                Contraseña = contraseña,
                Email = email
            };

            usuariosRegistrados.Add(nuevoUsuario);
            return true;
        }

        public async Task<bool> EnviarCorreoConfirmacion(string destinatarioEmail, string nombreUsuario)
        {
            var client = new SendGridClient(sendGridApiKey);

            var from = new EmailAddress("tu_email@dominio.com", "Tu Nombre");
            var subject = "Confirmación de registro";
            var to = new EmailAddress(destinatarioEmail, nombreUsuario);
            var plainTextContent = "¡Gracias por registrarte en nuestro sitio! Por favor, confirma tu correo electrónico.";
            var htmlContent = "<strong>¡Gracias por registrarte en nuestro sitio! Por favor, confirma tu correo electrónico.</strong>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            try
            {
                var response = await client.SendEmailAsync(msg);
                return response.StatusCode == System.Net.HttpStatusCode.Accepted;
            }
            catch (Exception ex)
            {
                // Maneja cualquier error de envío de correo aquí
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool IniciarSesion(string nombreUsuario, string contraseña)
        {
            // Verificar si el usuario y la contraseña coinciden con un usuario registrado
            var usuario = usuariosRegistrados.FirstOrDefault(u => u.NombreUsuario == nombreUsuario && u.Contraseña == contraseña);
            return usuario != null;
        }
    }
}
