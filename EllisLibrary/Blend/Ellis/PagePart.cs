using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Blend.Ellis.Filters;
using System.Collections;

namespace Blend.Ellis
{
    public class PagePart
    {
        public string Content
        {
            get
            {
                //return ParsedContent.InnerHtml;
                return ParsedContent.OwnerDocument.DocumentNode.WriteContentTo();
            }
            set
            {
                ParsedContent = Utils.ParseHtml(value).DocumentNode;
            }
        }

        public HtmlNode ParsedContent { get; set; }
        public string Name { get; set; }
        public bool HasMarkup { get; set; }
        public Page Page { get; set; }
        public List<Message> Messages { get; set; }
        public ParseTarget Target { get; set; }
        public int Ordinal { get; set; }
        public Hashtable Meta { get; set; }

        public string Key
        {
            get
            {
                if (Target.AllowMultiple)
                {
                    return String.Concat(Name, "-", Ordinal);
                }
                else
                {
                    return Name;
                }
            }
        }

        public bool HasContent
        {
            get
            {
                return (Content != null && Content.Trim().Length > 0);
            }
        }

        public PagePart()
        {
            Messages = new List<Message>();
            Meta = new Hashtable();
        }

        public void AddMessage(string sender, string type, string text)
        {
            Message thisMessage = new Message();
            thisMessage.Sender = sender;
            thisMessage.Type = type;
            thisMessage.Text = text;
            Messages.Add(thisMessage);
        }

        public void AddMessage(string sender, StandardMessageTypes type, string text)
        {
            AddMessage(sender, type.ToString(), text);
        }

        public void AddMessage(FilterBase sender, StandardMessageTypes type, string text)
        {
            AddMessage("Filter: " + sender.Name, type.ToString(), text);
        }

        public void AddMessage(FilterBase sender, string type, string text)
        {
            AddMessage("Filter: " + sender.Name, type, text);
        }
    }
}
