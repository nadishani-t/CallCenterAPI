using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfCallCenterAPI.Models
{
    public class API13Models
    {
    }

    public class APINO13Header
    {
        public string Status { get; set; }
        public string Description { get; set; }
    }


    //public class contract_number

    //{
    //    public string min { get; set; }
    //    public string max { get; set; }
    //}

    //public class Note
    //{
    //    public string startTime { get; set; }
    //    public string endTime { get; set; }
    //}

    //public class User
    //{
    //    public string startTime { get; set; }
    //    public string endTime { get; set; }
    //}


    public class Notes
    {
        public string contract_number { get; set; }
        public string strNote { get; set; }
        public string strUser { get; set; }


      //  public MarketingCodeRange marketing_code_range { get; set; }

    }
}