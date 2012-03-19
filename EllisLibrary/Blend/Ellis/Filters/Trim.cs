using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blend.Ellis.Filters
{
    class Trim : FilterBase
    {
        public override void Initialize()
        {

        }

        public override PagePart Execute(PagePart thisPart)
        {
            int lengthBeforeStripping = thisPart.Content.Length;
            string content = thisPart.Content;
            thisPart.Content = content.Trim();

            thisPart.AddMessage(this, StandardMessageTypes.Executed, String.Format("Length before: {0}; Length after: {1}", lengthBeforeStripping.ToString(), thisPart.Content.Length.ToString()));
            return thisPart;
        }
    }
}
