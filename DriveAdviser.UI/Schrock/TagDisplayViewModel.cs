using System;
using System.Threading.Tasks;
using DriveAdviser.Core;
using DriveAdviser.Core.Extensions;
using DriveAdviser.Core.Helpers;
using DriveAdviser.Core.Models;
using DriveAdviser.Core.Services;
using DriveAdviser.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Serilog;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace DriveAdviser.UI.Schrock
{
    public class TagDisplayViewModel : ViewModelBase
    {
        private readonly DriveAdviserApi api;
        private readonly Settings settings;
        private string _alertText;
        private ServerJsonResult _customer;
        private string _emailAddress;
        private bool _isBusy;
        private bool _isChildWindowOpen;
        private string _siTag;
        private string _statusText;

        public TagDisplayViewModel()
        {
            IsChildWindowOpen = true;
            settings = new Settings();
            api = new DriveAdviserApi();
            SaveSiTagCommand = new RelayCommand(SaveSiTag);
            OpenChildWindowCommand = new RelayCommand(OpenChildWindow);
            VerifyCustomerCommand = new RelayCommand(VerifyCustomer, (() => !IsBusy));
            EmailAddress = settings.GetSettingValueString(Headers.User, Names.EmailAddress);
        }

        public RelayCommand SaveSiTagCommand { get; private set; }
        public RelayCommand OpenChildWindowCommand { get; private set; }
        public RelayCommand VerifyCustomerCommand { get; private set; }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                Set(() => IsBusy, ref _isBusy, value); 
                VerifyCustomerCommand.RaiseCanExecuteChanged();
            }
        }

        public string StatusText
        {
            get {return _statusText; }
            set{Set(()=> StatusText, ref _statusText,  value); }
        }

        public string EmailAddress
        {
            get { return _emailAddress; }
            set { Set(() => EmailAddress, ref _emailAddress, value); }
        }

        public string SiTag
        {
            get { return _siTag; }
            set { Set(() => SiTag, ref _siTag, value); }
        }

        public ServerJsonResult Customer
        {
            get { return _customer; }
            set { Set(() => Customer, ref _customer, value); }
        }

        public string AlertText
        {
            get { return _alertText; }
            set { Set(() => AlertText, ref _alertText, value); }
        }

        public bool IsChildWindowOpen
        {
            get { return _isChildWindowOpen; }
            set { Set(() => IsChildWindowOpen, ref _isChildWindowOpen, value); }
        }

        private async void VerifyCustomer()
        {
            IsBusy = true;
            if (!settings.GetSettingValueBool(Headers.Computer, Names.Saved) &&
                !string.IsNullOrEmpty(settings.GetSettingValueString(Headers.User, Names.EmailAddress)))
            {
                StatusText = "Gathering System Information";
                await SystemInfo.GetSystemInfoAsync();
                
                Log.Information("Saving Computer");
          
                var result = api.SaveComputer(settings.GetSettingValueString(Headers.User, Names.EmailAddress),
                    SystemInfo.CpuName,
                    SystemInfo.HardDriveSize, SystemInfo.OsName, SystemInfo.CpuSpeed, SystemInfo.RamCapacity);
                if (result.Id_Computer.Success)
                {
                    Log.Information("Computer Saved");
                    settings.UpdateAndCreateSetting(Headers.Computer, Names.Saved, true);
                    settings.UpdateAndCreateSetting(Headers.Computer, Names.Id, result.Id_Computer.Id);
                    StatusText = "Computer Saved";
                }
                else
                {
                    StatusText="Unable to save computer";
                    IsBusy = false;
                    return;
                }
            }
            else
            {
                
            }
            var relation = api.CreateDatabaseRelation(SiTag, settings.GetSettingValueString(Headers.Computer, Names.Id));
            //Log.Information("{json}",JsonConvert.SerializeObject(relation,Formatting.Indented));
            if (relation.Success)
            {

                Log.Information("Database relation created");
                StatusText = "Creating Database Relation";
                Core.SiTag.WriteTag(SiTag);
                settings.UpdateAndCreateSetting(Headers.App, Names.Build, "Schrock");
                
                Log.Information("Restarting application");
                IsBusy = false;
                Administrator.RestartAsAdminAndExit("/tray");

                Application.Current.Shutdown(0);
            }
            else
            {
                Log.Information(relation.Msg);
                StatusText = "Unable to Create database relation";
                IsBusy = false;
            }
        }

        private void OpenChildWindow()
        {
            AlertText = string.Empty;
            IsChildWindowOpen = true;
        }


        private async void SaveSiTag()
        {
            IsBusy = true;
            if (EmailAddress.IsValidEmail())
            {
                settings.UpdateAndCreateSetting(Headers.User, Names.EmailAddress, EmailAddress);
                if (api.SaveUser(EmailAddress).Success)
                    settings.UpdateAndCreateSetting(Headers.User, Names.Saved, "True");
            }
            else
            {
                AlertText = "Please Enter a Valid Email Address";
                IsBusy = false;
                return;
            }

            var result = await api.GetCustomerBySiTag(SiTag);
            if (result.Success)
            {
                Customer = result;

                IsChildWindowOpen = false;
                IsBusy = false;
            }
            else
            {
                AlertText = result.Msg;
                IsBusy = false;
            }
        }
    }
}