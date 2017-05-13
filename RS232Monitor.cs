using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using MonadicCSharp;

namespace Monitor
{
    public class Rs232Monitor : IMonitor
    {
        private SerialPort m_Port;

        private MonitorSettings m_Settings;

        public MonitorSettings Settings { get { return m_Settings; } set { m_Settings = value; } }

        MonitorSettings IMonitor.Settings { get { return m_Settings; } set { m_Settings = value; } }

        private bool m_IsOpen;

        public bool IsOpen { get { return m_IsOpen; } }

        bool IMonitor.IsOpen { get { return m_IsOpen; } }

        public event MonitorDataReceivedEventHandler DataReceived;

        public Rs232Monitor()
        {
            m_Settings = new MonitorSettings();
            m_IsOpen = false;
        }

        private void m_Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this.DataReceived != null)
                DataReceived(this, new MonitorDataReceivedEventArgs(m_Port.ReadExisting()));
        }

        public MonitorResult Open()
        {
            if (!m_IsOpen)
            {
                try
                {
                    m_Port = new SerialPort();
                    m_Settings["Rs232MonitorName"].Just(o => m_Port.PortName = (string)o).Nothing(() => m_Port.PortName = "COM1");
                    m_Settings["Rs232MonitorBaudRate"].Just(o => m_Port.BaudRate = (int)o).Nothing(() => m_Port.BaudRate = 9600);
                    m_Settings["Rs232MonitorDataBits"].Just(o => m_Port.DataBits = (int)o).Nothing(() => m_Port.DataBits = 8);
                    m_Settings["Rs232MonitorParity"].Just(o => m_Port.Parity = (Parity)o).Nothing(() => m_Port.Parity = Parity.None);
                    m_Settings["Rs232MonitorStopBits"].Just(o => m_Port.StopBits = (StopBits)o).Nothing(() => m_Port.StopBits = StopBits.One);
                    m_Settings["Rs232MonitorHandshaking"].Just(o => m_Port.Handshake = (Handshake)o).Nothing(() => m_Port.Handshake = Handshake.None);
                    m_Settings["Rs232MonitorNewLine"].Just(o => m_Port.NewLine = (string)o).Nothing(() => m_Port.NewLine = "\\r\\n");

                    m_Port.DataReceived += new SerialDataReceivedEventHandler(m_Port_DataReceived);
                    m_Port.Open();
                    m_IsOpen = true;
                    return new MonitorResult(MonitorResults.Success, "Successfully opened the Rs232Monitor.");
                }
                catch (Exception e)
                {
                    m_IsOpen = false;
                    return new MonitorResult(MonitorResults.Error, e.Message);
                }
            }
            else return new MonitorResult(MonitorResults.Error, "Rs232Monitor is already open.");
        }

        MonitorResult IMonitor.Open()
        {
            return Open();
        }

        public MonitorResult Close()
        {
            if (m_IsOpen)
            {
                try
                {
                    m_Port.Close();
                    m_Port.Dispose();
                    m_Port = null;
                    m_IsOpen = false;
                    return new MonitorResult(MonitorResults.Success, "Successfully closed the Rs232Monitor.");
                }
                catch (Exception e)
                {
                    m_IsOpen = false;
                    return new MonitorResult(MonitorResults.Error, e.Message);
                }
            }
            else return new MonitorResult(MonitorResults.Error, "Rs232Monitor is already closed.");
        }

        MonitorResult IMonitor.Close()
        {
            return Close();
        }

        public MonitorResult Write(string message)
        {
            if (m_Port != null && m_Port.IsOpen)
            {
                m_Port.Write(message);
                return new MonitorResult(MonitorResults.Success, "Successfully wrote to the Rs232Monitor.");
            }
            else if (m_Port != null)
            {
                m_IsOpen = false;
                return new MonitorResult(MonitorResults.Error, "Port isn't open.");
            }
            else
            {
                m_IsOpen = false;
                return new MonitorResult(MonitorResults.Error, "Port is null.");
            }
        }

        MonitorResult IMonitor.Write(string message)
        {
            return Write(message);
        }
    }
}
