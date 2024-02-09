using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaginx.DomainModels.MonitorModels;

public class OsInfo
{
    //
    // 摘要:
    //     The familiy of the OS
    public string Family { get; set; }
    //
    // 摘要:
    //     The major version of the OS, if available
    public string Major { get; set; }
}
