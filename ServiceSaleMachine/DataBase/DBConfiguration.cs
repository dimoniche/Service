using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;

namespace ServiceSaleMachine
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

                        if ((xElement = xSettings.Element("Server")) != null) Server = xElement.Value;
                        if ((xElement = xSettings.Element("Database")) != null) Database = xElement.Value;
                        if ((xElement = xSettings.Element("UserID")) != null) UserID = xElement.Value;
                        if ((xElement = xSettings.Element("Password")) != null) Password = xElement.Value;
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
