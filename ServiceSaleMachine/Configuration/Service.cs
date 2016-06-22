using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ServiceSaleMachine
{
    public class Service
    {
        public string caption { get; set; }
        public string filename { get; set; }
        public int timework { get; set; }

        public Service()
        {

        }

        public Service(string caption, string filename, int timework)
        {
            this.caption = caption;
            this.filename = filename;
            this.timework = timework;
        }

        public XElement ToXml()
        {
            XElement xOut = null;
            xOut = new XElement("Service");

            xOut.Add(new XElement("caption", caption));
            xOut.Add(new XElement("filename", filename));
            xOut.Add(new XElement("timework", timework.ToString()));

            return xOut;
        }

        public static Service FromXml(XElement xObject)
        {
            if (xObject.Name != "Service") throw new ArgumentException(xObject.Name.LocalName);

            XElement xElement;
            XAttribute xAttribute;

            Service result = new Service();

            if ((xElement = xObject.Element("caption")) != null) result.caption = xElement.Value;
            if ((xElement = xObject.Element("filename")) != null) result.filename = xElement.Value;
            if ((xAttribute = xObject.Attribute("timework")) != null) result.timework = int.Parse(xAttribute.Value);

            return result;
        }
    }
}
