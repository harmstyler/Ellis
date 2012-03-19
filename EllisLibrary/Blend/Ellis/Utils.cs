using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Reflection;

namespace Blend.Ellis
{
    public static class Utils
    {
        public static HttpWebResponse MakeHttpRequest(string url)
        {
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                if (Config.Domain != null)
                {
                    request.Credentials = new NetworkCredential(Config.Username, Config.Password, Config.Domain);
                }
                else
                {
                    if (Config.Username != null)
                    {
                        request.Credentials = new NetworkCredential(Config.Username, Config.Password);
                    }
                }

                response = (HttpWebResponse) request.GetResponse();
            }
            catch (WebException e)
            {
                response = (HttpWebResponse)e.Response;
            }

            return response;
        }

        public static byte[] GetBytesFromResponse(HttpWebResponse response)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = response.GetResponseStream().Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }

        public static byte[] MakeBinaryHttpRequest(string url)
        {
            return GetBytesFromResponse(MakeHttpRequest(url));
        }

        public static HtmlDocument ParseHtml(string html)
        {
            byte[] byteArray = Config.Encoding.GetBytes(html);
            MemoryStream stream = new MemoryStream( byteArray );

            HtmlDocument doc = new HtmlDocument();
            doc.OptionOutputAsXml = true;
            doc.OptionFixNestedTags = true;
            
            doc.Load(stream, Config.Encoding);

            return doc;
        }

        public static object CreateObject(string className)
        {
            object thisObject = null;
            if (Type.GetType(className) != null)
            {
                //It's a local filter. Life is good and simple.
                thisObject = Activator.CreateInstance(Type.GetType(className));
            }
            else
            {
                //No, spin all loaded assemblies to find it
                //NOTE: This is super-frustrating.  If you have the assembly name in the class name ("MyNamespace.MyClass, MyAssembly") it should just work, but for some reason, that only works with assemblies in the /bin folder.   Since our assemblies are in /Extensions, it doesn't work automatically.  We have to spin the assemblies to find our class.
                foreach (Assembly thisAssembly in Config.ExtensionAssemblies)
                {
                    if (thisAssembly.GetType(className) != null)
                    {
                        thisObject = (object)Activator.CreateInstance(thisAssembly.GetType(className));
                    }

                    if (thisObject != null)
                    {
                        break;
                    }
                }
            }

            if (thisObject == null)
            {
                throw new Exception(String.Concat("Failed to load class ", className));
            }

            return thisObject;
        }

        public static string ConvertToMixedCase(string camelCase)
        {
             return camelCase[0].ToString().ToUpper() + camelCase.Substring(1);
        }

        public static string GetMD5Hash(string text)
        {
            Encoder enc = System.Text.Encoding.Unicode.GetEncoder();

            // Create a buffer large enough to hold the string
            byte[] unicodeText = new byte[text.Length * 2];
            enc.GetBytes(text.ToCharArray(), 0, text.Length, unicodeText, 0, true);

            // Now that we have a byte array we can ask the CSP to hash it
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(unicodeText);

            // Build the final string by converting each byte
            // into hex and appending it to a StringBuilder
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }

            // And return it
            return sb.ToString();
        }

public static string MakeString(params object[] text)
{
    if (text.Count() == 0)
    {
        return null;
    }

    if (text.Count() == 1 && text[0].GetType() == typeof(IEnumerable<string>) || text[0].GetType() == typeof(IOrderedEnumerable<string>))
    {
        List<string> list = (List<string>)text[0];
        text[0] = String.Join(Environment.NewLine, list.ToArray());
    }

    if (text[0].ToString().Contains("{0}"))
    {
        return String.Format(text.Select(s => s.ToString()).First(), text.Skip(1).Select(s => s.ToString()).ToArray());
    }

    return String.Concat(text.Select(s => s.ToString()).ToArray());
}

        public static string MakeString(IEnumerable<string> strings, string delimiter)
        {
            return String.Join(delimiter, strings.ToArray());
        }
    }
}
