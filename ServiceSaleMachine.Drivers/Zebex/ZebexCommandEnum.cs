using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSaleMachine.Drivers
{
    public enum ZebexCommandEnum
    {
        enable,
        disable,
        powerUp,
        sleep,
        wakeUp
    }

    public enum ZebexCommandCodeEnum
    {
        enable = 0x0E,
        disable = 0x0F,
        powerUp = 0x05,
        sleep = 0x12,
        wakeUp = 0x14
    }
}
