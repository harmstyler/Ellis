using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Blend.Ellis
{
    class ExportFile
    {
        public Job Job { get; set; }
        private XmlTextWriter Xml { get; set; }

        public ExportFile(Job thisJob)
        {
            Job = thisJob;

            //Instantiate
            Xml = new XmlTextWriter(ArtifactManager.GetArtifactPath("output.xml"), null);
            Xml.Formatting = Formatting.Indented;  //Pretty formatting

            //Write the root
            Xml.WriteStartDocument();
            Xml.WriteStartElement("output");
            Xml.WriteAttributeString("batchNumber", Config.BatchNumber.ToString());
        }

        public void ProcessUrl(Page url)
        {
            Xml.WriteStartElement("page");
            Xml.WriteAttributeString("url", url.Url.AbsoluteUri);

            foreach (PagePart thisPart in url.Parts)
            {
                Xml.WriteStartElement("part");
                Xml.WriteAttributeString("name", thisPart.Name);

                if (thisPart.HasContent)
                {
                    if (thisPart.HasMarkup)
                    {
                        Xml.WriteCData(thisPart.Content);
                    }
                    else
                    {
                        Xml.WriteString(thisPart.Content);
                    }
                }

                Xml.WriteStartElement("messages");
                foreach(Message thisMessage in thisPart.Messages)
                {
                    Xml.WriteStartElement("message");
                    Xml.WriteAttributeString("sender",thisMessage.Sender);
                    Xml.WriteAttributeString("type", thisMessage.Type);
                    Xml.WriteAttributeString("text", thisMessage.Text);
                    Xml.WriteEndElement();
                }
                Xml.WriteEndElement();

                Xml.WriteEndElement();
            }

            Xml.WriteEndElement();
        }

        public void Close()
        {
            //Xml.WriteEndDocument();
            Xml.Close();
        }
        
    }
}
