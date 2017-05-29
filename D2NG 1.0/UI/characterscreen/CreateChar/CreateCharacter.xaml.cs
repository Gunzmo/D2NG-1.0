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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace D2NG.UI
{
    /// <summary>
    /// Interaction logic for CreateCharacter.xaml
    /// </summary>
    public partial class CreateCharacter : UserControl
    {
        public delegate void _Create(int x, int y, string Name, bool Ladder, bool HardCore, bool Expansion);
        public event _Create Create;

        public delegate void _Back();
        public event _Back Back;
        int x, y;
        List<characterscreen.CreateChar.CharactarSelect> Chars;
        public CreateCharacter()
        {
            InitializeComponent();
            var amma = new characterscreen.CreateChar.CharactarSelect(100, 300, Tools.CharacterClass.Amazon);
            var assa = new characterscreen.CreateChar.CharactarSelect(200, 300, Tools.CharacterClass.Assassin);
            var necro = new characterscreen.CreateChar.CharactarSelect(300, 300, Tools.CharacterClass.Necromancer);
            var barb = new characterscreen.CreateChar.CharactarSelect(400, 300, Tools.CharacterClass.Barbarian);
            var pala = new characterscreen.CreateChar.CharactarSelect(540, 300, Tools.CharacterClass.Paladin);
            var soso = new characterscreen.CreateChar.CharactarSelect(600, 300, Tools.CharacterClass.Sorceress);
            var drood = new characterscreen.CreateChar.CharactarSelect(700, 300, Tools.CharacterClass.Druid);
            Chars = new List<characterscreen.CreateChar.CharactarSelect>() { amma, assa,necro,barb,pala,soso,drood };
            foreach (var c in Chars)
                c.Select += Select;
            Amazon.Children.Add(amma);
            Assassin.Children.Add(assa);
            Necromancer.Children.Add(necro);
            Barbarian.Children.Add(barb);
            Paladin.Children.Add(pala);
            Sorceress.Children.Add(soso);
            Druid.Children.Add(drood);
            this.BG.Source = Tools.WinAPI.BitmapToImageSource(new System.Drawing.Bitmap(Properties.Resources.bg));
        }

        private void Select(Tools.CharacterClass Class, int x, int y)
        {
            SelectedChar.Content = Class;
            this.x = x;
            this.y = y;
            foreach (var c in Chars)
                c.HideSelect();
            Chars.FirstOrDefault(c => c.Class == Class).SetSelect();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
            { Back(); }

        public void DisplayError(Tools.RealmCharacterActionResult Result)
        {
            switch (Result)
            {
                case Tools.RealmCharacterActionResult.InvalidCharacterName:
                    Warning.Content = "Invalid Character!";
                    break;
                case Tools.RealmCharacterActionResult.CharacterOverlap:
                    Warning.Content = "Character name already Exists!";
                    break;
            }
            WarningHolder.Visibility = Visibility.Visible;
            new Thread(() => {
                Thread.Sleep(3000);
                Dispatcher.Invoke(() => { 
                    WarningHolder.Visibility = Visibility.Hidden;
                });
            }).Start();
        }

        private void CharacterName_GotFocus(object sender, RoutedEventArgs e)
            { NamePlaceHolder.Visibility = Visibility.Hidden; }

        private void CharacterName_LostFocus(object sender, RoutedEventArgs e)
            { if (CharacterName.Text == "") NamePlaceHolder.Visibility = Visibility.Visible; }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
            { Create(x, y, CharacterName.Text, (bool)LadderBox.IsChecked, (bool)HardCoreBox.IsChecked, (bool)ExpansionBox.IsChecked); }
    }
}
