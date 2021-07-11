using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;

namespace VK_get
{
    class Request
    {
        public string Get(string url)
        {
            WebRequest req = WebRequest.Create(url);
            WebResponse res = req.GetResponse();
            Stream s = res.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string otvet = sr.ReadToEnd();
            otvet = HttpUtility.HtmlDecode(otvet);

            return otvet;
        }
    }
}
