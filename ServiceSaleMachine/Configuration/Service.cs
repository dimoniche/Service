using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ServiceSaleMachine
{
    public class Service
    {
        public int id;
        public string caption { get; set; }
        public string filename { get; set; }
        public int price;

        public List<Device> devs;

        public Service()
        {

        }

        public Service(int id, string caption, string filename)
        {
            this.id = id;
            this.caption = caption;
            this.filename = filename;
        }

        public XElement ToXml()
        {
            XElement xOut = null;
            xOut = new XElement("Service");

            xOut.Add(new XElement("id", id.ToString()));
            xOut.Add(new XElement("price", price.ToString()));
            xOut.Add(new XElement("caption", caption));
            xOut.Add(new XElement("filename", filename));

            if (devs.Count > 0)
            {
                XElement devices = new XElement("Devices");
                foreach (Device dev in devs)
                {
                    XElement newservice = dev.ToXml();
                    devices.Add(newservice);
                }
                xOut.Add(devices);
            }

            return xOut;
        }

        public static Service FromXml(XElement xObject)
        {
            if (xObject.Name != "Service") throw new ArgumentException(xObject.Name.LocalName);

            XElement xElement;

            Service result = new Service();

            if ((xElement = xObject.Element("id")) != null) result.id = int.Parse(xElement.Value);
            if ((xElement = xObject.Element("caption")) != null) result.caption = xElement.Value;
            if ((xElement = xObject.Element("filename")) != null) result.filename = xElement.Value;
            if ((xElement = xObject.Element("price")) != null) result.price = int.Parse(xElement.Value);

            // настройки сервисов
            if ((xElement = xObject.Element("Devices")) != null)
            {
                result.devs = new List<Device>();
                int i = 1;
                foreach (XElement xItem in xElement.Elements("Device"))
                {
                    Device tmp = Device.FromXml(xItem);
                    if (tmp.id == 0)
                    {
                        tmp.id = i;
                    }
                    result.devs.Add(tmp);
                    i++;
                }
            }

            return result;
        }

        public Device GetActualDevice()
        {
            Device dev = null;
            DateTime dt;
            int count = 0;

            for (int i = 1; i < devs.Count + 1; i++)
            {
                dt = GlobalDb.GlobalBase.GetLastRefreshTime(id, i);
                if (dt != null)
                {
                    count = GlobalDb.GlobalBase.GetWorkTime(id, i, dt);

                    if(count < devs[i-1].limitTime)
                    {
                        return devs[i - 1];
                    }
                }
                else
                {
                    count = GlobalDb.GlobalBase.GetWorkTime(id, i, new DateTime(2000,1,1));
                    if(count != 0)
                    {
                        return devs[i - 1];
                    }
                }
            }

            return dev;
        }
    }
}
