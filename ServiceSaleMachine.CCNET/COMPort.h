/** @file  COMPort.h: interface for the CCOMPort class.
* The file is a C++ header file containing all necessary CCOMPort class declarations.
*       
*
* \n Product: All RS232 communications
* \n Country: All
* \n Protocol: All RS232 based protocols
* 
*/
#if !defined(AFX_COMPORT_H__DD549000_FA50_11D2_96FF_C4AA5BC53095__INCLUDED_)
#define AFX_COMPORT_H__DD549000_FA50_11D2_96FF_C4AA5BC53095__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
#include <windows.h>


/**	\class CCOMPort
*	\brief	The CCOMPort class provides simple interface to RS232 communications. 
*	The class implements a wrapper for serial port I/O WINAPI.  
*/
namespace ServiceSaleMachine
{
	namespace CCNET
	{
		public class CCOMPort
		{
		public:

			void RTS(bool bRTS = true);	//!< Sets RTS signal to the specified state
			void DTR(bool bDTR = true);	//!< Sets DTR signal to the specified state
			int iTimeOut;				//!< Communication Timeout (ms)
			BOOL InitCOM(int, LPSTR);	//!< Initializes specified port using supplied format string
			BOOL InitCOM(LPSTR, int);	//!< Initializes currently open port using supplied format string and timeout value
			HANDLE GetHandle();			//!< Returns file handle associated with the COM port
			virtual BOOL Recieve(LPBYTE, int);	//!< Reads specified number of bytes into supplied buffer
			virtual BOOL Send(LPBYTE, int);		//!< Sends specified number of bytes from the specified buffer
			virtual BOOL InitCOM(int, LPSTR, int);//!< Initializes specified port using supplied format string and timeout value	
			void CloseCOM();					//!< Closes currently open port
			DWORD IsEnable(int);		//!< Returns Error code received during opening port 
			CCOMPort();					//!< default constructor
			virtual ~CCOMPort();		//!< Object destructor	

		private:
			int iCOM;					//!< COM port number 
			void FindCOM();				//!< Tryes to open 16 COM ports & fills in EnablePorts array
			HANDLE hCOMPort;			//!< File handle associated with the COM port
			DWORD EnablePorts[16];		//!< An array containing port enumeration results (error codes) for firs 16 COM ports
			HANDLE OpenCOM(int);		//!< Opens COM port for further operations
		};
	}
}

extern char S_COM9600[]; 
extern char S_COM19200[];

#endif // !defined(AFX_COMPORT_H__DD549000_FA50_11D2_96FF_C4AA5BC53095__INCLUDED_)
