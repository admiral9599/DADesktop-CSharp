using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DriveAdviser.Core;
using DriveAdviser.Core.Services;
using DriveAdviser.Services;
using DriveAdviser.UI.Drives;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows;
using System.Windows.Input;
using DriveAdviser.Core.Models;
using DriveAdviser.UI.Navigation;
using DriveAdviser.UI.UserReport;
using Microsoft.Practices.ServiceLocation;
using NavigationCommands = DriveAdviser.UI.Navigation.NavigationCommands;

namespace DriveAdviser.UI
{
    public class MainWindowViewModel : ViewModelBase, ISlideNavigationSubject
    {
        private int _activeSlideIndex;
        private bool _isChildWindowOpen;
        private string _alertText;
        private string _emailAddress;
        private readonly SlideNavigator _slideNavigator;
        public RelayCommand SaveEmailAddressCommand { get; private set; }
        public RelayCommand ExitProgramCommand{ get; private set; }
        public RelayCommand<IOpenable> OpenWindowCommand { get; private set; }
        public RelayCommand OpenWebsiteCommand { get; }
        public RelayCommand OpenUserReportCommand { get; private set; }
        //public RelayCommand<Drive> ShowDriveCommand { get; }
    
        private Version _version;

        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        public string AlertText
        {
            get { return _alertText; }
            set { _alertText = value; }
        }

        public Version Version
        {
            get { return _version; }
            set { Set(() => Version, ref _version, value); }
        }

        //private readonly SlideNavigator _slideNavigator;
        public bool IsChildWindowOpen
        {
            get { return _isChildWindowOpen; }
            set { _isChildWindowOpen = value; }
        }

        public MainWindowViewModel()
        {
            //Because MVVMLight does not support navigation for WPF we have to do our own. Check the Navigation Folder for all the classes.
            // We are not using Relay
            CommandManager.RegisterClassCommandBinding(typeof(MainWindow),new CommandBinding(NavigationCommands.ShowDriveCommand, ShowDriveExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(NavigationCommands.GoBackCommand,GoBackExecuted));
            //ShowDriveCommand = new RelayCommand<Drive>(ShowDrive);
            Version = Assembly.GetExecutingAssembly().GetName().Version;
            Slides = new object[] { DriveOverviewModel,DriveViewModel };
            _slideNavigator = new SlideNavigator(this, Slides);
            _slideNavigator.GoTo(0);

           
            ExitProgramCommand = new RelayCommand(ExitProgram);
            OpenWindowCommand = new RelayCommand<IOpenable>(OpenWindow);
            OpenWebsiteCommand = new RelayCommand(OpenWebsite);
            OpenUserReportCommand = new RelayCommand(OpenUserReport);




        }

        private void OpenUserReport()
        {
            var report = new UserReportView();
            report.Show();
        }


        private void GoBackExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _slideNavigator.GoBack();
        }

        private void ShowDriveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _slideNavigator.GoTo(
                IndexOfSlide<DriveViewModel>(),
                () => DriveViewModel.Show((Drive)e.Parameter));
        }

        private void OpenWindow(IOpenable window)
        {
            window?.Open();
        }

        private void ExitProgram()
        {

           Application.Current.Shutdown();
        }


        public object[] Slides { get; }

        public DriveOverviewViewModel DriveOverviewModel { get; } = new DriveOverviewViewModel(ServiceLocator.Current.GetInstance<IApi>());
        public DriveViewModel DriveViewModel { get; } = new DriveViewModel();
        
        public int ActiveSlideIndex
        {
            get { return _activeSlideIndex; }
            set { Set(() => ActiveSlideIndex, ref _activeSlideIndex, value); }
        }

        private void OpenWebsite()
        {
            Process.Start("http://www.driveadviser.com/#faq");

        }
        private int IndexOfSlide<TSlide>()
        {
            return Slides.Select((o, i) => new { o, i }).First(a => a.o.GetType() == typeof(TSlide)).i;
        }
    }
}
