using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Web;

namespace VK_get
{
    class Playlist
    {
        public string artist { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string aid { get; set; }
    }
}
