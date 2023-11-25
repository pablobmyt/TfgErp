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
using System.Windows.Threading;

namespace TfgErp
{
    /// <summary>
    /// Lógica de interacción para SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private string fullText = "Desarrollado por Pablo Matta con 💟";
        private int textIndex = 0;
        private DispatcherTimer typingTimer;
        public SplashScreen()
        {
            InitializeComponent();
            StartTypingAnimation();

        }

        private void StartTypingAnimation()
        {
            typingTimer = new DispatcherTimer();
            typingTimer.Interval = TimeSpan.FromMilliseconds(50); // Ajusta la velocidad según necesites
            typingTimer.Tick += TypingTimer_Tick;
            typingTimer.Start();
        }

        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            if (textIndex < fullText.Length)
            {
                animatedTextBlock.Text += fullText[textIndex];
                textIndex++;
            }
            else
            {
                typingTimer.Stop();
            }
        }
    }
}
