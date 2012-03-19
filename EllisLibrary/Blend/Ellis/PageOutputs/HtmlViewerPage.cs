using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Web.UI;

namespace Blend.Ellis.PageOutputs
{
    public class HtmlViewerPage : IPageOutput
    {
        public Hashtable Settings { get; set; }

        public void Execute(Page page)
        {
            ArtifactManager.SaveFileData(GetContent(page), String.Concat((string)Settings["Path"], page.Id, ".html"));
        }

        public HtmlViewerPage()
        {
            Settings = new Hashtable();
        }

        public string GetContent(Page page)
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

            StringWriter stringWriter3 = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter3))
            {
                writer.Write("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
                writer.RenderBeginTag("html");

                writer.RenderBeginTag("head");

                writer.AddAttribute("name", "Content-Type");
                writer.AddAttribute("content", "text/html;charset=" + Config.Encoding.BodyName);
                writer.RenderBeginTag("meta");
                writer.RenderEndTag();

                writer.AddAttribute("type", "text/css");
                writer.AddAttribute("rel", "stylesheet");
                writer.AddAttribute("href", "http://alexgorbatchev.com/pub/sh/current/styles/shCore.css");
                writer.RenderBeginTag("link");
                writer.RenderEndTag();

                writer.AddAttribute("type", "text/css");
                writer.AddAttribute("rel", "stylesheet");
                writer.AddAttribute("href", "http://alexgorbatchev.com/pub/sh/current/styles/shThemeDefault.css");
                writer.RenderBeginTag("link");
                writer.RenderEndTag();

                writer.AddAttribute("type", "text/css");
                writer.RenderBeginTag("style");
                writer.Write(style);
                writer.RenderEndTag();

                writer.RenderEndTag();

                writer.RenderBeginTag("body");

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "first");
                writer.RenderBeginTag(HtmlTextWriterTag.H3);
                writer.Write("Original URL");
                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Href, page.Url.OriginalString);
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(page.Url.OriginalString);
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.H3);
                writer.Write("Retrieved");
                writer.RenderEndTag();

                writer.Write(page.Retrieved.ToString("s"));

                if (page.Meta.Count > 0)
                {

                    writer.RenderBeginTag(HtmlTextWriterTag.H3);
                    writer.Write("Meta: Page");
                    writer.RenderEndTag();


                    foreach (DictionaryEntry meta in page.Meta)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Dl);

                        writer.RenderBeginTag(HtmlTextWriterTag.Dt);
                        writer.Write(meta.Key.ToString());
                        writer.RenderEndTag();

                        writer.RenderBeginTag(HtmlTextWriterTag.Dd);
                        writer.Write(meta.Value.ToString());
                        writer.RenderEndTag();

                        writer.RenderEndTag();
                    }

                }

                if (page.Messages.Count > 0)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.H3);
                    writer.Write("Messages: Page");
                    writer.RenderEndTag();

                    writer.AddAttribute("class", "messageTable");
                    writer.RenderBeginTag(HtmlTextWriterTag.Table);
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                    writer.AddAttribute("class", "messageTable-Sender");
                    writer.RenderBeginTag(HtmlTextWriterTag.Th);
                    writer.Write("Sender");
                    writer.RenderEndTag();

                    writer.AddAttribute("class", "messageTable-Type");
                    writer.RenderBeginTag(HtmlTextWriterTag.Th);
                    writer.Write("Type");
                    writer.RenderEndTag();

                    writer.AddAttribute("class", "messageTable-Sender");
                    writer.RenderBeginTag(HtmlTextWriterTag.Th);
                    writer.Write("Text");
                    writer.RenderEndTag();

                    writer.RenderEndTag();

                    foreach (Message message in page.Messages)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
                        writer.Write(message.Sender);
                        writer.RenderEndTag();

                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
                        writer.Write(message.Type);
                        writer.RenderEndTag();

                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
                        writer.Write(message.Text);
                        writer.RenderEndTag();

                        writer.RenderEndTag();
                    }

                    writer.RenderEndTag();

                }




                foreach (PagePart part in page.Parts)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.H3);
                    writer.Write(String.Concat("Part: ", part.Key));
                    writer.RenderEndTag();

                    if (part.HasMarkup)
                    {
                        writer.AddAttribute("type", "syntaxhighlighter");
                        writer.AddAttribute("class", "brush: html;");
                        writer.RenderBeginTag("script");
                        writer.Write("<![CDATA[");
                        writer.Write(part.Content);
                        writer.Write("]]></script>");
                        writer.RenderEndTag();
                    }
                    else
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Textarea);
                        writer.Write(part.Content);
                        writer.RenderEndTag();
                    }

                    if (part.Meta.Count > 0)
                    {

                        writer.RenderBeginTag(HtmlTextWriterTag.H4);
                        writer.Write(String.Concat("Meta: Part: ", part.Key));
                        writer.RenderEndTag();

                        foreach (DictionaryEntry meta in part.Meta)
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.Dl);

                            writer.RenderBeginTag(HtmlTextWriterTag.Dt);
                            writer.Write(meta.Key.ToString());
                            writer.RenderEndTag();

                            writer.RenderBeginTag(HtmlTextWriterTag.Dd);
                            writer.Write(meta.Value.ToString());
                            writer.RenderEndTag();

                            writer.RenderEndTag();
                        }
                    }

                    if (part.Messages.Count > 0)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.H4);
                        writer.Write(String.Concat("Messages: Part: ", part.Key));
                        writer.RenderEndTag();

                        writer.AddAttribute("class", "messageTable");
                        writer.RenderBeginTag(HtmlTextWriterTag.Table);
                        writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                        writer.AddAttribute("class", "messageTable-Sender");
                        writer.RenderBeginTag(HtmlTextWriterTag.Th);
                        writer.Write("Sender");
                        writer.RenderEndTag();

                        writer.AddAttribute("class", "messageTable-Type");
                        writer.RenderBeginTag(HtmlTextWriterTag.Th);
                        writer.Write("Type");
                        writer.RenderEndTag();

                        writer.AddAttribute("class", "messageTable-Sender");
                        writer.RenderBeginTag(HtmlTextWriterTag.Th);
                        writer.Write("Text");
                        writer.RenderEndTag();

                        writer.RenderEndTag();

                        foreach (Message message in part.Messages)
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                            writer.RenderBeginTag(HtmlTextWriterTag.Td);
                            writer.Write(message.Sender);
                            writer.RenderEndTag();

                            writer.RenderBeginTag(HtmlTextWriterTag.Td);
                            writer.Write(message.Type);
                            writer.RenderEndTag();

                            writer.RenderBeginTag(HtmlTextWriterTag.Td);
                            writer.Write(message.Text);
                            writer.RenderEndTag();

                            writer.RenderEndTag();
                        }

                        writer.RenderEndTag();

                    }

                }


                writer.AddAttribute("type", "text/javascript");
                writer.AddAttribute("src", "http://alexgorbatchev.com/pub/sh/current/scripts/shCore.js");
                writer.RenderBeginTag("script");
                writer.RenderEndTag();


                writer.AddAttribute("type", "text/javascript");
                writer.AddAttribute("src", "http://alexgorbatchev.com/pub/sh/current/scripts/shAutoloader.js");
                writer.RenderBeginTag("script");
                writer.RenderEndTag();

                writer.AddAttribute("type", "text/javascript");
                writer.AddAttribute("src", "http://alexgorbatchev.com/pub/sh/current/scripts/shBrushXml.js");
                writer.RenderBeginTag("script");
                writer.RenderEndTag();


                writer.AddAttribute("type", "text/javascript");
                writer.RenderBeginTag("script");
                writer.Write("SyntaxHighlighter.all();");
                writer.RenderEndTag();

                writer.RenderEndTag();
                writer.RenderEndTag();
            }

            return stringWriter3.ToString();

        }
    }
}
