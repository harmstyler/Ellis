using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Blend.Ellis
{
    public class Page
    {
        public Uri Url { get; set; }
        public string Content { get; set; }
        public List<PagePart> Parts { get; set; }
        public Job Job;
        public int HttpStatus { get; set; }
        public Hashtable Meta { get; set; }
        public string ContentType { get; set; }
        public DateTime Retrieved { get; set; }
        public string NamePart { get; set; }
        public List<Message> Messages { get; set; }

        public string Name
        {
            get
            {
                if (String.IsNullOrEmpty(NamePart))
                {
                    return Url.AbsolutePath;
                }
                else
                {
                    return Parts.Where(p => p.Name == NamePart).Single().Content.Length > 0 ? Parts.Where(p => p.Name == NamePart).Single().Content : Url.AbsolutePath;
                }
            }
        }

        public string Id
        {
            get
            {
                return Utils.GetMD5Hash(Url.OriginalString);
            }
        }


        public bool Parsable
        {
            get
            {
                return ContentType.Contains("text/html");
            }
        }

        public bool RetrievalError
        {
            get
            {
                return (HttpStatus == 0 || HttpStatus.ToString()[0] != '2');
            }
        }

        public Page(string url)
        {
            Url = new Uri(url);
            Content = String.Empty;
            Parts = new List<PagePart>();
            Meta = new Hashtable();
            Messages = new List<Message>();
        }

        public void Process()
        {
            Populate();
            ExtractParts();
            RunFilters();
        }
 

        /// <summary>
        /// Returns an absolute URL, based against THIS PAGE's location.  So, if you pass in "index.html," it will return a full URL correct against this page's current location in the site.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="includeDomain"></param>
        /// <returns></returns>
        public string GetContextualUrl(string relativeUrl, bool includeDomain)
        {
            //This is absolute already, just return it
            if (relativeUrl.StartsWith("http"))
            {
                if (!includeDomain)
                {
                    Uri thisUrl = new Uri(relativeUrl);
                    return thisUrl.LocalPath;
                }

                return relativeUrl;
            }

            //If this is starting from the "/", return it with the host and scheme
            if (relativeUrl.StartsWith("/"))
            {
                if (includeDomain)
                {
                    return AddDomainToUrl(relativeUrl);
                }

                return relativeUrl;
            }

            //If this is relative, append it to this page's directory
            string pageUrlDirectory = String.Join("", Url.Segments.Take(Url.Segments.Count() - 1).ToArray());
            string correctedUrl = new Uri(Url.Scheme + "://" + Url.Host + pageUrlDirectory + relativeUrl).LocalPath;

            if (includeDomain)
            {
                return AddDomainToUrl(correctedUrl);
            }
            else
            {
                return correctedUrl;
            }
        }

        public string AddDomainToUrl(string url)
        {
            if (url.StartsWith("http"))
            {
                return url;
            }

            return Url.Scheme + "://" + Url.Host + "/" + url.TrimStart(new Char[] { '/' });
        }


        public void Populate()
        {
            //Are we pulling this from cache
            if(Config.CachePath != null && File.Exists(String.Concat(Config.CachePath, Id)))
            {
                Content = File.ReadAllText(String.Concat(Config.CachePath, Id));

                // TODO: This is just wrong...
                HttpStatus = 200;
                ContentType = "text/html";
            }
            else if (Config.SkipIfNoCache)
            {
                //Set a retrieval error
                HttpStatus = 500;
            }
            else
            {
                //Make the request
                HttpWebResponse response = Utils.MakeHttpRequest(Url.OriginalString);

                Retrieved = DateTime.Now;

                //If we got something back...
                if (response != null)
                {
                    HttpStatus = (int)response.StatusCode;
                    ContentType = response.ContentType;

                    //Is this a parsable file?
                    if (Parsable)
                    {
                        //This is parsable content. Set the content.                
                        StreamReader thisSteamReader = new StreamReader(response.GetResponseStream(), Config.Encoding);
                        Content = thisSteamReader.ReadToEnd();

                        //Cache it
                        ArtifactManager.SaveFileData(Content, String.Concat("cache/", Id));
                    }
                    else
                    {
                        //This a binary file.  Save it and store the path
                        //TODO: the file path needs to be abstracted out
                        string localPath = ArtifactManager.SaveFileData(Utils.GetBytesFromResponse(response), String.Concat("downloaded-pages", Url.LocalPath));
                        Meta.Add("DownloadPath", localPath);
                        Messages.Add(new Message() { Sender = "Page Populate() Method", Type = StandardMessageTypes.Notice.ToString(), Text = "URL was non-parsable file Downloaded." });
                    }
                }
            }
        }

        public PagePart GetPart(string part)
        {
            // If they want the pseudo part...
            if (part == "_Url")
            {
                // Return a temporary, in-memory part

                return CreatePart("_Url", 0, this.Url.AbsoluteUri);
            }

            return Parts.Where(p => p.Name == part).FirstOrDefault();
        }

        public PagePart CreatePart(string name, int ordinal, string content)
        {
            PagePart thisPart = new PagePart();
            thisPart.Name = name;
            thisPart.Ordinal = ordinal;
            thisPart.Content = content;
            thisPart.Page = this;
            return thisPart;
        }

        public void SetPartContent(string partName, string content)
        {
            Parts.Where(p => p.Name == partName).Single().Content = content;
        }

        public  void ExtractParts()
        {
            foreach (ParseProfile thisProfile in Config.Profiles)
            {
                if (!Regex.IsMatch(Url.AbsoluteUri, thisProfile.MatchPattern))
                {
                    continue;
                }

                //If we got here, this is the profile we're using..

                //We're going to spin through all the targets that have a pattern defined
                NamePart = thisProfile.NamePart;

                foreach (ParseTarget thisTarget in thisProfile.Targets.Where(t => t.Patterns != null))
                {
                    //Parse into an HTML document
                    HtmlDocument thisHtmlDocument = Utils.ParseHtml(Content);

                    // Loop through all the provided patterns and stop when one of them doesn't return NULL. This will be the pattern we use.
                    string operativePattern = String.Empty;
                    foreach (string pattern in thisTarget.Patterns)
                    {
                        operativePattern = pattern;
                        if (thisHtmlDocument.DocumentNode.SelectNodes(operativePattern) != null)
                        {
                            break;
                        }
                    }

                    //Do we have the content
                    string content = String.Empty;
                    int ordinal = 0;
                    if (thisHtmlDocument.DocumentNode.SelectNodes(operativePattern) != null)
                    {
                        foreach (HtmlNode thisExtraction in thisHtmlDocument.DocumentNode.SelectNodes(operativePattern))
                        {
                            ordinal++;

                            //HtmlAgilityPack can't do attribute selection, so we have to handle this explicitly, which kinda sucks
                            if (thisTarget.Attribute == null)
                            {
                                content = thisExtraction.InnerHtml;
                            }
                            else
                            {
                                content = thisExtraction.Attributes[thisTarget.Attribute].Value;
                            }

                            //Initialize the PagePart we're developing from this
                            PagePart thisPagePart = CreatePart(thisTarget.Name, ordinal, content);
                            thisPagePart.HasMarkup = thisTarget.HasMarkup;
                            thisPagePart.Target = thisTarget;
                            thisPagePart.Ordinal = ordinal;
                            Parts.Add(thisPagePart);

                            //If we're not allowing multiples, exit after the first one
                            if (!thisTarget.AllowMultiple)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        //Initialize the PagePart we're developing from this
                        PagePart thisPagePart = CreatePart(thisTarget.Name, ordinal, content);
                        thisPagePart.HasMarkup = thisTarget.HasMarkup;
                        thisPagePart.Target = thisTarget;
                        thisPagePart.Ordinal = ordinal;
                        Parts.Add(thisPagePart);
                    }



                }

                //Don't execute any other profiles
                break;


            }
        }

        private void RunFilters()
        {
            // If there are no parts, then don't bother running filters
            if (Parts.Count == 0)
            {
                return;
            }

            if (!RetrievalError && Parsable)
            {
                for (int index = 0; index < Parts.Count; index++)
                {
                    foreach (FilterChain thisFilterChain in Parts[index].Target.FilterChains)
                    {
                        Parts[index] = thisFilterChain.Execute(Parts[index]);
                    }
                }
            }
        }



    }
}
