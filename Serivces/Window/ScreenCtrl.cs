using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Serivces.Window
{
    internal class ScreenCtrl
    {
        public ScreenCtrl(System.Windows.Window window)
        {
            IntPtr mWindowHandle = (new WindowInteropHelper(window)).Handle;
            HwndSource.FromHwnd(mWindowHandle).AddHook(new HwndSourceHook(WindowProc));
        }
        private static System.IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        static MONITORINFO? lPrimaryScreenInfo;

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            POINT lMousePosition;
            GetCursorPos(out lMousePosition);

            IntPtr lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
            lPrimaryScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
            {
                return;
            }

            IntPtr lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);

            var tempMmI = Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            if(tempMmI is MINMAXINFO lMmi)
            {
                if (lPrimaryScreen.Equals(lCurrentScreen) == true)
                {
                    lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                    lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                    lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
                    lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
                }
                else
                {
                    lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
                    lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
                    lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right - lPrimaryScreenInfo.rcMonitor.Left;
                    lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom - lPrimaryScreenInfo.rcMonitor.Top;
                }

                Marshal.StructureToPtr(lMmi, lParam, true);
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }


        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }
    }
}
