using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AirVitamin
{
    public class CostConfiguration
    {
        private const string FileName = "CostConfiguration.cfg";
        private const string RootName = "CostConfiguration";

        public CostTable CostSetting;

        public CostConfiguration()
        {
            CostSetting = new CostTable();
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
                    XElement xSettings = root.Element("CostSetting");

                    if (xSettings != null)
                    {
                        CostSetting = CostTable.FromXml(xSettings);
                    }

                    return true;
                }
            }
            catch (Exception e)
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
                XElement xSettings = new XElement("CostSetting");

                XElement table = CostSetting.ToXml();
                xSettings.Add(table);

                if (xSettings.HasElements)
                {
                    root.Add(xSettings);
                }

                root.Save(Globals.GetPath(PathEnum.Config) + "\\" + FileName);

                return true;
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }

            return false;
        }
    }
}
