using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveAdviser.Core;
using DriveAdviser.Core.Models;
using DriveAdviser.Core.Services;
using DriveAdviser.UI.Drives;
using DriveAdviser.UI.UserReport;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace DriveAdviser.UI
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider((() => SimpleIoc.Default));
            SimpleIoc.Default.Register<IApi,Api>();
            

            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<DriveOverviewViewModel>();
            SimpleIoc.Default.Register<UserReportViewModel>();
        }

        public static MainWindowViewModel MainWindowViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MainWindowViewModel>(); }
        }

        public static DriveOverviewViewModel DriveOverviewViewModel
        {
            get { return ServiceLocator.Current.GetInstance<DriveOverviewViewModel>(); }
        }

        public static UserReportViewModel UserReportViewModel
        {
            get { return ServiceLocator.Current.GetInstance<UserReportViewModel>(); }
        }
     
    }
}
