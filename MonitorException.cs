using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    public class MonitorException : Exception
    {
        public MonitorException() : base() { }
        public MonitorException(string message) : base(message) { }
    }
}
