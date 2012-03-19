using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Blend.Ellis.Filters
{
    class StripComments : FilterBase
    {
        public override void Initialize()
        {

        }

        public override PagePart Execute(PagePart thisPart)
        {
            int lengthBeforeStripping = thisPart.Content.Length;
            string content = thisPart.Content;

            thisPart.Content = Regex.Replace(content, @"<!--.*?-->", String.Empty, RegexOptions.Singleline);

            thisPart.AddMessage(this, StandardMessageTypes.Executed, String.Format("Length before: {0}; Length after: {1}; Comment characters removed: {2}", lengthBeforeStripping.ToString(), thisPart.Content.Length.ToString(), (lengthBeforeStripping - thisPart.Content.Length).ToString()));
            return thisPart;
        }
    }
}
