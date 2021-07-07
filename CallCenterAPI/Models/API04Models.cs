using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallCenterAPI.Models
{
    public class APINO04Head
    {

        public string customer_firstName { get; set; }
        public string customer_lastName { get; set; }
        public string nic { get; set; }
        public string customer_fullName { get; set; }
        public string address { get; set; }
        public string postal_code { get; set; }
        public string district { get; set; }
        public string province { get; set; }
        public string home_phone { get; set; }
        public string mobile_phone { get; set; }
        public string sms_number { get; set; }
        public string gender { get; set; }
        public string marital_status { get; set; }
        public List<hierer> contract_List { get; set; }
    }


    public class hierer
    {
        public string contract_number { get; set; }
        public List<guarantor> guarantor { get; set; }
    }


    public class guarantor
    {
        //   public string contract_number { get; set; }
        public string customer_number { get; set; }
        public string customer_firstName { get; set; }
        public string customer_lastName { get; set; }
        public string nic { get; set; }
        public string customer_fullName { get; set; }
        public string address { get; set; }
        public string postal_code { get; set; }
        public string home_phone { get; set; }
        public string mobile_phone { get; set; }
        public string sms_number { get; set; }

    }
}