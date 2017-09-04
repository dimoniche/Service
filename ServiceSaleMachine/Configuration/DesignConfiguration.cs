using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AirVitamin
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
                string fileName = Globals.GetPath(PathEnum.Config) + "\\" + FileName;
                if (File.Exists(fileName))
                {
                    XElement root = XElement.Load(fileName);
                    if (root.Name != RootName) return false;

                    XElement xElement;

                    // Настройки драйверов
                    XElement xSettings = root.Element("ImageNames");

                    if (xSettings != null)
                    {
                        if ((xElement = xSettings.Element("ButtonStartServices")) != null) Settings.ButtonStartServices = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonHelp")) != null) Settings.ButtonHelp = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonPhilosof")) != null) Settings.ButtonPhilosof = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonInter")) != null) Settings.ButtonInter = xElement.Value;

                        if ((xElement = xSettings.Element("ButtonService")) != null) Settings.ButtonService = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonService1")) != null) Settings.ButtonService1 = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonService2")) != null) Settings.ButtonService2 = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonService3")) != null) Settings.ButtonService3 = xElement.Value;

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

                        if ((xElement = xSettings.Element("ButtonRegister")) != null) Settings.ButtonRegister = xElement.Value;
                        if ((xElement = xSettings.Element("ButtonRemember")) != null) Settings.ButtonRemember = xElement.Value;

                        if ((xElement = xSettings.Element("ScreenSaver")) != null) Settings.ScreenSaver = xElement.Value;
                        if ((xElement = xSettings.Element("ScreenSaverVideo")) != null) Settings.ScreenSaverVideo = xElement.Value;
                        if ((xElement = xSettings.Element("ScreenSaverVideo1")) != null) Settings.ScreenSaverVideo1 = xElement.Value;
                        if ((xElement = xSettings.Element("ScreenSaverVideo2")) != null) Settings.ScreenSaverVideo2 = xElement.Value;
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

                if (xSettings.HasElements)
                {
                    root.Add(xSettings);
                }

                if (!File.Exists(Globals.GetPath(PathEnum.Config) + "\\" + FileName))
                {
                    Directory.CreateDirectory(Globals.GetPath(PathEnum.Config));
                    FileStream fs = File.Create(Globals.GetPath(PathEnum.Config) + "\\" + FileName);
                    fs.Close();
                }
                root.Save(Globals.GetPath(PathEnum.Config) + "\\" + FileName);
                return true;
            }
            catch (Exception e)
            {

            }
            return false;
        }
    }
}
