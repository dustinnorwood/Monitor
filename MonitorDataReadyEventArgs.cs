using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    public delegate void MonitorDataReadyEventHandler(object sender, MonitorDataReadyEventArgs e);

    public class MonitorDataReadyEventArgs : EventArgs
    {
        private readonly object m_Data;
        public object Data { get { return m_Data; } }
        public MonitorDataReadyEventArgs(object data)
            : base()
        {
            m_Data = data;
        }
    }
}
