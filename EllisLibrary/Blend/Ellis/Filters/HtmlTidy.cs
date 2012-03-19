using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mark.Tidy;

namespace Blend.Ellis.Filters
{
    class HtmlTidy : FilterBase
    {
        public override void Initialize()
        {

        }

        public override PagePart Execute(PagePart thisPart)
        {
            using (Document doc = new Document(thisPart.Content))
            {
                doc.ShowWarnings = false;
                doc.Quiet = true;
                doc.IndentBlockElements = AutoBool.Yes;
                doc.OutputXhtml = true;
                doc.OutputBodyOnly = AutoBool.Yes;
                doc.WrapAt = 10000;
                doc.CleanAndRepair();
                string parsed = doc.Save();


                thisPart.Content = parsed;

                thisPart.AddMessage(this, StandardMessageTypes.Executed, "Processed by HTMLTidy");
            }

            return thisPart;

        }
    }
}
