using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace D2NG.UI
{
    /// <summary>
    /// Interaction logic for Lubby.xaml
    /// </summary>

    public class Clan
    {
        public int ID;
        public string Name;
        public Clan(int ID, string Name)
        {
            this.ID = ID;
            this.Name = Name;
        }
    }

    public class Reports
    {
        public int ID;
        public string Account;
        public string Reporter;
        public string DateTime;
        public Reports(int ID, string Account, string Reporter, string DateTime)
        {
            this.ID = ID;
            this.Account = Account;
            this.Reporter = Reporter;
            this.DateTime = DateTime;
        }
    }
    public partial class Lubby : UserControl
    {
        public List<string> GameList = new List<string>();
        public List<string> GameListNOFilter = new List<string>();
        public List<Tools.D2NGGames> D2NGGamesNOFilter = new List<Tools.D2NGGames>();
        public List<Tools.D2NGGames> D2NGGames = new List<Tools.D2NGGames>();
        public List<Reports> Reports;
        public List<Tools.Mods> Mods = new List<Tools.Mods>();
        public static List<string> OnlineUsersList = new List<string>();
        public static List<string> ChannelAccountList = new List<string>();
        bool admin;
        Tools.Difficulty mode = 0;
        public static Tools.Region Realm;
        #region Callbacks

        public delegate void _JoinGame(string Gamename, string Password);
        public event _JoinGame JoinGame;
        public delegate void _D2NGJoinGame(int ID);
        public event _D2NGJoinGame D2NGJoinGame;
        public delegate void _CreateGame(string Gamename, string Password, bool IsListed, string Descripton, string MaxPlayers, Tools.Difficulty Mode);
        public event _CreateGame CreateGame;

        public delegate void _RefreshGames(bool Bnet, Tools.Difficulty diff);
        public event _RefreshGames RefreshGames;

        public delegate void _AddFriend(string FriendName);
        public event _AddFriend AddFriend;

        public delegate void _RemoveFriend(int ID);
        public event _RemoveFriend RemoveFriendCall;


        public delegate void _SendMSG(string MSG);
        public event _SendMSG SendMSG;

        public delegate void _Report(string username, Tools.ReportReason reason);
        public event _Report Report;

        public delegate void _Logout();
        public event _Logout CallLogout;

        public delegate void _Exit();
        public event _Exit Exit;

        public delegate void _Premote(string Username, int Status);
        public event _Premote Premote;

        public delegate void _Ban(string Username, DateTime BanTime, bool IP);
        public event _Ban Ban;

        public delegate void _RemoveReport(int report);
        public event _RemoveReport RemoveReportCallback;

        public delegate void _D2NGRegister(string username, string password, string email);
        public event _D2NGRegister D2NGRegister;

        public delegate void _D2NGLogin(string username, string password, Tools.Region Realm);
        public event _D2NGLogin D2NGLogin;
        #endregion

        public ObservableCollection<Reports> ReportsOC { get; set; }

        public Random rng = new Random();
        public Lubby()
        {
            InitializeComponent();
            FriendsList.ItemsSource = InGame.FriendsList.Friendslist;
            bnetGames.ItemsSource = GameList;
            D2NGGameList.ItemsSource = D2NGGames;
            ChannelUsers.ItemsSource = ChannelAccountList;
            FriendsList.Items.Refresh();
            bnetGames.Items.Refresh();
            D2NGGameList.Items.Refresh();
            ChannelUsers.Items.Refresh();
            //Add ADS URL PHP + Javascript!
            //ADS.Navigate("");
        }

        public void UpdateADS() { ADS.Refresh(); }

        #region Invokes
        internal void InvalidGame(Tools.CreateGameResult Result)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate ()
            {
                GameModeSelect.Visibility = Visibility.Hidden;
                switch (Result)
                {
                    case Tools.CreateGameResult.DeadHardcoreCharacter:
                        JoinMessages.Content = "Your Hardcore character is DEAD!";
                        break;
                    case Tools.CreateGameResult.GameAlreadyExists:
                        JoinMessages.Content = "Game Already Exists!";
                        break;
                    case Tools.CreateGameResult.InvalidGameName:
                        JoinMessages.Content = "Invalid Game Name!";
                        break;
                }
                JoinMessages.Visibility = Visibility.Visible;
                new System.Threading.Thread(() =>
                {
                    System.Threading.Thread.Sleep(3000);
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(delegate ()
                    {
                        JoinMessages.Visibility = Visibility.Hidden;
                        GameModeSelect.Visibility = Visibility.Visible;
                    }));
                }).Start();
            }));
        }
        internal void InvalidGame(D2NG.Tools.JoinGameResult Result)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate ()
            {
                GameModeSelect.Visibility = Visibility.Hidden;
                switch (Result)
                {
                    case Tools.JoinGameResult.DeadHardcoreCharacter:
                        JoinMessages.Content = "Your Hardcore character is DEAD!";
                        break;
                    case Tools.JoinGameResult.GameDoesNotExist:
                        JoinMessages.Content = "Game Does not Exist!";
                        break;
                    case Tools.JoinGameResult.GameFull:
                        JoinMessages.Content = "Game is Full!";
                        break;
                    case Tools.JoinGameResult.LevelRequirementsNotMet:
                        JoinMessages.Content = "Level Requierment not met!";
                        break;
                    case Tools.JoinGameResult.PasswordIncorrect:
                        JoinMessages.Content = "Incrrrect Password!";
                        break;
                    case Tools.JoinGameResult.UnableToJoinClassicGame:
                        JoinMessages.Content = "Unable to Join Classig Game!";
                        break;
                    case Tools.JoinGameResult.UnableToJoinExpansionGame:
                        JoinMessages.Content = "Unable to Join Expnasion Game!";
                        break;
                    case Tools.JoinGameResult.UnableToJoinHardcoreGame:
                        JoinMessages.Content = "Unable to Join Hardcore Game!";
                        break;
                    case Tools.JoinGameResult.UnableToJoinHellGame:
                        JoinMessages.Content = "Unable to Join Hell Game!";
                        break;
                    case Tools.JoinGameResult.UnableToJoinLadderGame:
                        JoinMessages.Content = "Unable to Join Ladder Game!";
                        break;
                    case Tools.JoinGameResult.UnableToJoinNightmareGame:
                        JoinMessages.Content = "Unable to Join Nightmare Game!";
                        break;
                }
                JoinMessages.Visibility = Visibility.Visible;
                new System.Threading.Thread(() =>
                {
                    System.Threading.Thread.Sleep(3000);
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(delegate ()
                    {
                        JoinMessages.Visibility = Visibility.Hidden;
                        GameModeSelect.Visibility = Visibility.Visible;
                    }));
                }).Start();
            }));
        }
        internal void UpdateGame(int ID, string[] Descriptions, int Players)
        {
            Tools.D2NGGames Game;
            lock (D2NGGamesNOFilter)
                Game = D2NGGamesNOFilter.FirstOrDefault(g => g.ID == ID);
            if (Game == null) return;
            Game.Descriptions = Descriptions.ToList();
            Game.Players = Players;
            lock (D2NGGames)
                D2NGGames = new List<Tools.D2NGGames>(D2NGGamesNOFilter);
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate ()
            { D2NGGameList.Items.Refresh(); }));
        }
        internal void RemoveD2NGGame(int ID)
        {
            var Game = D2NGGamesNOFilter.FirstOrDefault(g => g.ID == ID);
            if (Game == null) return;
            lock (GameListNOFilter)
                D2NGGamesNOFilter.Remove(Game);
            lock (D2NGGames)
                D2NGGames.Remove(Game);
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate ()
            {D2NGGameList.Items.Refresh();}));
        }
        internal void UpdateFriends(){
                FriendsList.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,  new Action(delegate (){FriendsList.Items.Refresh(); }));
        }
        internal void SetDifficulty()
        {
            switch (D2NGMainWindow.SelectedCharacter.Title)
            {
                case Tools.CharacterTitle.DestroyerF:
                case Tools.CharacterTitle.Destroyer:
                case Tools.CharacterTitle.SlayerF:
                case Tools.CharacterTitle.Slayer:
                case Tools.CharacterTitle.Countess:
                case Tools.CharacterTitle.Count:
                case Tools.CharacterTitle.Sir:
                case Tools.CharacterTitle.Dame:
                    radioButton4.IsEnabled = true;
                    D2NGNightmare.IsEnabled = true;
                    break;
                case Tools.CharacterTitle.Conquerer:
                //case Tools.CharacterTitle.ConquererF:
                case Tools.CharacterTitle.ChampionF:
                case Tools.CharacterTitle.Champion:
                case Tools.CharacterTitle.Duchess:
                case Tools.CharacterTitle.Duke:
                case Tools.CharacterTitle.Lady:
                case Tools.CharacterTitle.Lord:
                    radioButton5.IsEnabled = true;
                    D2NGHell.IsEnabled = true;
                    D2NGAll.IsEnabled = true;
                    break;
                // case Tools.CharacterTitle.GuardianF:
                case Tools.CharacterTitle.Guardian:
                case Tools.CharacterTitle.Matriarch:
                case Tools.CharacterTitle.Patriarch:
                case Tools.CharacterTitle.Queen:
                case Tools.CharacterTitle.King:
                case Tools.CharacterTitle.Baroness:
                case Tools.CharacterTitle.Baron:
                    radioButton5.IsEnabled = true;
                    D2NGHell.IsEnabled = true;
                    D2NGAll.IsEnabled = true;
                    break;
            }
        }
        internal void AddChannelUser(string User)
        {
            if(ChannelAccountList.FirstOrDefault(u => u == User) != null) return;
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate ()
            {
                lock (ChannelAccountList)
                    ChannelAccountList.Add(User);
                ChannelUsers.Items.Refresh();
            }));
        }
        internal void RemoveChannelUser(string User)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(delegate ()
                {
                    lock (ChannelAccountList)
                        ChannelAccountList.Remove(User);
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(delegate ()
                    {
                        ChannelUsers.Items.Refresh();
                    }));
            }));
        }
        internal void ClearChannelUser()
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate ()
            {
                lock (ChannelAccountList)
                    ChannelAccountList.Clear();
                ChannelUsers.Items.Refresh();
            }));
        }
        internal void ISMod(bool Admin)
        {

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate ()
            {
                var item = (TabItem)GeneralTabs.Items.GetItemAt(3); item.Visibility = Visibility.Visible;
                if (Admin)
                    AdminTab.Visibility = Visibility.Visible;
                OnlineUsers.ItemsSource = OnlineUsersList; OnlineUsers.Items.Refresh();
                Mods = new List<Tools.Mods>();
                ModList.ItemsSource = Mods;
                PromoteAdmin.IsEnabled = Admin;
                ReportList.DataContext = ReportsOC;
                Reports = new List<Reports>();
                ReportList.ItemsSource = Reports;
                this.admin = Admin;
            }));
        }
        internal void AddMod(string Username, bool status)
        {
            if (Mods.FirstOrDefault(m => m.AccountName == Username) != null) return;
            Mods.Add(new Tools.Mods(Username, status));
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate ()
            {  ModList.Items.Refresh();}));
        }
        internal void ModStatus(string Username, bool Online)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(delegate ()
                {
                    Tools.Mods mod = Mods.FirstOrDefault(m => m.AccountName == Username);
                    if (mod == null) return;
                    if (Online)
                        mod.color = 0x0F0;
                    else
                        mod.color = 0xF00;
                    ModList.Items.Refresh();
                }));
        }
        internal void Login(bool Logged)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate ()
            {
                if (Logged)
                {
                    SessionTabs.Visibility = Visibility.Hidden;
                    GeneralTabs.Visibility = Visibility.Visible;
                    D2NGGameTab.Visibility = Visibility.Visible;
                    checkBox2.IsEnabled = true;
                    checkBox.IsEnabled = true;
                }
                else
                    label9.Visibility = Visibility.Visible;
            }));
        }
        internal void Logout()
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate ()
            {
                SessionTabs.Visibility = Visibility.Visible;
                GeneralTabs.Visibility = Visibility.Hidden;
                D2NGGameTab.Visibility = Visibility.Hidden;
                label9.Visibility = Visibility.Hidden;
                checkBox.IsEnabled = false;
                checkBox2.IsEnabled = false;
            }));
        }
        internal void Register(bool Registerd)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate ()
            {
                if (Registerd)
                    SessionTabs.TabIndex = 0;
                else
                    label5.Visibility = Visibility.Visible;
            }));
        }
        internal void AddOnlineUsers(string User)
        {
            lock(OnlineUsersList)
                OnlineUsersList.Add(User);
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate ()
            { OnlineUsers.Items.Refresh();}));
        }
        internal void RemoveOnlineUsers(string User)
        {
            lock (OnlineUsersList)
                OnlineUsersList.Remove(User);
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(delegate ()
                {
                    OnlineUsers.Items.Refresh();
                }));
        }
        internal void AddReports(int ID, string Account, string Reporter, string DateTime)
        {
            Reports.Add(new Reports(ID, Account, Reporter, DateTime));
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate ()
            {ReportList.Items.Refresh(); }));
        }
        internal void RemoveReport(int Report)
        {
            var rep = Reports.FirstOrDefault(r => r.ID == Report);
            if (rep == null) return;
            Reports.Remove(rep);
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate ()
            {ReportList.Items.Refresh();}));
        }
        internal void Refreshgames(bool Bnet, dynamic Games)
        {
            if(Bnet)
            {
                var game = (Tools.GameList)Games;
                if(!GameListNOFilter.Contains(game.Name))
                {
                    if (string.IsNullOrEmpty(game.Name)) return;
                    GameListNOFilter.Add(game.Name);
                    GameList.Add(game.Name);
                    bnetGames.Items.Refresh();
                }
                
            }
            else
            {
                Tools.D2NGGames[] game = (Tools.D2NGGames[])Games;
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(delegate ()
                {
                    foreach (var Game in game)
                    {

                        if (D2NGGamesNOFilter.FirstOrDefault(G => G.Gamename == Game.Gamename) != null) continue;
                        D2NGGamesNOFilter.Add(Game);
                        D2NGGames.Add(Game);
                        D2NGGameList.Items.Refresh();

                    }
                }));
            }
        }
        #endregion

        #region JoinGame
        private void button_Click(object sender, EventArgs e) { JoinGame(textBox.Text, textBox1.Text); }
        private void button3_Click(object sender, EventArgs e)
        {
            var game = D2NGGames.FirstOrDefault(g => g.selected);
            if (game == null) return;
            D2NGJoinGame(game.ID);
        }
        #endregion

        #region Friends
        private void button6_Click(object sender, RoutedEventArgs e) { AddFriend(textBox7.Text); textBox7.Text = ""; }
        #endregion

        #region CreateGame
        private void button5_Click(object sender, RoutedEventArgs e)
            { D2NGMainWindow.ISD2NG = (bool)checkBox.IsChecked; CreateGame(GameName.Text, ((bool)checkBox.IsChecked ? rng.Next(10000, 40000).ToString() : Password.Text), (bool)checkBox2.IsChecked, Description.Text, MaxCharacters.Text, mode);}
        #endregion

        #region Refresh
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            GameList.Clear();
            GameListNOFilter.Clear();
            bnetGames.Items.Refresh();
            RefreshGames(true, Tools.Difficulty.Normal);
        }
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            D2NGGames.Clear();
            D2NGGamesNOFilter.Clear();
            D2NGGameList.Items.Refresh();
            RefreshGames(false, mode);
        }
        #endregion

        #region GameSelection
        private void D2NGGameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ListView = (ListView)sender;
            var game = (Tools.D2NGGames)ListView.SelectedItem;
            foreach (var ga in D2NGGames)
               ga.selected = false;
            game.selected = true;
            GameDescripton.Document.Blocks.Clear();
            foreach (var desc in game.Descriptions)
                GameDescripton.Document.Blocks.Add(new Paragraph(new Run(desc)){ LineHeight = 1 });
        }
        private void bnetGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textBox.Text = (string)bnetGames.SelectedItem;
            textBox1.Text = "";
        }
        #endregion

        #region Message
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                sendMSG();
        }
        private void button2_Click(object sender, RoutedEventArgs e) { sendMSG(); }

        void sendMSG()
        {
            
                if (textBox2.Text.ToLower() == "/?" || textBox2.Text.ToLower() == "/h" || textBox2.Text.ToLower() == "/help")
                {
                    MessageBox.Document.Blocks.Add(new Paragraph(new Run("/C <Channel> <Password> to join or create a channel")));
                    MessageBox.Document.Blocks.Add(new Paragraph(new Run("/L To leave channel and go to global channel")));
                    MessageBox.Document.Blocks.Add(new Paragraph(new Run("/W <Account> <Message> To Wisper a User")));
                    MessageBox.Document.Blocks.Add(new Paragraph(new Run("/r <Account> rewisper to a account")));
                    MessageBox.Document.Blocks.Add(new Paragraph(new Run("/sr <Account> to report a account")));
                    MessageBox.Document.Blocks.Add(new Paragraph(new Run("/clear, /cls To clear chatt window")));
                    goto End;
                }
                if (textBox2.Text.ToLower() == "/clear" || textBox2.Text.ToLower() == "/cls")
                {
                    MessageBox.Document.Blocks.Clear();
                    goto End;
                }
                if(textBox2.Text.Length >= 2)
                if(textBox2.Text.Substring(0,2) == "/sr")
                {
                    ReportNameHolder = textBox2.Text.Split(' ')[1];
                    ReportName.Content = ReportNameHolder;
                    ReportWindow.Visibility = Visibility.Visible;
                    goto End;
                }
                SendMSG(textBox2.Text);
            End:
                textBox2.Text = "";
        }
        public void AddMessage(string Message)
        { Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate () { MessageBox.Document.Blocks.Add(new Paragraph(new Run(Message)) { LineHeight = 1 }); MessageBox.ScrollToEnd(); })); }
        #endregion

        #region UI
        private void MessageBox_TextInput(object sender, TextCompositionEventArgs e) { e.Handled = false; }
        private void richTextBox1_TextInput(object sender, TextCompositionEventArgs e) { e.Handled = false; }
        private void Password_TextInput(object sender, TextCompositionEventArgs e) { e.Handled = !(bool)checkBox.IsChecked; }
        private void textBox_GotFocus(object sender, RoutedEventArgs e)
            {label13.Visibility = Visibility.Hidden;}
        private void textBox_LostFocus(object sender, RoutedEventArgs e)
            {if (textBox4.Text == "")    label13.Visibility = Visibility.Visible;}
        private void textBox7_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBox7.Text == "D2NG Username Name")
                textBox7.Text = "";
        }
        private void textBox7_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBox7.Text == "")
                textBox7.Text = "D2NG Username Name";
        }
        private void PremoteUserName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PremoteUserName.Text == "Username")
                PremoteUserName.Text = "";
        }
        private void PremoteUserName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PremoteUserName.Text == "")
                PremoteUserName.Text = "Username";
        }
        #endregion

        #region D2NG
        private void D2NGNormal_Checked(object sender, RoutedEventArgs e)
        {
            D2NGGames.Clear();
            foreach (var g in D2NGGamesNOFilter.Where(G => G.Difficulty == Tools.Difficulty.Normal))
                D2NGGames.Add(g);
            try { D2NGGameList.Items.Refresh(); } catch { }
        }
        private void D2NGNightmare_Checked(object sender, RoutedEventArgs e)
        {
            D2NGGames.Clear();
            foreach (var g in D2NGGamesNOFilter.Where(G => G.Difficulty == Tools.Difficulty.Nightmare))
                D2NGGames.Add(g);
            D2NGGameList.Items.Refresh();
        }
        private void D2NGHell_Checked(object sender, RoutedEventArgs e)
        {
            D2NGGames.Clear();
            foreach (var g in D2NGGamesNOFilter.Where(G => G.Difficulty == Tools.Difficulty.Hell))
                D2NGGames.Add(g);
            D2NGGameList.Items.Refresh();
        }
        private void D2NGAll_Checked(object sender, RoutedEventArgs e)
        {
            D2NGGames = new List<Tools.D2NGGames>(D2NGGamesNOFilter);
            D2NGGameList.Items.Refresh();
        }
        #endregion

        #region Search
        private void textBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
            GameList.Clear();
            if (textBox3.Text == "")
            {
                lock(GameList)
                    lock(GameListNOFilter)
                        foreach (var game in GameListNOFilter)
                            GameList.Add(game);
                bnetGames.Items.Refresh();
                return;
            }
            foreach (var game in GameListNOFilter.Where(g => g.ToLower().Contains(textBox3.Text.ToLower())))
                lock(GameList)
                    GameList.Add(game);
            bnetGames.Items.Refresh();
        }
        private void textBox4_TextChanged(object sender, TextChangedEventArgs e)
        {
            D2NGGames.Clear();
            if (textBox4.Text == "")
            {
                lock (D2NGGameList)
                    lock (D2NGGamesNOFilter)
                        foreach (var game in D2NGGamesNOFilter)
                            D2NGGames.Add(game);
                return;
            }
            lock (D2NGGameList)
                lock (D2NGGamesNOFilter)
                    foreach (var game in D2NGGamesNOFilter.Where(D => D.Gamename.ToLower().Contains(textBox4.Text.ToLower())))
                        D2NGGames.Add(game);
            D2NGGameList.Items.Refresh();
        }
        #endregion

        #region ACP
        private void button8_Click(object sender, RoutedEventArgs e)
        {
            if(!admin)
            {
                e.Handled = false;
                return;
            }
            int PremoteVal = 0;
            if ((bool)ModPremote.IsChecked)
                PremoteVal = 1;
            if ((bool)UserPremote.IsChecked)
                PremoteVal = 0;
            Premote(PremoteUserName.Text, PremoteVal);
        }
        private void button7_Click(object sender, RoutedEventArgs e) {Ban(ReportUserName.Text, banTime.DisplayDate, (bool)checkBox1.IsChecked);}
        private void OnlineUsers_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ListViewItem item = (ListViewItem)sender;
            PremoteUserName.Text = (string)item.Content;
        }
        private void ModList_SelectionChanged(object sender, SelectionChangedEventArgs e) { PremoteUserName.Text = (string)sender; }
        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var report = (Reports)sender;
            ReportUserName.Text = report.Account;
        }
        private void button10_Click(object sender, RoutedEventArgs e)
        {
            var report = Reports.FirstOrDefault(r => r.Account == ReportUserName.Text);
            if (report == null) return;
            RemoveReportCallback(report.ID);
        }


        #endregion

        private void button9_Click(object sender, RoutedEventArgs e) {Exit();}

        #region DIFFICULTY SELECTION
        private void radioButton3_Checked(object sender, RoutedEventArgs e){ mode = Tools.Difficulty.Normal; }
        private void radioButton4_Checked(object sender, RoutedEventArgs e){ mode = Tools.Difficulty.Nightmare; }
        private void radioButton5_Checked(object sender, RoutedEventArgs e){ mode = Tools.Difficulty.Hell; }
        #endregion

        #region D2NG UI
        private void textBox5_GotFocus(object sender, RoutedEventArgs e)
            { TextBox box = (TextBox)sender; if (box.Text == "Account") box.Text = ""; }
        private void textBox5_LostFocus(object sender, RoutedEventArgs e)
            { TextBox box = (TextBox)sender; if (box.Text == "") box.Text = "Account"; }
        private void passwordBox_GotFocus(object sender, RoutedEventArgs e)
            { D2NGPasswordPlaceHolder.Visibility = Visibility.Hidden; }
        private void passwordBox_LostFocus(object sender, RoutedEventArgs e)
            { if (passwordBox.Password == "") D2NGPasswordPlaceHolder.Visibility = Visibility.Visible;}
        private void textBox10_GotFocus(object sender, RoutedEventArgs e)
            { if (textBox10.Text == "Email") textBox10.Text = ""; }
        private void textBox10_LostFocus(object sender, RoutedEventArgs e)
            { if (textBox10.Text == "") textBox10.Text = "Email"; }
        private void passwordBox1_GotFocus(object sender, RoutedEventArgs e)
            { label6.Visibility = Visibility.Hidden; }
        private void passwordBox1_LostFocus(object sender, RoutedEventArgs e)
            { if (passwordBox1.Password == "")  label6.Visibility = Visibility.Visible;}
        private void passwordBox2_GotFocus(object sender, RoutedEventArgs e)
            {label7.Visibility = Visibility.Hidden;}
        private void passwordBox2_LostFocus(object sender, RoutedEventArgs e)
            { if (passwordBox2.Password == "") label7.Visibility = Visibility.Visible; }
        private void gameNamelostFocus(object sender, RoutedEventArgs e)
            { TextBox txt = (TextBox)sender; if (txt.Text == "") txt.Text = "Game name"; }
        private void gameNameGotfocus(object sender, RoutedEventArgs e)
            { TextBox txt = (TextBox)sender; if (txt.Text == "Game name") txt.Text = "";}
        private void textBox2_GotFocus(object sender, RoutedEventArgs e)
            { if (textBox2.Text == "Message") textBox2.Text = "";}
        private void textBox2_LostFocus(object sender, RoutedEventArgs e)
            { if (textBox2.Text == "") textBox2.Text = "Message";}
        private void passwordGotFocus(object sender, RoutedEventArgs e)
            { PasswordPlaceholder.Visibility = Visibility.Hidden; }
        private void passwordLostFocus(object sender, RoutedEventArgs e)
            { TextBox txt = (TextBox)sender; if (txt.Text == "") PasswordPlaceholder.Visibility = Visibility.Visible; }
        private void Password_GotFocus(object sender, RoutedEventArgs e)
        { label10.Visibility = Visibility.Hidden; }
        private void Password_LostFocus(object sender, RoutedEventArgs e)
        { if (Password.Text == "") label10.Visibility = Visibility.Visible; }

        private void Description_LostFocus(object sender, RoutedEventArgs e)
        { if (Description.Text == "") label11.Visibility = Visibility.Visible; }
        private void Description_GotFocus(object sender, RoutedEventArgs e)
        { label11.Visibility = Visibility.Hidden; }
        private void textBox3_GotFocus(object sender, RoutedEventArgs e)
        { label12.Visibility = Visibility.Hidden; }
        private void textBox3_LostFocus(object sender, RoutedEventArgs e)
        { if (textBox3.Text == "") label12.Visibility = Visibility.Visible; }
        #endregion

        #region D2NG Session
        private void button11_Click(object sender, RoutedEventArgs e) {D2NGLogin(textBox5.Text, passwordBox.Password, Realm); }

        private void button12_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox1.Password == passwordBox2.Password)
                D2NGRegister(textBox6.Text, passwordBox1.Password, textBox10.Text);
            else
                label.Visibility = Visibility.Visible;
        }

        #endregion

        private void button13_Click(object sender, RoutedEventArgs e) {CallLogout();}
        private void FriendsList_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var ListView = (ListView)sender;
            var Data = (InGame.FriendItem)ListView.SelectedItem;
            if (Data == null) return;
            FrameworkElement fe = e.Source as FrameworkElement;
            fe.ContextMenu = BuildMenu(Data.TypeOfFrirend);
        }
        ContextMenu BuildMenu(Tools.FriendType friend)
        {
            ContextMenu Menu = new ContextMenu();
            
            switch (friend)
            {
                case Tools.FriendType.Mutal:
                    MenuItem JoinFriendGame = new MenuItem();
                    JoinFriendGame.Header = "Join Game";
                    JoinFriendGame.Click += JoinFriendgame;
                    Menu.Items.Add(JoinFriendGame);
                    goto case Tools.FriendType.Owner;
                case Tools.FriendType.Owner:
                    MenuItem Removefriend = new MenuItem();
                    Removefriend.Header = "Remove Friend";
                    Removefriend.Click += RemoveFriend;
                    Menu.Items.Add(Removefriend);
                    return Menu;
                case Tools.FriendType.Slave:
                    MenuItem AcceptFriendReq = new MenuItem();
                    AcceptFriendReq.Header = "Accept Friend";
                    AcceptFriendReq.Click += AcceptFriendReq_Click;
                    Menu.Items.Add(AcceptFriendReq);
                    MenuItem Declinefriend = new MenuItem();
                    Declinefriend.Header = "Decline Friend";
                    Declinefriend.Click += RemoveFriend;
                    Menu.Items.Add(Declinefriend);
                    return Menu;
                default: return Menu;
            }
            
        }
        private void AcceptFriendReq_Click(object sender, RoutedEventArgs e)
        {
            var friend = InGame.FriendsList.Friendslist.FirstOrDefault(f => f.Selected);
            if (friend == null) return;
            AddFriend(friend.Account);
        }
        private void RemoveFriend(object sender, RoutedEventArgs e)
        {
            var friend = InGame.FriendsList.Friendslist.FirstOrDefault(f => f.Selected);
            if (friend == null) return;
            RemoveFriendCall(friend.ID);
        }
        private void JoinFriendgame(object sender, RoutedEventArgs e)
        {
            var friend = InGame.FriendsList.Friendslist.FirstOrDefault(f => f.Selected);
            if (friend == null) return;
            if (friend.GameID == 0) return;
            D2NGJoinGame(friend.GameID);
        }
        private void FriendsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var F in InGame.FriendsList.Friendslist)
                F.Selected = false;
            var item = (ListView)sender;
            var Friend = (InGame.FriendItem)item.SelectedItem;
            Friend.Selected = true;
        }
        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)checkBox.IsChecked)
                Password.IsEnabled = false;
            else
                Password.IsEnabled = true;
        }
        private void textBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox2.Text == "/r" && D2NGMainWindow.LastWisper != "")
            {
                textBox2.Text = "/w " + D2NGMainWindow.LastWisper + " ";
                textBox2.SelectionStart = textBox2.Text.Length;
               
            }
                
            
        }

        string ReportNameHolder;
        private void ChannelUsers_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu Menu = new ContextMenu();
            MenuItem Report = new MenuItem();
            Report.Header = "Report";
            Report.Click += ReportClick;
            Menu.Items.Add(Report);
            ReportNameHolder = (string)ChannelUsers.SelectedItem;
            var ListView = (ListView)sender;
            var Data = (string)ListView.SelectedItem;
            if (Data == null) return;
            FrameworkElement fe = e.Source as FrameworkElement;
            fe.ContextMenu = Menu;
        }
        private void ReportClick(object sender, RoutedEventArgs e)
        {
            ReportName.Content = ReportNameHolder;
            ReportWindow.Visibility = Visibility.Visible;
        }
        private void SendReport_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow.Visibility = Visibility.Hidden;
            Tools.ReportReason reasons = Tools.ReportReason.Hacks;
            switch (Reasons.SelectedIndex)
            {
                case 0:
                    reasons = Tools.ReportReason.Hacks;
                    break;
                case 1:
                    reasons = Tools.ReportReason.Spam;
                    break;
                case 2:
                    reasons = Tools.ReportReason.Scam;
                    break;
            }
            Report(ReportNameHolder, reasons);
        }
        private void CloseReport_Click(object sender, RoutedEventArgs e)
        {
            ReportNameHolder = string.Empty;
            ReportName.Content = ReportNameHolder;
            ReportWindow.Visibility = Visibility.Hidden;
        }
    }
}
