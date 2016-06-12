using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drivers.Zebex
{
    public enum ZebexCommandEnum
    {
        enable = 0x0E,
        disable = 0x0F,
        powerUp = 0x05,
        sleep = 0x12,
        wakeUp = 0x14
    }
}
