using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace D2NG.UI.characterscreen.CreateChar
{
    /// <summary>
    /// Interaction logic for CharactarSelect.xaml
    /// </summary>
    public partial class CharactarSelect : UserControl
    {
        int X, Y;
        public delegate void _Select(Tools.CharacterClass Class, int x, int y);
        public event _Select Select;
        public Tools.CharacterClass Class;
        public CharactarSelect(int X, int Y, Tools.CharacterClass Class)
        {
            
            InitializeComponent();
            this.Class = Class;
            this.X = X;
            this.Y = Y;
            switch (Class)
            {
                case Tools.CharacterClass.Amazon:
                    CharacterImage.Source = Tools.WinAPI.BitmapToImageSource(new Bitmap(Properties.Resources.AmA1));
                    break;
                case Tools.CharacterClass.Assassin:
                    CharacterImage.Source = Tools.WinAPI.BitmapToImageSource(new Bitmap(Properties.Resources.AssA1));
                    break;
                case Tools.CharacterClass.Barbarian:
                    CharacterImage.Source = Tools.WinAPI.BitmapToImageSource(new Bitmap(Properties.Resources.Barb1));
                    break;
                case Tools.CharacterClass.Druid:
                    CharacterImage.Source = Tools.WinAPI.BitmapToImageSource(new Bitmap(Properties.Resources.drood1));
                    break;
                case Tools.CharacterClass.Necromancer:
                    CharacterImage.Source = Tools.WinAPI.BitmapToImageSource(new Bitmap(Properties.Resources.Necro1));
                    break;
                case Tools.CharacterClass.Paladin:
                    CharacterImage.Source = Tools.WinAPI.BitmapToImageSource(new Bitmap(Properties.Resources.Pala1));
                    break;
                case Tools.CharacterClass.Sorceress:
                    CharacterImage.Source = Tools.WinAPI.BitmapToImageSource(new Bitmap(Properties.Resources.soso1));
                    break;
            }
        }

        public void SetSelect()
            { SelectBorder.Visibility = Visibility.Visible; }
        public void HideSelect()
            { SelectBorder.Visibility = Visibility.Hidden; }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        { Select(Class, X, Y); }
    }
}
