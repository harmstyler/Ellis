using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Blend.Ellis.Filters
{
    /// <summary>
    /// Remove all the HTML tags from the content
    /// </summary>
    public class StripTags : FilterBase
    {
        public override void Initialize()
        {
        }

        public override PagePart Execute(PagePart thisPart)
        {
            thisPart.Content = thisPart.ParsedContent.InnerText;

            return thisPart;
        }
    }
}
