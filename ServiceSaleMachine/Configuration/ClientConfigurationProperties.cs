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

        // настройки отображения - изображения
        public string ButtonFail;
        public string ButtonYes;
        public string ButtonUser;
        public string ButtonCheck;
        public string ButtonMoney;
        public string ButtonService;
        public string ButtonServiceEmpty;
        public string ButtonBack;
        public string ButtonForward;
        public string ButtonEnterUserName;
        public string ButtonEnterUserPasw;

        internal ClientConfigurationProperties()
		{
			
		}
	}
}
