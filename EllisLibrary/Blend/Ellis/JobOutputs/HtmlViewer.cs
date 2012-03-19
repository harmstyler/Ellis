using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Web.UI;
using Blend.Ellis.PageOutputs;

namespace Blend.Ellis.JobOutputs
{
    class HtmlViewer : JobOutputBase
    {
        public HtmlViewer()
        {
            Settings.Add("indexFileName", "viewer.html");
            Settings.Add("dataDirectoryName", "viewer-data");
        }
        
        public override void Execute(Job thisJob)
        {
            string style = @"
                body { font-family: arial; font-size: 12px; }
                textarea { width: 100%; font-family: Courier New; font-size: 15px; height: 19px; padding: 2px; }
                div.syntaxhighlighter { padding-bottom: 10px; }
                ol { padding-left: 2.5em; }
                li { margin-bottom: 2px; }
                dt { font-weight: bold; }
                h3,h4 { padding: 5px; margin-top: 30px; background: rgb(220,220,220) }
                h3.first { margin-top: 0px; }
		table.messageTable { margin-left: 20px; width: 100%; border-collapse: collapse; border-top: solid 1px rgb(220,220,220);}
table.messageTable th, table.messageTable td { border-bottom: solid 1px rgb(220,220,220); text-align: left; padding: 4px; }
table.messageTable th { background-color: rgb(240,240,240);
            ";

            //Render the frameset
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.RenderBeginTag("html");

                writer.AddAttribute("cols", "400,*");
                writer.RenderBeginTag("frameset");

                writer.AddAttribute("src", "viewer-data/_menu.html");
                writer.RenderBeginTag("frame");
                writer.RenderEndTag();

                writer.AddAttribute("name", "viewer");
                writer.AddAttribute("src", "viewer-data/_empty.html");
                writer.RenderBeginTag("frame");
                writer.RenderEndTag();

                writer.RenderEndTag();
                writer.RenderEndTag();
            }
            ArtifactManager.SaveFileData(stringWriter.ToString(), Settings["indexFileName"]);

            //Render the empty file
            ArtifactManager.SaveFileData("", "viewer-data/_empty.html");
            
            //Render the menu page
            StringWriter stringWriter2 = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter2))
            {
                writer.RenderBeginTag("html");

                writer.RenderBeginTag("head");
                writer.AddAttribute("type", "text/css");
                writer.RenderBeginTag("style");
                writer.Write(style);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag("body");

                writer.RenderBeginTag("ol");
                foreach (Page page in thisJob.Pages)
                {
                    writer.RenderBeginTag("li");

                    writer.AddAttribute("href", String.Concat(page.Id, ".html"));
                    writer.AddAttribute("target", "viewer");
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write(page.Name);
                    writer.RenderEndTag();

                    writer.RenderEndTag();
                }
                writer.RenderEndTag();

                writer.RenderEndTag();
                writer.RenderEndTag();
            }
            ArtifactManager.SaveFileData(stringWriter2.ToString(), "viewer-data/_menu.html");

            //Render the pages
            HtmlViewerPage viewerPage = new HtmlViewerPage();
            viewerPage.Settings.Add("Path", "viewer-data/");
            foreach (Page page in thisJob.Pages)
            {
                viewerPage.Execute(page);
            }

        }
    }
}
