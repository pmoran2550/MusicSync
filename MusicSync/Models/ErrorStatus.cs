using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSync
{
    public enum ErrorStatus
    {
        No_Error = 0,
        Duplicate,
        Missing_Songs,
        Missing_Albums,
        Other_Error
    }
}
