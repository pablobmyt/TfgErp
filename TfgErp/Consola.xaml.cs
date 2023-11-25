using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Drawing;
using EO.Internal;
using static System.Net.Mime.MediaTypeNames;
using System.Management;


namespace TfgErp
{
    /// <summary>
    /// Lógica de interacción para Consola.xaml
    /// </summary>
    public partial class Consola : Window, IWindowWithIcon
    {
        private TextPointer hostPromptPosition;

        public Consola()
        {
            InitializeComponent();
            PrintPrompt();

        }
        private void consoleTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                HandleEnterKey();
                e.Handled = true;
            }
            else if ((e.Key == Key.Back || e.Key == Key.Delete) && !CanEditText())
            {
                e.Handled = true;
            }
        }
        private void HandleEnterKey()
        {
            string command = GetCurrentCommand();
            if (!string.IsNullOrWhiteSpace(command))
            {
                command = command.Trim();
                ExecuteCommand(command);
            }
            PrintPrompt();
        }
        private bool CanEditText()
        {
            return consoleTextBox.CaretPosition.CompareTo(hostPromptPosition) > 0;
        }

        private string GetCurrentCommand()
        {
            TextRange textRange = new TextRange(hostPromptPosition, consoleTextBox.Document.ContentEnd);
            return textRange.Text.Replace("host:¬\\ ", "").Trim();
        }

        private string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private void ExecuteCommand(string command)
        {
            var textRange = new TextRange(hostPromptPosition, consoleTextBox.Document.ContentEnd);
             command = textRange.Text.Trim();
            command = command.Trim().Replace("\n", "");

            // Separa el comando y los argumentos
            string[] commandParts = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            switch (commandParts[0])
            {
                case "cd":
                    if (commandParts.Length > 1)
                    {
                        ChangeDirectory(commandParts[1]);
                    }
                    else
                    {
                        PrintWorkingDirectory();
                    }
                    break;
                case "pwd":
                    PrintWorkingDirectory();
                    break;
                case "sysinfo":
                frintInfo();
                    break;
                case "dir":
                    ListDirectoryContents();
                    break;
                case "clear":
                    ClearTerminal();
                    break;
                case "conexiones":
                    getConections();
                    break;
                case "exit":
                    consoleTextBox.AppendText("\nHasta luego ;)");
                    exitApp();
                    break;
                default:
                    consoleTextBox.AppendText("\nComando no reconocido.");

                    break;
            }
            AppendText("\n", Colors.Transparent); // Agrega un salto de línea con texto transparente.

        }


        private void AppendText(string text, System.Windows.Media.Color color)
        {
            Run run = new Run(text)
            {
                Foreground = new SolidColorBrush(color)
            };

            if (consoleTextBox.Document.Blocks.LastBlock is Paragraph para)
            {
                para.Inlines.Add(run);
            }
            else
            {
                Paragraph paragraph = new Paragraph(run);
                consoleTextBox.Document.Blocks.Add(paragraph);
            }

            // Mueve el cursor al final después de agregar texto.
            MoveCursorToEnd();
        }
        private void MoveCursorToEnd()
        {
            consoleTextBox.CaretPosition = consoleTextBox.Document.ContentEnd;
            consoleTextBox.ScrollToEnd();
        }


        public void exitApp()
        {
            Thread.Sleep(2000);
            this.Close();
        }

        public void  getConections()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            consoleTextBox.AppendText("\nLista de conexiones activas:");
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    consoleTextBox.AppendText("\n" + ip.ToString());
                }
            }
        }

        private void AppendColoredText(string text, System.Windows.Media.Color color)
        {
            Run coloredText = new Run(text)
            {
                Foreground = new SolidColorBrush(color)
            };

            // Comprueba si el último bloque es un párrafo y agrega el texto allí
            if (consoleTextBox.Document.Blocks.LastBlock is Paragraph lastParagraph)
            {
                lastParagraph.Inlines.Add(coloredText);
            }
            else
            {
                // Si no hay un último párrafo, crea uno nuevo
                Paragraph paragraph = new Paragraph(coloredText);
                paragraph.Margin = new Thickness(0); // Remueve el espacio extra entre líneas
                consoleTextBox.Document.Blocks.Add(paragraph);
            }

            // Mueve el cursor al final después de agregar texto
            MoveCursorToEnd();
        }

        private void ChangeDirectory(string path)
        {
            string newPath = System.IO.Path.Combine(currentDirectory, path);
            if (Directory.Exists(newPath))
            {
                currentDirectory = newPath;
            }
            else
            {
                consoleTextBox.AppendText("\nDirectorio no encontrado: " + path);
            }


        }

        private void PrintWorkingDirectory()
        {
            consoleTextBox.AppendText("\n" + currentDirectory.Replace(@"C:\Users\pabli\source\repos\TfgErp\TfgErp\bin\Debug\net6.0-windows", ""));
        }

        private void frintInfo()
        {
            // ASCII art para el símbolo "¬"
            string asciiArt = @"
                                                                                
                        @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@,                   
                       /@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#                   
                       /@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#                   
                       /@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#                   
                        &@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#                   
                                               &@@@@@@@@@@@@#                   
                                               &@@@@@@@@@@@@#                   
                                               &@@@@@@@@@@@@#                   
                                                .%&&&&&&&&#.                   
";

            // Obteniendo información del sistema real
            string hostName = Dns.GetHostName();
            OperatingSystem osInfo = Environment.OSVersion;
            TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount);

            // Formateando el tiempo de actividad
            string uptimeString = string.Format("{0} days, {1} hours, {2} mins",
                                                uptime.Days, uptime.Hours, uptime.Minutes);

            // Consiguiendo la cantidad de paquetes (este ejemplo asume que son aplicaciones instaladas en Windows)
            int packageCount = GetPackageCount();

            // Obteniendo información adicional del sistema
            string shellVersion = GetShellVersion();
            string resolution = GetScreenResolution();
            string de = GetDesktopEnvironment();
            string wmTheme = GetWindowManagerTheme();

            // Construyendo la información para mostrar
            string info = asciiArt +
                          $"Host: {hostName}\n" +
                          $"OS: {osInfo.Platform} v{osInfo.Version}\n" +
                          $"Uptime: {uptimeString}\n" +
                          $"Packages: {packageCount} (pacman)\n" +
                          $"Shell: {shellVersion}\n" +
                          $"Resolution: {resolution}\n" +
                          $"DE: {de}\n" +
                          $"WM Theme: {wmTheme}\n";

            // Agregar la información al consoleTextBox
            consoleTextBox.AppendText(info);
        }

        private int GetPackageCount()
        {
            // Lógica para obtener la cantidad de aplicaciones instaladas en Windows
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registry_key))
            {
                return key.GetSubKeyNames().Length;
            }
        }

        private string GetShellVersion()
        {
            // Reemplazar con la lógica adecuada para obtener la versión del shell
            return "PowerShell 7.1";
        }

        private string GetScreenResolution()
        {
            // Obteniendo la resolución de pantalla actual
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("\\root\\cimv2", "SELECT * FROM Win32_VideoController");

            foreach (ManagementObject obj in searcher.Get())
            {
                return $"{obj["CurrentHorizontalResolution"]}x{obj["CurrentVerticalResolution"]}";
            }
            return "Unknown";
        }

        private string GetDesktopEnvironment()
        {
            // Reemplazar con la lógica adecuada para obtener el entorno de escritorio
            return "Windows Desktop";
        }

        private string GetWindowManagerTheme()
        {
            // Reemplazar con la lógica adecuada para obtener el tema del gestor de ventanas
            return "Aero";
        }

        private void PrintPrompt()
        {
            string HOST = "host:¬\\ ";
            string promptPath = currentDirectory.Replace(@"C:\Users\pabli\source\repos\TfgErp\TfgErp\bin\Debug\net6.0-windows", "");
            string[] pathParts = promptPath.Split('\\');

            // Agrega el texto del host en azul y el path en verde, con barras en blanco
            AppendColoredText(HOST, System.Windows.Media.Color.FromRgb(85, 255, 255));
            foreach (var part in pathParts)
            {
                AppendColoredText(part, System.Windows.Media.Color.FromRgb(85, 255, 85));
                AppendColoredText("", System.Windows.Media.Color.FromRgb(255, 255, 255));
            }
            hostPromptPosition = consoleTextBox.CaretPosition.GetPositionAtOffset(-1); // Actualiza la posición después de agregar el prompt
        
    }


        private void ListDirectoryContents()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(currentDirectory);
            foreach (var item in dirInfo.GetFileSystemInfos())
            {
                consoleTextBox.AppendText("\n" + item.Name);
            }
        }

        private void ClearTerminal()
        {

            consoleTextBox.Document.Blocks.Clear();


        }


        private void consoleTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            consoleTextBox.ScrollToEnd();
        }

        public ImageSource GetIcon()
        {
            return new BitmapImage(new Uri("https://upload.wikimedia.org/wikipedia/commons/thumb/5/51/Windows_Terminal_logo.svg/1024px-Windows_Terminal_logo.svg.png", UriKind.RelativeOrAbsolute));
        }

        public string GetTitle()
        {
            // Retorna el título de la ventana
            return "System Terminal";
        }
    }
}

