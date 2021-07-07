using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfCallCenterAPI.Models
{
    public class APINO01Header
    {
        public List<Detail01> contracts { get; set; }
    }

    public class Detail01
    {
        public string customer_number { get; set; }
        public string contract_number { get; set; }
        public string due_date { get; set; }
        public double arrears_amount { get; set; }
        public int arrears_months { get; set; }
    }
}