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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TfgErp
{
    /// <summary>
    /// Lógica de interacción para ImageWithUrl.xaml
    /// </summary>
    public partial class ImageWithUrl : UserControl
    {
        public string Url { get; set; }

        public ImageWithUrl()
        {
            InitializeComponent();
        }

        public void SetImage(BitmapImage bitmap)
        {
            image.Source = bitmap;
        }
    }
}
