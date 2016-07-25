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

        // массив обрабатываемых номиналов купюр
        public int[] nominals = new int[24];

        // режим работы купюроприемника
        public int changeOn;

        // таймаут перехода в реклами или начальный экран
        public int timeout;

        // настройки сервисов
        public List<Service> services;


        internal ClientConfigurationProperties()
		{
			
		}
    }
}
