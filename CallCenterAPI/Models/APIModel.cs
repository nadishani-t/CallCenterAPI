using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallCenterAPI.Models
{
    public class APINO02Header
    {
        public string customer_id { get; set; }
        public List<string> contracts { get; set; }

    }



    public class APIModel
    {
        public int Type { get; set; }
        public string Message { get; set; }
        public PaymentModel Result { get; set; }
    }


    //API NO 5
    public class PaymentModel
    {
        public string contract_number { get; set; }
        public string customer_name { get; set; }
        public double cash_price { get; set; }
        public double down_payment { get; set; }
        public double amount_financed { get; set; }
        public double effective_rate { get; set; }
        public int period { get; set; }
        public double pre_paids { get; set; }
        public double rental { get; set; }
        public double total_interest { get; set; }
        public int type { get; set; }
        public List<Detail> details { get; set; }
    }

    public class Detail
    {
        public int slab { get; set; }
        public double capital { get; set; }
        public double interest { get; set; }
        public double net { get; set; }
        public double vat { get; set; }
        public double gross { get; set; }
    }


    // API 07

    public class APINO07Header
    {
        public string contract_number { get; set; }
        public string customer_name { get; set; }
        public List<Detail07> detail { get; set; }

    }
    public class Detail07
    {
        public string date { get; set; }
        public string receipt_no { get; set; }
        public string cheque_no { get; set; }
        public string mode { get; set; }
        public double amount { get; set; }
        public string status { get; set; }
        public string remark { get; set; }
        public string user { get; set; }
    }



    // API 08

    public class APINO08Header
    {
        public string contract_number { get; set; }
        public string customer_name { get; set; }
        public List<Returned> Returned { get; set; }
        public List<UnRealized> UnRealized { get; set; }
        public List<Realized> Realized { get; set; }
        public List<Cancelled> Cancelled { get; set; }
    }

    public class Returned
    {
        public string pd_cheque_number { get; set; }
        public string date { get; set; }
        public string customer { get; set; }
        public string deposited { get; set; }
        public string cheque_number { get; set; }
        public string branch { get; set; }
    }

    public class UnRealized
    {
        public string pd_cheque_number { get; set; }
        public string date { get; set; }
        public string customer { get; set; }
        public string deposited { get; set; }
        public string cheque_number { get; set; }
        public string branch { get; set; }
    }


    public class Realized
    {
        public string pd_cheque_number { get; set; }
        public string date { get; set; }
        public string customer { get; set; }
        public string deposited { get; set; }
        public string cheque_number { get; set; }
        public string branch { get; set; }
    }

    public class Cancelled
    {
        public string pd_cheque_number { get; set; }
        public string date { get; set; }
        public string customer { get; set; }
        public string deposited { get; set; }
        public string cheque_number { get; set; }
        public string branch { get; set; }
    }


    //API 10
    //public class APIModel
    //{
    //    public int Type { get; set; }
    //    public string Message { get; set; }
    //    public APINO10Header Result { get; set; }
    //}

    public class APINO10Header
    {
        public List<APINO10Det> CrdList { get; set; }
    }

    public class APINO10Det
    {
        public string Code { get; set; }
        public string Desc { get; set; }

    }


}