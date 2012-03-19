using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blend.Ellis.Filters
{
    class Replace : FilterBase
    {
        public override void Initialize()
        {
            //They have to have an oldString.  If they don't provide a newString, it's assumed to be empty (so, "remove" rather than "replace", really)
            EnsureSetting("oldString");
        }

        public override PagePart Execute(PagePart thisPart)
        {
            string content = thisPart.Content;
            int lengthBefore = content.Length;

            string newString = Settings.Exists("newString") ? Settings["newString"] : String.Empty;
            string oldString = Settings["oldString"];

            thisPart.Content = content.Replace(oldString, newString);

            thisPart.AddMessage(this, StandardMessageTypes.Executed, String.Format("Length before: {0}; Length after: {1}", lengthBefore.ToString(), thisPart.Content.Length.ToString()));
            return thisPart;
        }
    }
}
