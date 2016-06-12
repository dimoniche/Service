using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

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

					// Настройки
					XElement xSettings = root.Element("Settings");

					if (xSettings != null)
					{
						if ((xElement = xSettings.Element("comPortScanner")) != null) Settings.comPortScanner = xElement.Value;
                        if ((xElement = xSettings.Element("comPortBill")) != null) Settings.comPortBill = xElement.Value;
                        if ((xElement = xSettings.Element("comPortPrinter")) != null) Settings.comPortPrinter = xElement.Value;
                    }

					return true;
				}
			}
			catch
			{
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
                xSettings.Add(new XElement("comPortPrinter", Settings.comPortPrinter));

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
