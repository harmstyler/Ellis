using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Blend.Ellis.Filters
{
    public abstract class FilterBase
    {
        public string Name { get; set; }
        public FilterChain Parent { get; set; }
        public SettingsIndex Settings { get; set; }
        public abstract void Initialize();
        public abstract PagePart Execute(PagePart pagePart);

        public void EnsureSetting(string key)
        {
            if (!Settings.ContainsKey(key))
            {
                throw new Exception(String.Format("Setting key \"{0}\" does not exist for filter \"{1}\" in chain \"{2}\"", key, Name, Parent.Name));
            }
        }

        public void EnsureSetting(string key, string valueList)
        {
            //First make sure the setting exists
            EnsureSetting(key);

            //Split the allow values on the comma
            string thisValue = Settings[key];
            List<string> permissibleValues = valueList.Replace(" ", "").Split(new Char[] { ',' }).ToList<string>();
            if(!permissibleValues.Contains(thisValue))
            {
                throw new Exception(String.Format("Value \"{0}\" is not allowed for setting key \"{1}\" in filter \"{2}\" in chain \"{3}\". Permissable values are: \"{4}\".", thisValue, key, Name, Parent.Name, String.Join("\", \"", permissibleValues.ToArray())));
            }
        }

        public static FilterBase GetFilter(XmlNode settingsNode, FilterChain parentChain)
        {
            if (settingsNode.Attributes["name"] == null)
            {
                throw new Exception(String.Format("Filter \"{0}\" in chain \"{1}\" has no \"name\" attribute", settingsNode.Name, parentChain.Name));
            }

            FilterBase thisFilter = (FilterBase)Utils.CreateObject("Blend.Ellis.Filters." + Utils.ConvertToMixedCase(settingsNode.Name));

            //If we're still null, then this filter doesn't exist.  Life is hard that way.  People are mean, dogs bite, and cheeseburgers make you fat...
            if (thisFilter == null)
            {
                throw new Exception(String.Format("Cannot load filter \"{0}\" in chain \"{1}\"", settingsNode.Name));
            }
            
            //Load up its settings
            SettingsIndex settings = new SettingsIndex();
            foreach (XmlAttribute attribute in settingsNode.Attributes)
            {
                settings.Add(attribute.Name, attribute.Value);
            }
            thisFilter.Settings = settings;

            thisFilter.Name = settingsNode.Attributes["name"].Value;
            
            //Run the initialization
            thisFilter.Initialize();

            return thisFilter;
        }
    }
}
