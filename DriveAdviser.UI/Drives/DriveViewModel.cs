using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveAdviser.Core.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Serilog;

namespace DriveAdviser.UI.Drives
{
    public class DriveViewModel : ViewModelBase
    {
        private Drive _drive;
    
        
        public void Show(Drive drive)
        {
            if (drive == null) 
            {
                throw new ArgumentNullException(nameof(drive));
            }
            Drive = drive;
        }
        public Drive Drive
        {
            get { return _drive; }
            set { Set(() => Drive, ref _drive, value); }
        }
    }
}
