using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blend.Ellis
{
    public class SettingsIndex : Dictionary<string, string>
    {
        public new void Add(string key, string value)
        {

            if (this.ContainsKey(key))
            {
                this[key] = value;
            }
            else
            {
                base.Add(key, value);
            }
        }

        public bool Exists(string key)
        {
            return ContainsKey(key);
        }
    }
}
