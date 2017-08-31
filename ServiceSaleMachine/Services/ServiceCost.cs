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
        public List<Price> priceCash;

        /// <summary>
        /// Массив цен с платой с аккаунта
        /// </summary>
        public List<Price> priceAccount;

        public ServiceCost()
        {
            priceCash = new List<Price>();
            priceAccount = new List<Price>();
        }

        /// <summary>
        /// Вернуть цену услуги наличными
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int getPriceCashByAmount(int amount)
        {
            int price = 0;
            foreach (Price Price in priceCash)
            {
                if (Price.amount == amount)
                {
                    price = Price.price;
                    break;
                }
            }

            return price;
        }

        /// <summary>
        /// Вернуть цену услуги с аккаунта
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int getPriceAccountByAmount( int amount)
        {
            int price = 0;
            foreach (Price Price in priceAccount)
            {
                if (Price.amount == amount)
                {
                    price = Price.price;
                    break;
                }
            }

            return price;
        }

        public XElement ToXml()
        {
            XElement xOut = null;
            xOut = new XElement("ServiceCost");

            xOut.Add(new XElement("numberService", numberService));
            xOut.Add(new XElement("rangeStart", rangeStart));
            xOut.Add(new XElement("rangeStop", rangeStop));
            xOut.Add(new XElement("step", step));

            XElement Element = null;
            Element = new XElement("priceCash");

            foreach (Price price in priceCash)
            {
                XElement xprice = price.ToXml();
                Element.Add(xprice);
            }

            xOut.Add(Element);

            Element = new XElement("priceAccount");

            foreach (Price price in priceCash)
            {
                XElement xprice = price.ToXml();
                Element.Add(xprice);
            }

            xOut.Add(Element);

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
                cost.priceCash = new List<Price>();

                foreach (XElement xItem in xElement.Elements("priceItem"))
                {
                    cost.priceCash.Add(Price.FromXml(xItem));
                }
            }

            if ((xElement = xObject.Element("priceAccount")) != null)
            {
                cost.priceAccount = new List<Price>();

                foreach (XElement xItem in xElement.Elements("priceItem"))
                {
                    cost.priceAccount.Add(Price.FromXml(xItem));
                }
            }

            return cost;
        }
    }
}