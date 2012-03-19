using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blend.Ellis.Filters
{
    internal class RemoveAfterLast : FilterBase
    {
        public override void Initialize()
        {
            EnsureSetting("character");
        }

        public override PagePart Execute(PagePart thisPart)
        {
            if (!thisPart.Content.Contains(Settings["character"]))
            {
                thisPart.AddMessage(this, StandardMessageTypes.Notice, String.Format("Character \"{0}\" not found.", Settings["character"]));
            }

            int lengthBefore = thisPart.Content.Length;

            var parts = thisPart.Content.Split(Settings["character"].ToCharArray());

            thisPart.Content = String.Join(String.Empty, parts.Take(parts.Length - 1).ToArray());

            thisPart.AddMessage(this, StandardMessageTypes.Executed, String.Format("Length before: {0}; Length after: {1}", lengthBefore.ToString(), thisPart.Content.Length.ToString()));

            return thisPart;
        }
    }
}
