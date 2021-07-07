using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfCallCenterAPI.Models
{
    public class MarketingCodeRange
    {
        public string start { get; set; }
        public string end { get; set; }
    }

    public class RentalAmountRange
    {
        public double min { get; set; }
        public double max { get; set; }
    }

    public class BalanceAmountRange
    {
        public double min { get; set; }
        public double max { get; set; }
    }

    public class ArrearsAmountRange
    {
        public double min { get; set; }
        public double max { get; set; }
    }

    public class InsuranceAmountRange
    {
        public double min { get; set; }
        public double max { get; set; }
    }

    public class ArrearsMonths
    {
        public double min { get; set; }
        public double max { get; set; }
    }

    public class CapitalOutstanding
    {
        public double min { get; set; }
        public double max { get; set; }
    }

    public class RentalDueRange
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
    }

    public class ActivatedDateRange
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
    }

    public class Root
    {
        public MarketingCodeRange marketing_code_range { get; set; }
        public RentalAmountRange rental_amount_range { get; set; }
        public BalanceAmountRange balance_amount_range { get; set; }
        public ArrearsAmountRange arrears_amount_range { get; set; }
        public InsuranceAmountRange insurance_amount_range { get; set; }
        public ArrearsMonths arrears_months { get; set; }
        public CapitalOutstanding capital_outstanding { get; set; }
        public RentalDueRange rental_due_range { get; set; }
        public ActivatedDateRange activated_date_range { get; set; }
    }
}