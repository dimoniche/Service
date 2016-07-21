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

                        if ((xElement = xSettings.Element("offHardware")) != null) Settings.offHardware = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("offCheck")) != null) Settings.offCheck = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("offDataBase")) != null) Settings.offDataBase = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("offBill")) != null) Settings.offBill = int.Parse(xElement.Value);

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

                xSettings.Add(new XElement("offHardware", Settings.offHardware.ToString()));
                xSettings.Add(new XElement("offCheck", Settings.offCheck.ToString()));
                xSettings.Add(new XElement("offDataBase", Settings.offDataBase.ToString()));
                xSettings.Add(new XElement("offBill", Settings.offBill.ToString()));

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
