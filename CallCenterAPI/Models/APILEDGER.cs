using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallCenterAPI.Models
{
    public class APILEDGER
    {

        //public string cash_price { get; set; }
        //public string down_payment { get; set; }
        //public string amount_financed { get; set; }
        //public double effective_rate { get; set; }
        //public int period { get; set; }
        //public string pre_paids { get; set; }
        //public string rental { get; set; }
        //public string total_interest { get; set; }
        //public int type { get; set; }

        public List<LedgDet> contract_Ledger { get; set; }
    }



    public class StatusCrib
    {
        public string code { get; set; }
        public string description { get; set; }
    }


    public class CrdRcvOfficer
    {
        public string code { get; set; }
        public string description { get; set; }
    }


    public class LedgDet
    {
        public string contract_number { get; set; }


        public List<StatusCrib> Status_Crib { get; set; }
        public string customer_name { get; set; }

        public double rental { get; set; }
        public string Last_Receipt { get; set; }
        public string details_vehicle_property { get; set; }
        public double arrears { get; set; }
        public double insurance_due { get; set; }
        public string notes { get; set; }
        public string moratorium_link { get; set; }
        public string activated { get; set; }
        public string first_rental { get; set; }
        public string next_rental { get; set; }
        public string payment_date { get; set; }

        public List<CrdRcvOfficer> CrdRcvOfficer { get; set; }

        public double pre_paids { get; set; }
        public string type { get; set; }
        public int rentals_rvisd { get; set; }
        public double mor_int_balance { get; set; }
        public int duration { get; set; }
        public int rentals_posted { get; set; }
        public int shift_rentals { get; set; }
        public double insurance { get; set; }
        public double pd_cheques { get; set; }
        public double unrealized_chqs { get; set; }
        public double moratorium_Int { get; set; }

        //public string interest { get; set; }
        //public string net { get; set; }
        //public string vat { get; set; }
        //public string gross { get; set; }
    }
}