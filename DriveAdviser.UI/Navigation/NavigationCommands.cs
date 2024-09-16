using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveAdviser.UI.Navigation
{
   public static class NavigationCommands
    {
        public static RoutedCommand ShowDriveCommand = new RoutedCommand();
        public static RoutedCommand GoBackCommand = new RoutedCommand();
    }
}
