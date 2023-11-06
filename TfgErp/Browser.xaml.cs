using System;
using System.Collections.Generic;
using System.Linq;
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

namespace TfgErp
{
    /// <summary>
    /// Lógica de interacción para Browser.xaml
    /// </summary>
    public partial class Browser : Window, IWindowWithIcon
    {
        public Browser()
        {
            InitializeComponent();

        }

        public ImageSource GetIcon()
        {
            return new BitmapImage(new Uri("https://cdn-icons-png.flaticon.com/512/6392/6392819.png", UriKind.RelativeOrAbsolute));
        }

        public string GetTitle()
        {
            // Retorna el título de la ventana
            return "Browser by Vsoftware";
        }
    }
}
