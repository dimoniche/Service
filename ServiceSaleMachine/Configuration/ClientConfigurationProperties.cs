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
        public string adressBill = "3";
        public string comPortPrinter;
        public string NamePrinter;
        public string comPortControl;
        public int comPortControlSpeed = 9600;

        public string comPortModem;
        public int comPortModemSpeed = 9600;

        // номер телефона для отсылки СМС
        public string numberTelephoneSMS;
        // текст сообщения о исчерпании ресурса
        public string SMSMessageTimeEnd = "Ресурс прошел.";
        // текст сообщения о необходимости инкассации
        public string SMSMessageNeedCollect = "Необходима инкассация.";

        // Отключение железа
        public int offHardware = 1;
        // отключение оплату чеком
        public int offCheck = 1;
        // отключение БД
        public int offDataBase = 1;
        // отключение купюроприемник
        public int offBill = 1;
        // отключение управляющее устройство
        public int offControl = 1;
        // отключение модема
        public int offModem = 1;

        // массив обрабатываемых номиналов купюр
        public int[] nominals = new int[24];

        // режим работы купюроприемника
        public int changeOn = 0;

        // таймаут перехода в реклами или начальный экран
        public int timeout = 0;

        // настройки сервисов
        public List<Service> services;

        // поведение со сдачей
        public int changeToAccount = 0;
        public int changeToCheck = 0;

        // Максимальное количес тво банкнот для вызова инкассации
        public int MaxCountBankNote = 500;

        internal ClientConfigurationProperties()
		{
			
		}
    }
}
