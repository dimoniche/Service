using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSaleMachine.Drivers
{
    public class _Cassete
    {
        public byte Status;                 //!< Cassette status (one of the values from the \link CSStatus Possible cassette status codes \endlink group)
        public byte BillNumber;             //!< Number of bills in the cassete
        public byte BillType;               //!< A bill type assigned to the cassette
        public double dValue;               //!< Denomination of the bill type assigned to the cassette
        public byte UnloadLevel;            //!< Number of bills to leave in the cassette when unloading 
        public _Time UnloadTime;            //!< Time when start automatic unloading
        public int iBillsThisTransaction;   //!< Number of bills packed in the cassette during current transaction
        public double dTransactionAmount;   //!< Total amount packed in the cassette during current transaction
        public bool bUnload;                //!< A flag specifiying whether the cassette should be unloaded
        public bool bEscrow;                //!< A flag specifiying whether cassette is assigned to multi-escrow
        public byte Index;                  //!< A cassette index(position) for sorting purposes
        public byte DecimalPlaces;          //!< Number of decimal places to calculate denomination
        public int billsToUld;              //!< Number of bills to unload
        public byte[] sCountry = new byte[16];  //!< Country code assigned to the cassette

        /**	\brief	The default _Cassete constructor
		*/
        public _Cassete()
        {
            UnloadTime = new _Time();

            Status = 0xfe; dValue = UnloadTime.hours =
               BillNumber = UnloadLevel = 0;
            UnloadTime.mins = UnloadTime.secs = 0xff;
            bEscrow = bUnload = false;
            billsToUld = -1;
            iBillsThisTransaction = 0;
            dTransactionAmount = 0;
        }

    }
}
