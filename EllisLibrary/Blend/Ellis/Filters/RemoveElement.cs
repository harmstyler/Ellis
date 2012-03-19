using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Web;

namespace Blend.Ellis.Filters
{
    class RemoveElement : FilterBase
    {
        public override void Initialize()
        {
            EnsureSetting("pattern");
        }

        public override PagePart Execute(PagePart thisPart)
        {
            int counter = 0;

            string pattern = Settings["pattern"];

            pattern = pattern.Replace("&", "&amp;");

            bool preserveContents = Settings.Exists("preserveContents") ? Convert.ToBoolean(Settings["preserveContents"]) : false;


            if (thisPart.ParsedContent.SelectNodes(pattern) != null)
            {

                foreach (HtmlNode nodeToRemove in thisPart.ParsedContent.SelectNodes(pattern))
                {
                    counter++;
                    nodeToRemove.ParentNode.RemoveChild(nodeToRemove, preserveContents);
                }
            }

            thisPart.AddMessage(this, StandardMessageTypes.Executed, String.Format("Pattern: {0}; Removed {1} element(s).", pattern, counter.ToString()));
            return thisPart;
        }
    }
}
