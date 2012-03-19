using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;

namespace Blend.Ellis.JobOutputs
{
    class HtmlManifest : JobOutputBase
    {
        Dictionary<string, int> tags = new Dictionary<string, int>();

        public void Initialize(XmlNode settingsNode)
        {
            foreach (XmlAttribute attribute in settingsNode.Attributes)
            {
                Settings.Add(attribute.Name, attribute.Value);
            }
        }

        public override void Execute(Job thisJob)
        {
            foreach (Page page in thisJob.Pages)
            {
                try
                {


                    if (page.Parts == null)
                    {
                        continue;
                    }

                    foreach (PagePart part in page.Parts.Where(p => p.HasMarkup))
                    {
                        if (!part.HasContent || part.ParsedContent == null)
                        {
                            continue;
                        }

                        if (part.ParsedContent.SelectNodes("//*") == null)
                        {
                            continue;
                        }
                        foreach (HtmlNode node in part.ParsedContent.SelectNodes("//*"))
                        {
                            if (node.Attributes["class"] != null && Settings.ContainsKey("classExclusionPattern"))
                            {
                                if(Regex.IsMatch(node.Attributes["class"].Value, Settings["classExclusionPattern"]))
                                {
                                    continue;
                                }
                            }

                            if (node.Attributes["id"] != null && Settings.ContainsKey("idExclusionPattern"))
                            {
                                if (Regex.IsMatch(node.Attributes["id"].Value, Settings["idExclusionPattern"]))
                                {
                                    continue;
                                }
                            }


                            // Add it under the tag name
                            AddTag(node.Name);

                            // Add it under each class
                            if (node.Attributes["class"] != null)
                            {
                                foreach (
                                    string className in
                                        node.Attributes["class"].Value.Split(" ".ToCharArray(),
                                                                             StringSplitOptions.RemoveEmptyEntries))
                                {
                                    AddTag(String.Concat(node.Name, ".", className));
                                }
                            }

                            // Add it under the ID (filtering out .Net auto-IDs)
                            if (node.Attributes["id"] != null && !node.Attributes["id"].Value.StartsWith("ctl"))
                            {
                                AddTag(String.Concat(node.Name, "#", node.Attributes["id"].Value));
                            }

                            // Add it under form actions
                            if (node.Name == "form" && node.Attributes["action"] != null)
                            {
                                AddTag(String.Concat("form->", node.Attributes["action"].Value));
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Concat("HTMLManifest error on ", page.Url));
                }
            }

            var output = new StringBuilder();
            foreach (KeyValuePair<string, int> tag in tags.OrderBy(kvp => kvp.Key))
            {
                output.AppendFormat("{0} ({1})", tag.Key, tag.Value);
                output.Append(Environment.NewLine);
            }

            ArtifactManager.SaveFileData(output.ToString(), "reports/html-manifest.txt");

        }

        private void AddTag(string tag)
        {
            if (tags.ContainsKey(tag))
            {
                tags[tag]++;
            }
            else
            {
                tags.Add(tag, 1);
            }
        }
        
    }
}
