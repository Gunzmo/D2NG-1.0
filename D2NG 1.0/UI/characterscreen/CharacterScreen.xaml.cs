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
using D2NG.Tools;

namespace D2NG.UI.characterscreen
{
    /// <summary>
    /// Interaction logic for CharacterScreen.xaml
    /// </summary>
    public partial class CharacterScreen : UserControl
    {
        public delegate void _Select(int x, int y, Tools.CharacterInfo SelectedCharacter);
        public event _Select Select;

        public delegate void _CreateCharacter();
        public event _CreateCharacter CreateCharacter;

        public delegate void _ConvertToExpansion();
        public event _ConvertToExpansion ConvertToExpansion;

        public delegate void _Back();
        public event _Back Back;

        List<Character> Chars = new List<Character>();
        public CharacterScreen(Tools.CharacterList Characters)
        {
            InitializeComponent();
            for(int i = 0; i < Characters.Characters.Length; i++)
            {
                var Char = new Character(Characters.Characters[i], i);
                Char.Select += Char_Select;
                Chars.Add(Char);
                switch(i)
                {
                    case 0:
                        CharacterOne.Children.Add(Char);
                        Char.x = 100;
                        Char.y = 80;
                        continue;
                    case 1:
                        CharacterTwo.Children.Add(Char);
                        Char.x = 100;
                        Char.y = 400;
                        continue;
                    case 2:
                        CharacterThree.Children.Add(Char);
                        Char.x = 200;
                        Char.y = 80;
                        continue;
                    case 3:
                        CharacterFour.Children.Add(Char);
                        Char.x = 200;
                        Char.y = 400;
                        continue;
                    case 4:
                        CharacterFive.Children.Add(Char);
                        Char.x = 300;
                        Char.y = 80;
                        continue;
                    case 5:
                        CharacterSix.Children.Add(Char);
                        Char.x = 300;
                        Char.y = 400;
                        continue;
                    case 6:
                        CharacterSeven.Children.Add(Char);
                        Char.x = 400;
                        Char.y = 80;
                        continue;
                    case 7:
                        CharacterEight.Children.Add(Char);
                        Char.x = 400;
                        Char.y = 400;
                        continue;
                }
            }
        }
        int x, y;
        private void Char_Select(int x, int y, int ID, Tools.CharacterInfo CharInfo)
        {
           foreach (var Char in Chars.Where(c => c.ID != ID))
               Char.HideSelect();
           SelectedCharacter.Content = CharInfo.Name;
           Select(y, x, CharInfo);
           this.x = x;
           this.y = y;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (Chars.Count == 8) return;
            CreateCharacter();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
            {ConvertWarning.Visibility = Visibility.Visible;}

        private void button5_Click(object sender, RoutedEventArgs e)
            { ConvertWarning.Visibility = Visibility.Hidden; }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            ConvertToExpansion();
            ConvertWarning.Visibility = Visibility.Hidden;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Select(y, x, D2NGMainWindow.SelectedCharacter);
            Select(y, x, D2NGMainWindow.SelectedCharacter);
        }

        internal void ShowError(RealmStartupResult data)
        {
            switch(data)
            {
                case RealmStartupResult.InvalidCDKey:
                    CharScreenError.Content = "Invalid CD-Key";
                    break;
                case RealmStartupResult.TemporaryIPBan:
                    CharScreenError.Content = "Realm Down!";
                    break;
            }
            CharScreenError.Visibility = Visibility.Visible;
        }
    }
}
