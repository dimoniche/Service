using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSaleMachine.Drivers
{
    public class _BillRecord
    {
        const int MAX_CAS_NUMBER = 16;              //!< maximum number of cassettes supported by VMC

        public float Denomination;                  //!< Denomination of the bill
        public byte[] sCountryCode = new byte[4];   //!< Country or currency code
        public bool bRouted;			            //!< A bool variable specifiying whether the bill is forwarded to a cassette
    }
}
