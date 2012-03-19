using System;
using System.Collections.Generic;
using System.Web;

using Blend.Ellis.PageOutputs;

namespace Blend.Ellis
{
    public class WebTool : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string config = context.Request.QueryString["c"];
            string url = context.Request.QueryString["u"];

            string pathToConfig = String.Format("/configs/{0}.config", config);

            Config.BatchNumber = 0;
            Config.Load(context.Server.MapPath(pathToConfig));

            Page page = new Page(url);
            page.Process();

            HtmlViewerPage viewerPage = new HtmlViewerPage();
            context.Response.Write(viewerPage.GetContent(page));
        }
    }
}
