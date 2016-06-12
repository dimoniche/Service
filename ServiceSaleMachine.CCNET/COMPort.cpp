/** @file  COMPort.cpp: implementation of the CCOMPort class.
* The file contains implementation of the CCOMPort class.
*       
*
* \n Product: All RS232 communications
* \n Country: All
* \n Protocol: All RS232 based protocols
* 
*/

#include "stdafx.h"
#include "COMPort.h"
#include <msclr\marshal.h>

using namespace ServiceSaleMachine;
using namespace ServiceSaleMachine::CCNET;

using namespace System::Collections::Generic;
using namespace System::Globalization;
using namespace System::Runtime::InteropServices;
using namespace msclr::interop;

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif
char S_COM9600[]="9600,n,8,1";	//!<Comunnication format string setting 9600 bps, no parity, 8 databits, 1 stop bit
char S_COM19200[]="19200,n,8,1";//!<Comunnication format string setting 19200 bps, no parity, 8 databits, 1 stop bit
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

/**	Default class constructor
	
	The default constructor performs initialization of sensitive member variables and enumeration of the 
	COM ports inthe system
*/ 
CCOMPort::CCOMPort()
{
	hCOMPort=INVALID_HANDLE_VALUE;
	FindCOM();
    iTimeOut=1500;
	hCOMPort=INVALID_HANDLE_VALUE;
	
}

/**	Default class destructor
	
	The default destructor calls CloseCOM() function to close the COM port and free associated resources
*/ 
CCOMPort::~CCOMPort()
{
	CloseCOM();

}

/**	\brief	The CCOMPort::IsEnable function returning state of the selected COM port
	
	The function returns error code received during enumeratuion process for the specified COM port

	\param	iPort	a parameter of type int specifying COM port number to request error code for

	\return	DWORD - error code returned by GetLastError() function after CreateFile API function call:
					\n 0 - port is operational and free
					\n 2 - port is not present in the system
					\n otherwise - port is busy with other application or malfunctioning

	
*/
DWORD CCOMPort::IsEnable(int iPort)
{
	return EnablePorts[iPort];
}

/**	\brief	The CCOMPort::OpenCOM function is used to open the COM port specified by parameter

	\param	COMi	a parameter of type int specifiying which port to open

	\return	HANDLE - file handle associated with the open port returned by CreateFile API call

	
*/
HANDLE CCOMPort::OpenCOM(int COMi)
{
  DWORD dwError;
  char strCOM[10]="COM1";
  strCOM[3]+=(iCOM=COMi);
  _SECURITY_ATTRIBUTES Security;
  Security.bInheritHandle=true;
  Security.lpSecurityDescriptor=NULL;
  Security.nLength=sizeof(Security);
  /*hCOMPort = CreateFile(strCOM, GENERIC_READ | GENERIC_WRITE,
              0,NULL, OPEN_EXISTING,
	          FILE_ATTRIBUTE_NORMAL,
	          NULL);*/
	if (hCOMPort == INVALID_HANDLE_VALUE)
		dwError = GetLastError();
	else dwError=0;
  EnablePorts[COMi]=dwError;
  return hCOMPort;

}

/**	\brief	The CCOMPort::CloseCOM function closes the communication resource


	\return	void

	
*/
void CCOMPort::CloseCOM()
{
 if (hCOMPort != INVALID_HANDLE_VALUE) 
	PurgeComm(hCOMPort,-1);
 CloseHandle(hCOMPort);
 hCOMPort=INVALID_HANDLE_VALUE;
}



/**	\brief	The CCOMPort::InitCOM function initializes specified COM port

  
	The function opens the specified port and performs initialization of it using supplied 
	communication format string and timeout value. If the object is associated with COM port,
	which is already open, it will close the port first using CCOMPort::CloseCOM() function call

	\param	COMi	a parameter of type int specifiying COM port number to initialize
	\param	Str	a parameter of type LPSTR containing pointer to a communication format string
	\param	iTimeOut	a parameter of type int specifiying communication timeout value

	\return	BOOL - port initialization result. TRUE in the case of success, otherwise FALSE

	
*/
BOOL CCOMPort::InitCOM(int COMi, LPSTR Str,int iTimeOut=300)
{
   DCB dcb,dcb1;
   COMMTIMEOUTS CommTimeOuts;
   if(COMi<0) return FALSE;
   if(EnablePorts[COMi])return FALSE;
   PurgeComm(hCOMPort,-1);
   CloseCOM();
   SetupComm(OpenCOM(COMi),65535,0xffff);
   GetCommState(hCOMPort, &dcb);
   //if(!BuildCommDCB(Str,&dcb1))return FALSE;
 // Filling in the DCB
	  dcb.BaudRate = dcb1.BaudRate;
	  dcb.ByteSize = dcb1.ByteSize;
	  dcb.Parity = dcb1.Parity;
	  dcb.StopBits = dcb1.StopBits; 
      dcb.fBinary=1;          // binary mode, no EOF check
      dcb.fParity=0;          // enable parity checking
      dcb.fAbortOnError=FALSE; // abort reads/writes on error
	  dcb.fDtrControl=DTR_CONTROL_DISABLE;
	  dcb.fRtsControl=RTS_CONTROL_DISABLE;
	  dcb.fOutxCtsFlow=FALSE;
	  dcb.fOutxDsrFlow=FALSE;
	  dcb.fDsrSensitivity=FALSE;
	  dcb.fOutX=FALSE;
 //---------------
  if(!SetCommState(hCOMPort, &dcb))return FALSE;
  CommTimeOuts.ReadTotalTimeoutConstant=iTimeOut;
  CommTimeOuts.ReadTotalTimeoutMultiplier=11;
  CommTimeOuts.WriteTotalTimeoutConstant=200;
  CommTimeOuts.WriteTotalTimeoutMultiplier=11;
  return SetCommTimeouts( hCOMPort, &CommTimeOuts ) ;

 //------------------------
}

/**	\brief	The CCOMPort::Send function transmits specifiying number of bytes via COM port

	\param	Data	a parameter of type LPBYTE containing pointer to BYTE array with the data,
			which needs to be transmitted
	\param	Number	a parameter of type int containing number of bytes to transmit

	\return	BOOL - operation result. TRUE if specified number of bytes successfully transmitted, otherwise FALSE

	
*/
BOOL CCOMPort::Send(LPBYTE Data, int Number)
{
 
 BOOL bError;
 DWORD wBytes;
 bError=WriteFile(hCOMPort,Data,Number,&wBytes,NULL);

 bError=bError&&((DWORD)Number==wBytes);
 return bError;
}

/**	\brief	The CCOMPort::Recieve function receives specified number of bytes from the COM port

	\param	Buffer	a parameter of type LPBYTE - pointer to the BYTE array receiving data
	\param	Length	a parameter of type int - number of bytes to receive

	\return	BOOL - operation result. TRUE if specified number of bytes successfully received, otherwise FALSE

	
*/
BOOL CCOMPort::Recieve(LPBYTE Buffer, int Length)
{
 BOOL res;
 DWORD dwBytes;
 res=ReadFile(hCOMPort,Buffer,Length,&dwBytes,NULL);
 res=res&(dwBytes==(DWORD)Length);
 return res;
}

/**	\brief	The CCOMPort::FindCOM function performs enumeration of available COM ports

	The funcion opens and then closes consequently firs 16 COM ports and records received error codes into 
	the EnablePorts array. The obtained error codes can be later requested using CCOMPort::IsEnable(int iPort)
	function call in order to determine availability of the certain ports.

	\return	void

	
*/
void CCOMPort::FindCOM()
{
	  for (char i=0; i<16; i++)
	  {
		  OpenCOM(i);
		  CloseCOM();
	  }
}

/**	\brief	The CCOMPort::GetHandle function returns a file handle associated with the COM port


	\return	HANDLE - a file handle associated with the COM port or INVALID_HANDLE_VALUE 
					if the port is not open

	
*/
HANDLE CCOMPort::GetHandle()
{
	return hCOMPort;
}

/**	\brief	The CCOMPort::InitCOM function initializes currently open COM port

  
	The function performs reinitialization of the COM port associated with the object using supplied 
	communication format string and timeout value. 

	\param	Str	a parameter of type LPSTR containing pointer to a communication format string
	\param	TimeOut	a parameter of type int specifiying communication timeout value

	\return	BOOL - port initialization result. TRUE in the case of success, otherwise FALSE

	
*/
BOOL CCOMPort::InitCOM(LPSTR Str,int TimeOut)
{
   if(hCOMPort==INVALID_HANDLE_VALUE)return FALSE;
   return InitCOM(iCOM,Str,TimeOut);
}

/**	\brief	The CCOMPort::InitCOM function initializes specified COM port

  
	The function opens the specified port and performs initialization of it using supplied 
	communication format string and default timeout value. If the object is associated with COM port,
	which is already open, it will close the port first using CCOMPort::CloseCOM() function call

	\param	COMi	a parameter of type int specifiying COM port number to initialize
	\param	Str	a parameter of type LPSTR containing pointer to a communication format string

	\return	BOOL - port initialization result. TRUE in the case of success, otherwise FALSE

	
*/
BOOL CCOMPort::InitCOM(int COMi, LPSTR Str)
{
	return InitCOM(COMi,Str,iTimeOut); 
}

/**	\brief	The CCOMPort::DTR function sets DTR line to the state specified by parameter

	\param	bDTR	a parameter of type bool specifiying whether to set the line into the active state

	\return	void

	
*/
void CCOMPort::DTR(bool bDTR)
{
	EscapeCommFunction(hCOMPort,(bDTR)?SETDTR:CLRDTR);
	Sleep(1);
}
/**	\brief	The CCOMPort::RTS function sets RTS line to the state specified by parameter

	\param	bRTS	a parameter of type bool specifiying whether to set the line into the active state

	\return	void

	
*/
void CCOMPort::RTS(bool bRTS)
{
	EscapeCommFunction(hCOMPort,(bRTS)?SETRTS:CLRRTS);
	Sleep(1);
}
