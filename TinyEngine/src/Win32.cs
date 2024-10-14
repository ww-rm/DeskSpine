using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TinyEngine
{
    /// <summary>
    /// Win32 Sdk 包装类
    /// </summary>
    internal static class Win32
    {
        public const int GWL_STYLE = -16;
        public const int WS_SIZEBOX = 0x40000;
        public const int WS_BORDER = 0x800000;
        public const int WS_POPUP = unchecked((int)0x80000000);

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TOPMOST = 0x8;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int WS_EX_TOOLWINDOW = 0x80;
        public const int WS_EX_WINDOWEDGE = 0x100;
        public const int WS_EX_CLIENTEDGE = 0x200;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE;

        public const uint LWA_COLORKEY = 0x1;
        public const uint LWA_ALPHA = 0x2;

        public const IntPtr HWND_TOPMOST = -1;

        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_FRAMECHANGED = 0x0020;
        public const uint SWP_REFRESHLONG = SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED;

        public const int WM_SPAWN_WORKER = 0x052C; // 一个未公开的神秘消息

        public const uint SMTO_NORMAL = 0x0000;
        public const uint SMTO_BLOCK = 0x0001;
        public const uint SMTO_ABORTIFHUNG = 0x0002;
        public const uint SMTO_NOTIMEOUTIFNOTHUNG = 0x0008;

        public const uint GA_PARENT = 1;
        public const uint GW_OWNER = 4;

        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetLayeredWindowAttributes(IntPtr hWnd, ref uint crKey, ref byte bAlpha, ref uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint pcrKey, byte pbAlpha, uint pdwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetDoubleClickTime();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        public static TimeSpan GetLastInputElapsedTime()
        {
            LASTINPUTINFO lastInputInfo = new();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);

            uint idleTimeMillis = 1000;
            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint tickCount = (uint)Environment.TickCount;
                uint lastInputTick = lastInputInfo.dwTime;
                idleTimeMillis = tickCount - lastInputTick;
            }
            return TimeSpan.FromMilliseconds(idleTimeMillis);
        }

        public static IntPtr GetWorkerW()
        {
            var progman = FindWindow("Progman", null);
            if (progman == IntPtr.Zero)
                return IntPtr.Zero;
            IntPtr hWnd = IntPtr.Zero;
            do
            {
                hWnd = FindWindowEx(IntPtr.Zero, hWnd, "WorkerW", null);
                if (hWnd != IntPtr.Zero && GetWindow(hWnd, GW_OWNER) == progman)
                    break;
            } while (hWnd != IntPtr.Zero);
            return hWnd;
        }
    }
}
