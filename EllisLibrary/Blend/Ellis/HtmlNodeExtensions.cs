using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Blend.Ellis
{
    public static class HtmlNodeExtensions
    {
        public static HtmlNodeCollection SafeSelectNodes(this HtmlNode node, string xPath)
        {
            return node.SelectNodes(xPath) ?? new HtmlNodeCollection(node);
        }
    }
}
