using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AirVitamin.Drivers
{
    public class _BillStatus
    {
        public long Enabled;           //!< A bitmap describing which bill types are enabled
        public long Security;          //!< A bitmap describing which bill types are processed in High Security mode
        public long Routing;           //!< A bitmap describing which denominations are routed to a recycling cassette. Is a valid value only for BB units
    }
}
