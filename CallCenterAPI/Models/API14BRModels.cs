using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallCenterAPI.Models
{
    public class APINO14Header
    {
        public List<Detail14> Detail { get; set; }
    }


    public class Detail14
    {
        public string BrCode { get; set; }
        public string Descr { get; set; }

    }
}