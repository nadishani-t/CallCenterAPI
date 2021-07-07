using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfCallCenterAPI.Models
{
    public class APINO6Header
    {
        public string contract_number { get; set; }
        public List<Detail06> Detail_Ledger { get; set; }
    }


    public class Detail06
    {
        //   public string Code { get; set; }
        public string type { get; set; }
        public string voucher { get; set; }
        public string date { get; set; }
        public double debit { get; set; }
        public double credit { get; set; }
        public double balance { get; set; }
        public string narration { get; set; }
    }
}