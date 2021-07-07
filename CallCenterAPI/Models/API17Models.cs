using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallCenterAPI.Models
{
    public class API17Models
    {
    }

    public class APINO17Header
    {
        public string contract_number { get; set; }
        public List<Detail17> Detail_Ledger { get; set; }
    }


    public class Detail17
    {
        //   public string Code { get; set; }
        public string registration_number { get; set; }
        public string vehicle_class { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string manufacture_des { get; set; }
        public string color { get; set; }
     
    }
}