using System;
using DriveAdviser.Core;
using DriveAdviser.Core.Extensions;
using DriveAdviser.Core.Models;
using DriveAdviser.Core.Services;
using DriveAdviser.Helpers;
using DriveAdviser.Services;
using FluentScheduler;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Serilog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Serilog.Core;

namespace DriveAdviser.UI.Drives
{
    public class DriveOverviewViewModel : ViewModelBase
    {
        private readonly object _drivesLock = new object();

        //private readonly DriveAdviserApi api;
        private readonly IDriveInfo driveInfo;
        private Settings settings;
        private string _alertText;
        private ObservableCollection<Drive> _drives;
        private string _emailAddress;
        private bool _isBusy;
        private bool _isChildWindowOpen;
        private bool _isSaving;
        private readonly IApi _apiService;

        public DriveOverviewViewModel(IApi apiService)
        {
            Log.Information("Initializing Dependencies");
            driveInfo = new DriveInfo();
            Log.Debug("Loaded Drive Info");
            settings = new Settings();
            Log.Debug("Loaded Setting");
            _apiService = apiService;
            Log.Debug("Loaded API");
            Drives = new ObservableCollection<Drive>();
            Log.Debug("Loaded Collections");


            RefreshDrivesCommand = new RelayCommand(RefreshDrives, () => !IsBusy);
            SaveEmailAddressCommand = new RelayCommand(SaveEmailAddress, () => !IsSaving);
            ReEnterEmailCommand = new RelayCommand(ReEnterEmail);

            Init();
            Log.Information("Initialized");


            JobManager.AddJob(() => RefreshDrives(), s => s.ToRunEvery(12).Hours());
        }

        private void ReEnterEmail()
        {
            IsChildWindowOpen = true;
        }


        public RelayCommand SaveEmailAddressCommand { get; }
        public RelayCommand RefreshDrivesCommand { get; }
        public RelayCommand ReEnterEmailCommand { get; }


        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                Set(() => IsBusy, ref _isBusy, value);
                //When the ui is running in the tray a different thread is used
                //this thread is unable to set this property. Because the ui is in the 
                //tray we do not care is this is set. If the thread has no access we do 
                //nothing

                if (Application.Current.Dispatcher.CheckAccess())
                    RefreshDrivesCommand.RaiseCanExecuteChanged();
                else
                    Application.Current.Dispatcher.Invoke(() => RefreshDrivesCommand.RaiseCanExecuteChanged());
            }
        }

        public ObservableCollection<Drive> Drives
        {
            get => _drives;
            set
            {
                Set(() => Drives, ref _drives, value);
                //This synchronizes the threads when the program is running in the tray.
                //Without this the thread would not be able to update
                BindingOperations.EnableCollectionSynchronization(_drives, _drivesLock);
            }
        }

        public string EmailAddress
        {
            get => _emailAddress;
            set { Set(() => EmailAddress, ref _emailAddress, value); }
        }

        public string AlertText
        {
            get => _alertText;
            set { Set(() => AlertText, ref _alertText, value); }
        }


        public bool IsChildWindowOpen
        {
            get => _isChildWindowOpen;
            set { Set(() => IsChildWindowOpen, ref _isChildWindowOpen, value); }
        }

        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                Set(() => IsSaving, ref _isSaving, value);
                //Set the event for Relaycomand
                SaveEmailAddressCommand.RaiseCanExecuteChanged();
            }
        }


        private async void RefreshDrives()
        {
            Log.Information("Refreshing Drives on Thread: {0}", Thread.CurrentThread.ManagedThreadId);
            IsBusy = true;


            var drives = await driveInfo.GetDrives();
            Log.Debug("Drive info {@0}",drives);
            Drives = drives.ToObservableCollection();

            CheckForAlerts(Drives);
            Log.Information("Refreshed Drives on Thread: {0}", Thread.CurrentThread.ManagedThreadId);

            IsBusy = false;
        }

        private async void SaveEmailAddress()
        {
            if (!string.IsNullOrEmpty(EmailAddress) && EmailAddress.IsValidEmail())
            {
                IsSaving = true;
                IsBusy = true;

                settings.UpdateAndCreateSetting(Headers.User, Names.EmailAddress, EmailAddress);
                var userStatus = await _apiService.SaveUserAsync(EmailAddress);
                //Log.Information("{json}", userStatus);
                if (userStatus.Success)
                    if (
                        !string.IsNullOrEmpty(settings.GetSettingValueString(Headers.User, Names.EmailAddress)))
                    {
                        settings.UpdateAndCreateSetting(Headers.User, Names.Saved, "True");
                        await SystemInfo.GetSystemInfoAsync();
                        var result =
                            await
                                _apiService.SaveComputerInfoAsync(
                                    settings.GetSettingValueString(Headers.User, Names.EmailAddress));
                        if (result.Id_Computer.Success)
                        {
                            settings.UpdateAndCreateSetting(Headers.Computer, Names.Saved, true);
                            settings.UpdateAndCreateSetting(Headers.Computer, Names.Id, result.Id_Computer.Id);
                        }
                    }


                IsSaving = false;
                IsChildWindowOpen = false;
                var drives = await driveInfo.GetDrives();
                Drives.Clear();
                foreach (var drive in drives)
                    Drives.Add(drive);
                await Task.Run(() => CheckForAlerts(Drives));

                IsBusy = false;
            }
            else
            {
                AlertText = "Please enter a valid email address";
            }
        }

        private async void Init()
        {
            Log.Information("Initializing on Thread: {0}", Thread.CurrentThread.ManagedThreadId);
            Log.Debug("Getting System Info");
            await SystemInfo.GetSystemInfoAsync();
            //IsBusy = true;


            if (
                string.IsNullOrWhiteSpace(settings.GetSettingValueString(Headers.User,
                    Names.EmailAddress)))
            {
                Log.Information("Gathering Email");
                IsChildWindowOpen = true;
            }
            else
            {
                EmailAddress = settings.GetSettingValueString(Headers.User, Names.EmailAddress);

                await Application.Current.Dispatcher.InvokeAsync(RefreshDrives);

                while (!IsBusy)
                    await Task.Delay(100);

                Log.Information("Ending initializing on Thread: {0}", Thread.CurrentThread.ManagedThreadId);
            }
        }

        private void CheckForAlerts(IEnumerable<Drive> hardDrives)
        {
            if (!hardDrives.Any()) return;
            Log.Information("Checking For Alerts");
            if (Network.HasInternetConnection)
                settings = new Settings();
            if (!settings.GetSettingValueBool(Headers.Computer, Names.Saved) &&
                !string.IsNullOrEmpty(settings.GetSettingValueString(Headers.User, Names.EmailAddress)))
            {
                settings.UpdateAndCreateSetting(Headers.User, Names.Saved, "True");

                var result = _apiService
                    .SaveComputerInfoAsync(settings.GetSettingValueString(Headers.User, Names.EmailAddress)).Result;
                if (result.Id_Computer.Success)
                {
                    settings.UpdateAndCreateSetting(Headers.Computer, Names.Saved, true);
                    settings.UpdateAndCreateSetting(Headers.Computer, Names.Id, result.Id_Computer.Id);
                }
            }

            foreach (var hardDrive in hardDrives)
                if (
                    string.IsNullOrWhiteSpace(settings.GetSettingValueString(Headers.Drives,
                        hardDrive.Serial)))
                {
                    if (hardDrive.HealthPercentage == 100)
                    {
                        settings.UpdateAndCreateSetting(Headers.Drives, hardDrive.Serial,
                            hardDrive.HealthPercentage.ToString());
                        Log.Information("Saved Hard Drive {Serial}", hardDrive.Serial);
                    }


                    else if (hardDrive.HealthPercentage < 100)
                    {
                        if (settings.GetSettingValueBool(Headers.User, Names.Saved) &&
                            _apiService.EmailAlert(EmailAddress, hardDrive.DriveLetter, hardDrive.HealthPercentage,
                                settings.GetSettingValueString(Headers.Computer, Names.Id)).Result.Success)
                        {
                            settings.UpdateAndCreateSetting(Headers.Drives, hardDrive.Serial,
                                hardDrive.HealthPercentage.ToString());
                            Log.Information("Email Sent for Drive {DriveLetter} at {Health}%", hardDrive.DriveLetter,
                                hardDrive.HealthPercentage);
                        }
                        else if (_apiService.SaveUserAsync(EmailAddress).GetAwaiter().GetResult().Success &&
                                 _apiService.EmailAlert(EmailAddress, hardDrive.DriveLetter, hardDrive.HealthPercentage,
                                     settings.GetSettingValueString(Headers.Computer, Names.Id)).Result.Success)
                        {
                            settings.UpdateAndCreateSetting(Headers.User, Names.Saved, "True");
                            settings.UpdateAndCreateSetting(Headers.Drives, hardDrive.Serial,
                                hardDrive.HealthPercentage.ToString());
                            Log.Information("Email Sent for Drive {DriveLetter} at {Health}%", hardDrive.DriveLetter,
                                hardDrive.HealthPercentage);
                        }
                    }
                }
                else if (hardDrive.HealthPercentage <
                         settings.GetSettingValueInt(Headers.Drives.ToString(), hardDrive.Serial))
                {
                    if (
                        _apiService.EmailAlert(EmailAddress, hardDrive.DriveLetter, hardDrive.HealthPercentage,
                            settings.GetSettingValueString(Headers.Computer, Names.Id)).Result.Success)
                        settings.UpdateAndCreateSetting(Headers.Drives, hardDrive.Serial,
                            hardDrive.HealthPercentage.ToString());
                    Log.Information("Email Sent for Drive {DriveLetter} at {Health}%", hardDrive.DriveLetter,
                        hardDrive.HealthPercentage);
                }

            //You can enable this block if you screw up the algorithim and it makes bad hard drives appear.
            // This will change everything back to normal once you change the algorithim

            //else if (hardDrive.HealthPercentage > settings.GetSettingValueInt(Headers.Drives.ToString(),hardDrive.Serial))
            //{
            //    settings.UpdateAndCreateSetting(Headers.Drives, hardDrive.Serial,
            //           hardDrive.HealthPercentage.ToString());
            //}

            SaveDriveToServer(hardDrives);
        }

        private void SaveDriveToServer(IEnumerable<Drive> hardDrives)
        {
            try
            {
                hardDrives.ToList().ForEach(async d => await _apiService.SaveDriveAsync(EmailAddress, d.DriveLetter,
                    d.HealthPercentage, d.Serial, d.Model, d.TotalCapacity, d.FreeSpace, d.DriveType));
            }
            catch (Exception e)
            {
                Log.Warning(e, "Error Saving Drive");
                _apiService.LogExceptionAsync(e);
            }
        }
    }
}