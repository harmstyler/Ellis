using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Blend.Ellis
{
    public static class ArtifactManager
    {

        public static string GetArtifactPath(string path)
        {
            string returnValue = Path.Combine(Config.ArtifactPath, Config.BatchNumber.ToString());
            return Path.Combine(returnValue, path);
        }


        public static bool FileExists(string path)
        {
            return File.Exists(GetArtifactPath(path));
        }

        public static string SaveFileData(IList<string> items, string path)
        {
            return ArtifactManager.SaveFileData(String.Join(Environment.NewLine, items.ToArray()), path);
        }

        public static string SaveFileData(string text, string path)
        {
            path = path.TrimStart(new Char[] { '\\' });
            path = GetArtifactPath(path);

            EnsureDirectory(path);
            File.WriteAllText(path, text, Config.Encoding);

            return path;
        }


        public static string SaveFileData(byte[] data, string path)
        {
            path = path.TrimStart(new Char[] { '\\' });
            path = GetArtifactPath(path);

            EnsureDirectory(path);

            File.WriteAllBytes(path, data);

            return path;
        }

        public static string AppendFileData(string data, string path)
        {
            EnsureDirectory(path);

            File.AppendAllText(GetArtifactPath(path), data);

            return GetArtifactPath(path);
        }

        public static void EnsureDirectory(string path)
        {
            string directoryPath = Path.GetDirectoryName(path);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
