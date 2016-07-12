using System;
using System.Xml.Linq;

namespace ServiceSaleMachine
{
    public class Device
    {
        public int id;
        public int limitTime;
        public string caption;
        public int timework;

        public Device(int id, string caption, int limitTime,int timework)
        {
            this.id = id;
            this.caption = caption;
            this.limitTime = limitTime;
            this.timework = timework;
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
            xOut.Add(new XElement("limitTime", limitTime.ToString()));
            xOut.Add(new XElement("timework", timework.ToString()));

            return xOut;
        }

        public static Device FromXml(XElement xObject)
        {
            if (xObject.Name != "Device") throw new ArgumentException(xObject.Name.LocalName);

            XElement xElement;

            Device result = new Device();

            if ((xElement = xObject.Element("id")) != null) result.id = int.Parse(xElement.Value);
            if ((xElement = xObject.Element("caption")) != null) result.caption = xElement.Value;
            if ((xElement = xObject.Element("limitTime")) != null) result.limitTime = int.Parse(xElement.Value);
            if ((xElement = xObject.Element("timework")) != null) result.timework = int.Parse(xElement.Value);

            return result;
        }
    }
}