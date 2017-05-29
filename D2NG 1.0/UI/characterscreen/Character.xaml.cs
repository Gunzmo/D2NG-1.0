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
using System.Drawing;
using System.IO;

namespace D2NG.UI
{
    /// <summary>
    /// Interaction logic for Character.xaml
    /// </summary>
    public partial class Character : UserControl
    {
        public delegate void _Select(int x, int y, int ID, CharacterInfo characterInfo);
        public event _Select Select;
        public int x;
        public int y;
        public int ID;
        public CharacterInfo characterInfo;
        public Character(CharacterInfo charinfo, int ID)
        {
            characterInfo = charinfo;
            InitializeComponent();
            this.ID = ID;
            label.Content = (characterInfo.Title != CharacterTitle.Nooblar || characterInfo.Title != CharacterTitle.Nooblette || characterInfo.Title != CharacterTitle.CourageousNooblar || characterInfo.Title != CharacterTitle.CourageousNooblette
                || characterInfo.Title != CharacterTitle.DoublePlusNooblar || characterInfo.Title != CharacterTitle.DoublePlusNooblette || characterInfo.Title != CharacterTitle.NooblarWhoLikesChicken || characterInfo.Title != CharacterTitle.NoobletteWhoLikesChicken ? 
                "" : characterInfo.Title.ToString()) + characterInfo.Name;
            label1.Content = "LEVEL " + characterInfo.Level + " " + characterInfo.Class.ToString();
            if ((characterInfo.Flags & CharacterFlags.Expansion) == CharacterFlags.Expansion)
                label2.Content = "EXPANSION CHARACTER";
            if ((characterInfo.Flags & CharacterFlags.NoNLadder) != CharacterFlags.NoNLadder)
                label3.Content = "LADDER CHARACTER";
            label4.Content = "Expires:" + characterInfo.Expires.ToString();
            switch(characterInfo.Class)
            {
                case CharacterClass.Amazon:
                    CharIMG.Source = WinAPI.BitmapToImageSource(new Bitmap(Properties.Resources.AmA));
                    break;
                case CharacterClass.Assassin:
                    CharIMG.Source = WinAPI.BitmapToImageSource(new Bitmap(Properties.Resources.AssA));
                    break;
                case CharacterClass.Barbarian:
                    CharIMG.Source = WinAPI.BitmapToImageSource(new Bitmap(Properties.Resources.Barb));
                    break;
                case CharacterClass.Druid:
                    CharIMG.Source = WinAPI.BitmapToImageSource(new Bitmap(Properties.Resources.Drood));
                    break;
                case CharacterClass.Necromancer:
                    CharIMG.Source = WinAPI.BitmapToImageSource(new Bitmap(Properties.Resources.Necro));
                    break;
                case CharacterClass.Paladin:
                    CharIMG.Source = WinAPI.BitmapToImageSource(new Bitmap(Properties.Resources.Pala));
                    break;
                case CharacterClass.Sorceress:
                    CharIMG.Source = WinAPI.BitmapToImageSource(new Bitmap(Properties.Resources.SoSo));
                    break;
            }
        }
        
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            image1.Visibility = Visibility.Visible;
            Select(x, y, ID, characterInfo);
        }
        public void HideSelect(){ image1.Visibility = Visibility.Hidden; }
    }
}
