using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DriveAdviser.Core.Helpers
{
  
    public static class Interop
    {
       
        public const int HWND_BROADCAST = 0xffff;

        public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);


        [DllImport("user32", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string cls, string win);

        [DllImport("user32")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32")]
        public static extern bool OpenIcon(IntPtr hWnd);

        [DllImport("kernel32.dll")]
        private static extern void RtlZeroMemory(IntPtr dst, int length);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        public static void ZeroMemoryPointer(IntPtr pointer, int pointerSize)
        {
            RtlZeroMemory(pointer,pointerSize);
        }
    }


}
