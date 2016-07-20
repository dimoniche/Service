using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

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

        // Отключение железа
        public int offHardware;
        // отключение оплату чеком
        public int offCheck;
        // отключение БД
        public int offDataBase;
        // отключение купюроприемник
        public int offBill;

        // настройки сервисов
        public List<Service> services;

        internal ClientConfigurationProperties()
		{
			
		}


    }
}
