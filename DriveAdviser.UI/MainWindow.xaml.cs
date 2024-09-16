using DriveAdviser.Core;
using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using DriveAdviser.Core.Helpers;
using Serilog;


namespace DriveAdviser.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IOpenable
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
           
            Log.Debug("Hiding Window");

            Visibility = Visibility.Hidden;
            Log.Debug("Flushing Memory");
            FlushMemory();
           
            e.Cancel = true;
        }

        protected override void OnClosed(EventArgs e)
        {
           

            base.OnClosed(e);

           
            //var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            //if (hwndSource != null)
            //{
            //    hwndSource.RemoveHook(WndProc);
            //}
        }

        public void Open()
        {
            try
            {
                Visibility = Visibility.Visible;
                FlushMemory();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int SetProcessWorkingSetSize(IntPtr hProcess, int dwMinimumWorkingSetSize,
            int dwMaximumWorkingSetSize);

        public void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            //IntPtr windowHandle = (new WindowInteropHelper(this)).Handle;
            //HwndSource src = HwndSource.FromHwnd(windowHandle);
            //src.AddHook(new HwndSourceHook(WndProc));

            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(WndProc);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg != Interop.WM_SHOWME) return IntPtr.Zero;
            Visibility = Visibility.Visible;
            Log.Debug("Message Received - Opening Window");
            FlushMemory();

            return IntPtr.Zero;
        }

       
        
    }
}

