using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Monitor
{
    public delegate void ClientSocketCreated(Socket socket);

    public class CTCPRelay
    {
        Socket m_Server;
        Socket m_Client;

        public int PortNumber = 8000;
        public IPAddress Address;
        public ClientSocketCreated OnClientSocketCreated;

        public CTCPRelay()
        {
            IPAddress[] localIPs = Dns.GetHostEntry("").AddressList;
            foreach (IPAddress a in localIPs)
            {
                if (a.AddressFamily == AddressFamily.InterNetwork)
                {
                    Address = a;
                    break;
                }
            }
        }

        public Socket ClientSocket { get { return m_Client; } set { m_Client = value; } }
        public Socket ServerSocket { get { return m_Server; } }

        public void Start()
        {
            m_Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //			m_Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Bind();
        }

        public void Bind()
        {
            m_Server.Bind(new IPEndPoint(Address, PortNumber));
        }

        public void Stop()
        {
            CloseServerSocket();
            CloseClientSocket();
        }

        public void CloseServerSocket()
        {
            try
            {
                if (m_Server != null)
                {
                    try
                    {
                        m_Server.Shutdown(SocketShutdown.Both);
                    }
                    catch { }
                    finally
                    {
                        m_Server.Close();
                    }
                }
            }
            catch { }
        }

        public void CloseClientSocket()
        {
            try
            {
                if (m_Client != null)
                {
                    m_Client.Shutdown(SocketShutdown.Both);
                    m_Client.Close();
                    m_Client = null;
                }
            }
            catch { }
        }

        public bool IsClientConnected()
        {
            try
            {
                if (m_Client == null) return false;
                return !(m_Client.Poll(1, SelectMode.SelectRead) && m_Client.Available == 0);
            }
            catch (SocketException) { return false; }
        }
    }
}
