using DriveAdviser.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DriveAdviser.Core
{
    public interface IDriveInfo : ISmart
    {
        Task<List<Drive>> GetDrives();


       
    }
}
