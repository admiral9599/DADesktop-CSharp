using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DriveAdviser.Core.Extensions
{
    public static class ObjectExtensions
    {

        public static string Dump(this object obj)
        {
            return JsonConvert.SerializeObject(obj,Formatting.Indented);
        }
    }
}
