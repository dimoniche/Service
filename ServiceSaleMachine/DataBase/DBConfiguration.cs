using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;

namespace AirVitamin
{
    /// <summary>
    /// Настройки доступа к БД
    /// </summary>
    public class DBConfiguration
    {
        private const string RootName = "DBConfiguration";
        private const string FileName = "DBConfiguration.cfg";

        public string Server = "";
        public string Database = "";
        public string UserID = "";
        public string Password = "";

        // настройки резервирования
        public string folderBuckUp = "";
        public int AutomaticBuckUp = 1;
        public int PeriodBuckUp = 6;

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
                    XElement xSettings = root.Element("Settings");

                    if (xSettings != null)
                    {
                        if ((xElement = xSettings.Element("Server")) != null) Server = xElement.Value;
                        if ((xElement = xSettings.Element("Database")) != null) Database = xElement.Value;
                        if ((xElement = xSettings.Element("UserID")) != null) UserID = xElement.Value;
                        if ((xElement = xSettings.Element("Password")) != null) Password = xElement.Value;
                        if ((xElement = xSettings.Element("folderBuckUp")) != null) folderBuckUp = xElement.Value;

                        if ((xElement = xSettings.Element("AutomaticBuckUp")) != null) AutomaticBuckUp = int.Parse(xElement.Value);
                        if ((xElement = xSettings.Element("PeriodBuckUp")) != null) PeriodBuckUp = int.Parse(xElement.Value);
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

                xSettings.Add(new XElement("Server", Server));
                xSettings.Add(new XElement("Database", Database));
                xSettings.Add(new XElement("UserID", UserID));
                xSettings.Add(new XElement("Password", Password));

                xSettings.Add(new XElement("folderBuckUp", folderBuckUp));
                xSettings.Add(new XElement("AutomaticBuckUp", AutomaticBuckUp));
                xSettings.Add(new XElement("PeriodBuckUp", PeriodBuckUp));

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
