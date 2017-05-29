using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace D2NG.UI
{
    /// <summary>
    /// Interaction logic for D2NG.xaml
    /// </summary>
    public partial class D2NGMainWindow : Window
    {
        #region UI
        public Core core;
        D2Login d2Login;
        Lubby Lobby;
        CreateCharacter creatCharScreen;
        InGame.FriendsList InGameList;
        public static string LastWisper = string.Empty;
        public static ChattWindow InGameChatt;
        public static Hacks GameHacks;
        Connection connection;
        UI.characterscreen.CharacterScreen charScreen;
        public static Tools.CharacterInfo SelectedCharacter;
        public static bool ISD2NG = false;
        public delegate void _TopMost(bool TopMost);
        public event _TopMost TopMost;
        public D2NGMainWindow()
        {
            InitializeComponent();
            d2Login = new D2Login();
            connection = new Connection();
            Lobby = new Lubby();
            InGameChatt = new ChattWindow();
            creatCharScreen = new CreateCharacter();
            GameHacks = new Hacks();
            InGameList = new InGame.FriendsList();
            d2Login.StartD2 += D2Login_StartD2;
            Container.Children.Add(d2Login);
            connection.D2NGConnect += Connection_D2NGConnect;
            InGameList.Main_SelectFriend += Main_SelectFriend;
            connection.Connect();
        }
        private void Connection_D2NGConnect(bool Status)
        {

        }
        private void D2Login_StartD2()
        {
            System.Windows.Forms.OpenFileDialog openFiledialog = new System.Windows.Forms.OpenFileDialog();
            openFiledialog.Filter = "Diablo II|game.exe";
            if (openFiledialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var main = Application.Current.MainWindow as D2NG_1._0.MainWindow;
                ProcessStartInfo info = new ProcessStartInfo(openFiledialog.InitialDirectory + openFiledialog.FileName);
                info.UseShellExecute = true;
                info.Arguments = "-w";
                var diablo2 = Process.Start(info);
                core = new Core(diablo2, () => { this.Close(); }, Process.GetCurrentProcess().MainWindowHandle);
                d2Login.D2Register += core.Register;
                d2Login.Login += core.LoginBnet;
                core.Callback += Core_CallbackUpdateUI;
                GameHacks.Send += core.SendPacket;
                GameHacks.Receive += core.ReceivePacket;
                GameHacks.RevealMap += core.RevealMap;
                InGameChatt.SendMSG += InGameChatt_SendMSG;
                connection.AddGame += (Tools.D2NGGames[] Data) => { Lobby.Refreshgames(false, Data); };
                connection.OnlineUser += Lobby.AddOnlineUsers;
                connection.RemoveOnlineUser += Lobby.RemoveOnlineUsers;
                connection.JoinGame += core.JoinGame;
                connection.ISMOD += Lobby.ISMod;
                connection.AddMod += Lobby.AddMod;
                connection.RegisterC += Lobby.Register;
                connection.UpdateGame += Lobby.UpdateGame;
                connection.LoginCallback += Lobby.Login;
                connection.LogoutCallback += Lobby.Logout;
                connection.RemoveGame += Lobby.RemoveD2NGGame;
                connection.Message += (msg, id) =>
                {
                    if (id == -1 && !core.InGame || id == -2 && !core.InGame || id == 0 && !core.InGame)
                        Lobby.AddMessage(msg);
                    else if (id == -2 && core.InGame || id == 0 && core.InGame)
                        InGameChatt.AddMessage(msg);
                    if (id >= 1)
                    {
                        int pFrom = msg.IndexOf("[") + "[".Length;
                        int pTo = msg.LastIndexOf("]");
                        LastWisper = msg.Substring(pFrom, pTo - pFrom);
                        var Friend = UI.InGame.FriendsList.Friendslist.FirstOrDefault(f => f.ID == id);
                        if (Friend != null)
                        {
                            lock (Friend.Messages)
                                Friend.Messages.Add(msg);
                            if (core.InGame)
                            {
                                InGameChatt.AddMessage(msg);
                                InGameList.AnimateBackGround(id);
                            }
                            else
                                Lobby.AddMessage(msg);
                        }
                        else
                            InGameChatt.AddMessage(msg);
                    }
                };
                connection.FriendGame += (ID, GameID, GameName) =>
                {
                    InGame.FriendItem Friend;
                    lock (InGame.FriendsList.Friendslist)
                        Friend = InGame.FriendsList.Friendslist.FirstOrDefault(f => f.ID == ID);
                    if (Friend == null) return;
                    Friend.UpdateStatus(GameName, GameID);
                    if (core.InGame)
                        InGameList.UpdateFriendList();
                };
                connection.AddFriendCall += (InGame.FriendItem Friend) =>
                {
                    var Check = InGame.FriendsList.Friendslist.FirstOrDefault(f => f.ID == Friend.ID);
                    if (Check == null)
                        lock (InGame.FriendsList.Friendslist)
                            InGame.FriendsList.Friendslist.Add(Friend);
                    else
                        Check = Friend;
                    Lobby.UpdateFriends();
                };
                connection.FriendOnlineStatus += (ID, status) =>
                {
                    var Check = InGame.FriendsList.Friendslist.FirstOrDefault(f => f.ID == ID);
                    if (Check == null) return;
                    Check.UpdateFriendStatus(status);
                    if (core.InGame)
                       InGameList.UpdateFriendList();
                };
                connection.RequestChannelUsers += Lobby.AddChannelUser;
                creatCharScreen.Create += core.CreateCharacter;
                creatCharScreen.Back += () => { core.InvokeKeyPress(System.Windows.Forms.Keys.Escape); };
                Lobby.Exit += () => { core.InvokeKeyPress(System.Windows.Forms.Keys.Escape); };
                Lobby.RefreshGames += (bnet, diff) => { if (bnet) core.RefreshBnetGames(); else connection.RequestGames(diff); };
                Lobby.D2NGJoinGame += connection.SendJoin;
                Lobby.D2NGRegister += connection.Register;
                Lobby.D2NGLogin += connection.Login;
                Lobby.Ban += connection.BanUser;
                Lobby.Premote += connection.PremoteUser;
                Lobby.SendMSG += connection.SendMSG;
                Lobby.Report += connection.Report;
                Lobby.AddFriend += connection.AddFriend;
                Lobby.RemoveFriendCall += connection.RemoveFriend;
                Lobby.CreateGame += core.CreateGame;
                Lobby.RemoveReportCallback += connection.RemoveReport;
                Lobby.CallLogout += connection.Loggout;
                Lobby.JoinGame += core.JoinGame;
                core.Start();
                InGameList.AnimateBackGround(0);
            }
        }
        private void InGameChatt_SendMSG(string MSG)
        {
            if(MSG.Substring(0, 2) != "/w")
            { 
                var friend = InGame.FriendsList.Friendslist.FirstOrDefault(f => f.Selected);
                MSG = "/w " + friend.Account + " " + MSG;
                connection.SendMSG(MSG);
            }
            else
                connection.SendMSG(MSG);
        }
        internal void Main_SelectFriend()
        {
            InGameChatt.ClearChat();
            var fa = InGame.FriendsList.Friendslist.FirstOrDefault(f => f.Selected);
            foreach (var msg in fa.Messages)
                InGameChatt.AddMessage(msg);
        }
        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                DragMove();
        }
        #endregion
        private void Core_CallbackUpdateUI(int type, dynamic data)
        {
            switch (type)
            {
                case 1:
                    Dispatcher.Invoke(() =>
                    {
                        charScreen = new characterscreen.CharacterScreen((Tools.CharacterList)data);
                        charScreen.CreateCharacter += () => { core.InvokeMouseClick(50, 500); };
                        charScreen.ConvertToExpansion += core.ConvertToExpansion;
                        Container.Children.Clear();
                        Container.Children.Add(charScreen);
                        charScreen.Select += CharScreen_Select;
                        charScreen.Back += () => {
                            core.InvokeKeyPress(System.Windows.Forms.Keys.Escape);
                            Container.Children.Clear();
                            Container.Children.Add(d2Login);
                            Lobby.SetDifficulty();
                        };
                    });
                    return;
                case 3:
                    Dispatcher.Invoke(() =>
                    {
                        Container.Children.Clear();
                        Container.Children.Add(Lobby);
                        Lobby.SetDifficulty();
                    });
                    return;
                case 4:
                    Dispatcher.Invoke(() => { Lobby.Refreshgames(true, (Tools.GameList)data); });
                    return;
                case 5:
                    if (!(bool)data[0]) ISD2NG = false;
                    if (ISD2NG && (bool)data[0]) connection.CreateGame((string)data[1], (string)data[2], (bool)data[3], (string)data[4], Convert.ToInt32(data[5]), (Tools.Difficulty)data[6]);
                    Dispatcher.Invoke(() =>
                    {
                        if((bool)data[0])
                        {
                            Hide();
                            InGameChatt.Show();
                            GameHacks.Show();
                            InGameList.Show();
                            InGameChatt.Owner = Application.Current.MainWindow;
                            GameHacks.Owner = Application.Current.MainWindow;
                            InGameList.Owner = Application.Current.MainWindow;
                            Application.Current.MainWindow.Show();
                        }
                        else
                        {
                            Show();
                            InGameList.Hide();
                            InGameChatt.Hide();
                            GameHacks.Hide();
                            Application.Current.MainWindow.Hide();
                        }
                    });
                    return;
                case 7:
                    Dispatcher.Invoke(() =>
                    {
                        if (data)
                            Application.Current.MainWindow.Show();
                        else
                            Application.Current.MainWindow.Hide();

                        d2Login.button1.Visibility = Visibility.Hidden;
                        d2Login.SessionPanel.Visibility = Visibility.Visible;
                    });
                    return;
                case 8:
                    Dispatcher.Invoke(() =>
                    {
                        if (core.InGame && Application.Current.MainWindow.WindowState != WindowState.Minimized)
                        {
                            UPDATEINGAMECHATT();
                            InGameChatt.Show();
                        }
                        if (Application.Current.MainWindow.WindowState == WindowState.Minimized || Application.Current.MainWindow.Visibility == Visibility.Hidden)
                            InGameChatt.Hide();
                    });
                    return;
                case 9:
                    connection.SendJoin();
                    return;
                case 10:
                    connection.SendLeave();
                    return;
                case 11:
                    Lobby.InvalidGame((Tools.JoinGameResult)data);
                    return;
                case 12:
                    Lobby.InvalidGame((Tools.CreateGameResult)data);
                    break;
                case 13:
                    Dispatcher.Invoke(() =>
                    {
                        Container.Children.Clear();
                        Container.Children.Add(creatCharScreen);
                    });
                    break;
                case 14:
                    creatCharScreen.DisplayError((Tools.RealmCharacterActionResult)data);
                    break;
                case 15:
                    charScreen.ShowError((Tools.RealmStartupResult)data);
                    break;
                case 16:
                    Lobby.AddMessage(data);
                    break;
            }
        }
        public void UPDATEINGAMECHATT()
        {
            RECT rect;
            Tools.WinAPI.GetWindowRect(D2NG_1._0.MainWindow.GameHandler, out rect);
            InGameChatt.Left = rect.Left;
            InGameChatt.Top = rect.Bottom - 275;
            GameHacks.Left = rect.Left;
            GameHacks.Top = rect.Bottom - 475;
            InGameList.Left = rect.Left;
            InGameList.Top = rect.Top + 10;
            InGameList.SetWH(rect.Width / 4,rect.Height - 10);

            if (Tools.WinAPI.GetForegroundWindow() == D2NG_1._0.MainWindow.GameHandler)
            {
                TopMost(true);
                InGameChatt.Topmost = true;
                GameHacks.Topmost = true;
                InGameList.Topmost = true;
            }
            else
            {
                TopMost(false);
                InGameList.Topmost = false;
                InGameChatt.Topmost = false;
                GameHacks.Topmost = false;
            }
        }

        #region CharacterScreen
        private void CharScreen_Select(int x, int y, Tools.CharacterInfo CharInfo) { core.InvokeMouseClick(x, y); SelectedCharacter = CharInfo; if (connection.Logged) connection.UpdateCharacter(); }
        #endregion

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x4A)
            {
                Type type = new MessageHelper.COPYDATASTRUCT().GetType();
                MessageHelper.COPYDATASTRUCT copydatastruct = (MessageHelper.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, type);
                try
                {
                    IntPtr num = new IntPtr(wParam.ToInt32());
                    string str = string.Copy(copydatastruct.lpData);
                    core.HandleMessage(num, str);
                }
                catch { }
            }
            return IntPtr.Zero;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) { if (core != null) core.Dispose(); Application.Current.Shutdown(); }
        private void label_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) { }
        private void label_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) { this.Close(); }

        bool hidden = true;
        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (hidden)
                Application.Current.MainWindow.Show();
            else
                Application.Current.MainWindow.Hide();
            hidden = !hidden;
        }
    }
}
