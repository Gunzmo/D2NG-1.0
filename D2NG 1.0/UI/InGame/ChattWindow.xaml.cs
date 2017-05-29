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
    /// Interaction logic for ChattWindow.xaml
    /// </summary>
    public partial class ChattWindow : Window
    {
        bool minMax = false;

        public delegate void _SendMSG(string MSG);
        public event _SendMSG SendMSG;

        public ChattWindow()
        {
            InitializeComponent();
            this.Height = 56;
            this.Width = 21;
        }


        public void ClearChat() { Dispatcher.Invoke(() => { richTextBox.Document.Blocks.Clear(); }); }
        public void AddMessage(string Message)
            { Dispatcher.Invoke(() => { richTextBox.Document.Blocks.Add(new Paragraph(new Run(Message)) { LineHeight = 1 }); richTextBox.ScrollToEnd(); }); }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SendMSG(textBox.Text);
                textBox.Text = "";
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox.Text == "/r" && D2NGMainWindow.LastWisper != "")
            {
                textBox.Text = "/w " + D2NGMainWindow.LastWisper + " ";
                textBox.SelectionStart = textBox.Text.Length;

            }
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                Dispatcher.Invoke(() => { ChatLabel.Visibility = Visibility.Hidden; });
                while (minMax)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (Width >= 290 && Height >= 150)
                        {
                            Width = 290;
                            Height = 150;
                            minMax = false;
                            return;
                        }
                        if (Width >= 290)
                            Width = 290;
                        else
                            Width += 10;
                        if (Height >= 150)
                            Height = 150;
                        else
                            Height += 10;
                    });
                }
            }).Start();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                Dispatcher.Invoke(() => { ChatLabel.Visibility = Visibility.Visible; });
                while (!minMax)
                {
                    Dispatcher.Invoke(() =>
                    {
                    if (Width <= 21 && Height <= 56)
                    {
                        Width = 21;
                        Height = 56;
                        minMax = true;
                        return;
                    }
                    if (Width <= 21)
                        Width = 21;
                    else
                        Width -= 10;
                    if (Height <= 56)
                        Height = 56;
                    else
                        Height -= 10;
                    });
                }
            }).Start();
        }
    }
}
