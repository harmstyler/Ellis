using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Blend.Ellis
{
    public static class Log
    {
        public static void Write(params object[] text)
        {
            Write(Utils.MakeString(text));
        }

        public static void Write(string text)
        {
            text = DateTime.Now.ToLongTimeString() + " - " + text;
            Console.WriteLine(text);
            ArtifactManager.AppendFileData(text + Environment.NewLine, ArtifactManager.GetArtifactPath("log.txt"));
        }
    }
}
