using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blend.Ellis.Filters;
using System.Xml;

namespace Blend.Ellis
{
    public class FilterChain
    {
        public string Name { get; set; }
        public List<FilterBase> Filters { get; set; }

        public FilterChain(XmlNode chainNode)
        {
            Name = chainNode.Attributes["name"].Value;
            Filters = new List<FilterBase>();

            foreach(XmlNode filterNode in chainNode.SelectNodes("*"))
            {
                Filters.Add(FilterBase.GetFilter(filterNode, this));
            }
        }

        public PagePart Execute(PagePart thisPagePart)
        {
            foreach (FilterBase thisFilter in Filters)
            {
                thisPagePart = thisFilter.Execute(thisPagePart);
            }

            return thisPagePart;
        }

    }
}
