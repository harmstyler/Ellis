using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Blend.Ellis.JobOutputs
{
    class MetaTable : JobOutputBase
    {
         private List<string> metaNames = new List<string>();
         private StringBuilder fileText = new StringBuilder();

         public override void Execute(Job thisJob)
         {
             //First, run through the pages and compile the meta names
             foreach(Page thisPage in thisJob.Pages)
             {
                 foreach(DictionaryEntry metaValue in thisPage.Meta)
                 {
                     if(!metaNames.Contains(metaValue.Key.ToString()))
                     {
                         metaNames.Add(metaValue.Key.ToString());
                     }
                 }
             }

             //Now sort the meta in alpha order
             metaNames.Sort(new Comparison<string>(SortByName));

             //Set up the headings
             AddValue("Url");
             AddValue("Title");
             foreach(string metaName in metaNames)
             {
                 AddValue(metaName);
             }
             EndLine();

             //Now, run through the pages again, grabing the meta
             foreach(Page thisPage in thisJob.Pages)
             {
                //Set up the headings
                 AddValue(thisPage.Url.OriginalString);
                 AddValue(thisPage.Name);
                 foreach(string metaName in metaNames)
                 {
                     AddValue(thisPage.Meta.ContainsKey(metaName) ? thisPage.Meta[metaName].ToString() : String.Empty);
                 }
                 EndLine();
             }

             //Save the file
             ArtifactManager.SaveFileData(fileText.ToString(), "reports/meta-table.csv");
         }

         private void AddValue(string value)
         {
             fileText.AppendFormat("\"{0}\",", value);
         }

         private void EndLine()
         {
             fileText.Append(Environment.NewLine);
         }

         private int SortByName(string object1, string object2)
         {
             return object1.CompareTo(object2);
         }
    }
}
