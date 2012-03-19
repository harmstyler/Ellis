using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Blend.Ellis.Filters;

namespace Blend.Ellis
{
    public class ParseProfile
    {
        public string Name { get; set; }
        public string MatchType { get; set; }
        public string MatchPattern { get; set; }
        public List<ParseTarget> Targets { get; set; }
        public string NamePart { get; set; }

        public ParseProfile(XmlNode thisProfileNode)
        {
            Targets = new List<ParseTarget>();

            Name = thisProfileNode.Attributes["name"].Value;
            NamePart = thisProfileNode.Attributes["namePart"] != null ? thisProfileNode.Attributes["namePart"].Value : null;
            MatchPattern = thisProfileNode.Attributes["matchPattern"].Value;
            MatchType = thisProfileNode.Attributes["matchType"].Value;

            // TODO: You shouldn't be able to add two parts with the same name
            foreach (XmlNode targetNode in thisProfileNode.SelectNodes("target"))
            {
                Targets.Add(new ParseTarget(targetNode, this));
            }

        }

        
    }
}
