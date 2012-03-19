using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Blend.Ellis.Reports;

namespace Blend.Ellis.JobOutputs
{
    class FailedExtractionReport : JobOutputBase
    {
        public override void Execute(Job thisJob)
        {
            var pages = new List<string>();
            foreach (Page thisPage in thisJob.Pages)
            {
                if (thisPage.Parts.Count == 0)
                {
                    pages.Add(thisPage.Url.AbsoluteUri);
                }
            }

            var report = new PlainTextReport("Failed Extractions", "reports/failed-extractions.txt");
            report.Write(pages.OrderBy(s => s).ToList());
            report.Close();
        }
    }
}
