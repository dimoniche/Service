using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AirVitamin.Drivers
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
