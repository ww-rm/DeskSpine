using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeskSpine
{
    public class ShellNotifyIcon
    {
        /// <summary>
        /// 关联的窗口句柄
        /// </summary>
        public IntPtr Handle { get; private init; }

        /// <summary>
        /// 标识符
        /// </summary>
        public uint ID { get; private init; }

        public ShellNotifyIcon(NotifyIcon notifyIcon)
        {
            Type t = typeof(NotifyIcon);
            var windowField = t.GetField("_window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var idField = t.GetField("_id", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Handle = ((NativeWindow?)windowField?.GetValue(notifyIcon))?.Handle ?? 0;
            ID = (uint?)idField?.GetValue(notifyIcon) ?? 0;
        }

        /// <summary>
        /// 图标位置
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                Win32.RECT rect = new();
                Win32.NOTIFYICONIDENTIFIER niid = new();
                niid.cbSize = (uint)Marshal.SizeOf(niid);
                niid.hWnd = Handle;
                niid.uID = ID;
                Win32.Shell_NotifyIconGetRect(ref niid, ref rect);
                return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            }
        }

        /// <summary>
        /// 弹出气泡消息
        /// </summary>
        public bool ShowBalloonTip(string title, string info, IntPtr balloonIcon)
        {
            title ??= "";
            info ??= "";
            if (title.Length >= 64) title = title.Substring(0, 63);
            if (info.Length >= 256) info = info.Substring(0, 255);
            Win32.NOTIFYICONDATA nid = new();
            nid.cbSize = (uint)Marshal.SizeOf(nid);
            nid.hWnd = Handle;
            nid.uID = ID;
            nid.szInfoTitle = title;
            nid.szInfo = info;
            nid.hBalloonIcon = balloonIcon;
            nid.uFlags |= (Win32.NIF_INFO | Win32.NIF_REALTIME);
            nid.dwInfoFlags |= (Win32.NIIF_USER | Win32.NIIF_LARGE_ICON);
            return Win32.Shell_NotifyIcon(Win32.NIM_MODIFY, ref nid);
        }
    }

    internal static class Win32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NOTIFYICONDATA
        {
            public uint cbSize;                   // DWORD -> uint
            public IntPtr hWnd;                   // HWND -> IntPtr
            public uint uID;                      // UINT -> uint
            public uint uFlags;                   // UINT -> uint
            public uint uCallbackMessage;         // UINT -> uint
            public IntPtr hIcon;                  // HICON -> IntPtr

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTip;                  // WCHAR[128]

            public uint dwState;                  // DWORD -> uint
            public uint dwStateMask;              // DWORD -> uint

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szInfo;                 // WCHAR[256]

            public uint uTimeoutOrVersion;        // Union (uTimeout or uVersion)

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szInfoTitle;            // WCHAR[64]

            public uint dwInfoFlags;              // DWORD -> uint
            public Guid guidItem;                 // GUID -> Guid
            public IntPtr hBalloonIcon;           // HICON -> IntPtr
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NOTIFYICONIDENTIFIER
        {
            public uint cbSize;                   // DWORD -> uint
            public IntPtr hWnd;                   // HWND -> IntPtr
            public uint uID;                      // UINT -> uint
            public Guid guidItem;                 // GUID -> Guid
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public const uint NIM_ADD = 0;
        public const uint NIM_MODIFY = 1;
        public const uint NIM_DELETE = 2;
        public const uint NIM_SETFOCUS = 3;
        public const uint NIM_SETVERSION = 4;

        public const uint NIF_MESSAGE = 0x00000001;     // uCallbackMessage
        public const uint NIF_ICON = 0x00000002;        // hIcon
        public const uint NIF_TIP = 0x00000004;         // szTip
        public const uint NIF_STATE = 0x00000008;       // dwState
        public const uint NIF_INFO = 0x00000010;        // szInfo、szInfoTitle、dwInfoFlags
        public const uint NIF_GUID = 0x00000020;        // guidItem
        public const uint NIF_REALTIME = 0x00000040;    // 设置/获取图标的实时状态
        public const uint NIF_SHOWTIP = 0x00000080;     // 标准工具提示

        public const uint NIIF_NONE = 0;
        public const uint NIIF_INFO = 1;
        public const uint NIIF_WARNING = 2;
        public const uint NIIF_ERROR = 3;
        public const uint NIIF_USER = 4;
        public const uint NIIF_NOSOUND = 0x00000010;
        public const uint NIIF_LARGE_ICON = 0x00000020;
        public const uint NIIF_RESPECT_QUIET_TIME = 0x00000080;

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern int Shell_NotifyIconGetRect(ref NOTIFYICONIDENTIFIER identifier, ref RECT iconLocation);

    }
}
