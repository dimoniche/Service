using AirVitamin.Drivers;

namespace AirVitamin.Client
{
    internal class CheckError
    {
        public static ReasonEnum GetStatus(FormResultData data)
        {
            // читаем состояние устройства
            byte[] res;
            /*res = data.drivers.control.GetStatusControl(data.log);

            if (res != null)
            {
                if (res[0] == 0)
                {
                    data.stage = WorkerStateStage.ErrorControl;
                    return ReasonEnum.FormClose;
                }
            }*/

            res = data.drivers.control.GetStatusRelay(data.log);

            if (Globals.IsDebug == true && res == null)
            {
                res = new byte[4] { 0, 0, 0, 0 };
            }

            if (res != null)
            {
                // просто шлем смс - не выходим пока с ошибкой
                if (res[0] >= 1 && !data.IsInterError1)
                {
                    if (!data.IsSendSMS1)
                    {
                        Program.Log.Write(LogMessageType.Error, "Шлем СМС: РД1 - HIGH.");
                        data.drivers.modem.SendSMS("Упало давление кислорода", data.log);
                        data.IsSendSMS1 = true;
                    }

                    data.IsInterError1 = true;
                    data.stage = WorkerStateStage.Gas1_low;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД1 - HIGH.");

                    if (Globals.ClientConfiguration.Settings.offReserve == 1)
                    {
                        return ReasonEnum.FormClose;
                    }
                }
                else if (res[0] == 0)
                {
                    data.IsSendSMS1 = false;
                    data.IsInterError1 = false;
                }

                if (res[1] >= 1 && !data.IsInterError2)
                {
                    if (!data.IsSendSMS2)
                    {
                        Program.Log.Write(LogMessageType.Error, "Шлем СМС: РД2 - HIGH.");
                        data.drivers.modem.SendSMS("Низкое давление Газа 2", data.log);
                        data.IsSendSMS2 = true;
                    }

                    data.IsInterError2 = true;
                    data.stage = WorkerStateStage.Gas2_low;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД2 - HIGH.");
                }
                else if (res[1] == 0)
                {
                    data.IsSendSMS2 = false;
                    data.IsInterError2 = false;
                }

                if (res[2] >= 1 && !data.IsInterError3)
                {
                    if (!data.IsSendSMS3)
                    {
                        Program.Log.Write(LogMessageType.Error, "Шлем СМС: РД3 - HIGH.");
                        data.drivers.modem.SendSMS("Низкое давление Газа 3", data.log);
                        data.IsSendSMS3 = true;
                    }

                    data.IsInterError3 = true;
                    data.stage = WorkerStateStage.Gas3_low;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД3 - HIGH.");
                }
                else if (res[2] == 0)
                {
                    data.IsSendSMS3 = false;
                    data.IsInterError3 = false;
                }

                if (res[3] >= 1 && !data.IsInterError4)
                {
                    if (!data.IsSendSMS4)
                    {
                        Program.Log.Write(LogMessageType.Error, "Шлем СМС: РД4 - HIGH.");
                        data.drivers.modem.SendSMS("Низкое давление Газа 4", data.log);
                        data.IsSendSMS4 = true;
                    }

                    data.IsInterError4 = true;
                    data.stage = WorkerStateStage.Gas4_low;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД4 - HIGH.");

                    return ReasonEnum.FormClose;
                }
                else if (res[3] == 0)
                {
                    data.IsSendSMS4 = false;
                    data.IsInterError4 = false;
                }
            }

            if (Globals.ClientConfiguration.Settings.offPrinter == 0)
            {
                PrinterStatus status = data.drivers.printer.GetStatus();

                if ((status & (PrinterStatus.PRINTER_STATUS_PAPER_OUT
                             | PrinterStatus.PRINTER_STATUS_PAPER_JAM
                             | PrinterStatus.PRINTER_STATUS_PAPER_PROBLEM
                             | PrinterStatus.PRINTER_STATUS_DOOR_OPEN
                             | PrinterStatus.PRINTER_STATUS_ERROR)) > 0)
                {
                    if (Globals.ClientConfiguration.Settings.NoPaperWork == 0)
                    {
                        data.stage = WorkerStateStage.PaperEnd;
                        return ReasonEnum.FormClose;
                    }
                    else
                    {
                        if (data.PrinterError == false)
                        {
                            Program.Log.Write(LogMessageType.Error, "WAIT_MENU: кончилась бумага.");
                        }

                        data.PrinterError = true;
                    }
                }
                else if ((status & PrinterStatus.PRINTER_STATUS_OFFLINE) > 0)
                {
                    if (Globals.ClientConfiguration.Settings.NoPaperWork == 0)
                    {
                        data.stage = WorkerStateStage.ErrorPrinter;
                        return ReasonEnum.FormClose;
                    }
                    else
                    {
                        if (data.PrinterError == false)
                        {
                            Program.Log.Write(LogMessageType.Error, "WAIT_MENU: нет связи с принтером.");
                        }

                        data.PrinterError = true;
                    }
                }
                else
                {
                    if (data.PrinterError == true)
                    {
                        Program.Log.Write(LogMessageType.Error, "WAIT_MENU: ошибка принтера снялась.");
                    }

                    data.PrinterError = false;
                }
            }
            else
            {
                data.PrinterError = false;
            }

            return ReasonEnum.None;
        }
    }
}
