using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Blend.Ellis.Filters
{
    class ConvertRelativeUrls : FilterBase
    {
        public override void Initialize()
        {

        }

        public override PagePart Execute(PagePart thisPart)
        {
            int counter = 0;

            string pageUrlDirectory = String.Join("", thisPart.Page.Url.Segments.Take(thisPart.Page.Url.Segments.Count() - 1).ToArray());

            HtmlNode thisContent = thisPart.ParsedContent;

            if (thisContent.SelectNodes(".//a") == null)
            {
                //No links, return the part with no changes
                return thisPart;
            }


            foreach (HtmlNode link in thisContent.SelectNodes(".//a"))
            {
                //If it doesn't have an HREF, skip it...
                if (link.Attributes["href"] == null)
                {
                    continue;
                }

                string linkUrl = link.Attributes["href"].Value;

                //If it's already absolute, skip it
                if (linkUrl.StartsWith("/") || linkUrl.StartsWith("http"))
                {
                    continue;
                }

                //If it's a bookmark, skip it
                if (linkUrl.StartsWith("#"))
                {
                    continue;
                }

                //If it's javascript, skip it
                if (linkUrl.StartsWith("javascript"))
                {
                    continue;
                }

                //If it's a mailto, skip it
                if (linkUrl.StartsWith("mailto"))
                {
                    continue;
                }

                counter++;

                //If we get here, we have a relative link; this is a problem

                //Add the path to the current file to it
                linkUrl = String.Concat(pageUrlDirectory, linkUrl);

                //If it has parent paths, remove them
                if (linkUrl.Contains("/../"))
                {
                    //This is the worst hack ever written...ever.  I'm so sorry for this.  Don't tell my mother about it.
                    linkUrl = "/" + System.IO.Path.GetFullPath(linkUrl).Replace(System.IO.Path.GetFullPath(@"\"), "").Replace(@"\", "/");
                }

                //Replace the link
                link.Attributes["href"].Value = linkUrl;
            }

            thisPart.ParsedContent = thisContent;
            thisPart.AddMessage(this, StandardMessageTypes.Executed, String.Format("Corrected {0} link(s)", counter.ToString()));
            return thisPart;

        }
    }
}
