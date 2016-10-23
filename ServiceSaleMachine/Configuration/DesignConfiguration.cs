using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ServiceSaleMachine
{
    public class DesignConfiguration
    {
        private const string RootName = "DesignConfiguration";
        private const string FileName = "DesignConfiguration.cfg";

        public DesignConfigurationProperties Settings { get; private set; }

        public DesignConfiguration()
        {
            Settings = new DesignConfigurationProperties();
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
                        if ((xElement = xSettings.Element("PanelBackGround")) != null) Settings.PanelBackGround = xElement.Value;

                        if ((xElement = xSettings.Element("ButtonStartServices")) != null) Settings.ButtonStartServices = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonHelp")) != null) Settings.ButtonHelp = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonPhilosof")) != null) Settings.ButtonPhilosof = xElement.Value;

                        if ((xElement = xSettings.Element("ButtonService")) != null) Settings.ButtonService = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonService1")) != null) Settings.ButtonService1 = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonService2")) != null) Settings.ButtonService2 = xElement.Value;

                        if ((xElement = xSettings.Element("ButtonRetToMain")) != null) Settings.ButtonRetToMain = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonWhatsDiff")) != null) Settings.ButtonWhatsDiff = xElement.Value;
                        if ((xElement = xSettings.Element("LogoService1")) != null) Settings.LogoService1 = xElement.Value;
                        if ((xElement = xSettings.Element("LogoService2")) != null) Settings.LogoService2 = xElement.Value;

                        if ((xElement = xSettings.Element("ButtonGetOxigen")) != null) Settings.ButtonGetOxigen = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonTakeAwayMoney")) != null) Settings.ButtonTakeAwayMoney = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonreturnMoney")) != null) Settings.ButtonreturnMoney = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonOK")) != null) Settings.ButtonOK = xElement.Value;

                        if ((xElement = xSettings.Element("ButtonLogo")) != null) Settings.ButtonLogo = xElement.Value;

                        if ((xElement = xSettings.Element("ButtonFail")) != null) Settings.ButtonFail = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonYes")) != null) Settings.ButtonYes = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonUser")) != null) Settings.ButtonUser = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonCheck")) != null) Settings.ButtonCheck = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonMoney")) != null) Settings.ButtonMoney = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonMoneyToCheck")) != null) Settings.ButtonMoneyToCheck = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonMoneyToAccount")) != null) Settings.ButtonMoneyToAccount = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonServiceEmpty")) != null) Settings.ButtonServiceEmpty = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonBack")) != null) Settings.ButtonBack = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonForward")) != null) Settings.ButtonForward = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonNoForward")) != null) Settings.ButtonNoForward = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonEnterUserName")) != null) Settings.ButtonEnterUserName = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonEnterUserPasw")) != null) Settings.ButtonEnterUserPasw = xElement.Value;

                        if ((xElement = xSettings.Element("ScreenSaver")) != null) Settings.ScreenSaver = xElement.Value;
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
