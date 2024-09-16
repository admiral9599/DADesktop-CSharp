using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DriveAdviser.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using Ionic.Zip;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.ServiceLocation;

namespace DriveAdviser.UI.UserReport
{
   public class UserReportViewModel : ViewModelBase
    {
       
        private string _name;
        private string _errorText;
        private bool _isDialogOpen;
        public RelayCommand<IClosable> SendReportCommand { get; private set; }

        public UserReportViewModel()
        {
            


            //_window = window;
            SendReportCommand = new RelayCommand<IClosable>(SendReport);
        }

        private async void SendReport(IClosable window)
        {
            IsDialogOpen = true;
            using (var zip = new ZipFile())
            {
                zip.AddDirectory(Constants.IniFolder);
                zip.Save("zip.zip");
            }
            await Task.Delay(5000);
            window.Close();
        }


        //private async Task SendReport(IClosable window)
        //{
        //    IsDialogOpen = true;
        //    using (var zip = new ZipFile())
        //    {
        //        zip.AddDirectory(Constants.IniFolder);
        //        zip.Save("zip.zip");
        //    }

            

        //}

        public string Name
        {
            get { return _name; }
            set { Set(() => Name, ref _name, value); }
        }

        public string ErrorText
        {
            get { return _errorText; }
            set { Set(() => ErrorText, ref _errorText, value); }
        }

        public bool IsDialogOpen
        {
            get { return _isDialogOpen; }
            set { Set(() => IsDialogOpen, ref _isDialogOpen, value); }
        }
    }
}
