using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Blend.Ellis.Filters
{
    class DownloadAssets : FilterBase
    {
        public override void Initialize()
        {
            //this.EnsureSetting("directory");
        }

        public override PagePart Execute(PagePart thisPart)
        {
            int counter = 0;

            string pattern = "img";

            string directory = String.Empty;
            if (Settings.Exists("directory"))
            {
                directory = Settings["directory"];
            }
            else
            {
                directory = ArtifactManager.GetArtifactPath("downloaded-assets");
            }

            //Have any images?
            if (thisPart.ParsedContent.SelectNodes(pattern) != null)
            {
                //Loop through them
                foreach(HtmlNode image in thisPart.ParsedContent.SelectNodes(pattern))
                {
                    //If they're remote, skip it
                    if (image.Attributes["src"].Value.StartsWith("http"))
                    {
                        continue;
                    }

                    counter++;

                    //Get the corrected path with the domain (to make the request)
                    string assetPathWithDomain = thisPart.Page.GetContextualUrl(image.Attributes["src"].Value, true);

                    //Get the corrected path without the domain (to save the file)
                    string assetPathWithoutDomain = thisPart.Page.GetContextualUrl(image.Attributes["src"].Value, false);

                    //Add the directory from the settings
                    string newImagePath = String.Concat(directory, "/", assetPathWithoutDomain);

                    //Get the file data
                    Byte[] fileData = Utils.MakeBinaryHttpRequest(assetPathWithDomain);

                    //Save the file
                    ArtifactManager.SaveFileData(fileData, newImagePath);
                    
                    //Correct the reference to the image
                    image.Attributes["src"].Value = newImagePath;

                    thisPart.Meta.Add(String.Concat("Downloaded Asset ", counter.ToString()), assetPathWithoutDomain);
                }
            }

            thisPart.AddMessage(this, StandardMessageTypes.Executed, String.Format("Saved {0} image(s) locally.", counter.ToString()));

            return thisPart;
        }
    }
}
