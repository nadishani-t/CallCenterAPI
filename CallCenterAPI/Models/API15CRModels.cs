using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallCenterAPI.Models
{
    public class APINO15Header
    {
        public List<Detail15> Detail { get; set; }
    }


    public class Detail15
    {
        public string Name { get; set; }
        public string Mobile_No { get; set; }

    }
}