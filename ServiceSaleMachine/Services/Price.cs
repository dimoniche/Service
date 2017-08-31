using System;
using System.Xml.Linq;

namespace AirVitamin
{
    public class Price
    {
        public int price;
        public int amount;

        public XElement ToXml()
        {
            XElement xOut = null;
            xOut = new XElement("priceItem");

            xOut.Add(new XElement("price", price));
            xOut.Add(new XElement("amount", amount));

            return xOut;
        }

        public static Price FromXml(XElement xObject)
        {
            if (xObject.Name != "priceItem") throw new ArgumentException(xObject.Name.LocalName);

            XElement xElement;
            Price price = new Price();

            if ((xElement = xObject.Element("price")) != null) price.price = int.Parse(xElement.Value);
            if ((xElement = xObject.Element("amount")) != null) price.amount = int.Parse(xElement.Value);

            return price;
        }
    }
}