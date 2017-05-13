using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    public delegate void MonitorDataReceivedEventHandler(object sender, MonitorDataReceivedEventArgs e);

    public class MonitorDataReceivedEventArgs : EventArgs
    {
        private readonly object m_Data;
        public object Data { get { return m_Data; } }
        public MonitorDataReceivedEventArgs(object data)
            : base()
        {
            m_Data = data;
        }
    }
}
