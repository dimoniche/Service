/** @file  Command.cpp: implementation of the CCommand class.
*
* \n Product: All communications
* \n Country: All
* \n Protocol: All protocols
* 
*/

#include "stdafx.h"
#include "Command.h"
#include <msclr\marshal.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

#include <stdio.h>

using namespace ServiceSaleMachine;
using namespace ServiceSaleMachine::CCNET;

using namespace System::Collections::Generic;
using namespace System::Globalization;
using namespace System::Runtime::InteropServices;
using namespace msclr::interop;

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

/**	\brief Default object constructor
*/
CCommand::CCommand()
{
	iCode=0;
}

/**	\brief Default object destructor
*/
CCommand::~CCommand()
{

}

/**	\brief	The CCommand::GetData function returning pointer to the frame internal data


	\return	LPBYTE - pointer to the internal data array

	
*/
LPBYTE CCommand::GetData()
{
	return Data;
}

/**	\brief	The CCommand::GetCode function returns communication error code associated with frame.

  
	\return	int - error code associated with the frame. Can take any integer value and is protocol-specific.
				Typically is used to store transmission/reception error code, but can be used for other 
				purposes as well.	

	
*/
int CCommand::GetCode()
{
	return iCode;
}

/**	\brief	Second variant of the CCommand constructor

  The constructor constructs the object and performs its initialization based on the supplied parameters

	\param	inData	a parameter of type LPBYTE specifiying frame data to copy to the internal data storage.
	\param	Code	a parameter of type int specifiying communication error code to associate with the frame.
	\param	iLen	a parameter of type int specifiying length of the frame. If it is 0 the length stored 
	in the thid frame byte will be used.

	
*/
CCommand::CCommand(LPBYTE inData,int Code=0,int iLen)
{
	if(!(iCode=Code))	
	{
		if(!iLen)
			for (int i=0;i<inData[2];i++) Data[i]=inData[i];
		else
			for (int i=0;i<iLen;i++) Data[i]=inData[i];	
	}
}

/**	\brief	The CCommand::SetCode function sets the communication error code

	\param	Code	a parameter of type int specifiying value to set. The set value can be retrieved 
	later using CCommand::GetCode() function call.

	\return	void

	
*/
void CCommand::SetCode(int Code)
{
	iCode=Code;
}

/**	\brief	The CCommand::SetByte function sets specified byte in the internal data storage to the supplied value.

	\param	Byte	a parameter of type BYTE - value to set.
	\param	Index	a parameter of type int - zero-based index(position) of the byte to modify

	\return	void

	
*/
void CCommand::SetByte(BYTE Byte, int Index)
{		
	Data[Index]=Byte;
}

/**	\brief	The CCommand::ToString function formats internal data into a string

	The function outputs a string containing hexadecimal representation of the internal buffer. 
	The 2-character byte representations are separated with spaces.

	\param	pStr	a parameter of type char* - pointer to the destination string

	\return	char* -pointer to the destination string

	
*/
char* CCommand::ToString(char* pStr)
{
	char sTmp[16];
	strcpy(pStr,"");
	int iLen=(this->Data[2])?this->Data[2]:((Data[4]<<8)+Data[5]);
	for (int i=0; i<iLen;i++)
	{
		sprintf(sTmp,"%02X ",this->Data[i]);
		strcat(pStr,sTmp);
	}
	return pStr;
}
