using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace D2NG
{
    public class MessageHelper
    {
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL,
            SMTO_BLOCK,
            SMTO_ABORTIFHUNG,
            SMTO_NOTIMEOUTIFNOTHUNG = 8u
        }

        public struct COPYDATASTRUCT
        {
            public System.IntPtr dwData;

            public int cbData;

            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]
            public string lpData;
        }

        public const int SW_HIDE = 0;

        public const int SW_SHOW = 1;

        public const int SW_MINIMIZE = 6;

        public const int WM_USER = 1024;

        public const int WM_COPYDATA = 74;

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern int RegisterWindowMessage(string lpString);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool ShowScrollBar(System.IntPtr hWnd, int wBar, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)] bool bShow);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(System.IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        [System.Runtime.InteropServices.DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        internal static extern void MoveWindow(System.IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool SetWindowPos(System.IntPtr hWnd, System.IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern System.IntPtr FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int ShowWindow(int hwnd, int command);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern int SendMessage(System.IntPtr hWnd, int Msg, int wParam, ref MessageHelper.COPYDATASTRUCT lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool DestroyWindow(System.IntPtr hwnd);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern int SendMessage(System.IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern System.IntPtr SendMessageTimeout(System.IntPtr windowHandle, uint Msg, System.IntPtr wParam, ref MessageHelper.COPYDATASTRUCT lParam, MessageHelper.SendMessageTimeoutFlags flags, uint timeout, out System.IntPtr result);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern System.IntPtr SendMessageTimeout(System.IntPtr windowHandle, uint Msg, System.IntPtr wParam, System.IntPtr lParam, MessageHelper.SendMessageTimeoutFlags flags, uint timeout, out System.IntPtr result);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(System.IntPtr hWnd);

        public bool bringAppToFront(System.IntPtr hWnd)
        {
            return MessageHelper.SetForegroundWindow(hWnd);
        }

        public static int SendStringMessageToHandle(System.IntPtr hWnd, int wParam, string msg)
        {
            int result = 0;
            if (hWnd != System.IntPtr.Zero)
            {
                byte[] sarr = System.Text.Encoding.Default.GetBytes(msg);
                int len = sarr.Length;
                MessageHelper.COPYDATASTRUCT cds;
                cds.dwData = (System.IntPtr)100;
                cds.lpData = msg;
                cds.cbData = len + 1;
                result = MessageHelper.SendMessage(hWnd, 74, wParam, ref cds);
            }
            return result;
        }

        public int SendMessageToHandle(System.IntPtr hWnd, int Msg, int wParam, int lParam)
        {
            int result = 0;
            if (hWnd != System.IntPtr.Zero)
            {
                result = MessageHelper.SendMessage(hWnd, Msg, wParam, lParam);
            }
            return result;
        }

        public System.IntPtr FindWindowByName(string className, string windowName)
        {
            return MessageHelper.FindWindow(className, windowName);
        }
    }
}
