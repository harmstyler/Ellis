using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.Reflection;

namespace Blend.Ellis.JobOutputs
{
    public abstract class JobOutputBase
    {
        private SettingsIndex settings = new SettingsIndex();  // Hate to do a backing field, but I don't want to have a constructor, since this is an abstract class and I can't be sure it will run
        public SettingsIndex Settings
        {
            get { return settings; }
            set { settings = value; }
        }
        public abstract void Execute(Job thisJob);

        public void AddSetting(string key, string value)
        {
            if (Settings.ContainsKey(key))
            {
                Settings[key] = value;
            }
            else
            {
                Settings.Add(key, value);
            }
        }

        public static JobOutputBase GetJobOutput(XmlNode outputNode)
        {
            string className = String.Concat("Blend.Ellis.JobOutputs.", outputNode.Name[0].ToString().ToUpper() + outputNode.Name.Substring(1));

            JobOutputBase thisJobOutput = (JobOutputBase) Utils.CreateObject(className);

            foreach(XmlAttribute settingsAttribute in outputNode.Attributes)
            {
                thisJobOutput.Settings.Add(settingsAttribute.Name, settingsAttribute.Value);
            }

            if (thisJobOutput.HasXmlInitialize())
            {
                MethodInfo xmlConstructor = thisJobOutput.GetType().GetMethod("Initialize");
                xmlConstructor.Invoke(thisJobOutput, new object[] { outputNode });
            }
            else
            {
                if (thisJobOutput.HasEmptyInitialize())
                {
                    MethodInfo constructor = thisJobOutput.GetType().GetMethod("Initialize");
                    constructor.Invoke(thisJobOutput, new object[0]);
                }
            }
        
            return thisJobOutput;

        }

        public bool HasEmptyInitialize()
        {
            Type type = this.GetType();
            foreach (MethodInfo method in type.GetMethods())
            {
                if (method.Name == "Initialize" && method.GetParameters().Count() == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasXmlInitialize()
        {
            Type type = this.GetType();
            foreach (MethodInfo method in type.GetMethods())
            {
                if (method.Name == "Initialize" && method.GetParameters().Count() == 1 && method.GetParameters()[0].ParameterType == typeof(XmlNode))
                {
                    return true;
                }
            }

            return false;
        }

    }


}
