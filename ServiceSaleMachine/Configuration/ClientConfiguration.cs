using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;

namespace ServiceSaleMachine
{
	public class ClientConfiguration
	{
		private const string RootName = "ClientConfiguration";
		private const string FileName = "ClientConfiguration.cfg";

        public ClientConfigurationProperties Settings { get; private set; }

		public ClientConfiguration()
		{
			Settings = new ClientConfigurationProperties();
		}

        public int ServCount
        {
            get { return Settings.services.Count; }
            set { }
        }

        public Service ServiceByIndex(int aIndex)
        {
            return Settings.services[aIndex];
        }

        public bool Load()
		{
			try
			{
				string fileName = Globals.GetPath(PathEnum.Bin) + "\\" + FileName;
				if (File.Exists(fileName))
				{
					XElement root = XElement.Load(fileName);
					if (root.Name != RootName) return false;

					XElement xElement;

					// Настройки драйверов
					XElement xSettings = root.Element("Settings");

					if (xSettings != null)
					{
						if ((xElement = xSettings.Element("comPortScanner")) != null) Settings.comPortScanner = xElement.Value;
                        if ((xElement = xSettings.Element("adressBill")) != null) Settings.adressBill = xElement.Value;
                        if ((xElement = xSettings.Element("comPortBill")) != null) Settings.comPortBill = xElement.Value;
                        if ((xElement = xSettings.Element("comPortPrinter")) != null) Settings.comPortPrinter = xElement.Value;
                        if ((xElement = xSettings.Element("NamePrinter")) != null) Settings.NamePrinter = xElement.Value;
                        if ((xElement = xSettings.Element("comPortControl")) != null) Settings.comPortControl = xElement.Value;
                        if ((xElement = xSettings.Element("comPortModem")) != null) Settings.comPortModem = xElement.Value;
                        if ((xElement = xSettings.Element("comPortControlSpeed")) != null) Settings.comPortControlSpeed = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("comPortModemSpeed")) != null) Settings.comPortModemSpeed = int.Parse(xElement.Value);

                        if ((xElement = xSettings.Element("numberTelephoneSMS")) != null) Settings.numberTelephoneSMS = xElement.Value;
                        if ((xElement = xSettings.Element("SMSMessageTimeEnd")) != null) Settings.SMSMessageTimeEnd = xElement.Value;

                        if ((xElement = xSettings.Element("offHardware")) != null) Settings.offHardware = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("offCheck")) != null) Settings.offCheck = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("offDataBase")) != null) Settings.offDataBase = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("offBill")) != null) Settings.offBill = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("offControl")) != null) Settings.offControl = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("offModem")) != null) Settings.offModem = int.Parse(xElement.Value);

                        if ((xElement = xSettings.Element("changeOn")) != null) Settings.changeOn = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("timeout")) != null) Settings.timeout = int.Parse(xElement.Value);

                        if ((xElement = xSettings.Element("CommandControl1Open")) != null) Settings.CommandControl1Open = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("CommandControl1Close")) != null) Settings.CommandControl1Close = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("CommandControl2Open")) != null) Settings.CommandControl2Open = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("CommandControl2Close")) != null) Settings.CommandControl2Close = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("CommandControl3Open")) != null) Settings.CommandControl3Open = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("CommandControl3Close")) != null) Settings.CommandControl3Close = int.Parse(xElement.Value);

                        if ((xElement = xSettings.Element("changeToAccount")) != null) Settings.changeToAccount = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("changeToCheck")) != null) Settings.changeToCheck = int.Parse(xElement.Value);

                        if ((xElement = xSettings.Element("nominals")) != null)
                        {
                            Settings.nominals = new int[24];
                            int i = 0;
                            foreach (XElement xItem in xElement.Elements("nominal"))
                            {
                                int tmp = int.Parse(xItem.Value);
                                Settings.nominals[i++] = tmp;
                            }
                        }
                                            
                        // настройки сервисов
                        if ((xElement = xSettings.Element("services")) != null)
                        {
                            Settings.services = new List<Service>();
                            int i = 1;
                            foreach (XElement xItem in xElement.Elements("Service"))
                            {
                                Service tmp = Service.FromXml(xItem);
                                if(tmp.id == 0)
                                {
                                    tmp.id = i;
                                }
                                Settings.services.Add(tmp);
                                i++;
                            }
                        }
                    }

					return true;
				}
			}
			catch(Exception e)
			{
                Debug.Print(e.Message);
            }
			return false;
		}

		public bool Save()
		{
			try
			{
				XElement root = new XElement(RootName);

				// Настройки
				XElement xSettings = new XElement("Settings");

				xSettings.Add(new XElement("comPortScanner", Settings.comPortScanner));
                xSettings.Add(new XElement("comPortBill", Settings.comPortBill));
                xSettings.Add(new XElement("adressBill", Settings.adressBill));
                xSettings.Add(new XElement("comPortPrinter", Settings.comPortPrinter));
                xSettings.Add(new XElement("NamePrinter", Settings.NamePrinter));
                xSettings.Add(new XElement("comPortControl", Settings.comPortControl));
                xSettings.Add(new XElement("comPortModem", Settings.comPortModem));
                xSettings.Add(new XElement("comPortControlSpeed", Settings.comPortControlSpeed.ToString()));
                xSettings.Add(new XElement("comPortModemSpeed", Settings.comPortModemSpeed.ToString()));
                xSettings.Add(new XElement("numberTelephoneSMS", Settings.numberTelephoneSMS));
                xSettings.Add(new XElement("SMSMessageTimeEnd", Settings.SMSMessageTimeEnd));

                xSettings.Add(new XElement("offHardware", Settings.offHardware.ToString()));
                xSettings.Add(new XElement("offCheck", Settings.offCheck.ToString()));
                xSettings.Add(new XElement("offDataBase", Settings.offDataBase.ToString()));
                xSettings.Add(new XElement("offBill", Settings.offBill.ToString()));
                xSettings.Add(new XElement("offControl", Settings.offControl.ToString()));
                xSettings.Add(new XElement("offModem", Settings.offModem.ToString()));

                xSettings.Add(new XElement("changeOn", Settings.changeOn.ToString()));
                xSettings.Add(new XElement("timeout", Settings.timeout.ToString()));

                xSettings.Add(new XElement("CommandControl1Open", Settings.CommandControl1Open.ToString()));
                xSettings.Add(new XElement("CommandControl1Close", Settings.CommandControl1Close.ToString()));
                xSettings.Add(new XElement("CommandControl2Open", Settings.CommandControl2Open.ToString()));
                xSettings.Add(new XElement("CommandControl2Close", Settings.CommandControl2Close.ToString()));
                xSettings.Add(new XElement("CommandControl3Open", Settings.CommandControl3Open.ToString()));
                xSettings.Add(new XElement("CommandControl3Close", Settings.CommandControl3Close.ToString()));

                xSettings.Add(new XElement("changeToAccount", Settings.changeToAccount.ToString()));
                xSettings.Add(new XElement("changeToCheck", Settings.changeToCheck.ToString()));

                XElement element = new XElement("nominals");
                foreach (int nominal in Settings.nominals)
                {
                    XElement newservice = new XElement("nominal", nominal.ToString());
                    element.Add(newservice);
                }
                xSettings.Add(element);

                if (Settings.services.Count > 0)
                {
                    XElement services = new XElement("services");
                    foreach (Service service in Settings.services)
                    {
                        XElement newservice = service.ToXml();
                        services.Add(newservice);
                    }
                    xSettings.Add(services);
                }

                if (xSettings.HasElements)
				{
					root.Add(xSettings);
				}

                if (!File.Exists(Globals.GetPath(PathEnum.Bin) + "\\" + FileName))
                {
                    Directory.CreateDirectory(Globals.GetPath(PathEnum.Bin));
                    FileStream fs = File.Create(Globals.GetPath(PathEnum.Bin) + "\\" + FileName);
                    fs.Close();
                }
				root.Save(Globals.GetPath(PathEnum.Bin) + "\\" + FileName);
				return true;
			}
			catch(Exception e)
			{

			}
			return false;
		}
	}
}
