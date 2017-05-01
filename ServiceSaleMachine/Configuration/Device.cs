using System;
using System.Xml.Linq;

namespace AirVitamin
{
    public class Device
    {
        public int id;

        public string caption = "";

        public Device(int id, string caption)
        {
            this.id = id;
            this.caption = caption;
        }

        public Device()
        {
        }

        public XElement ToXml()
        {
            XElement xOut = null;
            xOut = new XElement("Device");

            xOut.Add(new XElement("id", id.ToString()));
            xOut.Add(new XElement("caption", caption));

            return xOut;
        }

        public static Device FromXml(XElement xObject)
        {
            if (xObject.Name != "Device") throw new ArgumentException(xObject.Name.LocalName);

            XElement xElement;

            Device result = new Device();

            if ((xElement = xObject.Element("id")) != null) result.id = int.Parse(xElement.Value);
            if ((xElement = xObject.Element("caption")) != null) result.caption = xElement.Value;

            return result;
        }
    }
}