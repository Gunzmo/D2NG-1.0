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

namespace D2NG.UI.InGame
{
    /// <summary>
    /// Interaction logic for FriendsList.xaml
    /// </summary>
    /// 
    public class FriendItem
    {
        public int ID { get; private set; }
        public string CharacterNameDisplay { get; set; }
        public string CharacterName { get; private set; }
        public string Account { get; private set; }
        public int GameID { get; private set; }
        string gameName;
        public string Status
        {
            get
            {
                if (CharacterName == string.Empty)
                    return "Offline";
                if (GameID != 0)
                    return "In Game: " + gameName;
                else
                    return "In Lobby";
            }
        }
        D2NG.Tools.FriendType _TypeOfFrirend;
        public D2NG.Tools.FriendType TypeOfFrirend { get { return _TypeOfFrirend; } private set { _TypeOfFrirend = value; } }
        public System.Windows.Media.Brush StatusSRC { get; private set; }
        public System.Windows.Media.Imaging.BitmapImage ClassImage { get; private set; }
        public bool Selected = false;
        public List<string> Messages = new List<string>();
        public bool Update = false;
        public FriendItem(int ID, string Account, string CharacterName, bool Online, D2NG.Tools.CharacterClass Class, D2NG.Tools.FriendType TypeOfFrirend)
        {
            CharacterNameDisplay = CharacterName;
            this.ID = ID;
            this.TypeOfFrirend = TypeOfFrirend;
            this.Account = Account;
            this.CharacterName = CharacterName;
            StatusSRC = (Online ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0)) : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 128, 128)));
            ChangeCharacter(Class);
        }
        public void UpdateCharacterName(string name)
        { CharacterName = name; }
        public void UpdateStatus(string Status, int GameID)
        { this.gameName = Status; this.GameID = GameID; }
        public void UpdateFriendStatus(bool Online)
        { StatusSRC = (Online ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0)) : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 128, 128))); }
        public void ChangeCharacter(D2NG.Tools.CharacterClass Class)
        {
            switch (Class)
            {
                case D2NG.Tools.CharacterClass.Amazon:
                    ClassImage = D2NG.Tools.WinAPI.BitmapToImageSource(new System.Drawing.Bitmap(D2NG.Properties.Resources.AmA));
                    break;
                case D2NG.Tools.CharacterClass.Assassin:
                    ClassImage = D2NG.Tools.WinAPI.BitmapToImageSource(new System.Drawing.Bitmap(D2NG.Properties.Resources.AssA));
                    break;
                case D2NG.Tools.CharacterClass.Barbarian:
                    ClassImage = D2NG.Tools.WinAPI.BitmapToImageSource(new System.Drawing.Bitmap(D2NG.Properties.Resources.Barb));
                    break;
                case D2NG.Tools.CharacterClass.Druid:
                    ClassImage = D2NG.Tools.WinAPI.BitmapToImageSource(new System.Drawing.Bitmap(D2NG.Properties.Resources.Drood));
                    break;
                case D2NG.Tools.CharacterClass.Necromancer:
                    ClassImage = D2NG.Tools.WinAPI.BitmapToImageSource(new System.Drawing.Bitmap(D2NG.Properties.Resources.Necro));
                    break;
                case D2NG.Tools.CharacterClass.Paladin:
                    ClassImage = D2NG.Tools.WinAPI.BitmapToImageSource(new System.Drawing.Bitmap(D2NG.Properties.Resources.Pala));
                    break;
                case D2NG.Tools.CharacterClass.Sorceress:
                    ClassImage = D2NG.Tools.WinAPI.BitmapToImageSource(new System.Drawing.Bitmap(D2NG.Properties.Resources.SoSo));
                    break;
                case D2NG.Tools.CharacterClass.Any:
                    ClassImage = D2NG.Tools.WinAPI.BitmapToImageSource(new System.Drawing.Bitmap(D2NG.Properties.Resources.None));
                    break;
            }
        }
        public void UpdateType(D2NG.Tools.FriendType type) { TypeOfFrirend = type; }
    }
    public partial class FriendsList : Window
    {
        public delegate void _Main_SelectFriend();
        public event _Main_SelectFriend Main_SelectFriend;
        bool minMax = false;
        public static List<FriendItem> Friendslist = new List<FriendItem>();

        int _MaxWidth = 0;
        int _MaxHeight = 0;
        public FriendsList()
        {
            InitializeComponent();
            FriendList.ItemsSource = Friendslist;
            FriendList.Items.Refresh();
            Width = 28;
            Height = 91;
            Scroller.Visibility = Visibility.Hidden;
        }

        public void SetWH(int w, int h) { _MaxWidth = w; _MaxHeight = h; }

        public void AnimateBackGround(int ID)
        {
            var Friend = Friendslist.FirstOrDefault(f => f.ID == ID);
            if (Friend != null)
            {
                Dispatcher.Invoke(() => {
                    Friend.CharacterNameDisplay = Friend.CharacterName + " (New Message)";
                    Friend.Update = true;
                    FriendList.Items.Refresh();
                });
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = (TreeView)sender;
            var Item = (FriendItem)treeView.SelectedItem;
            foreach (var item in Friendslist)
                item.Selected = false;
            Item.Selected = true;
            Main_SelectFriend();
            if (Item.Update)
            {
                Item.CharacterNameDisplay = Item.CharacterName;
                Item.Update = false;
                UpdateFriendList();
            }
        }


        internal void UpdateFriendList()
        { Dispatcher.Invoke(() => { FriendList.Items.Refresh(); }); }

        private void FriendsLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                Dispatcher.Invoke(() => { FriendsLabel.Visibility = Visibility.Hidden; Scroller.Visibility = Visibility.Visible; });
                while (minMax)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (Width >= _MaxWidth && Height >= _MaxHeight)
                        {
                            Width = _MaxWidth;
                            Height = _MaxHeight;
                            minMax = false;
                            return;
                        }
                        if (Width >= _MaxWidth)
                            Width = _MaxWidth;
                        else
                            Width += 25;
                        if (Height >= _MaxHeight)
                            Height = _MaxHeight;
                        else
                            Height += 25;
                    });
                }
            }).Start();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                Dispatcher.Invoke(() => { FriendsLabel.Visibility = Visibility.Visible; });
                while (!minMax)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (Width <= 28 && Height <= 91)
                        {
                            Width = 28;
                            Height = 91;
                            minMax = true;
                            return;
                        }

                        if (Width <= 28)
                            Width = 28;
                        else
                            Width -= 25;

                        if (Height <= 91)
                            Height = 91;
                        else
                            Height -= 25;
                    });
                }
                Dispatcher.Invoke(() => { Scroller.Visibility = Visibility.Hidden; });
            }).Start();
        }
    }
}
