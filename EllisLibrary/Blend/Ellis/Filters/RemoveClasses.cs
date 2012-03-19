using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Blend.Ellis.Filters
{
    class RemoveClasses : FilterBase
    {
        public override void Initialize()
        {
            EnsureSetting("tag");
        }

        public override PagePart Execute(PagePart thisPart)
        {
            int counter = 0;

            string pattern = Settings.Exists("tag") ? Settings["tag"] : "*";
            string className = Settings.Exists("class") ? Settings["class"] : "*";

            if (thisPart.ParsedContent.SelectNodes(pattern) != null)
            {

            foreach (HtmlNode matchingNode in thisPart.ParsedContent.SelectNodes(pattern))
            {
                if (matchingNode.Attributes["class"] == null)
                {
                    continue;
                }

                counter++;

                if (className == "*")
                {
                    matchingNode.Attributes["class"].Remove();
                    continue;
                }

 
                List<string> currentClasses = matchingNode.Attributes["class"].Value.Split(new Char[] { ' ' }).ToList<string>();
                currentClasses.Remove(className);

                //If we now have no classes, just remove the whole thing
                if (currentClasses.Count == 0)
                {
                    matchingNode.Attributes["class"].Remove();
                }
                else
                {
                    //Otherwise, set it back to what's left
                    matchingNode.Attributes["class"].Value = String.Join(" ", currentClasses.ToArray());
                }

            }
            }

            thisPart.AddMessage(this, StandardMessageTypes.Executed, String.Format(" Pattern: {0}; Class: {1}; Removed classes from {2} tag(s).", pattern, className, counter.ToString()));
            return thisPart;

        }
    }
}
