using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ServiceSaleMachine.Configuration
{
    class DesignConfiguration
    {
        private const string RootName = "DesignConfiguration";
        private const string FileName = "DesignConfiguration.cfg";

        public ClientConfigurationProperties Settings { get; private set; }
        public DesignConfigurationProperties Design { get; private set; }

        public DesignConfiguration()
        {
            Settings = new ClientConfigurationProperties();
        }

        public int ServCount
        {
            get { return Settings.services.Count; }
            set { }
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
                    XElement xSettings = root.Element("ImageNames");

                    if (xSettings != null)
                    {
                        if ((xElement = xSettings.Element("ButtonRetToMain")) != null) Design.ButtonRetToMain = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonLogo")) != null) Design.ButtonLogo = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonHelp")) != null) Design.ButtonHelp = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonStartServices")) != null) Design.ButtonStartServices = xElement.Value;

                        if ((xElement = xSettings.Element("ButtonFail")) != null) Design.ButtonFail = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonYes")) != null) Design.ButtonYes = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonUser")) != null) Design.ButtonUser = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonCheck")) != null) Design.ButtonCheck = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonMoney")) != null) Design.ButtonMoney = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonService")) != null) Design.ButtonService = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonServiceEmpty")) != null) Design.ButtonServiceEmpty = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonBack")) != null) Design.ButtonBack = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonForward")) != null) Design.ButtonForward = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonNoForward")) != null) Design.ButtonNoForward = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonEnterUserName")) != null) Design.ButtonEnterUserName = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonEnterUserPasw")) != null) Design.ButtonEnterUserPasw = xElement.Value;
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
                return false;

                XElement root = new XElement(RootName);

                // Настройки
                XElement xSettings = new XElement("Settings");

/*                xSettings.Add(new XElement("RetToMain", Settings.ButtonRetToMain));
                xSettings.Add(new XElement("ButtonFail", Settings.ButtonFail));
                xSettings.Add(new XElement("ButtonYes", Settings.ButtonYes));
                xSettings.Add(new XElement("ButtonUser", Settings.ButtonUser));
                xSettings.Add(new XElement("ButtonCheck", Settings.ButtonCheck));
                xSettings.Add(new XElement("ButtonMoney", Settings.ButtonMoney));
                xSettings.Add(new XElement("ButtonService", Settings.ButtonService));
                xSettings.Add(new XElement("ButtonServiceEmpty", Settings.ButtonServiceEmpty));
                xSettings.Add(new XElement("ButtonBack", Settings.ButtonBack));
                xSettings.Add(new XElement("ButtonForward", Settings.ButtonForward));
                xSettings.Add(new XElement("ButtonNoForward", Settings.ButtonNoForward));
                xSettings.Add(new XElement("ButtonEnterUserName", Settings.ButtonEnterUserName));
                xSettings.Add(new XElement("ButtonEnterUserPasw", Settings.ButtonEnterUserPasw));
                */

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
