using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallCenterAPI.Models
{
    public class APINO09Header
    {
        public string contract_number { get; set; }
        public string customer_name { get; set; }
        public List<Detail09> Detail { get; set; }
    }

    public class Detail09
    {
        public int seq_number { get; set; }
        public string date { get; set; }
        public string note { get; set; }
        public string user { get; set; }
    }
}