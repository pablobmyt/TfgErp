using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using Xceed.Wpf.Toolkit;

namespace TfgErp
{
    /// <summary>
    /// Lógica de interacción para Pintar.xaml
    /// </summary>
    public partial class Pintar : Window, IWindowWithIcon
    {
        public Pintar()
        {
            InitializeComponent();
            paintSurface.DefaultDrawingAttributes.Color = Colors.Black;
        }

        private void PencilButton_Click(object sender, RoutedEventArgs e)
        {
            paintSurface.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void EraserButton_Click(object sender, RoutedEventArgs e)
        {
            paintSurface.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorPicker colorPicker = new ColorPicker();
            colorPicker.SelectedColorChanged += (s, ev) =>
            {
                if (colorPicker.SelectedColor.HasValue)
                {
                    Color selectedColor = colorPicker.SelectedColor.Value;
                    paintSurface.DefaultDrawingAttributes.Color = selectedColor;
                }
            };

            Window window = new Window
            {
                Title = "Select Color",
                Content = colorPicker,
                SizeToContent = SizeToContent.WidthAndHeight,
            };
            window.ShowDialog();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                int margin = (int)paintSurface.Margin.Left;
                int width = (int)paintSurface.ActualWidth - margin * 2;
                int height = (int)paintSurface.ActualHeight - margin * 2;
                RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Default);
                rtb.Render(paintSurface);

                PngBitmapEncoder png = new PngBitmapEncoder();
                png.Frames.Add(BitmapFrame.Create(rtb));

                using (Stream fileStream = File.Create(saveFileDialog.FileName))
                {
                    png.Save(fileStream);
                }
            }
        }

        public ImageSource GetIcon()
        {
            return new BitmapImage(new Uri("https://cdn-icons-png.flaticon.com/512/6392/6392819.png", UriKind.RelativeOrAbsolute));
        }

        public string GetTitle()
        {
            // Retorna el título de la ventana
            return "Paint";
        }
    }
}
