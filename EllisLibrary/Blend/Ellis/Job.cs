using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Blend.Ellis.JobOutputs;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Collections;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Blend.Ellis
{
    public class Job
    {
        public List<Page> Pages { get; set; }    

        public Job(string urlFilePath, string parseSettingsFilePath, int batchNumber)
        {
            Config.BatchNumber = batchNumber;
            Config.Load(parseSettingsFilePath);
            
            Log.Write(String.Format("Batch: {0}", batchNumber.ToString()));

            LoadUrls(urlFilePath);
        }

        private void LoadUrls(string pathToUrlFile)
        {

            Pages = new List<Page>();

            //Load up the URLs
            List<string> urlsToProcess = new List<String>();
            foreach (string thisUrl in File.ReadAllLines(pathToUrlFile).Distinct())
            {
                string cleanedThisUrl = thisUrl.Trim();

                //If it's blank, skip it
                if (cleanedThisUrl.Length == 0)
                {
                    continue;
                }

                //If it's the break line, end the loop
                if (cleanedThisUrl.ToLowerInvariant() == "#end")
                {
                    break;
                }

                //If it's the start line, clear the current list
                if (cleanedThisUrl.ToLowerInvariant() == "#start")
                {
                    urlsToProcess.Clear();
                }

                //If it's a comment, skip it
                if (cleanedThisUrl[0] == '#')
                {
                    continue;
                }

                //Add it
                urlsToProcess.Add(cleanedThisUrl);
            }
            Log.Write(String.Format("Found {0} URL(s)", urlsToProcess.Count.ToString()));

            //Add them all as pages
            foreach(string url in urlsToProcess)
            {
                Page page = new Page(url);
                page.Job = this;
                Pages.Add(page);
            }

        }

        public void Execute()
        {
            int startTime = Environment.TickCount;

            int counter = 0;
            foreach (Page thisUrl in Pages)
            {
                counter++;

                thisUrl.Process();

                Log.Write(String.Format("{0}/{1}: {2}", counter.ToString(), Pages.Count.ToString(), thisUrl.Url.AbsoluteUri));
            }

            Log.Write(String.Format("Page processing complete. {0} Pages processed in {1} second(s)", counter.ToString(), ((Environment.TickCount - startTime)/1000).ToString()));

            foreach (JobOutputBase thisOutput in Config.Outputs)
            {
                Log.Write(String.Format("Executing output: {0}", thisOutput.ToString()));
                thisOutput.Execute(this);
            }
        }

    }
}
