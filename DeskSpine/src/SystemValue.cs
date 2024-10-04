using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeskSpine
{
    /// <summary>
    /// 任务栏方向枚举
    /// </summary>
    public enum EdgeDirection
    {
        Left = 0,       // ABE_LEFT
        Top = 1,        // ABE_TOP
        Right = 2,      // ABE_RIGHT
        Bottom = 3      // ABE_BOTTOM
    }

    /// <summary>
    /// 一些系统变量辅助方法
    /// </summary>
    public static class SystemValue
    {
        /// <summary>
        /// 获取 %LOCALAPPDATA% 目录路径
        /// </summary>
        public static string LocalAppdataDirectory { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        /// <summary>
        /// 获取系统主题颜色
        /// </summary>
        public static bool SystemUseLightTheme
        {
            get
            {
                using (RegistryKey personalizeKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                    return int.Parse(personalizeKey.GetValue("SystemUsesLightTheme", "0").ToString()) != 0;
            }
        }

        /// <summary>
        /// 任务栏方向
        /// </summary>
        public static EdgeDirection TaskbarDirection
        {
            get
            {
                // ABM_GETTASKBARPOS = 0x5
                APPBARDATA abData = new APPBARDATA();
                abData.cbSize = Marshal.SizeOf(abData);
                if (SHAppBarMessage(5, ref abData) != 0)
                {
                    return (EdgeDirection)abData.uEdge;
                }
                return EdgeDirection.Bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern uint SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);
    }
}
