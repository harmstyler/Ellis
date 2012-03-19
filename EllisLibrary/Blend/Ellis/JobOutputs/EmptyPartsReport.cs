using System;
using System.Collections.Generic;
using System.Linq;
using Blend.Ellis.Reports;

namespace Blend.Ellis.JobOutputs
{
    class EmptyPartsReport : JobOutputBase
    {
        public override void Execute(Job thisJob)
        {
            var badPages = new Dictionary<string, List<String>>();
            var partCount = new Dictionary<string, int>();

            foreach (Page page in thisJob.Pages.Where(p => p.HttpStatus == 200))
            {
                var contentLessParts = new List<string>();

                foreach (var part in page.Parts.Where(pp => !pp.HasContent))
                {
                    contentLessParts.Add(part.Name);

                    if (!partCount.ContainsKey(part.Name))
                    {
                        partCount.Add(part.Name, 1);
                    }
                    else
                    {
                        partCount[part.Name]++;
                    }
            }

                if (contentLessParts.Count > 0)
                {
                    badPages.Add(page.Url.AbsoluteUri, contentLessParts);
                }
            }

            var report = new PlainTextReport("Empty Parts", "reports/empty-parts.txt");

            report.Write(badPages.Count.ToString(), " page(s) with empty parts");
            
            foreach (var part in partCount.OrderByDescending(kvp => kvp.Value))
            {
                report.Write(part.Key, ": ", part.Value);
            }

            report.Write();
            
            foreach (var badPage in badPages)
            {
                report.Write("Page:  ", badPage.Key);
                report.Write("Parts: ", Utils.MakeString(badPage.Value, ", "));
                report.Write();
            }

            report.Close();
        }

    }
}
