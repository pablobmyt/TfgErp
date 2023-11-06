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
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting;



namespace TfgErp
{
    /// <summary>
    /// Lógica de interacción para Coder.xaml
    /// </summary>
    public partial class Coder : Window, IWindowWithIcon
    {

        ScriptEngine engine = Python.CreateEngine();
        string textoInicial = @"def sumar_numeros(a, b):
    resultado = a + b
    return resultado

# Este es un metodo de prueba para el compilador de python, siempre se ha de devolver el resultado en la variable resultado.
resultado = sumar_numeros(3, 4)
";

        public Coder()
        {
            InitializeComponent();
            

            TextRange textRange = new TextRange(
                rtb1.Document.ContentStart,
                rtb1.Document.ContentEnd
            );

            textRange.Text = textoInicial;


        }

        public ImageSource GetIcon()
        {
            return new BitmapImage(new Uri("https://cdn-icons-png.flaticon.com/512/69/69045.png", UriKind.RelativeOrAbsolute));
        }

        public string GetTitle()
        {
            // Retorna el título de la ventana
            return "Coder by Vsoftware";
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                // Aqui se define el contenido del rtb
                TextRange textRange = new TextRange(
      rtb1.Document.ContentStart,
      rtb1.Document.ContentEnd
  );
                var engine = Python.CreateEngine();
                if (!string.IsNullOrEmpty(textRange.Text))
                {
                    var scope = engine.CreateScope();
                 var source =   engine.CreateScriptSourceFromString(textRange.Text);
                    var compilation = source.Compile();
                    var result = compilation.Execute(scope);
                    if (scope.ContainsVariable("resultado"))
                    {
                        var resultText = scope.GetVariable("resultado").ToString();
                        MessageBox.Show(resultText);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



    }
    }

