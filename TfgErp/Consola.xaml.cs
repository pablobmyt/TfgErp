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

namespace TfgErp
{
    /// <summary>
    /// Lógica de interacción para Consola.xaml
    /// </summary>
    public partial class Consola : Window, IWindowWithIcon
    {
        public Consola()
        {
            InitializeComponent();
            PrintPrompt();

        }
        private void consoleTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Divide el texto en líneas
                var lines = consoleTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                // Encuentra la última línea no vacía que será el comando
                var lastCommand = lines.LastOrDefault(line => !string.IsNullOrWhiteSpace(line));

                if (!string.IsNullOrWhiteSpace(lastCommand))
                {
                    lastCommand = lastCommand.Replace("host:¬\\", "").Trim();

                    ExecuteCommand(lastCommand);
                }

                // Mueve el cursor al final del texto y añade una nueva línea
                consoleTextBox.CaretIndex = consoleTextBox.Text.Length;
                consoleTextBox.AppendText(Environment.NewLine);

                // Muestra el prompt para el próximo comando
                PrintPrompt();

                e.Handled = true;
            }
        }


        private string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private void ExecuteCommand(string command)
        {
            
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
            consoleTextBox.AppendText("\n" + @"
                    -/oyddmdhs+:.                pablo@nimbusos
               -odNMMMMMMMMNNmhy+-`             ----------------
             -yNMMMMMMMMMMMNNNmmdhy+-           OS: NimbusOS v1.0
           `omMMMMMMMMMMMMNmdmmmmddhhy/`        Host: Nimbus Cloud Compute v42
          omMMMMMMMMMMMNhhyyyohmdddhhhdo`       Kernel: 5.4.23-nimbus-lts
         .ydMMMMMMMMMMdhs++so/smdddhhhhdm+`     Uptime: 1 day, 4 hours, 37 mins
        oyhdmNMMMMMMMNdyooydmddddhhhhyhNMs.     Packages: 1024 (pacman)
       :omdddhhhhdmMNhhhhhhhdmNNNmhhhyymMh.     Shell: bash 5.0.16
       .ydmmddmmdmNMNdmmmmddmNMNmdmmdyhhdm.     Resolution: 1920x1080
        :dmmmmdmmdddmNNNNmmdmNNmdddmmdyhs:      DE: NimbusDE
         /dmmmmdmddmddNNNNmmmNNmdddmmdyo.       WM: NimbusWM
           /dmmmmdmmdmddNNNNNNmmdmmdmNh-        WM Theme: Nimbus
            `+ydmmmdmmdmddNNNNmmdmmdyo:         Theme: Nimbus-Light [GTK2/3]
               ./ymmdmmdmmdmddmmdys+:`          Icons: Nimbus [GTK2/3]
                   `.-/+oossoo+/-`              Terminal: nimbus-terminal

");
        }

        private void PrintPrompt()
        {
            string HOST = "host" + ":¬";
            string promptPath = currentDirectory.Replace(@"C:\Users\pabli\source\repos\TfgErp\TfgErp\bin\Debug\net6.0-windows", "");
            string modifiedPrompt = promptPath.Replace("\\", "/");



            consoleTextBox.AppendText(HOST + promptPath);
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

            consoleTextBox.Text = string.Empty;
            

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

