using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace AirVitamin
{
    public class ServiceCost
    {
        public int numberService;

        /// <summary>
        /// старт времени услуги
        /// </summary>
        public int rangeStart;

        /// <summary>
        /// Окончание времени услуги
        /// </summary>
        public int rangeStop;

        /// <summary>
        /// шаг услуги
        /// </summary>
        public int step;

        /// <summary>
        /// Массив цен за наличные
        /// </summary>
        public List<int> priceCash;

        /// <summary>
        /// Массив цен с платой с аккаунта
        /// </summary>
        public List<int> priceAccount;

        public ServiceCost()
        {
            priceCash = new List<int>();
            priceAccount = new List<int>();
        }

        public XElement ToXml()
        {
            XElement xOut = null;
            xOut = new XElement("ServiceCost");

            xOut.Add(new XElement("numberService", numberService));
            xOut.Add(new XElement("rangeStart", rangeStart));
            xOut.Add(new XElement("rangeStop", rangeStop));
            xOut.Add(new XElement("step", step));

            XElement xpriceCash = new XElement("priceCash");

            foreach (int price in priceCash)
            {
                xpriceCash.Add(new XElement("price", price));
            }

            xOut.Add(xpriceCash);

            XElement xpriceAccount = new XElement("priceAccount");

            foreach (int price in priceCash)
            {
                xpriceAccount.Add(new XElement("price", price));
            }

            xOut.Add(xpriceAccount);

            return xOut;
        }

        public static ServiceCost FromXml(XElement xObject)
        {
            if (xObject.Name != "ServiceCost") throw new ArgumentException(xObject.Name.LocalName);

            XElement xElement;
            ServiceCost cost = new ServiceCost();

            if ((xElement = xObject.Element("numberService")) != null) cost.numberService = int.Parse(xElement.Value);
            if ((xElement = xObject.Element("rangeStart")) != null) cost.rangeStart = int.Parse(xElement.Value);
            if ((xElement = xObject.Element("rangeStop")) != null) cost.rangeStop = int.Parse(xElement.Value);
            if ((xElement = xObject.Element("step")) != null) cost.step = int.Parse(xElement.Value);

            if ((xElement = xObject.Element("priceCash")) != null)
            {
                cost.priceCash = new List<int>();

                foreach (XElement xItem in xElement.Elements("price"))
                {
                    cost.priceCash.Add(int.Parse(xItem.Value));
                }
            }

            if ((xElement = xObject.Element("priceAccount")) != null)
            {
                cost.priceAccount = new List<int>();

                foreach (XElement xItem in xElement.Elements("price"))
                {
                    cost.priceAccount.Add(int.Parse(xItem.Value));
                }
            }

            return cost;
        }
    }
}