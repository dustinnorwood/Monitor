using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Monitor
{
    public class Rs232Params
    {
        public string Name = "COM1";
        public int BaudRate = 9600;
        public int DataBits = 8;
        public Parity Parity = Parity.None;
        public StopBits StopBits = StopBits.One;
        public Handshake Handshaking = Handshake.None;
        public string NewLine = "\\r\\n";

        public Rs232Params()
        {

        }

        public static Rs232Params LoadParams()
        {
            Rs232Params p = new Rs232Params();
            p.Name = (string)CDataSet.DataSet.Settings["Rs232MonitorName"] ?? "COM1";
            p.NewLine = (string)CDataSet.DataSet.Settings["Rs232MonitorNewLine"] ?? "\\r\\n";
            Utility.MaybeParse((string)CDataSet.DataSet.Settings["Rs232MonitorBaudRate"] ?? "9600")
                .Just(i => p.BaudRate = i)
                .Nothing(() => p.BaudRate = 9600);
            Utility.MaybeParse((string)CDataSet.DataSet.Settings["Rs232MonitorDataBits"] ?? "8")
                .Just(i => p.DataBits = i)
                .Nothing(() => p.DataBits = 8);

            switch ((string)CDataSet.DataSet.Settings["Rs232MonitorParity"] ?? "None")
            {
                case "Even":
                    p.Parity = Parity.Even;
                    break;
                case "Odd":
                    p.Parity = Parity.Odd;
                    break;
                case "Mark":
                    p.Parity = Parity.Mark;
                    break;
                case "Space":
                    p.Parity = Parity.Space;
                    break;
                default:
                    p.Parity = Parity.None;
                    break;
            }

            switch ((string)CDataSet.DataSet.Settings["Rs232MonitorStopBits"] ?? "1")
            {
                case "0":
                    p.StopBits = StopBits.None;
                    break;
                case "1.5":
                    p.StopBits = StopBits.OnePointFive;
                    break;
                case "2":
                    p.StopBits = StopBits.Two;
                    break;
                default:
                    p.StopBits = StopBits.One;
                    break;
            }

            switch ((string)CDataSet.DataSet.Settings["Rs232MonitorHandshaking"] ?? "None")
            {
                case "RTS/CTS":
                    p.Handshaking = Handshake.RequestToSend;
                    break;
                case "Xon/Xoff":
                    p.Handshaking = Handshake.XOnXOff;
                    break;
                case "RTS & Xon":
                    p.Handshaking = Handshake.RequestToSendXOnXOff;
                    break;
                default:
                    p.Handshaking = Handshake.None;
                    break;
            }

            return p;
        }


        public static void SaveParams(Rs232Params p)
        {
            CDataSet.DataSet.Settings["Rs232MonitorName"] = p.Name;
            CDataSet.DataSet.Settings["Rs232MonitorBaudRate"] = p.BaudRate.ToString();
            CDataSet.DataSet.Settings["Rs232MonitorDataBits"] = p.DataBits.ToString();

            switch (p.Parity)
            {
                case Parity.None:
                    CDataSet.DataSet.Settings["Rs232MonitorParity"] = "None";
                    break;
                case Parity.Even:
                    CDataSet.DataSet.Settings["Rs232MonitorParity"] = "Even";
                    break;
                case Parity.Odd:
                    CDataSet.DataSet.Settings["Rs232MonitorParity"] = "Odd";
                    break;
                case Parity.Mark:
                    CDataSet.DataSet.Settings["Rs232MonitorParity"] = "Mark";
                    break;
                case Parity.Space:
                    CDataSet.DataSet.Settings["Rs232MonitorParity"] = "Space";
                    break;
            }

            switch (p.StopBits)
            {
                case StopBits.None:
                    CDataSet.DataSet.Settings["Rs232MonitorStopBits"] = "0";
                    break;
                case StopBits.One:
                    CDataSet.DataSet.Settings["Rs232MonitorStopBits"] = "1";
                    break;
                case StopBits.OnePointFive:
                    CDataSet.DataSet.Settings["Rs232MonitorStopBits"] = "1.5";
                    break;
                case StopBits.Two:
                    CDataSet.DataSet.Settings["Rs232MonitorStopBits"] = "2";
                    break;
            }

            switch (p.Handshaking)
            {
                case Handshake.None:
                    CDataSet.DataSet.Settings["Rs232MonitorHandshaking"] = "None";
                    break;
                case Handshake.RequestToSend:
                    CDataSet.DataSet.Settings["Rs232MonitorHandshaking"] = "RTS/CTS";
                    break;
                case Handshake.XOnXOff:
                    CDataSet.DataSet.Settings["Rs232MonitorHandshaking"] = "Xon/Xoff";
                    break;
                case Handshake.RequestToSendXOnXOff:
                    CDataSet.DataSet.Settings["Rs232MonitorHandshaking"] = "RTS & CTS";
                    break;
            }

            CDataSet.DataSet.Settings["Rs232MonitorNewLine"] = p.NewLine;

            CDataSet.DataSet.SaveSetting();
        }
    }
}
