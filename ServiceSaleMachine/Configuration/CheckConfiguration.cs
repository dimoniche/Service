using System;
using System.IO;
using System.Xml.Linq;

namespace ServiceSaleMachine
{
    public class CheckConfiguration
    {
        private const string RootName = "CheckConfiguration";
        private const string FileName = "CheckConfiguration.cfg";

        public CheckConfigurationProperties Settings { get; private set; }

        public CheckConfiguration()
        {
            Settings = new CheckConfigurationProperties();
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
                        if ((xElement = xSettings.Element("firmsname")) != null) Settings.firmsname = xElement.Value;
                        if ((xElement = xSettings.Element("secondfirmsname")) != null) Settings.secondfirmsname = xElement.Value;
                        if ((xElement = xSettings.Element("PreviouslyService")) != null) Settings.PreviouslyService = xElement.Value;
                        if ((xElement = xSettings.Element("advert1")) != null) Settings.advert1 = xElement.Value;
                        if ((xElement = xSettings.Element("advert2")) != null) Settings.advert2 = xElement.Value;
                        if ((xElement = xSettings.Element("advert3")) != null) Settings.advert3 = xElement.Value;
                        if ((xElement = xSettings.Element("advert4")) != null) Settings.advert4 = xElement.Value;
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

                xSettings.Add(new XElement("firmsname", Settings.firmsname));
                xSettings.Add(new XElement("secondfirmsname", Settings.secondfirmsname));
                xSettings.Add(new XElement("PreviouslyService", Settings.PreviouslyService));
                xSettings.Add(new XElement("advert1", Settings.advert1));
                xSettings.Add(new XElement("advert2", Settings.advert2));
                xSettings.Add(new XElement("advert3", Settings.advert3));
                xSettings.Add(new XElement("advert4", Settings.advert4));

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
            catch (Exception e)
            {

            }
            return false;
        }
    }
}
