using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MonadicCSharp;

namespace Monitor
{
    public class CMonitor
    {
        private volatile CCircularBuffer<char> m_ReadBuffer;
        private string m_MonLinBuf;
        private APIState m_State;
        private string Prefix = "^";
        public string[] Delimiters = new string[] { "\r", "\n" };
        public List<APICommand> Commands;

        public event MonitorDataReadyEventHandler DataReady;

        private bool m_Running = false;

        private Thread m_Thread;

        private MonitorAPI m_API;

        private IMonitor m_Monitor;
        public IMonitor Monitor
        {
            get { return m_Monitor; }
            set
            {
                if (m_Monitor != null)
                {
                    m_Monitor.Close();
                    m_Monitor.DataReceived -= new MonitorDataReceivedEventHandler(m_Monitor_DataReceived);
                }
                m_Monitor = value;
                m_Monitor.DataReceived += new MonitorDataReceivedEventHandler(m_Monitor_DataReceived);
            }
        }

        public CMonitor()
        {
            m_API = new MonitorAPI(1024);
            m_MonLinBuf = "";
            m_ReadBuffer = new CCircularBuffer<char>(1024);
        }

        public MonitorResult Open()
        {
            Start();

            if (m_Monitor != null)
                return m_Monitor.Open();
            else return new MonitorResult(MonitorResults.Error, "Monitor is null.");
        }

        public MonitorResult Close()
        {
            Stop();
            if (m_Monitor != null)
                return m_Monitor.Close();
            else return new MonitorResult(MonitorResults.Success, "Successfully closed the monitor.");
        }

        private void Start()
        {
            if (m_Running)
                Stop();
            m_Running = true;
            m_Thread = new Thread(new ThreadStart(run));
            m_Thread.Priority = ThreadPriority.Lowest;
            m_Thread.Start();

        }

        private void Stop()
        {
            m_Running = false;
            m_Thread.Join();
        }

        void m_API_DataReady(string data)
        {
            if (m_Monitor != null)
                m_Monitor.Write(data);
        }

        void m_Monitor_DataReceived(object sender, MonitorDataReceivedEventArgs e)
        {
            Add((string)e.Data);
        }

        public MonitorResult Add(string data)
        {
            try
            {
                data.When((_ => m_ReadBuffer.Count + data.Length < m_ReadBuffer.Capacity), 1000)
                    .Just(d => d.ToCharArray().ForEach(c => m_ReadBuffer.Push(c)));

                return new MonitorResult(MonitorResults.Success, "Successfully added API Call.");
            }
            catch (TimeoutException e)
            {
                return new MonitorResult(MonitorResults.Error, e.Message);
            }
        }

        private void run()
        {
            while (m_Running)
            {
                if (!m_ReadBuffer.IsEmpty)
                {
                    m_MonLinBuf += m_ReadBuffer.Pop();
                    switch (m_State)
                    {
                        case APIState.None:
                            if (m_MonLinBuf == Prefix)
                            {
                                m_State = APIState.Command;
                                m_MonLinBuf = "";
                            }
                            break;
                        case APIState.Command:
                            bool delimited = false;
                            foreach (string s in Delimiters)
                            {
                                if (m_MonLinBuf.Length >= s.Length && m_MonLinBuf.Substring(m_MonLinBuf.Length - s.Length, s.Length) == s)
                                {
                                    delimited = true;
                                    m_MonLinBuf = m_MonLinBuf.Remove(m_MonLinBuf.Length - s.Length, s.Length);
                                    break;
                                }
                            }
                            if (delimited)
                            {
                                APIResult result = m_API.ProcessCommand(m_MonLinBuf);
                                result.Data.Just(d =>
                                {
                                    m_API_DataReady(d);
                                    if (this.DataReady != null)
                                        this.DataReady(this, new MonitorDataReadyEventArgs(d));
                                });

                                m_State = result.State;
                                m_MonLinBuf = "";
                            }
                            break;
                        case APIState.Message:
                            break;
                        case APIState.AuxCharacter:
                            break;
                        case APIState.Logo:
                            break;
                    }
                }
                else Thread.Sleep(10);
            }
        }
    }
}
