using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveAdviser.Core.Enums;
using DriveAdviser.Core.Models;

namespace DriveAdviser.Core
{
   public interface ISmart 
    {
        Task<SysInfoReturnCode> InitializeSmartAsync();
        Task<List<SmartAttributes>> ReadSmartAsync(int deviceId);
        Task<SysInfoReturnCode> RefreshSmart();
        void SmartCleanup();
    }
}
