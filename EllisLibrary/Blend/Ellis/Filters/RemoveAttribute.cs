using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Blend.Ellis.Filters
{
    class RemoveAttribute : FilterBase
    {
        public override void Initialize()
        {
            EnsureSetting("pattern");
            EnsureSetting("attribute");
        }

        public override PagePart Execute(PagePart thisPart)
        {
            int counter = 0;

            string pattern = Settings["pattern"];
            string attribute = Settings["attribute"];

            string value = null;
            if (Settings.ContainsKey("value"))
            {
                value = Settings["value"];
            }

            if (thisPart.ParsedContent.SelectNodes(pattern) != null)
            {
                foreach (HtmlNode thisNode in thisPart.ParsedContent.SelectNodes(pattern))
                {
                    if (thisNode.Attributes[attribute] != null)
                    {
                        //If we have a value, and this attribute doesn't match that value, skip it
                        if (value != null && value.ToLowerInvariant() != thisNode.Attributes[attribute].Value.ToLowerInvariant())
                        {
                            continue;
                        }

                        counter++;
                        thisNode.Attributes[attribute].Remove();
                    }
                }
            }

            thisPart.AddMessage(this, StandardMessageTypes.Executed, String.Format(" Pattern: {0}; Attribute: {1}; Removed {2} attribute(s).", pattern, attribute, counter.ToString()));
            return thisPart;

        }
    }
}
