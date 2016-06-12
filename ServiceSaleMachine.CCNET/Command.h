/** @file  Command.h: interface for the CCommand class.
* The file is a C++ header file containing CCommand class declaration.
*       
*
* \n Product: All communications
* \n Country: All
* \n Protocol: All protocols
* 
*/
#if !defined(AFX_COMMAND_H__0E446933_D01D_11D4_A0E8_00002165B0A2__INCLUDED_)
#define AFX_COMMAND_H__0E446933_D01D_11D4_A0E8_00002165B0A2__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <windows.h>


/**	\class CCommand
		\brief	The CCommand class representing a data frame used for communications

  The class contains a frame buffer and set of functions to work with it. 
  It can be use to wrap communication data of any protocol.
*/
namespace ServiceSaleMachine
{
	namespace CCNET
	{
		public class CCommand
		{
		public:
			char* ToString(char*);			//!< Formats internal frame data into a string, containing its hexadecimal representation
			void SetByte(BYTE, int);			//!< Sets specified byte in the buffer to the supplied value
			void SetCode(int);				//!< Sets transmission error code to the specified value
			CCommand(LPBYTE, int, int iLen = 0);//!< Constructor performing initialization of all object's variables
			int GetCode();					//!< Retrieves communication error code
			LPBYTE GetData();				//!< Retrieves pointer to the frame data
			CCommand();						//!< Default constructor
			virtual ~CCommand();			//!< Default destructor

		private:
			int iCode;						//!< Frame transmission error code. Valid only after the frame was transmitted or received
			BYTE Data[4096];				//!< A buffer containing frame data
		};
	}
}

#endif // !defined(AFX_COMMAND_H__0E446933_D01D_11D4_A0E8_00002165B0A2__INCLUDED_)
