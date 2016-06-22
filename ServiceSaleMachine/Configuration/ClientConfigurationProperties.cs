using System;
using System.Collections.Generic;

namespace ServiceSaleMachine
{
	public class ClientConfigurationProperties
	{
        // настройки драйверов
        public string comPortScanner;
        public string comPortBill;
        public string adressBill;
        public string comPortPrinter;
        public string NamePrinter;
        public string comPortControl;

        // настройки сервисов
        public List<Service> services;

        internal ClientConfigurationProperties()
		{
			
		}
	}
}
