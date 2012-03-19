using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blend.Ellis.Reports
{
    public class PlainTextReport : ReportBase
    {
        private List<string> output = new List<string>();

        public PlainTextReport(string name, string path) : base(name, path)
        {
        
        }

        public void Write(params object[] input)
        {
            output.Add(Utils.MakeString(input));
        }

        public override void Close()
        {
            ArtifactManager.SaveFileData(GetReportHeader() + String.Join(Environment.NewLine, output.ToArray<string>()), ArtifactManager.GetArtifactPath(Path));
        }


    }
}
