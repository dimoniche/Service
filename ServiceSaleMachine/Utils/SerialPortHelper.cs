﻿using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;

namespace ServiceSaleMachine
{
    public static class SerialPortHelper
    {
        public static string[] GetSerialPorts()
        {
            return SerialPort.GetPortNames().OrderBy(c => c).ToArray();
        }
    }
}
