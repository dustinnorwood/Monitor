using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Monitor
{
    public class TcpMonitor : IMonitor
    {
        private CTCPRelay m_Relay;

        private byte[] m_Buffer;

        public IPAddress IPAddress { get { return GetLocalIPAddress(); } }

        public int PortNumber { get { return m_Relay.PortNumber; } set { m_Relay.PortNumber = value; } }

        private MonitorSettings m_Settings;

        public MonitorSettings Settings { get { return m_Settings; } set { m_Settings = value; } }

        MonitorSettings IMonitor.Settings { get { return m_Settings; } set { m_Settings = value; } }

        public event MonitorDataReceivedEventHandler DataReceived;

        private bool m_IsOpen;

        public bool IsOpen { get { return m_IsOpen; } }

        bool IMonitor.IsOpen { get { return m_IsOpen; } }

        public TcpMonitor()
        {
            m_Relay = new CTCPRelay();
            m_Relay.PortNumber = 12000;
            m_Settings = new MonitorSettings();
            m_Buffer = new byte[2048];
            m_IsOpen = false;
        }

        public MonitorResult Open()
        {
            if (!m_IsOpen)
            {
                try
                {
                    Start();
                    m_IsOpen = true;
                    return new MonitorResult(MonitorResults.Success, "Successfully opened the TcpMonitor.");
                }
                catch (SocketException e)
                {
                    return new MonitorResult(MonitorResults.Error, e.Message);
                }
            }
            else return new MonitorResult(MonitorResults.Error, "TcpMonitor is already open.");
        }

        MonitorResult IMonitor.Open()
        {
            return Open();
        }

        public MonitorResult Close()
        {
            if (m_IsOpen)
            {
                m_Relay.Stop();
                m_IsOpen = false;
                return new MonitorResult(MonitorResults.Success, "Successfully closed the TcpMonitor.");
            }
            else return new MonitorResult(MonitorResults.Error, "TcpMonitor is already closed.");
        }

        MonitorResult IMonitor.Close()
        {
            return Close();
        }

        public MonitorResult Write(string message)
        {
            if (m_Relay.ClientSocket.Connected)
                m_Relay.ClientSocket.Send(Encoding.ASCII.GetBytes(message));
            return new MonitorResult(MonitorResults.Success, "Successfully wrote to the TcpMonitor.");
        }

        MonitorResult IMonitor.Write(string message)
        {
            return Write(message);
        }

        public void Start()
        {
            m_Relay.Start();
            m_Relay.ServerSocket.Listen(1);
            m_Relay.ServerSocket.BeginAccept(SocketAccepted, m_Relay.ServerSocket);
        }

        private void SocketAccepted(IAsyncResult ar)
        {
            try
            {
                m_Relay.ClientSocket = ((Socket)ar.AsyncState).EndAccept(ar);
                m_Relay.ClientSocket.BeginReceive(m_Buffer, 0, m_Buffer.Length, SocketFlags.None, ReceiveCallback, m_Relay.ClientSocket);
            }
            catch (ObjectDisposedException)
            {

            }
            catch (SocketException)
            {

            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            int i = m_Relay.ClientSocket.EndReceive(ar);
            if (i > 0 || m_Relay.IsClientConnected())
            {
                if (this.DataReceived != null)
                {
                    DataReceived(this, new MonitorDataReceivedEventArgs(Encoding.ASCII.GetString(m_Buffer, 0, i)));
                }
                m_Relay.ClientSocket.BeginReceive(m_Buffer, 0, m_Buffer.Length, SocketFlags.None, ReceiveCallback, m_Relay.ClientSocket);
            }
            else
            {
                m_Relay.CloseClientSocket();
                m_Relay.ServerSocket.BeginAccept(SocketAccepted, m_Relay.ServerSocket);
            }
        }

        private IPAddress GetLocalIPAddress()
        {
            IPAddress[] localIPs = Dns.GetHostEntry("").AddressList;
            foreach (IPAddress a in localIPs)
            {
                if (a.AddressFamily == AddressFamily.InterNetwork)
                {
                    return a;
                }
            }
            return IPAddress.None;
        }
    }
}
