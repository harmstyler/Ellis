using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Blend.Ellis.JobOutputs
{
    class DownloadedAssetReport : JobOutputBase
    {
        public override void Execute(Job thisJob)
        {
            List<string> files = new List<string>();
            foreach (Page thisPage in thisJob.Pages)
            {
                foreach (PagePart thisPart in thisPage.Parts)
                {
                    foreach (DictionaryEntry metaValue in thisPart.Meta)
                    {
                        if (metaValue.Key.ToString().StartsWith("Downloaded"))
                        {
                            files.Add(metaValue.Value.ToString());
                        }
                    }
                }
            }

            if (files.Count == 0)
            {
                return;
            }


            //Save the file
            ArtifactManager.SaveFileData(files.Distinct().OrderBy(s => s).ToList(), "reports/downloaded-assets.txt");
        }

    }
}
