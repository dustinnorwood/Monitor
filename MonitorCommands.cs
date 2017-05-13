using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonadicCSharp;

namespace Monitor
{
    public static class MonitorCommands
    {
        public static APICommand[] Commands = new APICommand[] {
        new APICommand("DT", a =>
            {
                APIResult api = new APIResult(APIState.None, new Maybe<string>());
                a.Nothing(() => api = new APIResult(APIState.None, new Maybe<string>(
                "^DT"
                + CDataSet.Clock.ToString("HHmmddMMyy")
                + ((int)CDataSet.Clock.DayOfWeek + 1).ToString("D1")
                + "\r\n")));
                return api;
            }),
        new APICommand("ER", a =>
            {
                APIResult api = new APIResult(APIState.None, new Maybe<string>());
                a.Nothing(() => api = new APIResult(APIState.None, new Maybe<string>(
                "^ER"
                + CDataSet.DataSet.PrinterStatus.ErrorCode.Value.ToString("X8")
                + CDataSet.DataSet.PrinterStatus.NozzleStateValue.Value.ToString("D2")
                + "\r\n")));
                return api;
            }),
        new APICommand("GF", a =>
            {
                APIResult api = new APIResult(APIState.None, new Maybe<string>());
                a.Just(s => CDataSet.DataSet.PrinterCommand.SendCommand("GF" + (s[0] == '1' ? "1" : "0")))
                .Nothing(() => api = new APIResult(APIState.None, new Maybe<string>(
                "^GF"
                + (CDataSet.DataSet.PrinterStatus.GutterFaultShutdown ? "1" : "0")
                + "\r\n")));
                return api;
            }),
        new APICommand("PU", a =>
            {
                APIResult api = new APIResult(APIState.None, new Maybe<string>());
                a.Just(s => Utility.MaybeParse(s).Just(i =>
                {
                    if (i >= 10 && i <= 50)
                    {
                        CDataSet.DataSet.PrinterStatus.SystemRunningPressure.Value = i;
                        CDataSet.DataSet.PrinterCommand.SendCommand("PU" + i.ToString());
                    }
                }))
                .Nothing(() => api = new APIResult(APIState.None, new Maybe<string>(
                "^PU"
                + (CDataSet.DataSet.PrinterStatus.SystemRunningPressure.Value.ToString())
                + "\r\n")));
                return api;
            }),
        new APICommand("VI", a =>
            {
                APIResult api = new APIResult(APIState.None, new Maybe<string>());
                a.Just(s => Utility.MaybeParse(s).Just(i =>
                {
                    if (i >= 25 && i <= 40)
                    {
                        CDataSet.DataSet.PrinterStatus.ViscositySetpoint.Value = i;
                        CDataSet.DataSet.PrinterCommand.SendCommand("VIS" + i.ToString());
                    }
                }))
                .Nothing(() => api = new APIResult(APIState.None, new Maybe<string>(
                "^VI"
                + (CDataSet.DataSet.PrinterStatus.ViscositySetpoint.Value.ToString())
                + "\r\n")));
                return api;
            }),
        new APICommand("ED", a =>
            {
                APIResult api = new APIResult(APIState.None, new Maybe<string>());
                a.Just(s => Utility.MaybeParse(s).Just(i =>
                {
                    if (i >= 1 && i <= 3650)
                    {
                        CDataSet.DataSet.PrinterStatus.ExpiryDays.Value = i;
                        CDataSet.DataSet.PrinterCommand.SendCommand("ED" + i.ToString());
                    }
                }))
                .Nothing(() => api = new APIResult(APIState.None, new Maybe<string>(
                "^ED"
                + (CDataSet.DataSet.PrinterStatus.ExpiryDays.Value.ToString())
                + "\r\n")));
                return api;
            }),
        new APICommand("LM", a =>
            {
                StringBuilder sb = new StringBuilder("^LM");
                CContainerCollection.GetMessageNames().Just(names =>
                {
                    sb.Append(names.Length.ToString()).Append("\r\n");
                    names.ForEach(s => sb.Append(s).Append("\r\n"));
                })
                    .Nothing(() => sb.Append("0\r\n"));
                return new APIResult(APIState.None, new Maybe<string>(sb.ToString()));
            }),
        new APICommand("MD", a =>
            {
                a.Just(m =>
                {
                    CContainerCollection.GetMessageNames().Just(names =>
                    {
                        string fullName = CFileManager.GetMessageFileName(m);
                        if (names.Contains(m) && System.IO.File.Exists(fullName))
                            System.IO.File.Delete(fullName);
                    });
                });
                return new APIResult(APIState.None, new Maybe<string>(((char)0x6).ToString()));
            }),
        new APICommand("MA", a =>
            {
                CContainerCollection.GetMessageNames().Just(names =>
                {
                    names.ForEach(s =>
                    {
                        string fullName = CFileManager.GetMessageFileName(s);
                        if (names.Contains(s) && System.IO.File.Exists(fullName))
                            System.IO.File.Delete(fullName);
                    });
                });
                return new APIResult(APIState.None, new Maybe<string>(((char)0x6).ToString()));
            }),
        new APICommand("MP", a =>
            {
                StringBuilder sb = new StringBuilder();
                a.Just(s =>
                    CMessage.CreateFromMessageFile(s)
                            .Just(m =>
                            {
                                CPrinterMessageProcess cpmp = new CPrinterMessageProcess(CDataSet.DataSet.PrinterCommand);
                                if (cpmp.Send(m))
                                {
                                    CDataSet.DataSet.PrinterStatus.CurrentPrinterMessage = s;
                                    sb.Append((char)0x6);
                                }
                                else sb.Append((char)0x9);
                            })
                            .Nothing(() => sb.Append((char)0x9)))
                .Nothing(() => sb.Append("^MP").Append(CDataSet.DataSet.PrinterStatus.CurrentPrinterMessage));
                return new APIResult(APIState.None, new Maybe<string>(sb.Append("\r\n").ToString()));
            }),
        new APICommand("PC", a =>
            {
                APIResult api = new APIResult(APIState.None, new Maybe<string>());
                a.Just(_ => CDataSet.DataSet.PrinterCommand.SendCommand("PC0"))
                .Nothing(() => api = new APIResult(APIState.None, new Maybe<string>(
                "^PC"
                + (CDataSet.DataSet.PrinterStatus.PrinterCounter.Value.ToString())
                + "\r\n")));
                return api;
            }),
        new APICommand("PE", a =>
            {
                APIResult api = new APIResult(APIState.None, new Maybe<string>());
                a.Just(s => CDataSet.DataSet.PrinterCommand.SendCommand("PE" + (s[0] == '1' ? "1" : "0")))
                .Nothing(() => api = new APIResult(APIState.None, new Maybe<string>(
                "^PE"
                + (CDataSet.DataSet.PrinterStatus.PrintEnabled ? "1" : "0")
                + "\r\n")));
                return api;
            }),
        new APICommand("PP", a =>
            {
                APIResult api = new APIResult(APIState.None, new Maybe<string>());
                a.Just(s => Utility.MaybeParse(s).Just(i =>
                {
                    if (i >= 0 && i <= 1)
                    {
                        CDataSet.DataSet.PrinterStatus.PECPolarity.Value = i != 0;
                        CDataSet.DataSet.PrinterCommand.SendCommand("PP" + i.ToString());
                    }
                }))
                .Nothing(() => api = new APIResult(APIState.None, new Maybe<string>(
                "^PP"
                + (CDataSet.DataSet.PrinterStatus.PECPolarity ? "1" : "0")
                + "\r\n")));
                return api;
            }),
        new APICommand("RD", a =>
            {
                APIResult api = new APIResult(APIState.None, new Maybe<string>());
                a.Just(s => Utility.MaybeParse(s).Just(i =>
                {
                    if (i >= 10 && i <= 10000)
                    {
                        CDataSet.DataSet.PrinterStatus.RepeatDelay.Value = i;
                        CDataSet.DataSet.PrinterCommand.SendCommand("RD" + i.ToString());
                    }
                }))
                .Nothing(() => api = new APIResult(APIState.None, new Maybe<string>(
                "^RD"
                + (CDataSet.DataSet.PrinterStatus.RepeatDelay.Value.ToString())
                + "\r\n")));
                return api;
            }),
        new APICommand("RP", a =>
            {
                APIResult api = new APIResult(APIState.None, new Maybe<string>());
                a.Just(s => Utility.MaybeParse(s).Just(i =>
                {
                    if (i >= 0 && i <= 3)
                    {
                        CDataSet.DataSet.PrinterStatus.RepeatMode.Value = i;
                        CDataSet.DataSet.PrinterCommand.SendCommand("RP" + i.ToString());
                    }
                }))
                .Nothing(() => api = new APIResult(APIState.None, new Maybe<string>(
                "^RP"
                + (CDataSet.DataSet.PrinterStatus.RepeatMode.Value.ToString())
                + "\r\n")));
                return api;
            }),
        new APICommand("UF", a =>
            {
                StringBuilder sb = new StringBuilder();
                a.Just(s =>
                {
                    MList<string> strs = new MList<string>(s.Split(','));
                    strs.Filled((x, xs) =>
                    {
                        Utility.MaybeParse(x).Just(i =>
                        {
                            if (i < 10)
                            {
                                xs.Filled((xp, xsp) =>
                                {
                                    CDataSet.DataSet.PrinterStatus.PrinterUserFields[i] = xp;
                                    CDataSet.DataSet.PrinterCommand.SendCommand("UF" + i.ToString() + "," + xp);
                                    sb.Append((char)0x6);
                                })
                                .Empty(() =>
                                {
                                    sb.Append("^UF").Append(i.ToString()).Append(',');
                                    sb.Append(CDataSet.DataSet.PrinterStatus.PrinterUserFields[i]).Append("\r\n");
                                });
                            }
                        })
                            .Nothing(() =>
                            {
                                CUserFieldContainer cufc = new CUserFieldContainer();
                                if (cufc.Load() && cufc[x] != null)
                                {

                                }
                            });
                    });
                })
                .Nothing(() => CDataSet.DataSet.PrinterStatus.PrinterUserFields.For((v, i) => sb.Append("^UF")
                                                                                                .Append(i.ToString())
                                                                                                .Append(",")
                                                                                                .Append(v)
                                                                                                .Append("\r\n")));
                return new APIResult(APIState.None, new Maybe<string>(sb.ToString()));
            })
        };
    }
}
