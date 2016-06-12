/**	 \mainpage CCNET Interface library 
 *
 *	The library is property of CashCode Co. Inc. and is copyright protected 
 *
 * \section intro_sec Introduction
 *
 * The CCNET Interface library provides low-level interface to CCNET communications.
 * The library contains set of C++ classes which can be easyly compiled into any C++ project built for WIN32 platform.
 *
*/
/** @file CCRSProtocol.cpp: implementation of the CCCRSProtocol class.
*
* The file contains implementation of the CCCRSProtocol class. It provides low-level interface
* to the CCNET communictions.	       
*
* \n Product: All CCNET communications
* \n Country: All
* \n Protocol: CCNET v2.4
* 
*/
#include "stdafx.h"
#include <msclr\marshal.h>
#include "CCRSProtocol.h"
#include <math.h>


#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

using namespace ServiceSaleMachine;
using namespace ServiceSaleMachine::CCNET;

using namespace System::Collections::Generic;
using namespace System::Globalization;
using namespace System::Runtime::InteropServices;
using namespace msclr::interop;

static int iBytesToRecieve;//!< A variable containing number of bytes to receive from the device at once



static int iRecievingError=0;//!< A variable containing a communication error code produced by last serial communication

char * sCOMparam=S_COM9600; //!<  A string holding format specifier for current communication (dfault 9600, no parity, 1 stop bit)

/**	\brief	The pow10 function calculates 10^Power

	\param	Power	a parameter of type int containing required power

	\return	double - 10^Power

	
*/
inline double pow10(int Power)
{
	double dRes=1;
	for(Power; Power>0; Power--) dRes*=10;
	return dRes;
}


//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

/**	\brief Default object constructor
*/
CCCRSProtocol::CCCRSProtocol()
{
	iCmdDelay=20;
	strcpy(Ident.BCCPUBoot,"N/A");
	strcpy(Ident.BCCPUVersion,"N/A");
	strcpy(Ident.BCCS1Boot,"N/A");
	strcpy(Ident.BCCS2Boot,"N/A");
	strcpy(Ident.BCCS3Boot,"N/A");
	strcpy(Ident.BCCSVersion,"N/A");
	strcpy(Ident.BCDispenserBoot,"N/A");
	strcpy(Ident.BCDispenserVersion,"N/A");
	strcpy(Ident.BVBootVersion,"N/A");
	strcpy(Ident.BVVersion,"N/A");
	strcpy(Ident.PartNumber,"N/A");
}

/**	\brief Default object destructor 
*/
CCCRSProtocol::~CCCRSProtocol()
{

}
//////////////////////////////////////////////////////////////////////
// COM Port utilities
//////////////////////////////////////////////////////////////////////
/**	\brief	The CCCRSProtocol::GetCOMPort function returns a pointer to the CCOMPort object 



	\return	CCOMPort* - pointer to the member CCOMPort object providing COM port I/O functionality

	
*/
CCOMPort* CCCRSProtocol::GetCOMPort()
{
	return &COMPort;
}

/**	\brief	The CCCRSProtocol::PortState function returns availability code of the selected COM port

	\param	iPort	a parameter of type int specifiying the port number

	\return	DWORD - an availability code returned by CCOMPort::IsEnable(int) function \see CCOMPort::IsEnable(int)

	
*/
DWORD CCCRSProtocol::PortState(int iPort)
{
	return COMPort.IsEnable(iPort);
}

/**	\brief	The CCCRSProtocol::InitCOM function performs initialization of the specified COM port

  The function calls the CCOMPort::InitCOM(int, LPSTR,int) member function of the COMPort object to open and
  initialize the specified COM port. The current communication format specifier string sCOMParam is used as a 
  format string in the function call.

  \see CCOMPort::InitCOM(int, LPSTR,int)


	\param	COMi	a parameter of type int specifiying the 0-based COM port number to initialize
	\param	iTo	a parameter of type int specifiying communication timeout value

	\return	BOOL - initialization result

	
*/
BOOL CCCRSProtocol::InitCOM(int COMi,int iTo)
{
	return COMPort.InitCOM(COMi,sCOMparam,iTimeout=iTo);
}


/**	\brief	The CCCRSProtocol::crc16_ccitt function performs simple CCITT calculations on single byte

  The function processes one byte and is used in iteration to calculate CRC16 value

	\param	data	a parameter of type unsigned char containing data byte to process
	\param	crc	a parameter of type unsigned short containing previous CRC16 value

	\return	unsigned short - newly calculated CRC16 value

	
*/
unsigned short CCCRSProtocol::crc16_ccitt(unsigned char data, unsigned short crc)
{	register unsigned short a=0x8408, d = crc, i;
	d^= data;
	for (i=0; i<8; i++)
	{if (d & 0x0001)
		{d>>= 1;
		d^=a;}
		else d>>=1;
	}
	return d;
}


/**	\brief	The CCCRSProtocol::CalculateCRC function calculates CRC16 of the buffer

  the function calculates CRC16 of the data in the supplied buffer by calling 
  the CCCRSProtocol::crc16_ccitt(unsigned char data, unsigned short crc) member function
  iterationally. The function is taking the data length from the buffer assuming that buffer 
  contains valid CCNET frame

	\param	pBuffer	a parameter of type unsigned char * - pointer to the frame data

	\return	WORD - CRC16 value

	
*/
WORD CCCRSProtocol::CalculateCRC(unsigned char *pBuffer)
{
	WORD wCRC=0,Len=(pBuffer[2])?pBuffer[2]:
									(((WORD)(pBuffer)[4])<<8)+(pBuffer)[5];
	for (int i=0;i<Len-2;i++)
		wCRC=crc16_ccitt(pBuffer[i],wCRC);
	return wCRC;
}


/**	\brief	The CCCRSProtocol::SendCommand function performs transmission of the supplied buffer and the response reception

  The function transmits the CCNET frame located in buffer BufOut and reads the device response into the BufIn. 
  If ACK or NAK message is transmitted the function returns immediately after transmission is completed.

	\param	BufOut	a parameter of type LPBYTE - a pointer to the output buffer containing CCNET frame.
	\param	BufIn	a parameter of type LPBYTE - a pointer to the input buffer to copy unit's response to.

	\return	int - a communication error code. Can be any value from \link CErrs Communication error codes \endlink group 

	
*/
int CCCRSProtocol::SendCommand(LPBYTE BufOut, LPBYTE BufIn)
{
	iRecievingError=RE_TIMEOUT;
	for(int iErrCount=0;iErrCount<1;iErrCount++)
	{
		iBytesToRecieve=6;
 		PurgeComm(COMPort.GetHandle(),PURGE_RXABORT|PURGE_TXABORT|PURGE_TXCLEAR|PURGE_RXCLEAR);
		if(!BufOut[2])
			COMPort.Send(BufOut,((WORD)BufOut[4]<<8)+BufOut[5]);
		else 
			COMPort.Send(BufOut,BufOut[2]);
		if((BufOut[3]==ACK)||(BufOut[3]==NAK))
			return iRecievingError=RE_NONE;
		
		if(COMPort.Recieve(BufIn,iBytesToRecieve))
		{
			if(BufIn[0]!=SYNC)
				iRecievingError=RE_SYNC;
			else
			{
				int iLen=((BufIn[2])?BufIn[2]:(BufIn[5]+((WORD)BufIn[4]<<8)))-iBytesToRecieve;
				if(iLen>0)
				{
				if(COMPort.Recieve((LPBYTE)BufIn+iBytesToRecieve,iLen))	
				{
					iRecievingError=RE_NONE;		
					break;
				}
				else 
				{
					iRecievingError=RE_DATA;
					PurgeComm(COMPort.GetHandle(),PURGE_RXABORT|PURGE_RXCLEAR);					
				}
				}
				else
				{
					iRecievingError=RE_NONE;
					break;
				}
			}
		}
	}
	return iRecievingError;
}


/**	\brief	The CCCRSProtocol::TransmitCMD function carries simple protocol exchange

  The function is a simple wrapper for the CCCRSProtocol::SendCommand(LPBYTE BufOut, LPBYTE BufIn) member function
  and performs the following actions:
	-# Complementing the output frame with the device address and CRC16
	-# Sending the frame and receiving a response using CCCRSProtocol::SendCommand(LPBYTE BufOut, LPBYTE BufIn) member function
	-# Checking received frame integrity (by CRC16 value)
	-# Returning the response wrapped in the CCommand object

	\param	Cmd	a parameter of type CCommand & containing output frame (should contain all required information except of device address and CRC)
	\param	Addr	a parameter of type BYTE containing the device address used for communication. 
					Refer to \link Addr Device address list \endlink for the valid values

	\return	CCommand - an object containing response data and communication error code

	
*/
CCommand CCCRSProtocol::TransmitCMD(CCommand &Cmd,BYTE Addr)
{	
	BYTE tmpBuffer[256];
	int i=(Cmd.GetData()[2])?(Cmd.GetData())[2]:
									(((WORD)(Cmd.GetData())[4])<<8)+(Cmd.GetData())[5];
	Cmd.SetByte(Addr,1);
	WORD wCRC=CalculateCRC(Cmd.GetData());
	Cmd.SetByte((BYTE)wCRC,i-2);
	Cmd.SetByte(wCRC>>8,i-1);
	cmdOut=Cmd;

	int iErrCode=SendCommand(Cmd.GetData(),tmpBuffer);
	if((!iErrCode)&&(Cmd.GetData()[3])&&(0xFF!=Cmd.GetData()[3]))
	{
		wCRC=tmpBuffer[(tmpBuffer[2])?tmpBuffer[2]:(((WORD)(tmpBuffer[4]))<<8)+tmpBuffer[5]-2]+
			(tmpBuffer[(tmpBuffer[2])?tmpBuffer[2]:(((WORD)(tmpBuffer[4]))<<8)+tmpBuffer[5]-1]<<8);
		if(CalculateCRC(tmpBuffer)!=wCRC)
			iErrCode=RE_CRC;
		cmdIn=CCommand(tmpBuffer,iErrCode,(tmpBuffer[2])?(tmpBuffer)[2]:
									(((WORD)(tmpBuffer)[4])<<8)+(tmpBuffer)[5]);	
		return cmdIn;
	}
	cmdIn=CCommand(tmpBuffer,iErrCode);	
	return cmdIn;
}

/**	\brief	The CCCRSProtocol::Transmit function carries complete protocol exchange

  The function is a simple wrapper for the CCCRSProtocol::TransmitCMD(CCommand CMD, BYTE Addr) member function
  and performs the following actions:
	-# Sending the frame and receiving a response using CCCRSProtocol::TransmitCMD(CCommand CMD, BYTE Addr) member function
	-# Checking the device response and determining whether ACK or NAK should be sent 
	-# Sending ACK or NAK message to the device or retransmitting the command up to 3 times untill communication is successfully completed
	-# Returning the response wrapped in the CCommand object

	\param	CMD	a parameter of type CCommand & containing output frame (should contain all required information except of device address and CRC)
	\param	Addr	a parameter of type BYTE containing the device address used for communication. 
					Refer to \link Addr Device address list \endlink for the valid values

	\return	CCommand - an object containing response data and communication error code

	
*/
CCommand CCCRSProtocol::Transmit(CCommand CMD, BYTE Addr)
{
	CCommand cmdRes,cmdACK;
	for(int i=0;i<3;i++)
	{
	cmdRes=TransmitCMD(CMD,Addr);
	cmdACK.SetByte(SYNC,0);
	cmdACK.SetByte(6,2);
	cmdACK.SetByte(ACK,3);
	
	if(cmdRes.GetCode()==RE_NONE)
	{

		if((ACK==cmdRes.GetData()[3])&&(cmdRes.GetData()[2]==6))
		{
			return cmdRes;
		}
		if((NAK==cmdRes.GetData()[3])&&(cmdRes.GetData()[2]==6))
		{

		if(iCmdDelay)Sleep(iCmdDelay);//5	
		}
		else
		{
		cmdACK.SetByte(ACK,3);
		TransmitCMD(cmdACK,Addr);
		if(iCmdDelay)Sleep(iCmdDelay);//5
		break;
		}
	}
	else
	{
		if(cmdRes.GetCode()!=RE_TIMEOUT)
		{
			cmdACK.SetByte(NAK,3);
			TransmitCMD(cmdACK,Addr);
			if(iCmdDelay)Sleep(iCmdDelay);//5
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

/**	\brief	The CCCRSProtocol::CmdReset function sends a RESET command to the device

	\param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

	\return	bool - true if command was acknowledged

	
*/
bool CCCRSProtocol::CmdReset(BYTE Addr)
{
	BYTE Data[256]={SYNC,0,6,RESET,0};
	CCommand cmd(Data,0,6);
	CCommand Response=Transmit(cmd,Addr);
	BYTE ack;
	if(!(iLastError=Response.GetCode()))
	{
		
		if((ack=Response.GetData()[3])!=ACK)
		{
			iLastError=(ack!=ST_INV_CMD)?ER_NAK:ER_INVALID_CMD;
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
bool CCCRSProtocol::CmdPoll(BYTE Addr)
{
	BYTE Data[256]={SYNC,0,6,POLL,0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd, Addr);
	if(!(iLastError=Response.GetCode()))
	{
		PollResults.Z1=Response.GetData()[3];
		PollResults.Z2=Response.GetData()[4];
		return true;
	}
	else 
	{
		PollResults.Z1=0;
		PollResults.Z2=0;
		return false;
	}
}

/**	\brief	The CCCRSProtocol::CmdStatus function sends STATUS REQUEST to the device

  The response status data is stored in the CCCRSProtocol::BillStatus member structure.

	\param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

	\return	bool - true if exchange was successfully completed

	
*/
bool CCCRSProtocol::CmdStatus(BYTE Addr)
{
	BYTE Data[256]={SYNC,0,6,GET_STATUS,0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd,Addr);
	if(!(iLastError=Response.GetCode()))
	{
		if((Response.GetData()[3]==ST_INV_CMD)&&(Response.GetData()[2]==6))
		{
			iLastError=ER_INVALID_CMD;
			BillStatus.Enabled=0;
			BillStatus.Security=0;
			BillStatus.Routing=0;
			return false;
		}
		BillStatus.Enabled=Response.GetData()[5]+((DWORD)Response.GetData()[4]<<8)+((DWORD)Response.GetData()[3]<<16);
		BillStatus.Security=Response.GetData()[8]+((DWORD)Response.GetData()[7]<<8)+((DWORD)Response.GetData()[6]<<16);
		BillStatus.Routing=Response.GetData()[11]+((DWORD)Response.GetData()[10]<<8)+((DWORD)Response.GetData()[9]<<16);
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
bool CCCRSProtocol::CmdIdentification(BYTE Addr)
{
	BYTE Data[256]={SYNC,0,6,IDENTIFICATION,0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd,Addr);
	if(!(iLastError=Response.GetCode()))
	{
		if((Response.GetData()[3]==ST_INV_CMD)&&(Response.GetData()[2]==6))
		{
			iLastError=ER_INVALID_CMD;
			return false;
		}
		strcpy(Ident.BCCPUBoot,"N/A");
		strcpy(Ident.BCCPUVersion,"N/A");
		strcpy(Ident.BCCS1Boot,"N/A");
		strcpy(Ident.BCCS2Boot,"N/A");
		strcpy(Ident.BCCS3Boot,"N/A");
		strcpy(Ident.BCCSVersion,"N/A");
		strcpy(Ident.BCDispenserBoot,"N/A");
		strcpy(Ident.BCDispenserVersion,"N/A");
		strcpy(Ident.BVBootVersion,"N/A");
		strcpy(Ident.BVVersion,"N/A");
		strcpy(Ident.PartNumber,"N/A");
		char sTemp[64];
		int iPos=3,iLen=15;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.PartNumber,sTemp);
		iLen=12;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.SN,sTemp);
		char *strTemp=(char*)Response.GetData()+iPos;
		

		Ident.DS1=0;iPos+=8;
		for(int i=0;i<7;i++)
		{
			Ident.DS1<<=8;
			Ident.DS1+=strTemp[i];
		}
		if(Response.GetData()[2]<109) return true;
		
		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BVBootVersion,sTemp);
		
		iLen=20;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BVVersion,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCCPUBoot,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCCPUVersion,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCDispenserBoot,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCDispenserVersion,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCCS1Boot,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCCS2Boot,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCCS3Boot,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCCSVersion,sTemp);
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
bool CCCRSProtocol::CmdHold(BYTE Addr)
{
	BYTE Data[256]={SYNC,0,6,HOLD,0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd, Addr);
	BYTE ack;
	if(!(iLastError=Response.GetCode()))
	{
		if((ack=Response.GetData()[3])!=ACK)
		{
			iLastError=(ack!=ST_INV_CMD)?ER_NAK:ER_INVALID_CMD;
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
bool CCCRSProtocol::CmdSetSecurity(DWORD wS,BYTE Addr)
{
	BYTE Data[256]={SYNC,0,9,SET_SECURITY,(BYTE)(wS>>16),(BYTE)(wS>>8),(BYTE)wS,0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd,Addr);
	BYTE ack;
	if(!(iLastError=Response.GetCode()))
	{
		if((ack=Response.GetData()[3])!=ACK)
		{
			iLastError=(ack!=ST_INV_CMD)?ER_NAK:ER_INVALID_CMD;
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
bool CCCRSProtocol::CmdBillType(DWORD enBill, DWORD escBill, BYTE Addr)
{
	BYTE Data[256]={SYNC,0,12,BILL_TYPE,
					(BYTE)(enBill>>16),(BYTE)(enBill>>8),(BYTE)enBill,
					(BYTE)(escBill>>16),(BYTE)(escBill>>8),(BYTE)escBill,
					0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd, Addr);
	BYTE ack;
	if(!(iLastError=Response.GetCode()))
	{
		if((ack=Response.GetData()[3])!=ACK)
		{
			iLastError=(ack!=ST_INV_CMD)?ER_NAK:ER_INVALID_CMD;
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
bool CCCRSProtocol::CmdPack(BYTE Addr)
{
	BYTE Data[256]={SYNC,0,6,PACK,0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd,Addr);
	BYTE ack;
	if(!(iLastError=Response.GetCode()))
	{
		if((ack=Response.GetData()[3])!=ACK)
		{
			iLastError=(ack!=ST_INV_CMD)?ER_NAK:ER_INVALID_CMD;
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
bool CCCRSProtocol::CmdReturn(BYTE Addr)
{
	BYTE Data[256]={SYNC,0,6,RETURN,0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd,Addr);
	BYTE ack;
	if(!(iLastError=Response.GetCode()))
	{
		if((ack=Response.GetData()[3])!=ACK)
		{
			iLastError=(ack!=ST_INV_CMD)?ER_NAK:ER_INVALID_CMD;
			return false;
		}
		else return true;

	}
	else 
	{
		return false;
	}
}



/**	\brief	The CCCRSProtocol::CmdCsStatus function sends GET CASSETTE STATUS request

  The function is applicable for Bill-To-Bill units only. It sends RECYCLING CASSETTE STATUS request 
  and translates the response data into the CCRSProtocol::Cassettes array so it contains the image of 
  the unit's cassettes.


	\return	bool - true if the request succeeded

	
*/
bool CCCRSProtocol::CmdCsStatus()
{
	BYTE Data[256]={SYNC,0,6,C_STATUS,0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd);
	if(!(iLastError=Response.GetCode()))
	{
		if((Response.GetData()[2]==6)&&(Response.GetData()[3]==ST_INV_CMD)) 
		{
			iLastError=ER_INVALID_CMD;
			for(int i=0;i<32;i+=2)
			{
			Cassetes[i/2].BillNumber=0;
			Cassetes[i/2].bEscrow=0;
			Cassetes[i/2].BillType=0;
			Cassetes[i/2].Status=CS_NU;
			} 
			return false;
		}
		for(int i=0;i<(Response.GetData()[2])-5;i+=2)
		{
			Cassetes[i/2].BillNumber=Response.GetData()[i+4];
		
			Cassetes[i/2].BillType=((Response.GetData()[i+3]) & 0x1F);
			Cassetes[i/2].bEscrow=((Cassetes[i/2].BillType==BT_ESCROW));
			if(!((Response.GetData()[i+3]) & 0x80))
				Cassetes[i/2].Status=CS_NU;
			else
				if((Cassetes[i/2].BillType==(BT_NO_TYPE& 0x1F)))
					Cassetes[i/2].Status=CS_NA;
							else Cassetes[i/2].Status=CS_OK;
		}
		for(int i;i<32;i+=2)
		{
			Cassetes[i/2].BillNumber=0;
			Cassetes[i/2].bEscrow=0;
			Cassetes[i/2].BillType=0;
			Cassetes[i/2].Status=CS_NU;
		} 
		return true;
	}
	else 
	{
		return false;
	}
}


/**	\brief	The CCCRSProtocol::CmdDispense function sends the old-style DISPENSE command to Bill-To-Bill unit

	\param	pBills	a parameter of type LPBYTE pointing to the array containing numbers of bills of each bill type to dispense.
			The position in the array should correspond to the bill type, the value 
			at that position - number of bils of that type to dispense.


	\return	bool - true if the command was acknowldged

	
*/
bool CCCRSProtocol::CmdDispense(LPBYTE pBills)
{
	BYTE Data[256]={SYNC,0,0,DISPENSE,0,0};
	DWORD dwUnload=0;
	BYTE ByteNum=23;//8;
	BYTE ArrPoint=7;
	for(int i=0;i<24;i++)
	{
		if (pBills[i]) { Data[ArrPoint++]=pBills[i]; dwUnload|=(1<<i);}
	}
	Data[2]=ArrPoint+2;
	Data[6]=(BYTE)dwUnload;
	Data[4]=(BYTE)(dwUnload>>16);
	Data[5]=(BYTE)(dwUnload>>8);
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd);
	BYTE ack;
	if(!(iLastError=Response.GetCode()))
	{
		if((ack=Response.GetData()[3])!=ACK)
		{
			iLastError=(ack!=ST_INV_CMD)?ER_NAK:ER_INVALID_CMD;
			return false;
		}
		else return true;
	}
	else 
	{
		return false;
	}
}

/**	\brief	The CCCRSProtocol::CmdUnload function sends UNLOAD command to Bill-To-Bill unit

	\param	Cass	a parameter of type BYTE specifiying the source cassette for operation
	\param	Num	a parameter of type BYTE specifiying number of bills to unload

	\return	bool - true if the command was acknowldged

	
*/
bool CCCRSProtocol::CmdUnload(BYTE Cass, BYTE Num) 
{
	BYTE Data[256]={SYNC,0,8,UNLOAD,0,0};
	WORD bUnload=0;
	Data[4]=Cass+1;
	Data[5]=Num;
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd);
	BYTE ack;
	if(!(iLastError=Response.GetCode()))
	{
		if((ack=Response.GetData()[3])!=ACK)
		{
			iLastError=(ack!=ST_INV_CMD)?ER_NAK:ER_INVALID_CMD;
			return false;
		}
		else return true;

	}
	else 
	{
		return false;
	}
}



/**	\brief	The CCCRSProtocol::CmdSetCasseteType function sends SET RECYCLING CASSETTE TYPE command to Bill-To-Bill unit

	\param	Cass	a parameter of type BYTE specifiying cassette to setup.
	\param	Status	a parameter of type BYTE specifiying a new cassette status. refer to \link CSStatus cassette status lest \endlink 
					for possible values
	\param	Type	a parameter of type BYTE specifiying bill type to assign to the cassette

	\return	bool - true if the command was acknowldged

	
*/
bool CCCRSProtocol::CmdSetCasseteType(BYTE Cass, BYTE Status, BYTE Type)
{
	BYTE Data[256]={SYNC,0,8,SET_CASSETES,Cass+1,0,0};
	CCommand cmd(Data,0);
	BYTE Y1=Type;
	BYTE g1=0x1F;
	if(Status==CS_NA) 
	{
		Y1=g1;
	}
	else
	{
		Y1=Type;
	}
	switch(Status)
	{
	case CS_ESCROW:
		Y1|=0x20;
		break;
	case CS_OK:
	case CS_MALFUNCTION:
	case CS_FULL:
		Y1|=0x80;
		break;
	case CS_NA:	
		Y1=0x40|g1;
		break;
	case CS_NU:
	default:
		break;
	}
	cmd.SetByte(Y1,5);
	CCommand Response=Transmit(cmd);
	BYTE ack;
	if(!(iLastError=Response.GetCode()))
	{
		if((ack=Response.GetData()[3])!=ACK)
		{
			iLastError=(ack!=ST_INV_CMD)?ER_NAK:ER_INVALID_CMD;
			return false;
		}
		else return true;
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
bool CCCRSProtocol::CmdGetBillTable(_BillRecord * BillTable,BYTE Addr)
{
	BYTE Data[256]={SYNC,0,6,GET_BILL_TABLE,0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd,Addr);
	if(!(iLastError=Response.GetCode()))
	{
		if((Response.GetData()[3]==ST_INV_CMD)&&(Response.GetData()[2]==6))
		{
			iLastError=ER_INVALID_CMD;
			for(int i=0;i<24;i++)
			{
			BillTable[i].Denomination=0;
			strcpy(BillTable[i].sCountryCode,"");
			}
			return false;
		}
		for(int i=0;i<(Response.GetData()[2])-5;i+=5)
		{
			BillTable[i/5].Denomination=Response.GetData()[i+3];
			char sTmp[5];
			strncpy(sTmp,(const char *)(Response.GetData()+i+4),3);
			sTmp[3]='\0';
			strcpy(BillTable[i/5].sCountryCode,sTmp);
			if(((Response.GetData())[i+7])&0x80)
			{
				for(int j=0; j<((Response.GetData()[i+7])&0x7F);j++)
					BillTable[i/5].Denomination/=10;
			}
			else
			{
				for(int j=0; j<((Response.GetData()[i+7])&0x7F);j++)
					BillTable[i/5].Denomination*=10;
			};
		}
		for(int i;i<24*5;i+=5)
		{
			BillTable[i/5].Denomination=0;
			strcpy(BillTable[i/5].sCountryCode,"");
		} 
		return true;
	
	}
	else 
	{
		return false;
	}
}



/**	\brief	The CCCRSProtocol::CmdDispenseNew function sends a new-style DISPENSE command

	\param	pBills	a parameter of type LPBYTE pointing to the array containing actual command parameters or 
			numbers of bills of each bill type to dispense depending on value of bTypes parameter.
			
	\param	bTypes	a parameter of type bool defining format of data in the pBills array:
					- false - the array contains numbers of bills of each bill type to dispense.
							The position in the array should correspond to the bill type, the value 
							at that position - number of bils of that type to dispense.
					- true - the array contains pairs (bill type),(bill number)

	\return	bool - true if the command was acknowldged
	

	
*/
bool CCCRSProtocol::CmdDispenseNew(LPBYTE pBills, bool bTypes)
{
	BYTE Data[256]={SYNC,0,0,DISPENSE,0,0,0,0,0,0,0,0,0,0,0,0};
	DWORD dwUnload=0;
	BYTE ByteNum=23;
	BYTE ArrPoint=4;
	if(bTypes)
	{
	for(int i=0;i<25;i++)
	{
		if (pBills[i]) { Data[ArrPoint++]=i;Data[ArrPoint++]=pBills[i]; }
	}
	}
	else
	{
	for(int i=0;i<6;i+=2)
	{
		if((pBills[i+1]!=0xff)){Data[ArrPoint++]=pBills[i];Data[ArrPoint++]=pBills[i+1];} 
	}
	}
	Data[2]=ArrPoint+2;
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd);
	BYTE ack;
	if(!(iLastError=Response.GetCode()))
	{
		if((ack=Response.GetData()[3])!=ACK)
		{
			iLastError=(ack!=ST_INV_CMD)?ER_NAK:ER_INVALID_CMD;
			return false;
		}
		else return true;

	}
	else 
	{
		return false;
	}

}


/**	\brief	The CCCRSProtocol::CmdBBTime function sends BB TIME command to Bill-To-Bill devices only

	\param	tTime	a parameter of type time_t & a structure containing time to set and receiving the obtained time.
	\param	bSet	a parameter of type bool defining whether to set or request time from the unit.

	\return	bool

	
*/
bool CCCRSProtocol::CmdBBTime(time_t &tTime, bool bSet)
{
	BYTE Data[256]={SYNC,0,(bSet)?13:6,SET_TIME,0,0};
	CCommand cmd(Data,0);
	if(bSet)
	{
		struct tm* ptmpTime=gmtime(&tTime);

		int		Seconds=ptmpTime->tm_sec,
				Mins=ptmpTime->tm_min,
				Hours=ptmpTime->tm_hour,
				Month=ptmpTime->tm_mon,
				Date=ptmpTime->tm_mday,
				Year=(ptmpTime->tm_year%100),
				Weekday=ptmpTime->tm_wday;
		cmd.SetByte(((Seconds/10)<<4)+(Seconds%10),4);
		cmd.SetByte(((Mins/10)<<4)+(Mins%10),5);
		cmd.SetByte(((Hours/10)<<4)+(Hours%10),6);
		cmd.SetByte(Weekday,7);
		cmd.SetByte(((Date/10)<<4)+(Date%10),8);
		cmd.SetByte(((Month/10)<<4)+(Month%10),9);
		cmd.SetByte(((Year/10)<<4)+(Year%10),10);
	}
	CCommand Response=Transmit(cmd,ADDR_BB);
	BYTE ack;
	if(!(iLastError=Response.GetCode()))
	{
		if((ack=Response.GetData()[3])==ST_INV_CMD)
		{
			iLastError=(ack!=ST_INV_CMD)?ER_NAK:ER_INVALID_CMD;
			return false;
		}
		else 
		{
		int		Seconds=Response.GetData()[3],
				Mins=Response.GetData()[4],
				Hours=Response.GetData()[5],
				Month=Response.GetData()[8],
				Date=Response.GetData()[7],
				Year=Response.GetData()[9],
				Weekday=Response.GetData()[6];
		if(!Month)
		{			
			return false;
		}
		struct tm tmpTime;
		tmpTime.tm_year=(Year>>4)*10+(Year&0x0f)+2000;
		tmpTime.tm_mon=(Month>>4)*10+(Month&0x0f);
		tmpTime.tm_mday=(Date>>4)*10+(Date&0x0f);
		tmpTime.tm_hour=(Hours>>4)*10+(Hours&0x0f);
		tmpTime.tm_min=(Mins>>4)*10+(Mins&0x0f);
		tmpTime.tm_sec=(Seconds>>4)*10+(Seconds&0x0f);
		tTime=mktime(&tmpTime);
		return true;
		}
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
bool CCCRSProtocol::CmdSetBarParams(BYTE Format, BYTE Length, BYTE Addr=ADDR_BB)
{
	BYTE Data[256]={SYNC,0,8,SET_BAR_PARAMS,
					Format, Length,
					0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd, Addr);
	BYTE ack;
	if(!(iLastError=Response.GetCode()))
	{
		if((ack=Response.GetData()[3])!=ACK)
		{
			iLastError=(ack!=ST_INV_CMD)?ER_NAK:ER_INVALID_CMD;
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
bool CCCRSProtocol::CmdExtractBarData(LPSTR sBar, BYTE Addr=ADDR_BB)
{
	BYTE Data[256]={SYNC,0,6,EXTRACT_BAR_DATA,0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd,Addr);
	if(!(iLastError=Response.GetCode()))
	{
		if((Response.GetData()[3]==ST_INV_CMD)&&(Response.GetData()[2]==6))
		{
			iLastError=ER_INVALID_CMD;
			return false;
		}
		strcpy(sBar,"");
		for (int i=3; i<Response.GetData()[2]-2;i++)
			sBar[i-3]= Response.GetData()[i];
		return true;
	}
	else 
	{
		return false;
	}
}

/**	\brief	The CCCRSProtocol::CmdIdentEx function sends EXTENDED IDENTIFICATION request

  The function sends EXTENDED IDENTIFICATION request and stores device identification in the member CCCRSProtocol::Ident structure.
  The request is supported by new Bill-To-Bill units only.

	\param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

	\return	bool - true if the exchange was successfully completed and data received

	
*/
bool CCCRSProtocol::CmdIdentExt(BYTE Addr)
{
	BYTE Data[256]={SYNC,0,6,IDENT_EXT,0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd,Addr);
	if(!(iLastError=Response.GetCode()))
	{
		if((Response.GetData()[3]==ST_INV_CMD)&&(Response.GetData()[2]==6))
		{
			iLastError=ER_INVALID_CMD;
			return false;
		}
		strcpy(Ident.BCCPUBoot,"N/A");
		strcpy(Ident.BCCPUVersion,"N/A");
		strcpy(Ident.BCCS1Boot,"N/A");
		strcpy(Ident.BCCS2Boot,"N/A");
		strcpy(Ident.BCCS3Boot,"N/A");
		strcpy(Ident.BCCSVersion,"N/A");
		strcpy(Ident.BCDispenserBoot,"N/A");
		strcpy(Ident.BCDispenserVersion,"N/A");
		strcpy(Ident.BVBootVersion,"N/A");
		strcpy(Ident.BVVersion,"N/A");
		strcpy(Ident.PartNumber,"N/A");
		char sTemp[64];
		int iPos=3,iLen=15;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.PartNumber,sTemp);
		iLen=12;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.SN,sTemp);
		char *strTemp=(char*)Response.GetData()+iPos;
		

		Ident.DS1=0;iPos+=8;
		for(int i=0;i<7;i++)
		{
			Ident.DS1<<=8;
			Ident.DS1+=strTemp[i];
		}
	
		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BVBootVersion,sTemp);
		
		iLen=20;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BVVersion,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCCPUBoot,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCCPUVersion,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCDispenserBoot,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCDispenserVersion,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCCS1Boot,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCCS2Boot,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCCS3Boot,sTemp);

		iLen=6;
		strncpy(sTemp,(char*)Response.GetData()+iPos,iLen);
		sTemp[iLen]=0;iPos+=iLen;
		strcpy(Ident.BCCSVersion,sTemp);
		return true;
	}
	else 
	{
		return false;
	}

}



/**	\brief	The CCCRSProtocol::CmdSetOptions function

	\param	dwOpt	a parameter of type DWORD containing bitmap with options to enable. refer to \link Options Options list \endlink
	\param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

	\return	bool - true if the command was acknowledged

	
*/
bool CCCRSProtocol::CmdSetOptions(DWORD dwOpt, BYTE Addr)
{
	BYTE Data[256]={SYNC,0,10,SET_OPTIONS,
					(BYTE)(dwOpt>>24),(BYTE)(dwOpt>>16),(BYTE)(dwOpt>>8),(BYTE)dwOpt,
					0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd, Addr);
	BYTE ack;
	if(!(iLastError=Response.GetCode()))
	{
		if((ack=Response.GetData()[3])!=ACK)
		{
			iLastError=(ack!=ST_INV_CMD)?ER_NAK:ER_INVALID_CMD;
			return false;
		}
		else return true;

	}
	else 
	{
		return false;
	}
}

/**	\brief	The CCCRSProtocol::CmdGetOptions function sends GET OPTIONS command

  The function reads currently set options from the device. Is applicable to new Bill-To-Bill units only.

	\param	dwOpt	a parameter of type DWORD & containing a reference to the DWORD variable receiving current options.
	\param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

	\return	bool- true if response successfully received

	
*/
bool CCCRSProtocol::CmdGetOptions(DWORD &dwOpt, BYTE Addr)
{
	BYTE Data[256]={SYNC,0,6,GET_OPTIONS,0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd,Addr);
	if(!(iLastError=Response.GetCode()))
	{
		if((Response.GetData()[3]==ST_INV_CMD)&&(Response.GetData()[2]==6))
		{
			iLastError=ER_INVALID_CMD;
			dwOpt=0;
			return false;
		}
		dwOpt=Response.GetData()[6]+((DWORD)Response.GetData()[5]<<8)+((DWORD)Response.GetData()[4]<<16)+((DWORD)Response.GetData()[3]<<24);
		return true;
	}
	else 
	{
		dwOpt=0;
		return false;
	}
}

/**	\brief	The CCCRSProtocol::CmdGetCRC32 function sends CRC32 request

	\param	dwCRC	a parameter of type DWORD & containing a reference to the variable receiving CRC32 of the firmware.
	\param	Addr	a parameter of type BYTE containing device address. Refer to \link Addr Device address list \endlink for the valid values

	\return	bool - true if the request was answered

	
*/
bool CCCRSProtocol::CmdGetCRC32(DWORD &dwCRC, BYTE Addr)
{
	BYTE Data[256]={SYNC,0,6,CRC32,0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd,Addr);
	if(!(iLastError=Response.GetCode()))
	{
		if((Response.GetData()[3]==ST_INV_CMD)&&(Response.GetData()[2]==6))
		{
			iLastError=ER_INVALID_CMD;
			dwCRC=0;
			return false;
		}
		dwCRC=Response.GetData()[6]+((DWORD)Response.GetData()[5]<<8)+((DWORD)Response.GetData()[4]<<16)+((DWORD)Response.GetData()[3]<<24);
		return true;
	}
	else 
	{
		dwCRC=0;
		return false;
	}
}



/**	\brief	The CCCRSProtocol::CmdPowerRecovery function sens POWER RECOVERY command

  The function sends POWER RECOVERY command and stores the response data in the supplied parameters.
  Is applicable to Bill-To-Bill units only.

	\param	State	a parameter of type BYTE & containing a reference to the variable receiving the Power Up state.
	\param	pBuffer	a parameter of type LPBYTE containing pointer to the array receiving state extension data.
	\param	iLen	a parameter of type int & containing a reference to a variable receiving length of the state extension data.

	\return	bool - true if the request was successfully answered.

	
*/
bool CCCRSProtocol::CmdPowerRecovery(BYTE &State, LPBYTE pBuffer, int &iLen)
{
	BYTE Data[256]={SYNC,0,6,POWER_RECOVERY,0,0};
	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd);
	if(!(iLastError=Response.GetCode()))
	{
		if((Response.GetData()[2]==6)&&(Response.GetData()[3]==ST_INV_CMD)) 
		{
			iLastError=ER_INVALID_CMD;
			for(int i=0;i<iLen;i++)
			{
				pBuffer[i]=0;
				iLen=0;
			} 
			return false;
		}
		State=(Response.GetData())[3];
		int iCmdLen=(Response.GetData()[2])-6;
		if(iCmdLen>iLen)
		{
			iLen=-1;
			return false;
		}
		else iLen=iCmdLen;
		for(int i=0;i<iCmdLen;i++)
		{
			pBuffer[i]=(Response.GetData())[i+4];
		} 
		return true;
	}
	else 
	{
		return false;
	}
}

/**	\brief	The CCCRSProtocol::CmdEmptyDispenser function sends EMPTY DISPENSER command

  The function sends EMPTY DISPENSER command only supported by Bill-To-Bill units

	
	\return	bool - true if the command was acknowledged

	
*/
bool CCCRSProtocol::CmdEmptyDispenser()
{
	BYTE Data[256]={SYNC,0,6,EMPTY_DISPENSER,0,0};

	CCommand cmd(Data,0);
	CCommand Response=Transmit(cmd);
	BYTE ack;
	if(!(iLastError=Response.GetCode()))
	{
		if((ack=Response.GetData()[3])!=ACK)
		{
			iLastError=(ack!=ST_INV_CMD)?ER_NAK:ER_INVALID_CMD;
			return false;
		}
		else return true;
	}
	else 
	{
		return false;
	}
}

/** @} */