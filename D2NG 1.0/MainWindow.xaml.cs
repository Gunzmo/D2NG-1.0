using D2NG;
using D2NG.UI;
using D2NG_Final.Pointers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace D2NG_1._0
{
    /// <summary>
    /// Ingame Logic aswell as Friends Class
    /// </summary>
    /// TODO
    /// Fix Crashes at start!
 
    public partial class MainWindow : Window
    {


        private enum ResizeDirection
        {
            Left = 61441,
            Right = 61442,
            Top = 61443,
            TopLeft = 61444,
            TopRight = 61445,
            Bottom = 61446,
            BottomLeft = 61447,
            BottomRight = 61448,
        }
        public D2NGMainWindow LobbyWinow;
        public static IntPtr GameHandler;

        public MainWindow()
        {
            InitializeComponent();
            LobbyWinow = new D2NGMainWindow();
            LobbyWinow.TopMost += (top) => {
                Dispatcher.Invoke(() => {
                    this.Topmost = top;
                });
            };
            LobbyWinow.Show();
            Hide();
           
        }

        #region Rezise
        private void ResizeWindow(ResizeDirection direction)
            {D2NG.Tools.WinAPI.SendMessage(new WindowInteropHelper(this).Handle, D2NG.Tools.WinAPI.WM_SYSCOMMAND, (IntPtr)direction, IntPtr.Zero);}
        protected void ResetCursor(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                this.Cursor = Cursors.Arrow;
            }
        }
        protected void Resize(object sender, MouseButtonEventArgs e)
        {
            var clickedShape = sender as System.Windows.Shapes.Shape;

            switch (clickedShape.Name)
            {
                case "ResizeN":
                    this.Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "ResizeE":
                    this.Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "ResizeS":
                    this.Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "ResizeW":
                    this.Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "ResizeNW":
                    this.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "ResizeNE":
                    this.Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "ResizeSE":
                    this.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                case "ResizeSW":
                    this.Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                default:
                    break;
            }
        }
        protected void DisplayResizeCursor(object sender, MouseEventArgs e)
        {
            var clickedShape = sender as System.Windows.Shapes.Shape;

            switch (clickedShape.Name)
            {
                case "ResizeN":
                case "ResizeS":
                    this.Cursor = Cursors.SizeNS;
                    break;
                case "ResizeE":
                case "ResizeW":
                    this.Cursor = Cursors.SizeWE;
                    break;
                case "ResizeNW":
                case "ResizeSE":
                    this.Cursor = Cursors.SizeNWSE;
                    break;
                case "ResizeNE":
                case "ResizeSW":
                    this.Cursor = Cursors.SizeNESW;
                    break;
                default:
                    break;
            }
        }
        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                DragMove();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Point relativePoint = GameLocation.TransformToAncestor(this).Transform(new Point(0, 0));
            D2NG.Tools.WinAPI.MoveWindow(GameHandler, (int)relativePoint.X, (int)relativePoint.Y, (int)GameLocation.ActualWidth, (int)GameLocation.ActualHeight, false);
        }
        bool minMax = false;
        private void label1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(!minMax)
            {
                minMax = true;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                minMax = false;
                WindowState = WindowState.Normal;
            }
        }
        private void label2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { WindowState = WindowState.Minimized; }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                label1_MouseLeftButtonDown(sender, e);
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {e.Cancel = true;}

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (LobbyWinow.core != null)
                LobbyWinow.core.InvokeKeyPress((System.Windows.Forms.Keys)KeyInterop.VirtualKeyFromKey(e.Key));
        }
    }
}
