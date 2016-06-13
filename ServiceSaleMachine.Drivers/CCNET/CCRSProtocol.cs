using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServiceSaleMachine.Drivers
{
    public class CCRSProtocol
    {
        #region Константы
        const byte SYNC = 0x02; //!< synchronization byte 
        const byte ACK = 0x00;  //!< ACK code
        const byte NAK = 0xFF;  //!< NAK code
        const byte ST_INV_CMD = 0x30;//!< INVALID COMMAND response

        /**	\defgroup Addr Device Addresses
        * @{
*/
        const byte ADDR_BB = 0x01; //!< Address for Bill-To-Bill units
        const byte ADDR_CHANGER = 0x02; //!< Address for Coin Changer
        const byte ADDR_FL = 0x03; //!< Address for Bill Validators
        const byte ADDR_CR = 0x04; //!< Address for Smart Card Reader
                                   /**@} */

        /**	\defgroup Cmds Interface commands
        * @{
*/
        const byte RESET = 0x30; //!<REST command code
        const byte GET_STATUS = 0x31; //!<STATUS REQUEST command code
        const byte SET_SECURITY = 0x32; //!<SET SECURITY command code
        const byte POLL = 0x33; //!<POLL command code
        const byte BILL_TYPE = 0x34; //!<BILL TYPE command code
        const byte PACK = 0x35; //!<PACK command code
        const byte RETURN = 0x36; //!<RETURN command code
        const byte IDENTIFICATION = 0x37;//!<IDENTIFICATION command code
        const byte IDENT_EXT = 0x3E;//!<EXTENDED IDENTIFICATION command code
        const byte HOLD = 0x38;//!<HOLD command code
        const byte C_STATUS = 0x3B;//!<RECYCLING CASSETTE STATUS REQUEST command code
        const byte DISPENSE = 0x3C;//!<DISPENSE command code
        const byte UNLOAD = 0x3D;//!<UNLOAD command code
        const byte SET_CASSETES = 0x40;//!<SET RECYCLING CASSETTE TYPE command code
        const byte GET_BILL_TABLE = 0x41;//!<BILL TABLE REQUEST command code	
        const byte DOWNLOAD = 0x50;//!<DOWNLOAD command code
        const byte CRC32 = 0x51;//!<CRC32 REQUEST command code
        const byte SET_TIME = 0x62;//!<SET BB TIME command code
        const byte SET_BAR_PARAMS = 0x39;//!<SET BARCODE PARAMETERS command code
        const byte EXTRACT_BAR_DATA = 0x3A;//!<EXTRACT BARCODE DATA command code
        const byte POWER_RECOVERY = 0x66;//!<POWER RECOVERY command code
        const byte EMPTY_DISPENSER = 0x67;//!<EMPTY DISPENSER command code
        const byte SET_OPTIONS = 0x68;//!<SET OPTIONS command code
        const byte GET_OPTIONS = 0x69;//!<GET OPTIONS command code
                                      /**@} */

        /**	\defgroup Options Options
        * Describes options supported by CCNET (as a bitmap)
        * @{
*/
        //Options (bitmap)
        const long OPT_LED_INHIBIT = 0x80000000;//!< Turn OFF LEDs of the bezel in the DISABLED state
        const long OPT_KEEP_BILL = 0x40000000;//!< Hold bill after ejection on the input roller
        const long OPT_LOOK_TAPE = 0x20000000; //!< Use improved algorithm for tape detection
        const long OPT_TURN_SWITCH = 0x10000000; //!< Turn switch after packing a bill
                                                  /**@} */
        // States
        /**	\defgroup States CCNET states and events
        * 
        * @{
*/
        const byte ST_POWER_UP = 0x10;//!< POWER UP state
        const byte ST_POWER_BILL_ESCROW = 0x11;//!< POWER UP WITH BILL IN ESCROW state
        const byte ST_POWER_BILL_STACKER = 0x12;//!< POWER UP WITH BILL IN STACKER state
        const byte ST_INITIALIZE = 0x13;//!< INITIALIZING state
        const byte ST_IDLING = 0x14;//!< IDLING state
        const byte ST_ACCEPTING = 0x15;//!< ACCEPTING state
        const byte ST_PACKING = 0x17;//!< STACKING/PACKING state

        const byte ST_RETURNING = 0x18;//!< RETURNING state
        const byte ST_DISABLED = 0x19;//!< UNIT DISABLED state
        const byte ST_HOLDING = 0x1A;//!< HOLDING state
        const byte ST_BUSY = 0x1B;//!< Device is busy
        const byte ST_REJECTING = 0x1C;//!< REJECTING state. Followed by a rejection code
                                       //Rejection codes
                                       /**	\defgroup RCodes Rejection codes
                                       * 
                                       * @{
*/
        const byte RJ_INSERTION = 0x60; //!< Rejection because of insertion problem
        const byte RJ_MAGNETIC = 0x61; //!< Rejection because of invalid magnetic pattern
        const byte RJ_REMAINING = 0x62; //!< Rejection because of other bill remaining in the device
        const byte RJ_MULTIPLYING = 0x63; //!< Rejection because of multiple check failures
        const byte RJ_CONVEYING = 0x64; //!< Rejection because of conveying 
        const byte RJ_IDENT = 0x65; //!< Rejection because of identification failure
        const byte RJ_VRFY = 0x66; //!< Rejection because of verification failure
        const byte RJ_OPT = 0x67; //!< Rejection because of optical pattern mismatch
        const byte RJ_INHIBIT = 0x68; //!< Rejection because the denomination is inhibited
        const byte RJ_CAP = 0x69; //!< Rejection because of capacity sensor pattern mismatch
        const byte RJ_OPERATION = 0x6A; //!< Rejection because of operation error
        const byte RJ_LNG = 0x6C; //!< Rejection because of invalid bill length
        const byte RJ_UV = 0x6D; //!< Rejection because of invalid UV pattern
        const byte RJ_BAR = 0x92; //!< Rejection because of unrecognized barcode
        const byte RJ_BAR_LNG = 0x93; //!< Rejection because of invalid barcode length
        const byte RJ_BAR_START = 0x94; //!< Rejection because of invalid barcode start sequence
        const byte RJ_BAR_STOP = 0x95; //!< Rejection because of invalid barcode stop sequence
                                                /**@} */

        const byte ST_DISPENSING = 0x1D;//!< DISPENSING state
        const byte ST_UNLOADING = 0x1E;//!< UNLOADING state 
        const byte ST_SETTING_CS_TYPE = 0x21;//!< SETTING RECYCLING CASSETTE TYPE state
        const byte ST_DISPENSED = 0x25;//!< DISPENSED event
        const byte ST_UNLOADED = 0x26;//!< UNLOADED event
        const byte ST_BILL_NUMBER_ERR = 0x28;//!< INVALID BILL NUMBER event
        const byte ST_CS_TYPE_SET = 0x29;//!< RECYCLING CASSETTE TYPE SET event
        const byte ST_ST_FULL = 0x41;//!< DROP CASSETTE IS FULL state
        const byte ST_BOX = 0x42;//!< DROP CASSETTE REMOVED state 
        const byte ST_BV_JAMMED = 0x43;//!< JAM IN VALIDATOR state
        const byte ST_ST_JAMMED = 0x44;//!< JAM IN STACKER state
        const byte ST_CHEATED = 0x45;//!< CHEATED event
        const byte ST_PAUSED = 0x46;//!< PAUSED state
        const byte ST_FAILURE = 0x47;//!< FAILURE state

        //Failure codes
        /**	\defgroup FCodes Failure codes
        * 
        * @{
*/
        const byte FLR_STACKER = 0x50; //!< Stacking mechanism failure
        const byte FLR_TR_SPEED = 0x51; //!< Invalid speed of transport mechanism
        const byte FLR_TRANSPORT = 0x52; //!< Transport mechanism failure
        const byte FLR_ALIGNING = 0x53; //!< Aligning mechanism failure
        const byte FLR_INIT_CAS = 0x54; //!< Initial cassette status failure
        const byte FLR_OPT = 0x65; //!< Optical channel failure
        const byte FLR_MAG = 0x66; //!< Inductive channel failure
        const byte FLR_CAP = 0x67; //!< Capacity sensor failure
                                   /**@} */

        // Credit events
        const byte ST_PACKED = 0x81;    /**< A bill has been packed. 2nd byte - 0xXY:
															\n X-bill type
															\n Y-Packed into:
															\n 0-BOX, else - Cassette Y;

											*/
        const byte ESCROW = 0x80; //!< A bill is held in the escrow position	
        const byte RETURNED = 0x82; //!< A bill was returned
                                    /**@} */
        // Cassetes status
        /**	\defgroup CSStatus Possible cassette status codes
        * 
        * @{
*/
        const byte CS_OK = 0;   //!< Cassette is present and operational
        const byte CS_FULL = 1; //!< Cassette is full
        const byte CS_NU = 0xFE;//!< Cassette is not present
        const byte CS_MALFUNCTION = 0xFF;//!< Cassette is malfunctioning
        const byte CS_NA = 0xFD;//!< Cassette is not assigned to any denomination
        const byte CS_ESCROW = 0xFC;//!< Cassette is assigned to multi-escrow 
                                    /**@} */
        /**	\defgroup BTs Predefined bill type values
        * 
        * @{
*/
        const byte BT_ESCROW = 24; //!< Bill type associated with the escrow cassette
        const byte BT_NO_TYPE = 0x1f; //!< Invalid bill type
        const byte BT_BAR = 23; //!< Bill type associated with barcode coupon
                                /**@} */
        // Error codes 
        /**	\defgroup ErrCode CCNET Interface error codes
        *
        * @{
*/
        /** \defgroup CErrs Communication error codes
            
              The codes related to phisical data transmission and frame integrity

           @{
*/
        const int RE_NONE = 0; //!< No error happened
        const int RE_TIMEOUT = -1;//!< Communication timeout
        const int RE_SYNC = -2;//!< Synchronization error (invalid synchro byte)
        const int RE_DATA = -3;//!< Data reception error
                               /**@} */
        const int RE_CRC = -4;//!< CRC error

        /** \defgroup LErrs Logical error codes

          The codes related to the interface logic

           @{
*/
        const int ER_NAK = -5;//!< NAK received
        const int ER_INVALID_CMD = -6;//!< Invalid command response received 
        const int ER_EXECUTION = -7;//!< Execution error response received
        const int ERR_INVALID_STATE = -8;//!< Invalid state received

        #endregion

        public int BillAdr = 1;

        static int ibytesToRecieve;         //!< A variable containing number of bytes to receive from the device at once
        static int iRecievingError = 0;     //!< A variable containing a communication error code produced by last serial communication

        CCommand cmdIn;             //!< A variable to store current device responses
        CCommand cmdOut;            //!< A variable to store controller commands
        CCOMPort COMPort;           //!< A COM port to work with

        public int iCmdDelay;       //!< Delay between two consequtive commands
        public int iTimeout;        //!< Communication timeout value
        public int iLastError;      //!< A variable storing error code generated during last serial I/O operation

        // Protocol structures

        /**	\struct _BillStatus
            \brief	The _BillStatus struct describing response to the STATUS REQUEST command
        */
        public _BillStatus BillStatus = new _BillStatus();  //!< Variable containing the most recent response to the STATUS REQUEST

        /**	\struct _Identification
			\brief	The _Identification struct contains identification of the device
		*/
        public _Identification Ident = new _Identification();//!< A variable containing current device identification

        /**	\struct _PollResults
			\brief	The _PollResults struct containing 2 first bytes of the response to the POLL command
		*/
        public _PollResults PollResults = new _PollResults(); //!< A variable keeping last POLL result

        public List<_Cassete> Cassetes = new List<_Cassete>();       //!< List of the cassettes 
        public _Cassete EscrCassete = new _Cassete();                //!< Escrow cassette

        public CCRSProtocol()
        {
            iCmdDelay = 20;

            Ident.BCCPUBoot = Encoding.ASCII.GetBytes("N/A");
            Ident.BCCPUVersion = Encoding.ASCII.GetBytes("N/A");
            Ident.BCCS1Boot = Encoding.ASCII.GetBytes("N/A");
            Ident.BCCS2Boot = Encoding.ASCII.GetBytes("N/A");
            Ident.BCCS3Boot = Encoding.ASCII.GetBytes("N/A");
            Ident.BCCSVersion = Encoding.ASCII.GetBytes("N/A");
            Ident.BCDispenserBoot = Encoding.ASCII.GetBytes("N/A");
            Ident.BCDispenserVersion = Encoding.ASCII.GetBytes("N/A");
            Ident.BVBootVersion = Encoding.ASCII.GetBytes("N/A");
            Ident.BVVersion = Encoding.ASCII.GetBytes("N/A");
            Ident.PartNumber = Encoding.ASCII.GetBytes("N/A");
        }

        public void openPort(string com_port)
        {
            if (COMPort == null)
            {
                COMPort = new CCOMPort();

                COMPort.OpenCOM(com_port);
            }
        }

        public void closePort()
        {
            if (COMPort != null)
            {
                COMPort.CloseCOM();
                COMPort = null;
            }
        }

        public CCOMPort GetCOMPort()
        {
            return COMPort;
        }

        public long PortState(int iPort)
        {
            return COMPort.IsEnable(iPort);
        }

        public bool InitCOM(string COMi)
        {
            return COMPort.OpenCOM(COMi);
        }

        ushort crc16_ccitt(byte data, ushort crc)
        {
            ushort a = 0x8408, d = crc, i;

            d ^= data;

            for (i = 0; i < 8; i++)
            {
                if ((d & 0x0001) > 0)
                {
                    d >>= 1;
                    d ^= a;
                }
                else d >>= 1;
            }
            return d;
        }

        ushort CalculateCRC(byte[] Buffer)
        {
            ushort wCRC = 0, Len = (ushort)((Buffer[2] > 0) ? Buffer[2] : (((ushort)(Buffer)[4]) << 8) + (Buffer)[5]);

            //wCRC = GetCRC16(Buffer, (ushort)(Len - 2));

            for (int i = 0; i < Len - 2; i++)
                wCRC = crc16_ccitt(Buffer[i], wCRC);

            return wCRC;
        }

        ushort GetCRC16(byte[] bufData, ushort sizeData)
        {
            ushort CRC, i;
            byte j;
            CRC = 0;
            for (i = 0; i < sizeData; i++)
            {
                CRC ^= bufData[i];
                for (j = 0; j < 8; j++)
                {
                    if ((CRC & 0x0001) > 0) { CRC >>= 1; CRC ^= 0x08408; }
                    else CRC >>= 1;
                }
            }
            return CRC;
        }
   
        int SendCommand(byte[] BufOut, byte[] BufIn)
        {
            iRecievingError = RE_TIMEOUT;
            for (int iErrCount = 0; iErrCount < 1; iErrCount++)
            {
                ibytesToRecieve = 6;

                //PurgeComm(COMPort.GetHandle(), PURGE_RXABORT | PURGE_TXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR);
                COMPort.GetHandle().DiscardInBuffer();
                COMPort.GetHandle().DiscardOutBuffer();

                if (BufOut[2] == 0)
                    COMPort.Send(BufOut, ((int)BufOut[4] << 8) + BufOut[5]);
                else
                    COMPort.Send(BufOut, BufOut[2]);

                if ((BufOut[3] == ACK) || (BufOut[3] == NAK))
                    return iRecievingError = RE_NONE;

                if (COMPort.Recieve(BufIn, ibytesToRecieve))
                {
                    if (BufIn[0] != SYNC)
                        iRecievingError = RE_SYNC;
                    else
                    {
                        int iLen = ((BufIn[2] > 0) ? BufIn[2] : (BufIn[5] + ((int)BufIn[4] << 8))) - ibytesToRecieve;
                        if (iLen > 0)
                        {

                            if (COMPort.Recieve(BufIn,ibytesToRecieve, iLen))
                            {
                                iRecievingError = RE_NONE;
                                break;
                            }
                            else
                            {
                                iRecievingError = RE_DATA;

                                //PurgeComm(COMPort.GetHandle(), PURGE_RXABORT | PURGE_RXCLEAR);
                                //COMPort.GetHandle().DiscardInBuffer();
                                //COMPort.GetHandle().DiscardOutBuffer();
                            }
                        }
                        else
                        {
                            iRecievingError = RE_NONE;
                            break;
                        }
                    }
                }
            }
            return iRecievingError;
        }

        CCommand TransmitCMD(CCommand Cmd, byte Addr)
        {
            byte[] tmpBuffer = new byte[256];

            int i = (Cmd.GetData()[2] > 0) ? (Cmd.GetData())[2] :
                                            (((ushort)(Cmd.GetData())[4]) << 8) + (Cmd.GetData())[5];
            Cmd.SetByte(Addr, 1);
            ushort wCRC = CalculateCRC(Cmd.GetData());
            Cmd.SetByte((byte)wCRC, i - 2);
            Cmd.SetByte((byte)(wCRC >> 8), i - 1);
            cmdOut = Cmd;

            int iErrCode = SendCommand(Cmd.GetData(), tmpBuffer);
            if ((iErrCode == 0) && (Cmd.GetData()[3] > 0) && (0xFF != Cmd.GetData()[3]))
            {
                wCRC = (ushort)(tmpBuffer[(tmpBuffer[2] > 0) ? tmpBuffer[2] : (((ushort)(tmpBuffer[4])) << 8) + tmpBuffer[5] - 2] +
                    (tmpBuffer[(tmpBuffer[2] > 0) ? tmpBuffer[2] : (((ushort)(tmpBuffer[4])) << 8) + tmpBuffer[5] - 1] << 8));

                if (CalculateCRC(tmpBuffer) != wCRC)
                    iErrCode = RE_CRC;
                cmdIn = new CCommand(tmpBuffer, iErrCode, (tmpBuffer[2] > 0) ? (tmpBuffer)[2] :
                                            (((ushort)(tmpBuffer)[4]) << 8) + (tmpBuffer)[5]);
                return cmdIn;
            }
            cmdIn = new CCommand(tmpBuffer, iErrCode);
            return cmdIn;
        }

        CCommand Transmit(CCommand CMD, byte Addr = 0x03)
        {
            CCommand cmdRes = new CCommand(), cmdACK = new CCommand();
            for (int i = 0; i < 3; i++)
            {
                cmdRes = TransmitCMD(CMD, Addr);
                cmdACK.SetByte(SYNC, 0);
                cmdACK.SetByte(6, 2);
                cmdACK.SetByte(ACK, 3);

                if (cmdRes.GetCode() == RE_NONE)
                {

                    if ((ACK == cmdRes.GetData()[3]) && (cmdRes.GetData()[2] == 6))
                    {
                        return cmdRes;
                    }
                    if ((NAK == cmdRes.GetData()[3]) && (cmdRes.GetData()[2] == 6))
                    {

                        if (iCmdDelay > 0) Thread.Sleep(iCmdDelay);//5	
                    }
                    else
                    {
                        cmdACK.SetByte(ACK, 3);
                        TransmitCMD(cmdACK, Addr);
                        if (iCmdDelay > 0) Thread.Sleep(iCmdDelay);//5
                        break;
                    }
                }
                else
                {
                    if (cmdRes.GetCode() != RE_TIMEOUT)
                    {
                        cmdACK.SetByte(NAK, 3);
                        TransmitCMD(cmdACK, Addr);
                        if (iCmdDelay > 0) Thread.Sleep(iCmdDelay);//5
                    }
                }
            }

            return cmdRes;
        }

        //////////////////////////////////////////////////////////////////////
        // CCNET Commands implementation
        /** \defgroup CCNETCommands CCNET protocol commands and requests

            The group contains member functions providing interface to CCNET commands and requests.
            All functions return a bool result showing whether operation was successfully completed.
            In the case of error the error code (refer to \link ErrCode Possible error codes \endlink) 
            is stored in the CCCRSProtocol::iLastError member variable, which can be used in further analysis.

          @{
        */
        public bool Cmd(CCNETCommandEnum command,byte adr, object param1 = null, object param2 = null)
        {
            switch(command)
            {
                case CCNETCommandEnum.Reset:
                    return CmdReset(adr);
                    break;
                case CCNETCommandEnum.Poll:
                    return CmdPoll(adr);
                    break;
                case CCNETCommandEnum.Status:
                    return CmdStatus(adr);
                    break;
                case CCNETCommandEnum.Information:
                    return CmdIdentification(adr);
                    break;
                case CCNETCommandEnum.Hold:
                    return CmdHold(adr);
                    break;
                case CCNETCommandEnum.SetSecurity:
                    return CmdSetSecurity((long)param1,adr);
                    break;
                case CCNETCommandEnum.BillType:
                    return CmdBillType((long)param1, (long)param2, adr);
                    break;
                case CCNETCommandEnum.Pack:
                    return CmdPack(adr);
                    break;
                case CCNETCommandEnum.Return:
                    return CmdReturn(adr);
                    break;
                case CCNETCommandEnum.SetBarParams:
                    return CmdSetBarParams((byte)param1, (byte)param2, adr);
                    break;
                case CCNETCommandEnum.ExtractBarData:
                    return CmdExtractBarData((byte[])param1, adr);
                    break;
                case CCNETCommandEnum.GetBillTable:
                    return CmdGetBillTable((_BillRecord[])param1, adr);
                    break;
            }

            return false;
        }

        /**	\brief	The CCCRSProtocol::CmdReset function sends a RESET command to the device

            \param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

            \return	bool - true if command was acknowledged    
        */
        bool CmdReset(byte Addr)
        {
            byte[] Data = new byte[256];
            Data[0] = SYNC;
            Data[1] = 0;
            Data[2] = 6;
            Data[3] = RESET;
            Data[4] = 0;
            Data[5] = 0;

            CCommand cmd = new CCommand(Data,0, 6);

            CCommand Response = Transmit(cmd, Addr);
            byte ack;

            iLastError = Response.GetCode();

            if(iLastError == 0)
            {
                if ((ack = Response.GetData()[3]) != ACK)
                {
                    iLastError = (ack != ST_INV_CMD) ? ER_NAK : ER_INVALID_CMD;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /**	\brief	The CCCRSProtocol::CmdPoll function sends POLL command to the device

          The function sends POLL command and fills bytes Z1 and Z2 of the response into the CCCRSProtocol::PollResults structure.

            \param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

            \return	bool - true if exchange was successfully completed    
        */
        bool CmdPoll(byte Addr)
        {
            byte[] Data = new byte[256];
            Data[0] = SYNC;
            Data[1] = 0;
            Data[2] = 6;
            Data[3] = POLL;
            Data[4] = 0;
            Data[5] = 0;

            CCommand cmd = new CCommand(Data,0);

            CCommand Response = Transmit(cmd, Addr);
            iLastError = Response.GetCode();

            if (iLastError == 0)
            {
                PollResults.Z1 = Response.GetData()[3];
                PollResults.Z2 = Response.GetData()[4];
                return true;
            }
            else
            {
                PollResults.Z1 = 0;
                PollResults.Z2 = 0;
                return false;
            }
        }

        /**	\brief	The CCCRSProtocol::CmdStatus function sends STATUS REQUEST to the device

          The response status data is stored in the CCCRSProtocol::BillStatus member structure.

            \param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

            \return	bool - true if exchange was successfully completed    
        */
        bool CmdStatus(byte Addr)
        {
            byte[] Data = new byte[256];
            Data[0] = SYNC;
            Data[1] = 0;
            Data[2] = 6;
            Data[3] = GET_STATUS;
            Data[4] = 0;
            Data[5] = 0;

            CCommand cmd = new CCommand(Data,0);

            CCommand Response = Transmit(cmd, Addr);
            iLastError = Response.GetCode();

            if (iLastError == 0)
            {
                if ((Response.GetData()[3] == ST_INV_CMD) && (Response.GetData()[2] == 6))
                {
                    iLastError = ER_INVALID_CMD;
                    BillStatus.Enabled = 0;
                    BillStatus.Security = 0;
                    BillStatus.Routing = 0;
                    return false;
                }
                BillStatus.Enabled = Response.GetData()[5] + ((ushort)Response.GetData()[4] << 8) + ((ushort)Response.GetData()[3] << 16);
                BillStatus.Security = Response.GetData()[8] + ((ushort)Response.GetData()[7] << 8) + ((ushort)Response.GetData()[6] << 16);
                BillStatus.Routing = Response.GetData()[11] + ((ushort)Response.GetData()[10] << 8) + ((ushort)Response.GetData()[9] << 16);
                return true;
            }
            else
            {
                return false;
            }
        }

        /**	\brief	The CCCRSProtocol::CmdIdentification function sends IDENTIFICATION request

          The function sends IDENTIFICATION request and stores device identification in the member CCCRSProtocol::Ident structure.
          The function supports both new and old identification formats of Bill-To-Bill units.

            \param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

            \return	bool - true if the exchange was successfully completed and data received    
        */
        bool CmdIdentification(byte Addr)
        {
            byte[] Data = new byte[256];
            Data[0] = SYNC;
            Data[1] = 0;
            Data[2] = 6;
            Data[3] = IDENTIFICATION;
            Data[4] = 0;
            Data[5] = 0;

            CCommand cmd = new CCommand(Data,0);
            CCommand Response = Transmit(cmd, Addr);

            iLastError = Response.GetCode();

            if (iLastError == 0)
            {
                if ((Response.GetData()[3] == ST_INV_CMD) && (Response.GetData()[2] == 6))
                {
                    iLastError = ER_INVALID_CMD;
                    return false;
                }

                Ident.BCCPUBoot = Encoding.ASCII.GetBytes("N/A");
                Ident.BCCPUVersion = Encoding.ASCII.GetBytes("N/A");
                Ident.BCCS1Boot = Encoding.ASCII.GetBytes("N/A");
                Ident.BCCS2Boot = Encoding.ASCII.GetBytes("N/A");
                Ident.BCCS3Boot = Encoding.ASCII.GetBytes("N/A");
                Ident.BCCSVersion = Encoding.ASCII.GetBytes("N/A");
                Ident.BCDispenserBoot = Encoding.ASCII.GetBytes("N/A");
                Ident.BCDispenserVersion = Encoding.ASCII.GetBytes("N/A");
                Ident.BVBootVersion = Encoding.ASCII.GetBytes("N/A");
                Ident.BVVersion = Encoding.ASCII.GetBytes("N/A");
                Ident.PartNumber = Encoding.ASCII.GetBytes("N/A");

                byte[] sTemp = new byte[64];
                int iPos = 3, iLen = 15;

                Array.Copy(Response.GetData(), iPos, sTemp, 0, iLen);
                sTemp[iLen] = 0; iPos += iLen;
                Array.Copy(sTemp, 0, Ident.PartNumber, 0, sTemp.Length);

                iLen = 12;
                Array.Copy(Response.GetData(), iPos, sTemp, 0, iLen);
                sTemp[iLen] = 0; iPos += iLen;
                Array.Copy(sTemp, 0, Ident.SN, 0, sTemp.Length);

                byte[] strTemp = new byte[256];

                Array.Copy(Response.GetData(), iPos, strTemp, 0, iLen);

                Ident.DS1 = 0; iPos += 8;
                for (int i = 0; i < 7; i++)
                {
                    Ident.DS1 <<= 8;
                    Ident.DS1 += strTemp[i];
                }

                if (Response.GetData()[2] < 109) return true;

                iLen = 6;
                Array.Copy(Response.GetData(), iPos, sTemp, 0, iLen);
                sTemp[iLen] = 0; iPos += iLen;
                Array.Copy(sTemp, 0, Ident.BVBootVersion, 0, sTemp.Length);

                iLen = 20;
                Array.Copy(Response.GetData(), iPos, sTemp, 0, iLen);
                sTemp[iLen] = 0; iPos += iLen;
                Array.Copy(sTemp, 0, Ident.BVVersion, 0, sTemp.Length);

                iLen = 6;
                Array.Copy(Response.GetData(), iPos, sTemp, 0, iLen);
                sTemp[iLen] = 0; iPos += iLen;
                Array.Copy(sTemp, 0, Ident.BCCPUBoot, 0, sTemp.Length);

                iLen = 6;
                Array.Copy(Response.GetData(), iPos, sTemp, 0, iLen);
                sTemp[iLen] = 0; iPos += iLen;
                Array.Copy(sTemp, 0, Ident.BCCPUVersion, 0, sTemp.Length);

                iLen = 6;
                Array.Copy(Response.GetData(), iPos, sTemp, 0, iLen);
                sTemp[iLen] = 0; iPos += iLen;
                Array.Copy(sTemp, 0, Ident.BCDispenserBoot, 0, sTemp.Length);

                iLen = 6;
                Array.Copy(Response.GetData(), iPos, sTemp, 0, iLen);
                sTemp[iLen] = 0; iPos += iLen;
                Array.Copy(sTemp, 0, Ident.BCDispenserVersion, 0, sTemp.Length);

                iLen = 6;
                Array.Copy(Response.GetData(), iPos, sTemp, 0, iLen);
                sTemp[iLen] = 0; iPos += iLen;
                Array.Copy(sTemp, 0, Ident.BCCS1Boot, 0, sTemp.Length);

                iLen = 6;
                Array.Copy(Response.GetData(), iPos, sTemp, 0, iLen);
                sTemp[iLen] = 0; iPos += iLen;
                Array.Copy(sTemp, 0, Ident.BCCS2Boot, 0, sTemp.Length);

                iLen = 6;
                Array.Copy(Response.GetData(), iPos, sTemp, 0, iLen);
                sTemp[iLen] = 0; iPos += iLen;
                Array.Copy(sTemp, 0, Ident.BCCS3Boot, 0, sTemp.Length);

                iLen = 6;
                Array.Copy(Response.GetData(), iPos, sTemp, 0, iLen);
                sTemp[iLen] = 0; iPos += iLen;
                Array.Copy(sTemp, 0, Ident.BCCSVersion, 0, sTemp.Length);

                return true;
            }
            else
            {
                return false;
            }
        }

        /**	\brief	The CCCRSProtocol::CmdHold function sends HOLD command to the device

            \param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

            \return	bool - true if exchange successfully completed    
        */
        bool CmdHold(byte Addr)
        {
            byte[] Data = new byte[256];
            Data[0] = SYNC;
            Data[1] = 0;
            Data[2] = 6;
            Data[3] = HOLD;
            Data[4] = 0;
            Data[5] = 0;

            CCommand cmd = new CCommand(Data,0);
            CCommand Response = Transmit(cmd, Addr);
            byte ack;

            iLastError = Response.GetCode();

            if (iLastError == 0)
            {
                if ((ack = Response.GetData()[3]) != ACK)
                {
                    iLastError = (ack != ST_INV_CMD) ? ER_NAK : ER_INVALID_CMD;
                    return false;
                }
                else return true;

            }
            else
            {
                return false;
            }
        }

        /**	\brief	The CCCRSProtocol::CmdSetSecurity function sends SET SECURITY LEVELS command

	        \param	wS	a parameter of type DWORD - a bitmap containing security levels to set
	        \param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

	        \return	bool - true if exchange successfully completed
        */
        bool CmdSetSecurity(long wS, byte Addr)
        {
            byte[] Data = new byte[256];
            Data[0] = SYNC;
            Data[1] = 0;
            Data[2] = 9;
            Data[3] = SET_SECURITY;
            Data[4] = (byte)(wS >> 16);
            Data[5] = (byte)(wS >> 8);
            Data[6] = (byte)wS;
            Data[7] = 0;
            Data[8] = 0;

            CCommand cmd = new CCommand(Data,0);
            CCommand Response = Transmit(cmd, Addr);
            byte ack;

            iLastError = Response.GetCode();

            if (iLastError == 0)
            {
                if ((ack = Response.GetData()[3]) != ACK)
                {
                    iLastError = (ack != ST_INV_CMD) ? ER_NAK : ER_INVALID_CMD;
                    return false;
                }
                else return true;

            }
            else
            {
                return false;
            }
        }

        /**	\brief	The CCCRSProtocol::CmdBillType function sends ENABLE BILL TYPE command

	        \param	enBill	a parameter of type DWORD - a bitmap containing 1 in the positions corresponding to the enabled bill types
	        \param	escBill	a parameter of type DWORD - a bitmap containing 1 in the positions corresponding to bill type processed with escrow
	        \param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

	        \return	bool- true if the command was acknowledged
        */
        bool CmdBillType(long enBill, long escBill, byte Addr)
        {
            byte[] Data = new byte[256];
            Data[0] = SYNC;
            Data[1] = 0;
            Data[2] = 12;
            Data[3] = BILL_TYPE;
            Data[4] = (byte)(enBill >> 16);
            Data[5] = (byte)(enBill >> 8);
            Data[6] = (byte)enBill;
            Data[7] = (byte)(escBill >> 16);
            Data[8] = (byte)(escBill >> 8);
            Data[9] = (byte)escBill;

            Data[10] = 0;
            Data[11] = 0;

            CCommand cmd = new CCommand(Data,0);
            CCommand Response = Transmit(cmd, Addr);
            byte ack;

            iLastError = Response.GetCode();

            if (iLastError == 0)
            {
                if ((ack = Response.GetData()[3]) != ACK)
                {
                    iLastError = (ack != ST_INV_CMD) ? ER_NAK : ER_INVALID_CMD;
                    return false;
                }
                else return true;

            }
            else
            {
                return false;
            }
        }

        /**	\brief	The CCCRSProtocol::CmdPack function sends PACK command

	    \param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

	    \return	bool - true if the command was acknowledged
        */
        bool CmdPack(byte Addr)
        {
            byte[] Data = new byte[256];
            Data[0] = SYNC;
            Data[1] = 0;
            Data[2] = 6;
            Data[3] = PACK;

            Data[4] = 0;
            Data[5] = 0;

            CCommand cmd = new CCommand(Data,0);
            CCommand Response = Transmit(cmd, Addr);
            byte ack;
            iLastError = Response.GetCode();

            if (iLastError == 0)
            {
                if ((ack = Response.GetData()[3]) != ACK)
                {
                    iLastError = (ack != ST_INV_CMD) ? ER_NAK : ER_INVALID_CMD;
                    return false;
                }
                else return true;

            }
            else
            {
                return false;
            }
        }

        /**	\brief	The CCCRSProtocol::CmdReturn function sends RETURN command

            \param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

            \return	bool - true if the command was acknowledged  
        */
        bool CmdReturn(byte Addr)
        {
            byte[] Data = new byte[256];
            Data[0] = SYNC;
            Data[1] = 0;
            Data[2] = 6;
            Data[3] = RETURN;

            Data[4] = 0;
            Data[5] = 0;

            CCommand cmd = new CCommand(Data,0);
            CCommand Response = Transmit(cmd, Addr);
            byte ack;
            iLastError = Response.GetCode();

            if (iLastError == 0)
            {
                if ((ack = Response.GetData()[3]) != ACK)
                {
                    iLastError = (ack != ST_INV_CMD) ? ER_NAK : ER_INVALID_CMD;
                    return false;
                }
                else return true;

            }
            else
            {
                return false;
            }
        }

        /**	\brief	The CCCRSProtocol::CmdSetBarParams function sends SET BARCODE PARAMETERS command

            \param	Format	a parameter of type BYTE specifiying barcode format
            \param	Length	a parameter of type BYTE specifiying barcode length
            \param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

            \return	bool - true if the command was acknowledged    
        */
        bool CmdSetBarParams(byte Format, byte Length, byte Addr = ADDR_BB)
        {
            byte[] Data = new byte[256];
            Data[0] = SYNC;
            Data[1] = 0;
            Data[2] = 8;
            Data[3] = SET_BAR_PARAMS;
            Data[4] = Format;
            Data[5] = Length;

            Data[6] = 0;
            Data[7] = 0;

            CCommand cmd = new CCommand(Data,0);
            CCommand Response = Transmit(cmd, Addr);
            byte ack;

            iLastError = Response.GetCode();

            if (iLastError == 0)
            {
                if ((ack = Response.GetData()[3]) != ACK)
                {
                    iLastError = (ack != ST_INV_CMD) ? ER_NAK : ER_INVALID_CMD;
                    return false;
                }
                else return true;

            }
            else
            {
                return false;
            }
        }

        /**	\brief	The CCCRSProtocol::CmdExtractBarData function sends EXTRACT BARCODE DATA command

	        \param	sBar	a parameter of type LPSTR containing pointer to a zero-terminated string receiving the barcode value.
	        \param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

	        \return	bool - true if the response was successfully received
        */
        bool CmdExtractBarData(byte[] sBar, byte Addr = ADDR_BB)
        {
            byte[] Data = new byte[256];
            Data[0] = SYNC;
            Data[1] = 0;
            Data[2] = 6;
            Data[3] = EXTRACT_BAR_DATA;
            Data[4] = 0;
            Data[5] = 0;


            CCommand cmd = new CCommand(Data,0);
            CCommand Response = Transmit(cmd, Addr);

            iLastError = Response.GetCode();

            if (iLastError == 0)
            {
                if ((Response.GetData()[3] == ST_INV_CMD) && (Response.GetData()[2] == 6))
                {
                    iLastError = ER_INVALID_CMD;
                    return false;
                }

                //strcpy(sBar, "");
                for (int i = 3; i < Response.GetData()[2] - 2; i++)
                    sBar[i - 3] = Response.GetData()[i];

                return true;
            }
            else
            {
                return false;
            }
        }

        /**	\brief	The CCCRSProtocol::CmdGetBillTable function sends BILL TABLE request

            \param	BillTable	a parameter of type _BillRecord * containing pointer to the _BillRecord array receiving the bill table.
                    Position in the array corresponds to the bill type and the structure at the position describes that bill type.
            \param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

            \return	bool - true if the response was successfully received

            
*/
        bool CmdGetBillTable(_BillRecord[] BillTable, byte Addr)
        {
            byte[] Data = new byte[256];
            Data[0] = SYNC;
            Data[1] = 0;
            Data[2] = 6;
            Data[3] = GET_BILL_TABLE;
            Data[4] = 0;
            Data[5] = 0;

            CCommand cmd = new CCommand(Data,0);
            CCommand Response = Transmit(cmd, Addr);

            iLastError = Response.GetCode();

            if (iLastError == 0)
            {
                int i = 0;

                if ((Response.GetData()[3] == ST_INV_CMD) && (Response.GetData()[2] == 6))
                {
                    iLastError = ER_INVALID_CMD;
                    for (i = 0; i < 24; i++)
                    {
                        BillTable[i].Denomination = 0;
                        BillTable[i].sCountryCode = Encoding.ASCII.GetBytes("");
                    }
                    return false;
                }

                for (i = 0; i < (Response.GetData()[2]) - 5; i += 5)
                {
                    BillTable[i / 5].Denomination = Response.GetData()[i + 3];

                    byte[] sTmp = new byte[5];
                    Array.Copy(Response.GetData(), i + 4, sTmp, 0, 3);
                    sTmp[3] = 0;
                    Array.Copy(sTmp, 0, BillTable[i / 5].sCountryCode, 0, sTmp.Length);

                    if ((((Response.GetData())[i + 7]) & 0x80) > 0)
                    {
                        for (int j = 0; j < ((Response.GetData()[i + 7]) & 0x7F); j++)
                            BillTable[i / 5].Denomination /= 10;
                    }
                    else
                    {
                        for (int j = 0; j < ((Response.GetData()[i + 7]) & 0x7F); j++)
                            BillTable[i / 5].Denomination *= 10;
                    }
                }

                for (int j = i; i < 24 * 5; i += 5)
                {
                    BillTable[i / 5].Denomination = 0;
                    BillTable[i / 5].sCountryCode = Encoding.ASCII.GetBytes("");
                }
                return true;

            }
	        else 
	        {
		        return false;
	        }
        }

    }
}
