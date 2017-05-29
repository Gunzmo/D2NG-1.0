using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace D2NG.UI
{
    /// <summary>
    /// Interaction logic for Hacks.xaml
    /// </summary>
    public partial class Hacks : Window
    {
        public delegate void _Send(string Data);
        public event _Send Send;
        public delegate void _Receive(string Data);
        public event _Receive Receive;
        public delegate void _RevealMap();
        public event _RevealMap RevealMap;
        static int StaicMaxWidth = 340;
        bool MinMaxing = false;
        public Hacks()
        {
            InitializeComponent();
            Hide();
            Width = 18;
        }

        private void button_Click(object sender, RoutedEventArgs e)
            { Send(textBox.Text); }

        private void button1_Click(object sender, RoutedEventArgs e)
            { Receive(textBox1.Text); }

        private void button2_Click(object sender, RoutedEventArgs e)
            { RevealMap(); }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                if (MinMaxing)
                    return;
                MinMaxing = true;
                Dispatcher.Invoke(() => { HackLabel.Visibility = Visibility.Hidden; });
                while (MinMaxing)
                {
                    Dispatcher.Invoke(() => {
                        if (Width > StaicMaxWidth)
                        {
                            Width = StaicMaxWidth;
                            MinMaxing = false;
                            return;
                        }
                        this.Width += 10;
                    });
                }
            }).Start();
        }
        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                if (MinMaxing)
                    return;
                MinMaxing = true;

                    Dispatcher.Invoke(() => { HackLabel.Visibility = Visibility.Visible; });
                    while (MinMaxing)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (Width <= 18)
                            {
                                Width = 18;
                                MinMaxing = false;
                                return;
                            }
                            if (Width < 18)
                                Width = 18;
                            else
                                this.Width -= 10;
                        });
                }

            }).Start();
        }
    }
}
