using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blend.Ellis.Reports
{
    public abstract class ReportBase
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public abstract void Close();

        public ReportBase(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string GetReportHeader()
        {
            string headerTemplate = @"Name:      {0}
Generated: {1}
Batch:     {2}
--------------------------------------------

";

            return String.Format(
                headerTemplate,
                Name,
                DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString(),
                Config.BatchNumber
                );

        }
    }
}
