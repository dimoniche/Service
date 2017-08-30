using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AirVitamin
{
    /// <summary>
    /// Таблица переменной стоимости для услуг
    /// </summary>
    public class CostTable
    {
        public List<ServiceCost> Tables;

        public CostTable()
        {
            Tables = new List<ServiceCost>();
        }

        public XElement ToXml()
        {
            XElement xOut = null;
            xOut = new XElement("Tables");

            if (Tables.Count > 0)
            {
                foreach (ServiceCost table in Tables)
                {
                    XElement newcost = table.ToXml();
                    xOut.Add(newcost);
                }
            }

            return xOut;
        }

        public static CostTable FromXml(XElement xObject)
        {
            CostTable table = new CostTable();
            XElement xElement;

            if ((xElement = xObject.Element("Tables")) != null)
            {
                foreach (XElement xItem in xElement.Elements("ServiceCost"))
                {
                    ServiceCost cost = ServiceCost.FromXml(xItem);
                    table.Tables.Add(cost);
                }
            }

            return table;
        }
    }
}
