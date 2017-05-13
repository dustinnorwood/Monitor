using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    public static class MonitorExtension
    {
        public static void OpenMonitor()
        {
            object monitor = CDataSet.DataSet.Settings["MonitorMode"];
            if (monitor != null)
            {
                switch ((string)monitor)
                {
                    case "TCP/IP":
                        TcpMonitor tcp = new TcpMonitor();

                        CDataSet.DataSet.Monitor.Monitor = tcp;
                        CDataSet.DataSet.Monitor.Monitor.Open();
                        break;
                    case "RS-232":
                        Rs232Monitor rs232 = new Rs232Monitor();
                        Rs232Params p = Rs232Params.LoadParams();
                        rs232.Settings.Add("Rs232MonitorName", p.Name);
                        rs232.Settings.Add("Rs232MonitorBaudRate", p.BaudRate);
                        rs232.Settings.Add("Rs232MonitorDataBits", p.DataBits);
                        rs232.Settings.Add("Rs232MonitorParity", p.Parity);
                        rs232.Settings.Add("Rs232MonitorStopBits", p.StopBits);
                        rs232.Settings.Add("Rs232MonitorHandshaking", p.Handshaking);
                        rs232.Settings.Add("Rs232MonitorNewLine", p.NewLine);
                        CDataSet.DataSet.Monitor.Monitor = rs232;
                        CDataSet.DataSet.Monitor.Monitor.Open();
                        break;
                }
            }
        }

        public static MonitorResult CloseMonitor()
        {
            if (CDataSet.DataSet.Monitor != null && CDataSet.DataSet.Monitor.Monitor != null)
                return CDataSet.DataSet.Monitor.Monitor.Close();
            else return new MonitorResult(MonitorResults.Success, "Successfully shut down monitor.");
        }

        public static void RunTerminal(string exePath, string args)
        {
            try
            {
                CDataSet.DataSet.Statusbar.EnableTimer(false);
                //  m_Timer.Enabled = false;
                CDataSet.DataSet.ShutdownConnections();
                System.Diagnostics.Process proc = System.Diagnostics.Process.Start(exePath, args);
                proc.WaitForExit();
            }
            catch
            {
            }
            CDataSet.DataSet.CreateConnections();
            // m_Timer.Enabled = true;
            CDataSet.DataSet.Statusbar.EnableTimer(true);
        }

        public static void RunRs232Terminal()
        {
            RunTerminal(CDataSet.DataSet.ApplicationDirectory + "\\CodeJetSerialPortTerminal.exe", "");
        }
    }
}
