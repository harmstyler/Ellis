using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Blend.Ellis.Filters
{
    class RemoveDomainFromPaths : FilterBase
    {
        public override void Initialize()
        {
            EnsureSetting("removeDomains");
        }

        public override PagePart Execute(PagePart thisPart)
        {
            int counter = 0;

            List<string> domainsToRemove = Settings["removeDomains"].Split(new Char[] {','}).ToList<string>();

            if (thisPart.ParsedContent.SelectNodes(".//a") == null)
            {
                return thisPart;
            }

            foreach (HtmlNode link in thisPart.ParsedContent.SelectNodes(".//a"))
            {
                if (link.Attributes["href"] == null)
                {
                    continue;
                }

                foreach (string domainToRemove in domainsToRemove)
                {
                    if (link.Attributes["href"].Value.Contains(domainToRemove))
                    {
                        counter++;
                        link.Attributes["href"].Value = link.Attributes["href"].Value.Replace(String.Concat("http://", domainToRemove), "");
                        link.Attributes["href"].Value = link.Attributes["href"].Value.Replace(String.Concat("https://", domainToRemove), "");
                    }
                }
            }

            thisPart.AddMessage(this, StandardMessageTypes.Executed, String.Format("Removed {0} domain(s)", counter.ToString()));

            return thisPart;


        }
    }
}
