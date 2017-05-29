using Microsoft.Win32;
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

namespace D2NG.UI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class D2Login : UserControl
    {
        public delegate void _D2Login(string Account, string Password, Tools.Region Region);
        public event _D2Login Login;

        public delegate void _D2Register(string Account, string Password, string Email,Tools.Region Region);
        public event _D2Register D2Register;

        public delegate void _StartD2();
        public event _StartD2 StartD2;
        public D2Login()
        {
            InitializeComponent();
            textBox.Text = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Blizzard Entertainment\Diablo II", "Last BNet", "Account");
            switch((string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Blizzard Entertainment\Diablo II", "Preferred Realm", "Europe"))
            {
                case "Europe":
                    comboBox.SelectedIndex = 0;
                    break;
                case "U.S. East":
                    comboBox.SelectedIndex = 1;
                    break;
                case "U.S. West":
                    comboBox.SelectedIndex = 2;
                    break;
                case "Asia":
                    comboBox.SelectedIndex = 3;
                    break;
            }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Tools.Region region = Tools.Region.Europe;
            switch (comboBox.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem:    ", ""))
            {
                case "Europe":
                    region = Tools.Region.Europe;
                    break;
                case "Asia":
                    region = Tools.Region.Asia;
                    break;
                case "U.S. West":
                    region = Tools.Region.USW;
                    break;
                case "U.S. East":
                    region = Tools.Region.USE;
                    break;
            }
            Login(textBox.Text, passwordBox.Password, region);
            Lubby.Realm = region;
            passwordBox.Password = "";
        }
        private void textBox_GotFocus(object sender, RoutedEventArgs e) { if (textBox.Text == "Account") textBox.Text = ""; }
        private void textBox_LostFocus(object sender, RoutedEventArgs e) { if (textBox.Text == "") textBox.Text = "Account"; }
        private void passwordBox_LostFocus(object sender, RoutedEventArgs e) { if (passwordBox.Password == "") PasswordPlaceHoder.Visibility = Visibility.Visible; }
        private void passwordBox_GotFocus(object sender, RoutedEventArgs e) { PasswordPlaceHoder.Visibility = Visibility.Hidden; }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            StartD2();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            LoginPanel.Visibility = Visibility.Visible;
            RegisterGrid.Visibility = Visibility.Hidden;
        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            LoginPanel.Visibility = Visibility.Hidden;
            RegisterGrid.Visibility = Visibility.Visible;
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Tools.Region region = Tools.Region.Europe;
            switch (comboBox.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem:    ", ""))
            {
                case "Europe":
                    region = Tools.Region.Europe;
                    break;
                case "Asia":
                    region = Tools.Region.Asia;
                    break;
                case "U.S. West":
                    region = Tools.Region.USW;
                    break;
                case "U.S. East":
                    region = Tools.Region.USE;
                    break;
            }
            D2Register(RegisterAccount.Text, RegisterPassword.Password, RegisterEmail.Text, region);
        }

        private void RegisterPassword_LostFocus(object sender, RoutedEventArgs e)
            {if (RegisterPassword.Password == "") PswdPlaceHolder.Visibility = Visibility.Visible;}

        private void RegisterPassword_GotFocus(object sender, RoutedEventArgs e)
            {PswdPlaceHolder.Visibility = Visibility.Hidden;}
    }
}
