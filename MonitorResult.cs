using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    public enum MonitorResults { Success, Error };

    public class MonitorResult
    {
        private readonly MonitorResults m_Result;
        public MonitorResults Result { get { return m_Result; } }

        private readonly string m_Message;
        public string Message { get { return m_Message; } }

        public MonitorResult(MonitorResults result, string message)
        {
            m_Result = result;
            m_Message = message;
        }

        public MonitorResult OnSuccess(Action<string> action)
        {
            if (this.Result == MonitorResults.Success)
                action(this.Message);
            return this;
        }

        public MonitorResult OnError(Action<string> action)
        {
            if (this.Result == MonitorResults.Error)
                action(this.Message);
            return this;
        }
    }
}
