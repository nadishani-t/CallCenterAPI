using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallCenterAPI.Models
{
    public class API_TEST01Header
    {
        public List<Detail_T2> Detail { get; set; }
    }

  
    public class Detail_T2
    {
        public string BankCode { get; set; }
        public string BankName { get; set; }

    } 
}