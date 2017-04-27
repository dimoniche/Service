using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AirVitamin.Drivers
{
    public class CCommand
    {
        int iCode;                                  //!< Frame transmission error code. Valid only after the frame was transmitted or received
        byte[] Data = new byte[4096];				//!< A buffer containing frame data

        public CCommand()
        {
            iCode = 0;
        }

        public CCommand(byte[] inData,int Code = 0,int iLen = 0)
        {
            iCode = Code;

            if (iCode == 0)
	        {
		        if(iLen == 0)
			        for (int i = 0; i<inData[2];i++) Data[i] = inData[i];
		        else
			        for (int i = 0; i<iLen;i++) Data[i] = inData[i];	
	        }
        }

        public byte[] GetData()
        {
            return Data;
        }

        public int GetCode()
        {
            return iCode;
        }

        public void SetCode(int Code)
        {
            iCode = Code;
        }

        public void SetByte(byte Byte, int Index)
        {
            Data[Index] = Byte;
        }

        /*public string ToString(char* pStr)
        {
            char sTmp[16];
            strcpy(pStr, "");
            int iLen = (this->Data[2]) ? this->Data[2] : ((Data[4] << 8) + Data[5]);
            for (int i = 0; i < iLen; i++)
            {
                sprintf(sTmp, "%02X ", this->Data[i]);
                strcat(pStr, sTmp);
            }
            return pStr;
        }*/

    }
}
