using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSaleMachine.Drivers
{
    public enum CCNETCommandEnum
    {
        Reset = 0,
        Poll,
        Status,
        Information,
        Hold,
        SetSecurity,
        BillType,
        Pack,
        Return,
        SetBarParams,
        ExtractBarData,
        GetBillTable,
        GetCassetteStatus,
    }
}
