/** @file  CCRSProtocol.h: interface for the CCCRSProtocol class
* The file is a C++ header file containing all necessary declarations for CCNET protocol.
*       
*
* \n Product: All CCNET devices
* \n Country: All
* \n Protocol: CCNET
* \n Protocol Version: 2.4.2
*/

#if !defined(AFX_CCRSPROTOCOL_H__5BF7AFD2_D008_11D4_A0E8_00002165B0A2__INCLUDED_)
#define AFX_CCRSPROTOCOL_H__5BF7AFD2_D008_11D4_A0E8_00002165B0A2__INCLUDED_

#include "COMPort.h"	
#include "Command.h"	
#include "VMCConst.h"
#include <time.h>

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000


extern char* cCOMparam;  //!< external string containig current communication format string for COM port initialization


const BYTE SYNC =0x02;	//!< synchronization byte 
const BYTE ACK  =0x00;	//!< ACK code
const BYTE NAK	=0xFF;	//!< NAK code
const char ST_INV_CMD=0x30;//!< INVALID COMMAND response

/**	\defgroup Addr Device Addresses
* @{
*/
const BYTE ADDR_BB		=0x01; //!< Address for Bill-To-Bill units
const BYTE ADDR_CHANGER =0x02; //!< Address for Coin Changer
const BYTE ADDR_FL		=0x03; //!< Address for Bill Validators
const BYTE ADDR_CR		=0x04; //!< Address for Smart Card Reader
/**@} */

/**	\defgroup Cmds Interface commands
* @{
*/
const BYTE RESET		=0x30; //!<REST command code
const BYTE GET_STATUS	=0x31; //!<STATUS REQUEST command code
const BYTE SET_SECURITY	=0x32; //!<SET SECURITY command code
const BYTE POLL			=0x33; //!<POLL command code
const BYTE BILL_TYPE	=0x34; //!<BILL TYPE command code
const BYTE PACK			=0x35; //!<PACK command code
const BYTE RETURN		=0x36; //!<RETURN command code
const BYTE IDENTIFICATION=0x37;//!<IDENTIFICATION command code
const BYTE IDENT_EXT	=0x3E;//!<EXTENDED IDENTIFICATION command code
const BYTE HOLD			=0x38;//!<HOLD command code
const BYTE C_STATUS		=0x3B;//!<RECYCLING CASSETTE STATUS REQUEST command code
const BYTE DISPENSE		=0x3C;//!<DISPENSE command code
const BYTE UNLOAD		=0x3D;//!<UNLOAD command code
const BYTE SET_CASSETES	=0x40;//!<SET RECYCLING CASSETTE TYPE command code
const BYTE GET_BILL_TABLE=0x41;//!<BILL TABLE REQUEST command code	
const BYTE DOWNLOAD		 =0x50;//!<DOWNLOAD command code
const BYTE CRC32		 =0x51;//!<CRC32 REQUEST command code
const BYTE SET_TIME		 =0x62;//!<SET BB TIME command code
const BYTE SET_BAR_PARAMS					=0x39;//!<SET BARCODE PARAMETERS command code
const BYTE EXTRACT_BAR_DATA					=0x3A;//!<EXTRACT BARCODE DATA command code
const BYTE POWER_RECOVERY					=0x66;//!<POWER RECOVERY command code
const BYTE EMPTY_DISPENSER					=0x67;//!<EMPTY DISPENSER command code
const BYTE SET_OPTIONS						=0x68;//!<SET OPTIONS command code
const BYTE GET_OPTIONS						=0x69;//!<GET OPTIONS command code
/**@} */

/**	\defgroup Options Options
* Describes options supported by CCNET (as a bitmap)
* @{
*/
//Options (bitmap)
	const DWORD OPT_LED_INHIBIT=0x80000000;//!< Turn OFF LEDs of the bezel in the DISABLED state
	const DWORD OPT_KEEP_BILL=0x40000000;//!< Hold bill after ejection on the input roller
	const DWORD OPT_LOOK_TAPE=0x20000000; //!< Use improved algorithm for tape detection
	const DWORD OPT_TURN_SWITCH=0x10000000; //!< Turn switch after packing a bill
/**@} */
// States
/**	\defgroup States CCNET states and events
* 
* @{
*/
const char ST_POWER_UP				=0x10;//!< POWER UP state
const char ST_POWER_BILL_ESCROW		=0x11;//!< POWER UP WITH BILL IN ESCROW state
const char ST_POWER_BILL_STACKER	=0x12;//!< POWER UP WITH BILL IN STACKER state
const char ST_INITIALIZE			=0x13;//!< INITIALIZING state
const char ST_IDLING				=0x14;//!< IDLING state
const char ST_ACCEPTING				=0x15;//!< ACCEPTING state
const char ST_PACKING				=0x17;//!< STACKING/PACKING state
const char ST_RETURNING				=0x18;//!< RETURNING state
const char ST_DISABLED				=0x19;//!< UNIT DISABLED state
const char ST_HOLDING				=0x1A;//!< HOLDING state
const char	ST_BUSY					=0x1B;//!< Device is busy
const char ST_REJECTING				=0x1C;//!< REJECTING state. Followed by a rejection code
	//Rejection codes
/**	\defgroup RCodes Rejection codes
* 
* @{
*/
	const char RJ_INSERTION			=0x60; //!< Rejection because of insertion problem
	const char RJ_MAGNETIC			=0x61; //!< Rejection because of invalid magnetic pattern
	const char RJ_REMAINING			=0x62; //!< Rejection because of other bill remaining in the device
	const char RJ_MULTIPLYING		=0x63; //!< Rejection because of multiple check failures
	const char RJ_CONVEYING			=0x64; //!< Rejection because of conveying 
	const char RJ_IDENT				=0x65; //!< Rejection because of identification failure
	const char RJ_VRFY				=0x66; //!< Rejection because of verification failure
	const char RJ_OPT				=0x67; //!< Rejection because of optical pattern mismatch
	const char RJ_INHIBIT			=0x68; //!< Rejection because the denomination is inhibited
	const char RJ_CAP				=0x69; //!< Rejection because of capacity sensor pattern mismatch
	const char RJ_OPERATION			=0x6A; //!< Rejection because of operation error
	const char RJ_LNG				=0x6C; //!< Rejection because of invalid bill length
	const char RJ_UV				=0x6D; //!< Rejection because of invalid UV pattern
	const unsigned char RJ_BAR		=0x92; //!< Rejection because of unrecognized barcode
	const unsigned char RJ_BAR_LNG	=0x93; //!< Rejection because of invalid barcode length
	const unsigned char RJ_BAR_START=0x94; //!< Rejection because of invalid barcode start sequence
	const unsigned char RJ_BAR_STOP	=0x95; //!< Rejection because of invalid barcode stop sequence
/**@} */

const char ST_DISPENSING			=0x1D;//!< DISPENSING state
const char ST_UNLOADING				=0x1E;//!< UNLOADING state 
const char ST_SETTING_CS_TYPE		=0x21;//!< SETTING RECYCLING CASSETTE TYPE state
const char ST_DISPENSED				=0x25;//!< DISPENSED event
const char ST_UNLOADED				=0x26;//!< UNLOADED event
const char ST_BILL_NUMBER_ERR		=0x28;//!< INVALID BILL NUMBER event
const char ST_CS_TYPE_SET			=0x29;//!< RECYCLING CASSETTE TYPE SET event
const char ST_ST_FULL				=0x41;//!< DROP CASSETTE IS FULL state
const char ST_BOX					=0x42;//!< DROP CASSETTE REMOVED state 
const char ST_BV_JAMMED				=0x43;//!< JAM IN VALIDATOR state
const char ST_ST_JAMMED				=0x44;//!< JAM IN STACKER state
const char ST_CHEATED				=0x45;//!< CHEATED event
const char ST_PAUSED				=0x46;//!< PAUSED state
const char ST_FAILURE				=0x47;//!< FAILURE state

	//Failure codes
/**	\defgroup FCodes Failure codes
* 
* @{
*/
	const char FLR_STACKER			=0x50; //!< Stacking mechanism failure
	const char FLR_TR_SPEED			=0x51; //!< Invalid speed of transport mechanism
	const char FLR_TRANSPORT		=0x52; //!< Transport mechanism failure
	const char FLR_ALIGNING			=0x53; //!< Aligning mechanism failure
	const char FLR_INIT_CAS			=0x54; //!< Initial cassette status failure
	const char FLR_OPT				=0x65; //!< Optical channel failure
	const char FLR_MAG				=0x66; //!< Inductive channel failure
	const char FLR_CAP				=0x67; //!< Capacity sensor failure
/**@} */

// Credit events
const BYTE ST_PACKED				=0x81;	/**< A bill has been packed. 2nd byte - 0xXY:
															\n X-bill type
															\n Y-Packed into:
															\n 0-BOX, else - Cassette Y;

											*/
const BYTE ESCROW					=0x80; //!< A bill is held in the escrow position	
const BYTE RETURNED					=0x82; //!< A bill was returned
/**@} */
// Cassetes status
/**	\defgroup CSStatus Possible cassette status codes
* 
* @{
*/
const BYTE CS_OK			=0;	//!< Cassette is present and operational
const BYTE CS_FULL			=1; //!< Cassette is full
const BYTE CS_NU			=0xFE;//!< Cassette is not present
const BYTE CS_MALFUNCTION	=0xFF;//!< Cassette is malfunctioning
const BYTE CS_NA			=0xFD;//!< Cassette is not assigned to any denomination
const BYTE CS_ESCROW		=0xFC;//!< Cassette is assigned to multi-escrow 
/**@} */
/**	\defgroup BTs Predefined bill type values
* 
* @{
*/
const BYTE BT_ESCROW		=24; //!< Bill type associated with the escrow cassette
const BYTE BT_NO_TYPE		=0x1f; //!< Invalid bill type
const BYTE BT_BAR			=23; //!< Bill type associated with barcode coupon
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
const int RE_NONE			=0; //!< No error happened
const int RE_TIMEOUT		=-1;//!< Communication timeout
const int RE_SYNC			=-2;//!< Synchronization error (invalid synchro byte)
const int RE_DATA			=-3;//!< Data reception error
/**@} */
const int RE_CRC			=-4;//!< CRC error

/** \defgroup LErrs Logical error codes

  The codes related to the interface logic

   @{
*/
const int ER_NAK			=-5;//!< NAK received
const int ER_INVALID_CMD	=-6;//!< Invalid command response received 
const int ER_EXECUTION		=-7;//!< Execution error response received
const int ERR_INVALID_STATE	=-8;//!< Invalid state received
/**@} */
/**@} */
// Class Declaration
/**	\class CCCRSProtocol
	\brief	The CCCRSProtocol class providing low-level communication functions for the CCNET protocol

*/

namespace ServiceSaleMachine
{
	namespace CCNET
	{
		public class CCCRSProtocol
		{
		private:
			CCommand cmdIn; //!< A variable to store current device responses
			CCommand cmdOut;//!< A variable to store controller commands
			CCOMPort COMPort; //!< A COM port to work with
		private:
			WORD CalculateCRC(unsigned char*);
			CCommand Transmit(CCommand CMD, BYTE Addr = ADDR_BB);
			unsigned short crc16_ccitt(unsigned char, unsigned short);
			CCommand TransmitCMD(CCommand&, BYTE);
			int SendCommand(LPBYTE BufOut, LPBYTE BufIn);

		public:
			int iCmdDelay;	//!< Delay between two consequtive commands
			int iTimeout;	//!< Communication timeout value
			int iLastError; //!< A variable storing error code generated during last serial I/O operation

		public:
			CCCRSProtocol();
			~CCCRSProtocol();



			// Protocol commands	
			bool CmdEmptyDispenser();
			bool CmdPowerRecovery(BYTE&, LPBYTE, int&);
			bool CmdGetCRC32(DWORD&, BYTE);
			bool CmdGetOptions(DWORD&, BYTE);
			bool CmdSetOptions(DWORD, BYTE);
			bool CmdIdentExt(BYTE);
			bool CmdExtractBarData(LPSTR sBar, BYTE Addr);
			bool CmdSetBarParams(BYTE Format, BYTE Length, BYTE Addr);
			bool CmdBBTime(time_t&, bool bSet = false);
			bool CmdDispenseNew(LPBYTE, bool bTypes = true);
			bool CmdGetBillTable(_BillRecord*, BYTE Addr = ADDR_BB);
			bool CmdSetCasseteType(BYTE, BYTE, BYTE);
			bool CmdReset(BYTE Addr = ADDR_BB);
			bool CmdPoll(BYTE Addr = ADDR_BB);
			bool CmdStatus(BYTE Addr = ADDR_BB);
			bool CmdUnload(BYTE, BYTE);//Unload bills 
			bool CmdDispense(LPBYTE);//  
			bool CmdCsStatus();
			bool CmdReturn(BYTE Addr = ADDR_BB);
			bool CmdPack(BYTE Addr = ADDR_BB);
			bool CmdBillType(DWORD, DWORD, BYTE Addr = ADDR_BB);
			bool CmdSetSecurity(DWORD, BYTE Addr = ADDR_BB);
			bool CmdHold(BYTE Addr = ADDR_BB);
			bool CmdIdentification(BYTE Addr = ADDR_BB);
			//////////////////////////
			// COM port related functions
			BOOL InitCOM(int, int);
			DWORD PortState(int);
			CCOMPort* GetCOMPort();



			//Protocol structures

			/**	\struct _BillStatus
				\brief	The _BillStatus struct describing response to the STATUS REQUEST command

			*/
			struct _BillStatus
			{
				DWORD Enabled; //!< A bitmap describing which bill types are enabled
				DWORD Security; //!< A bitmap describing which bill types are processed in High Security mode
				DWORD Routing; //!< A bitmap describing which denominations are routed to a recycling cassette. Is a valid value only for BB units
			}BillStatus;//!< Variable containing the most recent response to the STATUS REQUEST

		/**	\struct _Identification
			\brief	The _Identification struct contains identification of the device

		*/
			struct _Identification
			{
				// Identification command fields
				char PartNumber[16];//!< Firmware part number 
				char SN[13];//!< Device's serial number
				__int64 DS1;//!< Device's asset number
				// Extended identification command fiels
				char BVBootVersion[7];//!< Boot version of the validating head (is reported in response to EXTENDED IDENTIFICATION command)
				char BVVersion[21];//!< Firmware version of the validating head (is reported in response to EXTENDED IDENTIFICATION command)

				char BCCPUBoot[7];//!< Boot version of the central controller (is reported in response to EXTENDED IDENTIFICATION command)
				char BCCPUVersion[7];//!<Firmware version of the central controller (is reported in response to EXTENDED IDENTIFICATION command)

				char BCDispenserBoot[7];//!< Boot version of the dispenser (is reported in response to EXTENDED IDENTIFICATION command)
				char BCDispenserVersion[7];//!< Firmware version of the dispenser (is reported in response to EXTENDED IDENTIFICATION command)

				char BCCS1Boot[7];//!< Boot version of the cassette 1 (is reported in response to EXTENDED IDENTIFICATION command)
				char BCCS2Boot[7];//!< Boot version of the cassette 2 (is reported in response to EXTENDED IDENTIFICATION command)
				char BCCS3Boot[7];//!< Boot version of the cassette 3 (is reported in response to EXTENDED IDENTIFICATION command)
				char BCCSVersion[7];//!< Firmware version of the cassettes (is reported in response to EXTENDED IDENTIFICATION command)
			}Ident;//!< A variable containing current device identification

		/**	\struct _PollResults
			\brief	The _PollResults struct containing 2 first bytes of the response to the POLL command

		*/
			struct _PollResults
			{
				BYTE Z1,//!< State
					Z2;//!< State extension or substate

			} PollResults; //!< A variable keeping last POLL result

			_Cassete	Cassetes[16], //!< List of the cassettes 
				EscrCassete;  //!< Escrow cassette


		};
	}
}
#endif // !defined(AFX_CCRSPROTOCOL_H__5BF7AFD2_D008_11D4_A0E8_00002165B0A2__INCLUDED_)
