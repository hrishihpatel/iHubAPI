using System;
using System.Collections.Generic;

namespace iHubAPI.Models
{
    public class Status
    {
        public int StatusCode { get; set; }
        public List<string> StatusDetails { get; set; }
        public string StatusMessage { get; set; }
    }
}
