using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blend.Ellis;
using System.IO;

namespace EllisCommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            string helpText = @"
Usage
-----
Two arguments are required.

1. Path to a file of URLs, one per line.
2. Path to a parse settings config file.";

            //Make sure we have enough arguments
            if (args.Count() < 2)
            {
                Console.WriteLine(helpText);
                Environment.Exit(1);
            }

           
            //This is the master error handler.  If it throws an error anywhere inside the guts, it will bubble up to here.
            //try
            //{
         
                //Ensure we have a URL file
                string urlFilePath = args[0].ToString();
                if (!File.Exists(urlFilePath))
                {
                    throw new Exception(String.Concat("Unable to find URL file: ", urlFilePath));
                }

                //Ensure we have a parse settings file
                string parseSettingsFilePath = args[1].ToString();
                if (!File.Exists(parseSettingsFilePath))
                {
                    throw new Exception(String.Concat("Unable to find parse settings file: ", parseSettingsFilePath));
                }

                //Get a batch number
                TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                int batchNumber = (int)t.TotalSeconds;

                //Kick off the job
                var thisJob = new Job(urlFilePath, parseSettingsFilePath, batchNumber);
                thisJob.Execute();
           
            /*}
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message + Environment.NewLine + e.StackTrace);
            }*/
          

        }
    }
}
