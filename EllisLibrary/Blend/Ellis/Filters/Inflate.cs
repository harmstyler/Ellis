using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blend.Ellis.Filters;

namespace Blend.Ellis.Filters
{
    class Inflate : FilterBase
    {
        public override void Initialize()
        {
            EnsureSetting("pattern");
        }

        public override PagePart Execute(PagePart thisPart)
        {
            string pattern = Settings["pattern"];
            string result = String.Empty;

            if (thisPart.ParsedContent.SelectSingleNode(pattern) != null)
            {
                thisPart.Content = thisPart.ParsedContent.SelectSingleNode(pattern).InnerHtml;
                result = "Found and inflated";
            }
            else
            {
                result = "Not found";
            }

            thisPart.AddMessage(this, StandardMessageTypes.Executed, String.Format(" Pattern: {0}; Result: {1}", pattern, result ));
            return thisPart;
        }
    }
}
