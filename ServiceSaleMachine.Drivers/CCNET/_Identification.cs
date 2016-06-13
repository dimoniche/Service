using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSaleMachine.Drivers
{
    public class _Identification
    {
        // Identification command fields
        public byte[] PartNumber;           //!< Firmware part number 
        public byte[] SN;                   //!< Device's serial number
        public Int64 DS1;                   //!< Device's asset number
                                            // Extended identification command fiels
        public byte[] BVBootVersion;       //!< Boot version of the validating head (is reported in response to EXTENDED IDENTIFICATION command)
        public byte[] BVVersion;           //!< Firmware version of the validating head (is reported in response to EXTENDED IDENTIFICATION command)

        public byte[] BCCPUBoot;           //!< Boot version of the central controller (is reported in response to EXTENDED IDENTIFICATION command)
        public byte[] BCCPUVersion;        //!<Firmware version of the central controller (is reported in response to EXTENDED IDENTIFICATION command)

        public byte[] BCDispenserBoot;     //!< Boot version of the dispenser (is reported in response to EXTENDED IDENTIFICATION command)
        public byte[] BCDispenserVersion;  //!< Firmware version of the dispenser (is reported in response to EXTENDED IDENTIFICATION command)

        public byte[] BCCS1Boot;           //!< Boot version of the cassette 1 (is reported in response to EXTENDED IDENTIFICATION command)
        public byte[] BCCS2Boot;           //!< Boot version of the cassette 2 (is reported in response to EXTENDED IDENTIFICATION command)
        public byte[] BCCS3Boot;           //!< Boot version of the cassette 3 (is reported in response to EXTENDED IDENTIFICATION command)
        public byte[] BCCSVersion;         //!< Firmware version of the cassettes (is reported in response to EXTENDED IDENTIFICATION command)

        public _Identification()
        {
            DS1 = new Int64();

            PartNumber = new byte[16];
            SN = new byte[13];
            BVBootVersion = new byte[7];
            BVVersion = new byte[21];
            BCCPUBoot = new byte[7];
            BCCPUVersion = new byte[7];
            BCDispenserBoot = new byte[7];
            BCDispenserVersion = new byte[7];
            BCDispenserVersion = new byte[7];
            BCCS1Boot = new byte[7];
            BCCS2Boot = new byte[7];
            BCCS3Boot = new byte[7];
            BCCSVersion = new byte[7];
        }
    }
}
