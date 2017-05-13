using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    public interface IMonitor
    {
        MonitorSettings Settings { get; set; }
        MonitorResult Open();
        MonitorResult Close();
        MonitorResult Write(string message);
        bool IsOpen { get; }
        event MonitorDataReceivedEventHandler DataReceived;
    }
}
