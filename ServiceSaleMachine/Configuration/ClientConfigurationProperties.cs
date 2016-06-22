using System;

namespace ServiceSaleMachine
{
	public class ClientConfigurationProperties
	{
        public string comPortScanner;
        public string comPortBill;
        public string adressBill;
        public string comPortPrinter;
        public string NamePrinter;
        public string comPortControl;

        internal ClientConfigurationProperties()
		{
			
		}
	}
}
