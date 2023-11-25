using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TfgErp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);



            Task.Run(() =>
            {
              
                Thread.Sleep(6000); //8000 es la medida perfecta, se baja duracion para agilizar desarrollo

              
                Dispatcher.Invoke(() =>
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show(); // Muestra la ventana principal.
                    Application.Current.MainWindow.Close();
                });
            });

        }

    }


}
