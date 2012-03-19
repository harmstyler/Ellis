using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Blend.Ellis.Filters
{
    class SetAttribute : FilterBase
    {
        public override void Initialize()
        {
            EnsureSetting("pattern");
            EnsureSetting("attribute");
            EnsureSetting("value");
        }

        public override PagePart Execute(PagePart thisPart)
        {
            int counter = 0;

            HtmlNode thisContent = thisPart.ParsedContent;

            string pattern = Settings["pattern"];
            string attribute = Settings["attribute"];
            string value = Settings["value"];

            if (thisContent.SelectNodes(pattern) == null)
            {
                //No such pattern. Return the part with no changes.
                return thisPart;
            }

            foreach (HtmlNode thisNode in thisContent.SelectNodes(pattern))
            {
                counter++;

                if (thisNode.Attributes[attribute] != null)
                {
                    thisNode.Attributes[attribute].Value = value;
                }
                else
                {
                    thisNode.Attributes.Add(attribute, value);
                }
            }

            thisPart.ParsedContent = thisContent;

            thisPart.AddMessage(this, StandardMessageTypes.Executed, String.Format("Pattern: {0}; Attribute: {1}; Value: {2} Set attribute on (3) element(s)", pattern, attribute, value, counter.ToString()));
            return thisPart;

        }
    }
}
