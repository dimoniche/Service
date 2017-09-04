using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace AirVitamin
{

    public class DesignConfigurationProperties
    {
        // настройки отображения - изображения
        const string defpicture = "default.png";

        public string ButtonStartServices = defpicture;
        public string ButtonHelp = defpicture;
        public string ButtonPhilosof = defpicture;
        public string ButtonInter = defpicture;
        public string ButtonService = defpicture;
        public string ButtonService1 = defpicture;
        public string ButtonService2 = defpicture;
        public string ButtonService3 = defpicture;
        public string ButtonRetToMain = defpicture;
        public string ButtonWhatsDiff = defpicture;
        public string LogoService1 = defpicture;
        public string LogoService2 = defpicture;
        public string LogoService3 = defpicture;
        public string ButtonGetOxigen = defpicture;
        public string ScreenSaver = defpicture;
        public string ScreenSaverVideo = defpicture;
        public string ScreenSaverVideo1 = defpicture;
        public string ScreenSaverVideo2 = defpicture;

        public string ButtonTakeAwayMoney = defpicture;
        public string ButtonreturnMoney = defpicture;
        public string ButtonOK = defpicture;

        public string ButtonLogo = defpicture;
        public string ButtonFail = defpicture;
        public string ButtonYes = defpicture;
        public string ButtonUser = defpicture;
        public string ButtonCheck = defpicture;
        public string ButtonMoney = defpicture;
        public string ButtonMoneyToAccount = defpicture;
        public string ButtonMoneyToCheck = defpicture;
        public string ButtonServiceEmpty = defpicture;
        public string ButtonBack = defpicture;
        public string ButtonForward = defpicture;
        public string ButtonNoForward = defpicture;
        public string ButtonEnterUserName = defpicture;
        public string ButtonEnterUserPasw = defpicture;
        public string ButtonRegister = defpicture;
        public string ButtonRemember = defpicture;

        public string ButtonMinus = defpicture;
        public string ButtonPlus = defpicture;
        public string ButtonOplata = defpicture;

        internal DesignConfigurationProperties()
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
            catch (Exception e)
            {
            }
        }

    }
}
