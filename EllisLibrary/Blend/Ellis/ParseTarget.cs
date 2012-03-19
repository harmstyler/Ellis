using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using Blend.Ellis.Filters;
using HtmlAgilityPack;

namespace Blend.Ellis
{
    public class ParseTarget
    {
        public string Name { get; set; }
        public bool HasMarkup { get; set; }
        public List<string> Patterns { get; set; }
        public bool Optional { get; set; }
        public string Attribute { get; set; }
        public string FallbackPattern { get; set; }
        public List<FilterChain> FilterChains { get; set; }
        public ParseProfile Profile { get; set; }
        public bool AllowMultiple { get; set; }

        public ParseTarget(XmlNode targetNode, ParseProfile profile)
        {
            Profile = profile;

            Name = targetNode.Attributes["name"].Value;

            Patterns = new List<string>();

            Patterns.Add(targetNode.Attributes["pattern"] != null ? targetNode.Attributes["pattern"].Value : null);
            if (targetNode.SelectNodes("altPattern") != null)
            {
                foreach (XmlNode altPatternNode in targetNode.SelectNodes("altPattern"))
                {
                    Patterns.Add(altPatternNode.Attributes["pattern"].Value);
                }
            }

            FallbackPattern = targetNode.Attributes["fallbackPattern"] != null ? targetNode.Attributes["fallbackPattern"].Value : null;
            Attribute = targetNode.Attributes["attribute"] != null ? targetNode.Attributes["attribute"].Value : null;
            HasMarkup = targetNode.Attributes["hasMarkup"] != null ? Convert.ToBoolean(targetNode.Attributes["hasMarkup"].Value) : false;
            Optional = targetNode.Attributes["optional"] != null ? Convert.ToBoolean(targetNode.Attributes["optional"].Value) : true;
            AllowMultiple = targetNode.Attributes[""] != null ? Convert.ToBoolean(targetNode.Attributes["allowMultiple"].Value) : false;

            
            FilterChains = new List<FilterChain>();
            if (targetNode.Attributes["filterChains"] != null && targetNode.Attributes["filterChains"].Value.Trim().Length > 0)
            {
                foreach (string filterChainName in targetNode.Attributes["filterChains"].Value.Replace(" ", String.Empty).Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (Config.FilterChains[filterChainName] == null)
                    {
                        throw new Exception(String.Format("Filter chain \"{0}\" does not exist.", filterChainName));
                    }

                    FilterChains.Add((FilterChain)Config.FilterChains[filterChainName]);
                }
            }
        }
    }
}
