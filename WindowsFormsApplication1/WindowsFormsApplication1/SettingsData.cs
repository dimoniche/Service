using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public class Service
    {
        public string caption { get; set; }
        public string filename { get; set; }
        public int timework { get; set; }


    }



//---------------------------------------------------------------------------------------------


    public class SettingsData
    {
        List<Service> services;

        public SettingsData()
        {
            services = new List<Service> { };
        }

        public int ServCount
        {
            get { return services.Count; }
            set { }
        }

        public Service ServiceByIndex(int aIndex)
        {
            return services[aIndex]; 
        }


        public int LoadXml()
        {
            XmlDocument doc = new XmlDocument();
            string xmlname = "d:\\work\\TermServices\\Service\\MyTests\\bin\\services.xml";

            if (System.IO.File.Exists(xmlname))
            {
                try
                {
                    doc.Load(xmlname);
                }
                catch
                { return 1; }
            }



            XmlElement xRoot = doc.DocumentElement;
            foreach (XmlNode node in xRoot)
            {
                Debug.Print(node.Name.ToString());
                string filename = "d:\\work\\TermServices\\Service\\MyTests\\bin\\";//Environment.CurrentDirectory+ "\\..\\";
                string caption = "";
                int timework = 0;
                foreach (XmlNode servnode in node.ChildNodes)
                {
                    if (servnode.Name == "image")
                        filename = filename + servnode.InnerText;
                    if (servnode.Name == "caption")
                        caption = servnode.InnerText;
                    if (servnode.Name == "timework")
                        int.TryParse(servnode.InnerText, out timework);
                    //                    Debug.Print(attr.Value.ToString());
                }

                if (System.IO.File.Exists(filename))
                    {

                        Service serv = new Service();
                        serv.filename = filename;
                        serv.caption = caption;
                        serv.timework = timework;
                        services.Add(serv);
                    }
            }
            return 0;

        }
    }

}
