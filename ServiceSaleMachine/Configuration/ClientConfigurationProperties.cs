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

        // настройки отображения - изображения
        const string defpicture = "default.png";
        public string ButtonStartServices = defpicture;
        public string ButtonLogo = defpicture;
        public string ButtonHelp = defpicture;
        public string ButtonFail = defpicture;
        public string ButtonYes = defpicture;
        public string ButtonUser = defpicture;
        public string ButtonCheck = defpicture;
        public string ButtonMoney = defpicture;
        public string ButtonService = defpicture;
        public string ButtonServiceEmpty = defpicture;
        public string ButtonBack = defpicture;
        public string ButtonForward = defpicture;
        public string ButtonNoForward = defpicture;
        public string ButtonEnterUserName = defpicture;
        public string ButtonEnterUserPasw = defpicture;

        internal ClientConfigurationProperties()
		{
			
		}

        public void LoadPictureBox(PictureBox pbx, string path)
        {
            string fname = Globals.GetPath(PathEnum.Image) + "\\" + path;

            if (!File.Exists(fname))
            {
                fname = Globals.GetPath(PathEnum.Image) + "\\" + defpicture;
            }
            try
            {
                pbx.Load(fname);
            }
            catch(Exception e)
            {
            }
        }

    }
}
