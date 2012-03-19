using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Blend.Ellis.Filters
{
    /// <summary>
    /// Swaps an old tag for a new tag
    /// </summary>
    public class SwapTags : FilterBase
    {
        public override void Initialize()
        {
            EnsureSetting("old");
            EnsureSetting("new");
        }

        public override PagePart Execute(PagePart thisPart)
        {
            int counter = 0;

            string oldTag = Settings["old"].TrimStart('/');
            string newTag = Settings["new"].TrimStart('/');

            

            if (thisPart.ParsedContent.SelectNodes("//" + oldTag) == null)
            {
                return thisPart;
            }

            foreach (HtmlNode matchingNode in thisPart.ParsedContent.SelectNodes("//" + oldTag))
            {
                counter++;
                matchingNode.Name = newTag;
            }

            thisPart.AddMessage(this, StandardMessageTypes.Executed, String.Format("Old tag: {0}; New tag: {1}; Swapped {2} tag(s).", oldTag, newTag, counter.ToString()));
            return thisPart;
        }
    }
}
