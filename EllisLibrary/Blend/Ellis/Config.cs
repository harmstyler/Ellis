using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Blend.Ellis.JobOutputs;
using System.Xml;
using System.IO;
using System.Reflection;

namespace Blend.Ellis
{
    public static class Config
    {
        public static List<ParseProfile> Profiles { get; set; }
        public static Hashtable FilterChains { get; set; }
        public static List<JobOutputBase> Outputs { get; set; }

        public static string CachePath { get; set; }
        public static bool SkipIfNoCache { get; set; }

        public static string Domain { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }

        public static int BatchNumber { get; set; }
        public static string ArtifactPath { get; set; }

        public static List<Assembly> ExtensionAssemblies { get; set; }

        public static Encoding Encoding { get; set; }

        public static void Load(string pathToSettingsFile)
        {
            Profiles = new List<ParseProfile>();
            FilterChains = new Hashtable();
            ExtensionAssemblies = new List<Assembly>();
            
            //Parse the settings and set up all the parsing gear...
            XmlDocument parseSettings = new XmlDocument();
            parseSettings.Load(pathToSettingsFile);

            //We need to load the ArtifactPath before we can log anthing
            if (parseSettings.SelectSingleNode("/ellis").Attributes["artifactPath"] == null)
            {
                throw new Exception("You must set an artifactPath.");
            }
            ArtifactPath = parseSettings.SelectSingleNode("/ellis").Attributes["artifactPath"].Value;

            //Load up all the extension assemblies
            string extensionDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extensions");
            if (Directory.Exists(extensionDirectoryPath))
            {
                foreach (FileInfo file in new DirectoryInfo(extensionDirectoryPath).GetFiles("*.dll"))
                {
                    Assembly thisAssembly = Assembly.LoadFrom(file.FullName);
                    Config.ExtensionAssemblies.Add(thisAssembly);
                    Log.Write(String.Format("Loaded extension assembly: {0}", thisAssembly.GetName().Name));
                }
            }

            if (parseSettings.SelectSingleNode("/ellis").Attributes["populateFromCache"] != null)
            {
                CachePath = parseSettings.SelectSingleNode("/ellis").Attributes["populateFromCache"].Value;
            }

            if (parseSettings.SelectSingleNode("/ellis").Attributes["domain"] != null)
            {
                Domain = parseSettings.SelectSingleNode("/ellis").Attributes["domain"].Value;
            }

            if (parseSettings.SelectSingleNode("/ellis").Attributes["username"] != null)
            {
                Username = parseSettings.SelectSingleNode("/ellis").Attributes["username"].Value;
            }

            if (parseSettings.SelectSingleNode("/ellis").Attributes["password"] != null)
            {
                Password = parseSettings.SelectSingleNode("/ellis").Attributes["password"].Value;
            }

            if (parseSettings.SelectSingleNode("/ellis").Attributes["encoding"] != null)
            {
                Encoding = Encoding.GetEncoding(parseSettings.SelectSingleNode("/ellis").Attributes["encoding"].Value);
            }
            else
            {
                Encoding = Encoding.UTF8;
            }



            SkipIfNoCache = false;
            if (parseSettings.SelectSingleNode("/ellis").Attributes["skipIfNoCache"] != null)
            {
                SkipIfNoCache = Convert.ToBoolean(parseSettings.SelectSingleNode("/ellis").Attributes["skipIfNoCache"].Value);
            }


            //Populate the FilterChains
            foreach (XmlNode filterChainNode in parseSettings.SelectNodes("//filterChain"))
            {
                FilterChains.Add(filterChainNode.Attributes["name"].Value, new FilterChain(filterChainNode));
                Log.Write(String.Format("Added filter chain: {0}", filterChainNode.Attributes["name"].Value));
            }

            //Populate the Profiles
            foreach (XmlNode profileNode in parseSettings.SelectNodes("//profile"))
            {
                Profiles.Add(new ParseProfile(profileNode));
                Log.Write(String.Format("Added profile: {0}", profileNode.Attributes["name"].Value));
            }

            //Populate the Outputs
            Outputs = new List<JobOutputBase>();
            Outputs.Add(new SingleXmlFile());   // We always do this output, no matter what
            if (parseSettings.SelectNodes("/ellis/jobOutputs") != null)
            {
                foreach (XmlNode outputNode in parseSettings.SelectNodes("/ellis/jobOutputs/*"))
                {
                    Outputs.Add(JobOutputBase.GetJobOutput(outputNode));
                    Log.Write("Added job output: ", outputNode.Name);
                }
            }
        }

    }
}
