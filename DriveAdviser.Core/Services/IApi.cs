using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DriveAdviser.Core.Models;

namespace DriveAdviser.Core.Services
{
    public interface IApi
    {
        Task<ServerJsonResult> SaveUserAsync(string emailAddress);

        Task<ServerJsonResult> SaveComputerInfoAsync(string emailAddress);

        Task<ServerJsonResult> GetCustomerBySiTagAsync(string siTag);

        Task<ServerJsonResult> SaveDriveAsync(string emailAddress, string driveLetter, int healthPercentage,string serialNumber, string model, int totalCapacity, int freeSpace, string driveType);

        Task<ServerJsonResult> EmailAlert(string emailAddress, string driveletter, int healthPercentage,string computerId);

        Task<ServerJsonResult> CheckForSchrockInnovationsIp(string ip);

        Task<ServerJsonResult> LogExceptionAsync(Exception exception, string systemInfo = "");

        Task<ServerJsonResult> LogExceptionAsync(string message, string stackTrace, string innerMessage,string innerStackTrace,string computerId, string systemInfo = "");
       
    }
}
