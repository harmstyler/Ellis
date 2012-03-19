using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using Blend.Ellis;
using System.IO;

namespace Blend.Ellis.JobOutputs
{
    class SingleXmlFile : JobOutputBase
    {
        public Job Job { get; set; }
        private XmlTextWriter Xml { get; set; }

        public override void Execute(Job thisJob)
        {
            Job = thisJob;

            //Instantiate
            StringWriter xmlString = new StringWriter();
            Xml = new XmlTextWriter(xmlString);
            Xml.Formatting = Formatting.Indented;  //Pretty formatting

            //Write the root
            //Xml.WriteStartDocument();
            Xml.WriteStartElement("output");
            Xml.WriteAttributeString("batchNumber", Config.BatchNumber.ToString());

            foreach (Page thisPage in thisJob.Pages)
            {
                Xml.WriteStartElement("page");
                Xml.WriteAttributeString("url", thisPage.Url.AbsoluteUri);
                Xml.WriteAttributeString("retrieved", thisPage.Retrieved.ToString("s"));
                Xml.WriteAttributeString("valid", thisPage.HttpStatus == 200 ? "true" : "false");
                Xml.WriteAttributeString("retrievalStatusCode", thisPage.HttpStatus.ToString());


                foreach (DictionaryEntry meta in thisPage.Meta)
                {
                    Xml.WriteStartElement("meta");
                    Xml.WriteAttributeString("name", meta.Key.ToString());
                    Xml.WriteAttributeString("content", meta.Value.ToString());
                    Xml.WriteEndElement();
                }

                foreach (PagePart thisPart in thisPage.Parts)
                {
                    Xml.WriteStartElement("part");
                    Xml.WriteAttributeString("name", thisPart.Key);

                    if (thisPart.HasContent)
                    {
                        if (thisPart.HasMarkup)
                        {
                            string cleanContent = thisPart.Content;
                            cleanContent = cleanContent.Replace("<![CDATA[", String.Empty);
                            cleanContent = cleanContent.Replace("]]>", String.Empty);
                            Xml.WriteCData(cleanContent);
                        }
                        else
                        {
                            Xml.WriteString(thisPart.Content);
                        }
                    }

                    foreach (DictionaryEntry meta in thisPart.Meta)
                    {
                        Xml.WriteStartElement("meta");
                        Xml.WriteAttributeString("name", meta.Key.ToString());
                        Xml.WriteAttributeString("content", meta.Value.ToString());
                        Xml.WriteEndElement();
                    }

                    Xml.WriteStartElement("messages");
                    foreach (Message thisMessage in thisPart.Messages)
                    {
                        Xml.WriteStartElement("message");
                        Xml.WriteAttributeString("sender", thisMessage.Sender);
                        Xml.WriteAttributeString("type", thisMessage.Type);
                        Xml.WriteAttributeString("text", thisMessage.Text);
                        Xml.WriteEndElement();
                    }
                    Xml.WriteEndElement();

                    Xml.WriteEndElement();

                }

                Xml.WriteEndElement();

            }

            Xml.WriteEndElement();
            Xml.Close();

            ArtifactManager.SaveFileData(xmlString.ToString(), "output.xml");
        }

    }
}

