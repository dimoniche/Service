using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace AirVitamin
{
    public class Service
    {
        public int id;
        public string caption { get; set; }

        /// <summary>
        /// Здесь цена в зависимости от куда берем деньги
        /// </summary>
        public int price;

        public int priceAccount;
        public int priceCash;

        /// <summary>
        /// Набор цен услуг
        /// </summary>
        public ServiceCost cost;

        public List<Device> devs;

        // задержка перед оказанием услуги
        public int timeBefore = 10;
        // время оказания услуги
        public int timework = 10;
        // пауза между процедурами
        public int timePause = 20;

        public Service()
        {

        }

        public Service(int id, string caption)
        {
            this.id = id;
            this.caption = caption;
        }

        public XElement ToXml()
        {
            XElement xOut = null;
            xOut = new XElement("Service");

            xOut.Add(new XElement("id", id.ToString()));
            //xOut.Add(new XElement("price", price.ToString()));
            xOut.Add(new XElement("priceAccount", priceAccount.ToString()));
            xOut.Add(new XElement("priceCash", priceCash.ToString()));
            xOut.Add(new XElement("caption", caption));
            xOut.Add(new XElement("timeBefore", timeBefore));
            xOut.Add(new XElement("timework", timework));
            xOut.Add(new XElement("timePause", timePause));

            //if (devs.Count > 0)
            //{
            //    XElement devices = new XElement("Devices");
            //    foreach (Device dev in devs)
            //    {
            //        XElement newservice = dev.ToXml();
            //        devices.Add(newservice);
            //    }
            //    xOut.Add(devices);
            //}

            return xOut;
        }

        public static Service FromXml(XElement xObject)
        {
            if (xObject.Name != "Service") throw new ArgumentException(xObject.Name.LocalName);

            XElement xElement;

            Service result = new Service();

            if ((xElement = xObject.Element("id")) != null) result.id = int.Parse(xElement.Value);
            if ((xElement = xObject.Element("caption")) != null) result.caption = xElement.Value;
            //if ((xElement = xObject.Element("price")) != null) result.price = int.Parse(xElement.Value);
            if ((xElement = xObject.Element("priceCash")) != null) result.priceCash = int.Parse(xElement.Value);
            if ((xElement = xObject.Element("priceAccount")) != null) result.priceAccount = int.Parse(xElement.Value);
            if ((xElement = xObject.Element("timeBefore")) != null) result.timeBefore = int.Parse(xElement.Value);
            if ((xElement = xObject.Element("timework")) != null) result.timework = int.Parse(xElement.Value);
            if ((xElement = xObject.Element("timePause")) != null) result.timePause = int.Parse(xElement.Value);

            // настройки сервисов
            //if ((xElement = xObject.Element("Devices")) != null)
            //{
            //    result.devs = new List<Device>();
            //    int i = 1;
            //    foreach (XElement xItem in xElement.Elements("Device"))
            //    {
            //        Device tmp = Device.FromXml(xItem);
            //        if (tmp.id == 0)
            //        {
            //            tmp.id = i;
            //        }
            //        result.devs.Add(tmp);
            //        i++;
            //    }
            //}

            return result;
        }
    }
}
