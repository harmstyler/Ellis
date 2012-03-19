using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blend.Ellis
{
    public static class ObjectExtensions
    {
        public static string DoubleQuoted(this object theObject)
        {
            return "\"" + theObject.ToString() + "\"";
        }
    }
}
