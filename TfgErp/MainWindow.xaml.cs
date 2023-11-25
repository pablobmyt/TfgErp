﻿using EO.WebBrowser;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TfgErp.Meteo;
using Path = System.IO.Path;

namespace TfgErp
{


    public partial class MainWindow : Window
    {
        private int nextRow = 0;
        private int nextCol = 0;
        private static string TERMINAL_TITLE = "System Terminal";
        public double FullScreenWidth { get; set; }
        public double FullScreenHeight { get; set; }



        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            initializeMethods();

            FullScreenWidth = SystemParameters.FullPrimaryScreenWidth;
            FullScreenHeight = SystemParameters.FullPrimaryScreenHeight;


        }

        private void initializeMethods()
        {
            MainGrid.MouseRightButtonDown += Grid_MouseRightButtonDown;
            SetRandomBackground();
            textBox.Visibility = Visibility.Hidden;
            menuInicioSesion.IsOpen = true;
            fondoOscurecido.Visibility = Visibility.Visible;
            panelClima.IsEnabled = false;
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            HourText1.Text = DateTime.Now.ToString("HH:mm");
            HourText2.Text = DateTime.Now.ToString("HH:mm");

        }


        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            shutdownPopup.IsOpen = true;
        }

        private void ShutdownSystem_Click(object sender, RoutedEventArgs e)
        {
            shutdownAnimationPopup.IsOpen = true;

            CreateLoadingDots();

            AnimateText();

            StartLoadingAnimation();

            var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                Application.Current.Shutdown();
            };
            timer.Start();
        }

        private void CreateLoadingDots()
        {
            int dotsCount = 8;
            double radius = 40;
            for (int i = 0; i < dotsCount; i++)
            {
                double angle = i * Math.PI * 2 / dotsCount;
                double x = radius + radius * Math.Cos(angle);
                double y = radius + radius * Math.Sin(angle);

                var dot = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.White
                };

                Canvas.SetLeft(dot, x);
                Canvas.SetTop(dot, y);
                loadingCanvas.Children.Add(dot);
            }
        }


        private void StartLoadingAnimation()
        {
            var rotateTransform = new RotateTransform();
            loadingCanvas.RenderTransform = rotateTransform;
            loadingCanvas.RenderTransformOrigin = new Point(0.5, 0.5);

            var rotateAnimation = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(1)))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };

            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
        }


        private void AnimateText()
        {
            int count = 0;
            var animationTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
            animationTimer.Tick += (s, args) =>
            {
                shutdownText.Text = "Apagando Equipo" + new string('.', count);
                count = (count + 1) % 4;
            };
            animationTimer.Start();
        }


        private void CloseSession_Click(object sender, RoutedEventArgs e)
        {
            
            shutdownPopup.IsOpen = false;
        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            CreateAndShowContextMenu(grid);
        }

        private void CreateAndShowContextMenu(Grid grid)
        {
            // Comprobar si el Grid ya tiene un menú contextual
            if (grid.ContextMenu == null)
            {
                var contextMenu = new System.Windows.Controls.ContextMenu();

                foreach (IWindowWithIcon item in getWindowsInstances())
                {
                   if(item.GetTitle() == TERMINAL_TITLE)
                    {
                        var menuItem = new System.Windows.Controls.MenuItem { Header = item.GetTitle() };
                        menuItem.Click += MenuItem_Click; // Añadir manejador de eventos
                        contextMenu.Items.Add(menuItem);

                    }

                }

                // Asignar el nuevo menú contextual al grid
                grid.ContextMenu = contextMenu;

                // Mostrar el menú contextual
                grid.ContextMenu.PlacementTarget = grid;
                grid.ContextMenu.IsOpen = true;
            }
        }






        private List<IWindowWithIcon?> getWindowsInstances()
        {
            List<IWindowWithIcon?> windows = new List<IWindowWithIcon?>();

            var windowTypes = Assembly.GetExecutingAssembly().GetTypes()
                                      .Where(t => typeof(IWindowWithIcon).IsAssignableFrom(t)
                                               && !t.IsAbstract
                                               && t != typeof(MainWindow)); // Excluir MainWindow

            foreach (Type windowType in windowTypes)
            {
                // Evitar instanciar la MainWindow.
                if (windowType.Name == "MainWindow")
                    continue;

                var windowInstance = (IWindowWithIcon)Activator.CreateInstance(windowType);
                windows.Add(windowInstance);
               

               

            }
            return windows;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem clickedItem = sender as System.Windows.Controls.MenuItem;

            if (clickedItem != null)
            {
                string windowTitle = clickedItem.Header.ToString();

                Window? windowToOpen = (Window)getWindowsInstances().FirstOrDefault(window => window.GetTitle().Equals(windowTitle));

                // Si la ventana existe, mostrarla
                if (windowToOpen != null)
                {
                    windowToOpen.Show(); 
                }
                else
                {
                    MessageBox.Show("Ventana no encontrada.");
                }
            }
        }

        private void MostrarClima(object sender, RoutedEventArgs e)
        {
            loadInfo();
            panelClima.Visibility = Visibility.Visible;
        }

        // Método para ocultar el panel "panelClima" al retirar el ratón del cuadro de texto "cuadroClima"
        private void OcultarClima(object sender, RoutedEventArgs e)
        {
            panelClima.Visibility = Visibility.Collapsed;
        }



        private void loadInfo()
        {
            // Ejecuta la operación asincrónica en un hilo separado
            Task.Run(async () =>
            {
                ClimaInfo climaInfo = await ClimaInfo.ObtenerDatosClimaticosAsync();

                // Actualiza la interfaz de usuario en el hilo de la interfaz de usuario
                Dispatcher.Invoke(() =>
                {
                    if (climaInfo != null)
                    {
                        // Actualiza los valores en las etiquetas existentes
                        LatitudText.Text = climaInfo.Latitude.ToString();
                        LongitudText.Text = climaInfo.Longitude.ToString();
                        TimeZoneText.Text = climaInfo.TimeZone;
                        ElevationText.Text = climaInfo.Elevation;

                        // Actualiza los nuevos campos de hora y temperatura
                        HourText.Text = DateTime.Now.ToString("HH:mm"); // Cambia esto por la hora real
                        TemperatureText.Text = climaInfo.HourlyTemperature[0].ToString() + "ºC"; // Supongo que quieres mostrar la primera temperatura del array

                    }
                    else
                    {
                        MessageBox.Show("No se pudo obtener la información climática.");
                    }
                });
            });
        }

        // Método para mostrar el menú de inicio de sesión y oscurecer el fondo
        private void MostrarMenuInicioSesion(object sender, RoutedEventArgs e)
        {
            menuInicioSesion.IsOpen = true;
            fondoOscurecido.Visibility = Visibility.Visible;
        }

        
        // Método para ocultar el menú de inicio de sesión y restaurar el fondo
        private void OcultarMenuInicioSesion()
        {
            menuInicioSesion.IsOpen = false;
            fondoOscurecido.Visibility = Visibility.Collapsed;
            HourText1.Visibility = Visibility.Hidden;
            textBox.Visibility = Visibility.Visible;
            toolBar.Visibility = Visibility.Visible;
            blurContainer.Effect = null;
        }

        // Evento de clic en el botón "Iniciar Sesión"
        private void IniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            if (Login.Login.iniciarSesion(userNameText.Text, passWordText.Password))
            {
                OcultarMenuInicioSesion();
            }


        }

        private void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            var stackPanel = new StackPanel
            {
                Width = 400,
                Height = 200,
                Background = Brushes.White
            };

            var windowTypes = Assembly.GetExecutingAssembly().GetTypes()
                                      .Where(t => typeof(IWindowWithIcon).IsAssignableFrom(t)
                                               && !t.IsAbstract
                                               && t != typeof(MainWindow)); // Excluir MainWindow

            foreach (Type windowType in windowTypes)
            {
                if (windowType.Name == "MainWindow")
                    continue;

                var windowInstance = (IWindowWithIcon)Activator.CreateInstance(windowType);

                var windowButton = new Button
                {
                    Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                {
                    new Image
                    {
                        Source = windowInstance.GetIcon(),
                        Width = 16,
                        Height = 16,
                        Margin = new Thickness(0, 0, 5, 0)
                    },
                    new TextBlock {
                        Text = windowInstance.GetTitle(),
                        FontSize = 12
                    }
                }
                    },
                    Margin = new Thickness(5),
                    // Estilos personalizados aquí
                    Background = new LinearGradientBrush(Colors.LightBlue, Colors.SlateBlue, 45.0),
                    Style = (Style)FindResource("RoundedButton"),
                    Padding = new Thickness(10)
                };

                // Animaciones al pasar el ratón (opcional)
                windowButton.MouseEnter += (s, args) => windowButton.Background = Brushes.LightSkyBlue;
                windowButton.MouseLeave += (s, args) => windowButton.Background = new LinearGradientBrush(Colors.LightBlue, Colors.SlateBlue, 45.0);

                windowButton.Click += (s, args) =>
                {
                    if (windowType.Name != "MainWindow")
                    {
                        var window = (Window)windowInstance;
                        window.Show();
                    }
                };

                stackPanel.Children.Add(windowButton);
            }

            var popup = new Popup
            {
                PlacementTarget = sender as UIElement,
                Placement = PlacementMode.Top,
                StaysOpen = false,
                Child = stackPanel
            };

            popup.IsOpen = true;
        }





        public static string FormatUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("La URL proporcionada es inválida.");
            }

            if (!url.StartsWith("https://"))
            {
                if (url.StartsWith("www."))
                {
                    url = "https://" + url;
                }
                else
                {
                    url = "https://www." + url;
                }
            }
            else if (!url.StartsWith("https://www."))
            {
                url = "https://www." + url.Substring(8);
            }

            return url;
        }

        private void OnAddImageButtonClick(object sender, RoutedEventArgs e)
        {

            string url = textBox.Text;
            if (!string.IsNullOrEmpty(url))
            {
                if (nextRow < 4 && nextCol < 4)
                {
                    LoadFavicon(url, nextRow, nextCol);

                    nextCol++;
                    if (nextCol >= 4)
                    {
                        nextCol = 0;
                        nextRow++;
                    }
                }
                else
                {
                    MessageBox.Show("La parrilla está llena.");
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingresa una URL.");
            }
        }

        private void SetRandomBackground()
        {
            
            var assembly = Assembly.GetExecutingAssembly();

            var resourceNames = assembly.GetManifestResourceNames();
            string imagesFolder = "TfgErp.Imagenes.Fondos"; 
            var images = resourceNames.Where(name => name.StartsWith(imagesFolder) && name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)).ToList();

            if (images.Count == 0)
                return;

            Random rnd = new Random();
            string selectedResourceName = images[rnd.Next(images.Count)];

            var resourceStream = assembly.GetManifestResourceStream(selectedResourceName);
            if (resourceStream != null)
            {
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = resourceStream;
                imageSource.EndInit();

                ImageBrush brush = new ImageBrush
                {
                    ImageSource = imageSource
                };

                this.Background = brush;
            }
        }



        private void LoadFavicon(string url, int row, int col)
        {
            try
            {
                var client = new WebClient();
                var uri = new Uri(new Uri(FormatUrl(url)), "/favicon.ico");
                var faviconUrl = uri.AbsoluteUri;
                var stream = client.OpenRead(faviconUrl);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.EndInit();

                var imageWithUrl = new ImageWithUrl
                {
                    Url = FormatUrl(url)
                };
                imageWithUrl.SetImage(bitmap);

                imageWithUrl.MouseLeftButtonUp += (sender, e) =>
                {
                    var browserWindow = new Browser();
                    browserWindow.webBrowser.Navigate(((ImageWithUrl)sender).Url);
                    browserWindow.Show();
                };

                Grid.SetRow(imageWithUrl, row);
                Grid.SetColumn(imageWithUrl, col);
                mainGrid.Children.Add(imageWithUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el favicon: " + ex.Message);
            }
        }
        }
}


