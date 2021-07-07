using CallCenterAPI.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using WcfCallCenterAPI.Models;

namespace CallCenterAPI
{
    public class clsCommen
    {
        private static string db = ConfigurationManager.ConnectionStrings["Connection String"].ConnectionString;
        public APINO6Header getArrearsBreakdownByContractNumber(string contract_number, string strUser)
        {
            string strResult = "";
            string strSQL = "";
            APINO6Header pay = new APINO6Header();

            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {

                    OracleCommand objCmd = new OracleCommand("AMSBACKUP.SP_REPPLHSEQ", con);

                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("SZCONTFROM", OracleDbType.Varchar2).Value = contract_number;
                    objCmd.Parameters.Add("SZCONTTO", OracleDbType.Varchar2).Value = contract_number;
                    objCmd.Parameters.Add("SZUSER", OracleDbType.Varchar2).Value = strUser;
                    objCmd.Parameters.Add("DTRDATE", OracleDbType.Date).Value = DateTime.Now.Date;

                    objCmd.ExecuteNonQuery();


                    strSQL = "SELECT T.SZCONTRACTNO, T.SZVOUCHER as \"voucher\"," +
                        "  CASE WHEN T.DTDATE IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( T.DTDATE,'yyyy:mm:dd hh:mi:ss') END AS \"date\" ," +
                        // "T.DTDATE as \"date\","+
                        " T.SZTYPE as \"type\",  " +
                        " T.SZDESC as \"narration\", T.DDBT as \"debit\", T.DCDT as \"credit\",( L.DBALANCE +T.DDBT ) - T.DCDT AS  balance ,   " +
                        " T.ICNT AS ISEQ FROM AMSBACKUP.TBLTEMPLDGR  T " +
                        " INNER JOIN AMSBACKUP.VW_LOANINFO L ON T.SZCONTRACTNO =L.SZCONTRACTNO " +
                      " WHERE T.SZUSERNAME = '" + strUser + "' ORDER BY T.SZCONTRACTNO, T.ROWID";

                    DataTable dsDet = new DataTable();

                    OracleCommand dbCommand = new OracleCommand(strSQL, con);
                    OracleDataAdapter da = new OracleDataAdapter(dbCommand);
                    da.Fill(dsDet);


                    List<Detail06> detailsList = new List<Detail06>();

                    if (dsDet.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsDet.Rows)
                        {
                            var detail = new Detail06()
                            {
                                type = row["type"].ToString(),
                                voucher = row["voucher"].ToString(),
                                date = row["date"].ToString(),
                                debit = double.Parse(row["debit"].ToString()),
                                credit = double.Parse(row["credit"].ToString()),
                                balance = double.Parse(row["balance"].ToString()),
                                narration = row["narration"].ToString(),


                            };
                            detailsList.Add(detail);
                        }
                    }

                    pay.contract_number = contract_number;

                    pay.Detail_Ledger = detailsList;

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return pay;
        }

       

        //API NO 01
        public APINO01Header getRecoveryData(Root Search)
        {
            string strSQL = "";
            APINO01Header contractList = new APINO01Header();

            DataTable dsSch = new DataTable();

            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {
                    strSQL = "SELECT  L.SZCUSTCODE AS customer_number , L.SZCONTRACTNO AS \"contract_number\" , " +
                        "CASE WHEN L.DTNXTRENTAL IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( L.DTNXTRENTAL,'yyyy:mm:dd hh:mi:ss') END AS \"due_date\" " +
                        " , L.DARREARS AS \"arrears_amount\", L.IRARRMONTHS AS \"arrears_months\" FROM AMSBACKUP.VW_LOANINFO L WHERE L.BOOLACTIVE=1 " +
                              " ";

                    if (Search.marketing_code_range != null)
                    {
                        if (Search.marketing_code_range.start != null && Search.marketing_code_range.end != null)
                        {
                            strSQL = QryBulder(strSQL, " SZCRDOFF BETWEEN '" + Search.marketing_code_range.start + "' AND '" + Search.marketing_code_range.end + "' ");
                        }
                    }
                    if (Search.rental_amount_range != null)
                    {
                        if (Search.rental_amount_range.min.ToString() != null && Search.rental_amount_range.max.ToString() != null)
                        {
                            strSQL = QryBulder(strSQL, "DRENTAL BETWEEN " + Search.rental_amount_range.min + " AND " + Search.rental_amount_range.max + " ");
                        }
                    }
                    if (Search.balance_amount_range != null)
                    {
                        if (Search.balance_amount_range.min.ToString() != null && Search.balance_amount_range.max.ToString() != null)
                        {
                            strSQL = QryBulder(strSQL, "DBALANCE BETWEEN " + Search.balance_amount_range.min + " AND  " + Search.balance_amount_range.max + " ");
                        }
                    }

                    if (Search.insurance_amount_range != null)
                    {
                        if (Search.insurance_amount_range.min.ToString() != null && Search.insurance_amount_range.max.ToString() != null)
                        {
                            strSQL = QryBulder(strSQL, "DINSARR BETWEEN " + Search.insurance_amount_range.min + " AND " + Search.insurance_amount_range.max + " ");
                        }
                    }

                    if (Search.arrears_months != null)
                    {
                        if (Search.arrears_months.min.ToString() != null && Search.arrears_months.max.ToString() != null)
                        {
                            strSQL = QryBulder(strSQL, "IRARRMONTHS BETWEEN " + Search.arrears_months.min + " AND " + Search.arrears_months.max + " ");
                        }
                    }

                    if (Search.capital_outstanding != null)
                    {
                        if (Search.capital_outstanding.min.ToString() != null && Search.capital_outstanding.max.ToString() != null)
                        {
                            strSQL = QryBulder(strSQL, "DCAPDUE BETWEEN " + Search.capital_outstanding.min + " AND " + Search.capital_outstanding.max + " ");
                        }
                    }

                    if (Search.rental_due_range != null)
                    {
                        if (Search.rental_due_range.startTime != null && Search.rental_due_range.endTime != null)
                        {
                            strSQL = QryBulder(strSQL, "DTNXTRENTAL BETWEEN TO_DATE('" + Search.rental_due_range.startTime + "','YYYY-MM-DD HH-MI-SS') AND TO_DATE('" + Search.rental_due_range.endTime + "','YYYY-MM-DD HH-MI-SS') ");
                        }
                    }

                    if (Search.activated_date_range != null)
                    {
                        if (Search.activated_date_range.startTime != null && Search.activated_date_range.endTime != null)
                        {
                            strSQL = QryBulder(strSQL, "DTACTDATE BETWEEN TO_DATE('" + Search.activated_date_range.startTime + "','YYYY-MM-DD HH-MI-SS') AND TO_DATE('" + Search.activated_date_range.endTime + "','YYYY-MM-DD HH-MI-SS') ");
                        }
                    }

                    OracleCommand dbCommand2 = new OracleCommand(strSQL, con);
                    OracleDataAdapter da2 = new OracleDataAdapter(dbCommand2);
                    da2.Fill(dsSch);

                    List<Detail01> detailsList2 = new List<Detail01>();
                    detailsList2.Clear();
                    if (dsSch.Rows.Count > 0)
                    {
                        foreach (DataRow row1 in dsSch.Rows)
                        {
                            var Detail01 = new Detail01()
                            {
                                customer_number = row1["customer_number"].ToString(),
                                contract_number = row1["contract_number"].ToString(),
                                due_date = row1["due_date"].ToString(),
                                arrears_amount = double.Parse(row1["arrears_amount"].ToString()),
                                arrears_months = int.Parse(row1["arrears_months"].ToString()),

                            };
                            detailsList2.Add(Detail01);
                        }
                    }
                    contractList.contracts = detailsList2;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return contractList;

        }


        // API NO 2
        public APINO02Header getContractIDsByCustomerNumber(string customer_id)
        {
            string strSQL = "";
            APINO02Header contractList = new APINO02Header() { customer_id = customer_id };

            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {
                    DataTable dsSch = new DataTable();

                    strSQL = "SELECT  L.SZCONTRACTNO AS Contracts FROM AMSBACKUP.VW_LOANINFO L " +
                      " WHERE L.SZCUSTCODE ='" + customer_id + "'   ";

                    OracleCommand dbCommand2 = new OracleCommand(strSQL, con);
                    OracleDataAdapter da2 = new OracleDataAdapter(dbCommand2);
                    da2.Fill(dsSch);


                    List<string> contractDetailsList = new List<string>();
                    if (dsSch.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsSch.Rows)
                        {
                            contractDetailsList.Add(row["Contracts"].ToString());
                        }
                    }

                    contractList.contracts = contractDetailsList;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return contractList;

        }


        //API NO 3
        public APILEDGER getContractLedgersByContractNumbers(List<string> contracts)
        {
            APILEDGER ledgh = new APILEDGER();
            if (contracts.Count > 0)
            {
                string strSQL = "";

                using (OracleConnection con = new OracleConnection(db))
                {
                    con.Open();
                    try
                    {
                        DataTable dsCus = new DataTable();
                        DataTable dsSch = new DataTable();
                        DataTable dsCrib = new DataTable();
                        DataTable dsCrd = new DataTable();

                        for (int i = 0; i < contracts.Count; i++)
                        {

                            strSQL = "SELECT L.SZCONTRACTNO AS contract_number , P.\"Customer Name\"  AS customer_name " +
                                " FROM AMSBACKUP.VW_LOANINFO L  " +
                                " INNER JOIN AMSBACKUP.VW_PROPOSAL P ON L.SZCONTRACTNO=P.\"Contract No\"  " +
                                " WHERE L.SZCONTRACTNO ='" + contracts[i] + "' ";


                            OracleCommand dbCommand = new OracleCommand(strSQL, con);
                            OracleDataAdapter da = new OracleDataAdapter(dbCommand);
                            da.Fill(dsCus);


                        }

                        //  List<LedgHead> detailsList2 = new List<LedgHead>();
                        List<LedgDet> detailsList = new List<LedgDet>();

                        if (dsCus.Rows.Count > 0)
                        {
                            foreach (DataRow row in dsCus.Rows)
                            {
                                strSQL = "SELECT  L.SZCONTRACTNO AS \"contract_number\",  P.\"Customer Name\"  AS hierer, DRENTAL AS \"rental\" , " +
                                    " CASE WHEN L.DTLASTREC IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( L.DTLASTREC,'yyyy:mm:dd hh:mi:ss') END AS \"Last_Receipt\" , " +
                                    " P.\"Asset\" AS \"details_vehicle_property\",  DARREARS AS \"arrears\", DINSUDUE AS \"insurance_due\", SZNOTES AS \"notes\" , " +
                                    " SZMRSEC AS \"moratorium_link\" , " +
                                    " CASE WHEN L.DTACTDATE IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( L.DTACTDATE,'yyyy:mm:dd hh:mi:ss') END  AS \"activated\" , " +
                                    " CASE WHEN L.DTFSTRENTAL IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( L.DTFSTRENTAL,'yyyy:mm:dd hh:mi:ss') END AS \"first_rental\" , " +
                                     " CASE WHEN L.DTNXTRENTAL IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( L.DTNXTRENTAL,'yyyy:mm:dd hh:mi:ss')  END AS \"next_rental\"," +
                                    " CASE WHEN L.DTPAYDATE IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( L.DTPAYDATE,'yyyy:mm:dd hh:mi:ss')  END AS \"payment_date\" , " +
                                    " DPREPAID AS \"pre_paids\"  , " +
                                    " DTYPE AS \"type\" , DREVRENTALS AS \"rentals_rvisd\" , DINTBALMOR AS \"mor_int_balance\", " +
                                    " DPERIOD AS \"Duration\" , (L.DRPOSTED- L.DPREPAID) AS \"rentals_posted\" , IOFFRNTLS AS \"shift_rentals\" ," +
                                    " DINSARR AS \"insurance\" , L.DPDCHQCNT AS \"pd_cheques\" ,  L.DCHEQUE AS \"unrealized_chqs\",L.DINTACCMOR AS \"moratorium_Int\" " +
                                    " FROM AMSBACKUP.VW_LOANINFO L " +
                                    "INNER JOIN AMSBACKUP.VW_PROPOSAL P ON L.SZCONTRACTNO=P.\"Contract No\" " +
                                    " WHERE L.SZCONTRACTNO ='" + row["contract_number"].ToString() + "' ";

                                dsSch.Clear();
                                OracleCommand dbCommand2 = new OracleCommand(strSQL, con);
                                OracleDataAdapter da2 = new OracleDataAdapter(dbCommand2);
                                da2.Fill(dsSch);

                                List<StatusCrib> detailsList2 = new List<StatusCrib>();

                                strSQL = "SELECT l.fstatus as \"description\" , l.szfstat as \"code\" FROM AMSBACKUP.VW_LOANINFO  L  " +
                                         " INNER JOIN AMSBACKUP.VW_PROPOSAL P ON L.SZCONTRACTNO = P.\"Contract No\" where L.SZCONTRACTNO ='" + row["contract_number"].ToString() + "'   ";

                                dsCrib.Clear();
                                OracleCommand dbCommand3 = new OracleCommand(strSQL, con);
                                OracleDataAdapter da3 = new OracleDataAdapter(dbCommand3);
                                da3.Fill(dsCrib);
                                if (dsCrib.Rows.Count > 0)
                                {
                                    foreach (DataRow row2 in dsCrib.Rows)
                                    {
                                        var StatusCrib = new StatusCrib()
                                        {
                                            code = row2["code"].ToString(),
                                            description = row2["description"].ToString(),

                                        };
                                        detailsList2.Add(StatusCrib);
                                    }
                                }


                                List<CrdRcvOfficer> detailsList4 = new List<CrdRcvOfficer>();
                                strSQL = "SELECT L.SZRCVOFF  AS \"code\" , SZMDETAIL AS \"Description\" FROM AMSBACKUP.VW_LOANINFO L INNER JOIN AMSBACKUP.TBLMASTER_DET M ON L.SZRCVOFF = M.SZFCODE WHERE M.SZMCODE='CRDO' AND " +
                                    " L.SZCONTRACTNO ='" + row["contract_number"].ToString() + "' ";

                                dsCrd.Clear();
                                OracleCommand dbCommand4 = new OracleCommand(strSQL, con);
                                OracleDataAdapter da4 = new OracleDataAdapter(dbCommand4);
                                da4.Fill(dsCrd);
                                if (dsCrd.Rows.Count > 0)
                                {
                                    foreach (DataRow row4 in dsCrd.Rows)
                                    {
                                        var CrdRcvOfficer = new CrdRcvOfficer()
                                        {
                                            code = row4["code"].ToString(),
                                            description = row4["Description"].ToString(),

                                        };
                                        detailsList4.Add(CrdRcvOfficer);
                                    }
                                }

                                if (dsSch.Rows.Count > 0)
                                {
                                    foreach (DataRow row1 in dsSch.Rows)
                                    {
                                        var LedgDet = new LedgDet()
                                        {
                                            contract_number = row1["contract_number"].ToString(),

                                            Status_Crib = detailsList2,

                                            customer_name = row1["hierer"].ToString(),
                                            rental = double.Parse(row1["rental"].ToString()),

                                            Last_Receipt = row1["Last_Receipt"].ToString(),
                                            details_vehicle_property = row1["details_vehicle_property"].ToString(),
                                            arrears = double.Parse(row1["arrears"].ToString()),
                                            insurance_due = double.Parse(row1["insurance_due"].ToString()),
                                            notes = row1["notes"].ToString(),
                                            moratorium_link = row1["moratorium_link"].ToString(),
                                            activated = row1["activated"].ToString(),
                                            first_rental = row1["first_rental"].ToString(),
                                            next_rental = row1["next_rental"].ToString(),

                                            payment_date = row1["payment_date"].ToString(),

                                            CrdRcvOfficer = detailsList4,

                                            pre_paids = double.Parse(row1["pre_paids"].ToString()),
                                            type = row1["type"].ToString(),
                                            rentals_rvisd = int.Parse(row1["rentals_rvisd"].ToString()),
                                            mor_int_balance = double.Parse(row1["mor_int_balance"].ToString()),
                                            duration = int.Parse(row1["duration"].ToString()),
                                            rentals_posted = int.Parse(row1["rentals_posted"].ToString()),
                                            shift_rentals = int.Parse(row1["shift_rentals"].ToString()),
                                            insurance = double.Parse(row1["insurance"].ToString()),
                                            pd_cheques = double.Parse(row1["pd_cheques"].ToString()),

                                            unrealized_chqs = double.Parse(row1["unrealized_chqs"].ToString()),
                                            moratorium_Int = double.Parse(row1["moratorium_Int"].ToString()),

                                        };
                                        detailsList.Add(LedgDet);
                                    }
                                }
                                ledgh.contract_Ledger = detailsList;
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }


            }
            return ledgh;

        }


        //API NO 4
        public APINO04Head getCustomerDetailsByCustomerNumber(string customer_id)
        {
            string strResult = "";
            string strSQL = "";
            APINO04Head pay = new APINO04Head();

            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {
                    strSQL = "SELECT  C.SZCUSTCODE AS \"customer_number\"  , C.SZFSTNAME AS \"customer_firstName\", " +
                      " C.SZLSTNAME AS \"customer_lastName\" ,C.SZNIC AS \"nic\" ,C.SZFULNAME AS \"customer_fullName\" ,  " +
                      " C.SZADD1    AS \"address\" , C.SZPOSTCODE AS \"postal_code\", C.SZDIST AS \"district\" , " +
                      " C.SZPROV AS \"province\",C.SZTPH AS \"home_phone\",SZTPM as \"mobile_phone\" , " +
                      " C.SZSMSNO AS \"sms_number\",C.SZGENDER AS \"gender\" ,C.SZMSTATUS AS \"marital_status\"  FROM  " +
                      " MILCOMMON.TBLCUSTOMER C   WHERE C.SZCUSTCODE ='" + customer_id + "' ";

                    DataTable dsCus = new DataTable();
                    DataTable dsCon = new DataTable();
                    DataTable dsGua = new DataTable();

                    OracleCommand dbCommand = new OracleCommand(strSQL, con);
                    OracleDataAdapter da = new OracleDataAdapter(dbCommand);
                    da.Fill(dsCus);

                    foreach (DataRow row in dsCus.Rows)
                    {
                        strSQL = "SELECT L.SZCONTRACTNO AS \"contract_number\" FROM AMSBACKUP.VW_LOANINFO L WHERE SZCUSTCODE='" + row["customer_number"].ToString() + "' ";
                        OracleCommand dbCommand2 = new OracleCommand(strSQL, con);
                        OracleDataAdapter da2 = new OracleDataAdapter(dbCommand2);
                        da2.Fill(dsCon);
                    }

                    List<hierer> detailsList = new List<hierer>();


                    if (dsCon.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsCon.Rows)
                        {
                            var hierer = new hierer()
                            {
                                contract_number = (row["contract_number"].ToString()),
                            };
                            detailsList.Add(hierer);

                            strSQL = "SELECT L.SZCONTRACTNO AS \"contract_number\", C.SZCUSTCODE AS \"customer_number\"  , C.SZFSTNAME AS \"customer_firstName\", " +
                              " C.SZLSTNAME AS \"customer_lastName\" ,C.SZNIC AS \"nic\" ,C.SZFULNAME AS \"customer_fullName\" ,  " +
                              " C.SZADD1 AS \"address\" , C.SZPOSTCODE AS \"postal_code\", C.SZDIST AS \"district\" , " +
                              " C.SZPROV AS \"province\",C.SZTPH AS \"home_phone\",C.SZTPM as \"mobile_phone\" , " +
                              " C.SZSMSNO AS \"sms_number\",C.SZGENDER AS \"gender\" ,C.SZMSTATUS AS \"marital_status\"  " +
                             " FROM AMSBACKUP.VW_LOANINFO L INNER JOIN  AMSBACKUP.VW_GUARANTOR G ON L.SZCONTRACTNO= G.SZCONTRACTNO " +
                             " INNER JOIN MILCOMMON.TBLCUSTOMER  C ON G.SZCUSTCODE = C.SZCUSTCODE WHERE L.SZCONTRACTNO ='" + row["contract_number"].ToString() + "' GROUP BY L.SZCONTRACTNO,C.SZCUSTCODE" +
                             " ,C.SZFSTNAME,C.SZLSTNAME,C.SZNIC,C.SZFULNAME,C.SZADD1,C.SZPOSTCODE,C.SZDIST,C.SZPROV,C.SZTPH,C.SZTPM,C.SZSMSNO,C.SZGENDER,C.SZMSTATUS ";

                            dsGua.Clear();
                            OracleCommand dbCommand3 = new OracleCommand(strSQL, con);
                            OracleDataAdapter da3 = new OracleDataAdapter(dbCommand3);
                            da3.Fill(dsGua);

                            List<guarantor> detailsList2 = new List<guarantor>();
                            detailsList2.Clear();
                            if (dsGua.Rows.Count > 0)
                            {
                                foreach (DataRow row1 in dsGua.Rows)
                                {

                                    var guarantor = new guarantor()
                                    {
                                        customer_number = row1["customer_number"].ToString(),
                                        customer_firstName = row1["customer_firstName"].ToString(),
                                        customer_lastName = row1["customer_lastName"].ToString(),
                                        nic = row1["nic"].ToString(),
                                        customer_fullName = row1["customer_fullName"].ToString(),
                                        address = row1["address"].ToString(),
                                        postal_code = row1["postal_code"].ToString(),
                                        home_phone = row1["home_phone"].ToString(),
                                        mobile_phone = row1["mobile_phone"].ToString(),
                                        sms_number = row1["sms_number"].ToString(),
                                    };
                                    detailsList2.Add(guarantor);
                                }
                            }

                            hierer.guarantor = detailsList2;
                        }
                    }

                    if (dsCus.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsCus.Rows)
                        {
                            pay.customer_firstName = row["customer_firstName"].ToString();
                            pay.customer_lastName = row["customer_lastName"].ToString();
                            pay.nic = row["nic"].ToString();
                            pay.customer_fullName = row["customer_fullName"].ToString();
                            pay.address = (row["address"].ToString());
                            pay.postal_code = (row["postal_code"].ToString());
                            pay.district = row["district"].ToString();
                            pay.province = row["province"].ToString();
                            pay.home_phone = row["home_phone"].ToString();
                            pay.mobile_phone = row["mobile_phone"].ToString();
                            pay.sms_number = row["sms_number"].ToString();
                            pay.gender = row["gender"].ToString();
                            pay.marital_status = row["marital_status"].ToString();
                            pay.contract_List = detailsList;

                        }
                    }
                    strResult = new JavaScriptSerializer().Serialize(pay);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return pay;
        }


        //API NO 05
        public PaymentModel getPaymentScheduleByContractNumber(string contract_number)
        {
            string strResult = "";
            string strSQL = "";
            PaymentModel pay = new PaymentModel();

            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {
                    strSQL = "SELECT L.SZCONTRACTNO AS contract_number , P.\"Customer Name\"  AS customer_name , " +
                        " L.DCASHPRICE AS cash_price , " +
                        " CASE WHEN SZCALCTYPE ='SD' THEN DCAPACT ELSE DCAPAMT END AS amount_financed ," +
                        " L.DDOWNPAY AS down_paymen ," +
                        " L.DEFFRATE AS effective_rate ," +
                        " L.DPERIOD AS period ," +
                        " L.DPREPAID AS pre_paids , " +
                        " L.DRENTAL AS rental ," +
                        " L.DINTAMT AS total_interest ," +
                        " L.DTYPE AS type " +
                        " FROM AMSBACKUP.VW_LOANINFO L  " +
                        " INNER JOIN AMSBACKUP.VW_PROPOSAL P ON L.SZCONTRACTNO=P.\"Contract No\"  " +
                        " WHERE L.SZCONTRACTNO ='" + contract_number + "' ";

                    DataTable dsCus = new DataTable();
                    DataTable dsSch = new DataTable();

                    OracleCommand dbCommand = new OracleCommand(strSQL, con);
                    OracleDataAdapter da = new OracleDataAdapter(dbCommand);
                    da.Fill(dsCus);

                    strSQL = "SELECT ISLAB AS slab, NVL(DCRENTAL,0) AS capital, NVL(DIRENTAL,0) AS interest," +
                        "(NVL(DCRENTAL,0) + NVL(DIRENTAL,0)) AS net,  NVL(DVAT,0) AS vat, (NVL(DCRENTAL,0) + NVL(DIRENTAL,0) + NVL(DVAT,0)) AS gross" +
                        " FROM AMSBACKUP.TBLSCHEDULE WHERE SZCONTRACTNO ='" + contract_number + "' order by ISLAB ";

                    OracleCommand dbCommand2 = new OracleCommand(strSQL, con);
                    OracleDataAdapter da2 = new OracleDataAdapter(dbCommand2);
                    da2.Fill(dsSch);


                    List<Detail> detailsList = new List<Detail>();
                    if (dsSch.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsSch.Rows)
                        {
                            var detail = new Detail()
                            {
                                slab = int.Parse(row["slab"].ToString()),
                                capital = double.Parse(row["capital"].ToString()),
                                interest = double.Parse(row["interest"].ToString()),
                                net = double.Parse(row["net"].ToString()),
                                vat = double.Parse(row["vat"].ToString()),
                                gross = double.Parse(row["gross"].ToString())
                            };
                            detailsList.Add(detail);
                        }
                    }

                    if (dsCus.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsCus.Rows)
                        {
                            pay.contract_number = row["contract_number"].ToString();
                            pay.customer_name = row["customer_name"].ToString();
                            pay.cash_price = double.Parse(row["cash_price"].ToString());
                            pay.amount_financed = double.Parse(row["amount_financed"].ToString());
                            pay.down_payment = double.Parse(row["down_paymen"].ToString());
                            pay.effective_rate = double.Parse(row["effective_rate"].ToString());
                            pay.period = int.Parse(row["period"].ToString());
                            pay.pre_paids = double.Parse(row["pre_paids"].ToString());
                            pay.rental = double.Parse(row["rental"].ToString());
                            pay.total_interest = double.Parse(row["total_interest"].ToString());
                            pay.type = int.Parse(row["type"].ToString());
                            pay.details = detailsList;
                        }
                    }
                    strResult = new JavaScriptSerializer().Serialize(pay);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return pay;
        }



        //API NO 07
        public APINO07Header getPDChequesByContractNumber(string contract_number)
        {
            string strResult = "";
            string strSQL = "";
            APINO07Header pay = new APINO07Header();
            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {
                    strSQL = "SELECT P.\"Contract No\" AS contract_number , P.\"Customer Name\"  AS  customer_name FROM AMSBACKUP.VW_PROPOSAL P" +
                        " WHERE P.\"Contract No\" ='" + contract_number + "' ";

                    DataTable dsCus = new DataTable();
                    DataTable dsSch = new DataTable();

                    OracleCommand dbCommand = new OracleCommand(strSQL, con);
                    OracleDataAdapter da = new OracleDataAdapter(dbCommand);
                    da.Fill(dsCus);

                    strSQL = "SELECT CASE WHEN DATE_ IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( DATE_,'yyyy:mm:dd hh:mi:ss') END  AS \"date\", PDCHQNO AS \"receipt_no\",  CHQNO AS \"cheque_no\" ,  " +
                    " 'cheque' AS \"mode\" ,nvl( AMOUNT,0) AS \"amount\" , " +
                    "   CASE WHEN DEPOSITED = 0 AND Cancel = 0 THEN 'Pending' " +
                    " WHEN DEPOSITED = 1 AND Cancel = 0 THEN 'Deposited' " +
                    " ELSE 'Cancelled' " +
                    " END AS \"status\" ,MEMO AS \"remark\" ,USER_ AS \"user\"   " +
                    " FROM RCB.RBPDCHQ " +
                    " WHERE REFNO ='" + contract_number + "' ";

                    OracleCommand dbCommand2 = new OracleCommand(strSQL, con);
                    OracleDataAdapter da2 = new OracleDataAdapter(dbCommand2);
                    da2.Fill(dsSch);


                    List<Detail07> detailsList = new List<Detail07>();
                    if (dsSch.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsSch.Rows)
                        {
                            var detail = new Detail07()
                            {
                                date = row["date"].ToString(),
                                receipt_no = row["receipt_no"].ToString(),
                                cheque_no = row["cheque_no"].ToString(),
                                mode = row["mode"].ToString(),
                                amount = double.Parse(row["amount"].ToString()),
                                status = row["status"].ToString(),
                                remark = row["remark"].ToString(),
                                user = row["user"].ToString(),
                            };
                            detailsList.Add(detail);
                        }
                    }

                    if (dsCus.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsCus.Rows)
                        {
                            pay.contract_number = row["contract_number"].ToString();
                            pay.customer_name = row["customer_name"].ToString();
                            pay.detail = detailsList;
                        }
                    }
                    strResult = new JavaScriptSerializer().Serialize(pay);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return pay;

        }



        // API 08
       public APINO08Header getChequePaymentHistoryByContractNumber(string contract_number)
        {
            string strResult = "";
            string strSQL = "";
            APINO08Header pay = new APINO08Header();

            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {
                    strSQL = "SELECT P.\"Contract No\" AS contract_number , P.\"Customer Name\"  AS  customer_name FROM AMSBACKUP.VW_PROPOSAL P" +
                        " WHERE P.\"Contract No\" ='" + contract_number + "' ";

                    DataTable dsCus = new DataTable();
                    DataTable dsSch = new DataTable();

                    OracleCommand dbCommand = new OracleCommand(strSQL, con);
                    OracleDataAdapter da = new OracleDataAdapter(dbCommand);
                    da.Fill(dsCus);

                    strSQL = "SELECT SZRECNO AS \"pd_cheque_number\" , " +
                     " CASE WHEN DTPROCESS IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( DTPROCESS,'yyyy:mm:dd hh:mi:ss') END  AS \"date\" , " +
                     " SZCUSTNAME AS \"customer\" ," +
                     " CASE WHEN DTDATE IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( DTDATE,'yyyy:mm:dd hh:mi:ss') END AS  \"deposited\" , " +
                     " SZCHQNO AS \"cheque_number\" , BRANCH_CODE AS \"branch\" " +
                     " FROM AMSBACKUP.VW_CONTCHQ_DET WHERE SZCONTRACTNO ='" + contract_number + "'  AND " +
                     " STATUS ='Realized' ";

                    OracleCommand dbCommand1 = new OracleCommand(strSQL, con);
                    OracleDataAdapter da1 = new OracleDataAdapter(dbCommand1);
                    da1.Fill(dsSch);


                    strSQL = "SELECT SZRECNO AS \"pd_cheque_number\" , " +
                   " CASE WHEN DTPROCESS IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( DTPROCESS,'yyyy:mm:dd hh:mi:ss') END  AS \"date\" , " +
                   " SZCUSTNAME AS \"customer\" ," +
                    " CASE WHEN DTDATE IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( DTDATE,'yyyy:mm:dd hh:mi:ss') END AS  \"deposited\" , " +
                   " SZCHQNO AS \"cheque_number\" , BRANCH_CODE AS \"branch\" " +
                   " FROM AMSBACKUP.VW_CONTCHQ_DET WHERE SZCONTRACTNO ='" + contract_number + "'  AND " +
                   " STATUS ='UnRealized' ";

                    OracleCommand dbCommand2 = new OracleCommand(strSQL, con);
                    OracleDataAdapter da2 = new OracleDataAdapter(dbCommand2);
                    da2.Fill(dsSch);


                    strSQL = "SELECT SZRECNO AS \"pd_cheque_number\" , " +
                " CASE WHEN DTPROCESS IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( DTPROCESS,'yyyy:mm:dd hh:mi:ss') END  AS \"date\" , " +
                 " SZCUSTNAME AS \"customer\" ," +
                   " CASE WHEN DTDATE IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( DTDATE,'yyyy:mm:dd hh:mi:ss') END AS  \"deposited\" , " +
                 " SZCHQNO AS \"cheque_number\" , BRANCH_CODE AS \"branch\" " +
                 " FROM AMSBACKUP.VW_CONTCHQ_DET WHERE SZCONTRACTNO ='" + contract_number + "' AND " +
                 " STATUS ='Returned' ";

                    OracleCommand dbCommand3 = new OracleCommand(strSQL, con);
                    OracleDataAdapter da3 = new OracleDataAdapter(dbCommand3);
                    da3.Fill(dsSch);

                    strSQL = "SELECT SZRECNO AS \"pd_cheque_number\" , " +
                  " CASE WHEN DTPROCESS IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( DTPROCESS,'yyyy:mm:dd hh:mi:ss') END  AS \"date\" , " +
                  " SZCUSTNAME AS \"customer\" ," +
                    " CASE WHEN DTDATE IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( DTDATE,'yyyy:mm:dd hh:mi:ss') END AS  \"deposited\" , " +
                  " SZCHQNO AS \"cheque_number\" , BRANCH_CODE AS \"branch\" " +
                  " FROM AMSBACKUP.VW_CONTCHQ_DET WHERE SZCONTRACTNO ='" + contract_number + "' AND " +
                  " STATUS ='Cancelled' ";

                    OracleCommand dbCommand4 = new OracleCommand(strSQL, con);
                    OracleDataAdapter da4 = new OracleDataAdapter(dbCommand4);
                    da4.Fill(dsSch);


                    List<Realized> detailsList1 = new List<Realized>();
                    List<UnRealized> detailsList2 = new List<UnRealized>();
                    List<Returned> detailsList3 = new List<Returned>();
                    List<Cancelled> detailsList4 = new List<Cancelled>();

                    if (dsSch.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsSch.Rows)
                        {
                            var Realized = new Realized()
                            {
                                pd_cheque_number = row["pd_cheque_number"].ToString(),
                                date = row["date"].ToString(),
                                customer = row["customer"].ToString(),
                                deposited = row["deposited"].ToString(),
                                cheque_number = row["cheque_number"].ToString(),
                                branch = row["branch"].ToString(),

                            };
                            detailsList1.Add(Realized);
                        }

                        foreach (DataRow row in dsSch.Rows)
                        {
                            var UnRealized = new UnRealized()
                            {
                                pd_cheque_number = row["pd_cheque_number"].ToString(),
                                date = row["date"].ToString(),
                                customer = row["customer"].ToString(),
                                deposited = row["deposited"].ToString(),
                                cheque_number = row["cheque_number"].ToString(),
                                branch = row["branch"].ToString(),

                            };
                            detailsList2.Add(UnRealized);
                        }

                        foreach (DataRow row in dsSch.Rows)
                        {
                            var Returned = new Returned()
                            {
                                pd_cheque_number = row["pd_cheque_number"].ToString(),
                                date = row["date"].ToString(),
                                customer = row["customer"].ToString(),
                                deposited = row["deposited"].ToString(),
                                cheque_number = row["cheque_number"].ToString(),
                                branch = row["branch"].ToString(),

                            };
                            detailsList3.Add(Returned);
                        }

                        foreach (DataRow row in dsSch.Rows)
                        {
                            var Cancelled = new Cancelled()
                            {
                                pd_cheque_number = row["pd_cheque_number"].ToString(),
                                date = row["date"].ToString(),
                                customer = row["customer"].ToString(),
                                deposited = row["deposited"].ToString(),
                                cheque_number = row["cheque_number"].ToString(),
                                branch = row["branch"].ToString(),

                            };
                            detailsList4.Add(Cancelled);
                        }
                    }

                    if (dsCus.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsCus.Rows)
                        {
                            pay.contract_number = row["contract_number"].ToString();
                            pay.customer_name = row["customer_name"].ToString();

                            pay.Realized = detailsList1;
                            pay.UnRealized = detailsList2;
                            pay.Returned = detailsList3;
                            pay.Cancelled = detailsList4;
                        }
                    }
                    strResult = new JavaScriptSerializer().Serialize(pay);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return pay;

        }


        //API NO 09
        public APINO09Header getNotesByContractNumber(string contract_number)
        {
            string strResult = "";
            string strSQL = "";
            APINO09Header pay = new APINO09Header();

            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {
                    strSQL = "SELECT P.\"Contract No\" AS contract_number , P.\"Customer Name\"  AS  customer_name FROM AMSBACKUP.VW_PROPOSAL P" +
                        " WHERE P.\"Contract No\" ='" + contract_number + "' ";

                    DataTable dsCus = new DataTable();
                    DataTable dsSch = new DataTable();

                    OracleCommand dbCommand = new OracleCommand(strSQL, con);
                    OracleDataAdapter da = new OracleDataAdapter(dbCommand);
                    da.Fill(dsCus);

                    strSQL = "SELECT N.ISEQNO AS \"seq_number\", " +
                        // " N.DTMEMODATE AS \"date\", "+
                        "CASE WHEN N.DTMEMODATE IS NULL THEN '0000:00:00 00:00:00' ELSE  TO_CHAR( N.DTMEMODATE,'yyyy:mm:dd hh:mi:ss') END AS \"date\" " +
                        " ,N.SZMEMO AS \"note\" , N.SZUSER AS \"user\" FROM AMSBACKUP.TBLCONTNOTES N " +
                        " WHERE SZCONTRACTNO ='" + contract_number + "' ";
                    OracleCommand dbCommand1 = new OracleCommand(strSQL, con);
                    OracleDataAdapter da1 = new OracleDataAdapter(dbCommand1);
                    da1.Fill(dsSch);


                    List<Detail09> detailsList = new List<Detail09>();

                    if (dsSch.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsSch.Rows)
                        {
                            var detail = new Detail09()
                            {
                                seq_number = int.Parse(row["seq_number"].ToString()),
                                date = row["date"].ToString(),
                                note = row["note"].ToString(),
                                user = row["user"].ToString(),

                            };
                            detailsList.Add(detail);
                        }
                    }

                    if (dsCus.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsCus.Rows)
                        {
                            pay.contract_number = row["contract_number"].ToString();
                            pay.customer_name = row["customer_name"].ToString();
                            pay.Detail = detailsList;
                        }
                    }
                    strResult = new JavaScriptSerializer().Serialize(pay);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return pay;

        }



        // API 10
        public APINO10Header getCribStatusList()
        {
            string strResult = "";
            string strSQL = "";
            APINO10Header pay = new APINO10Header();

            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {
                    strSQL = "SELECT SZFCODE AS \"Code\",SZMDETAIL AS \"Desc\" FROM AMSBACKUP.TBLMASTER_DET WHERE SZMCODE='CFS' ";
                    OracleCommand dbCommand1 = new OracleCommand(strSQL, con);

                    DataTable dsCus = new DataTable();

                    OracleCommand dbCommand = new OracleCommand(strSQL, con);
                    OracleDataAdapter da = new OracleDataAdapter(dbCommand);
                    da.Fill(dsCus);

                    List<APINO10Det> DetailList = new List<APINO10Det>();

                    if (dsCus.Rows.Count > 0)
                    {
                        foreach (DataRow row2 in dsCus.Rows)
                        {
                            var APINO10Det = new APINO10Det()
                            {
                                Code = row2["Code"].ToString(),
                                Desc = row2["Desc"].ToString(),

                            };
                            DetailList.Add(APINO10Det);
                        }
                        pay.CrdList = DetailList;
                    }
                    else
                    {
                        APIModel apiModel = new APIModel { Type = 0, Message = "Fail", Result = null };
                        strResult = new JavaScriptSerializer().Serialize(apiModel);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return pay;

        }



        // API 12
        public APINO12Header getNoteCodeList()
        {
            // using APINO12MO
            string strResult = "";
            string strSQL = "";
            APINO12Header contractList = new APINO12Header();


            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {
                    strSQL = "SELECT N.szNCode AS NoteCode , N.SZDESC  AS  Descr FROM AMSBACKUP.TBLNOTES N";

                    DataTable dsCus = new DataTable();
                    DataTable dsSch = new DataTable();

                    OracleCommand dbCommand = new OracleCommand(strSQL, con);
                    OracleDataAdapter da = new OracleDataAdapter(dbCommand);
                    da.Fill(dsCus);
                    List<Detail12> detailsList = new List<Detail12>();

                    if (dsCus.Rows.Count > 0)
                    {

                        foreach (DataRow row in dsCus.Rows)
                        {
                            var detail = new Detail12()
                            {
                                NoteCode = row["NoteCode"].ToString(),
                                Descr = row["Descr"].ToString(),
                            };
                            detailsList.Add(detail);
                        }
                    }

                    contractList.Detail = detailsList;

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return contractList;

        }


        // API NO 13
        public APINO13Header AddNoteToContract(Notes Notes)
        {
            string strSQL = "";
            string strResult = "";
            APINO13Header pay = new APINO13Header();
            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {
                    int iSq = 0;

                    strSQL = "SELECT NVL(MAX(ISEQNO),0) + 1 AS iSeq FROM AMSBACKUP.TBLCONTNOTES WHERE SZCONTRACTNO = '" + Notes.contract_number + "' ";

                    DataTable dsTbl = new DataTable();

                    OracleCommand dbCommand = new OracleCommand(strSQL, con);
                    OracleDataAdapter da = new OracleDataAdapter(dbCommand);
                    da.Fill(dsTbl);
                    if (dsTbl.Rows.Count > 0)
                    {
                        iSq = Convert.ToInt16(dsTbl.Rows[0]["iSeq"].ToString());
                    }

                    strSQL = "INSERT INTO AMSBACKUP.TBLCONTNOTES " +
                    "  (ISEQNO, SZCONTRACTNO, IMEMOTYPE, DTMEMODATE,SZMEMO,SZUSER) " +
                   "  VALUES  (" + iSq + ", '" + Notes.contract_number + "',1, '" + DateTime.Now.ToString("dd-MMM-yyyy") + "' ,'" + Notes.strNote + "' ,'" + Notes.strUser + "') ";

                    OracleCommand cmdInsert = new OracleCommand(strSQL, con);
                    cmdInsert.ExecuteNonQuery();

                    pay.Status = "OK";
                    pay.Description = "Successfully Updated";

                }
                catch (Exception ex)
                {
                    pay.Status = Notes.strNote;
                    pay.Description = "Updation Error";
                }
                strResult = new JavaScriptSerializer().Serialize(pay);
            }


            return pay;
        }


        //API NO 14
        public APINO14Header getBranchList()
        {
            string strSQL = "";
            APINO14Header contractList = new APINO14Header();

            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {
                    strSQL = "SELECT SZBRANCH_CODE AS BrCode , B.SZBRANCH_NAME AS Descr FROM MILCOMMON.TBLBRANCH B ORDER BY SZBRANCH_CODE ";

                    DataTable dsCus = new DataTable();
                    DataTable dsSch = new DataTable();

                    OracleCommand dbCommand = new OracleCommand(strSQL, con);
                    OracleDataAdapter da = new OracleDataAdapter(dbCommand);
                    da.Fill(dsCus);
                    List<Detail14> detailsList = new List<Detail14>();

                    if (dsCus.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsCus.Rows)
                        {
                            var detail = new Detail14()
                            {
                                BrCode = row["BrCode"].ToString(),
                                Descr = row["Descr"].ToString(),
                            };
                            detailsList.Add(detail);
                        }
                    }

                    contractList.Detail = detailsList;

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return contractList;

        }



        // API NO 15
        public APINO15Header getCrdOffDetail(string strOffCode)
        {
            string strResult = "";
            string strSQL = "";
            APINO15Header pay = new APINO15Header();

            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {

                    strSQL = "SELECT M.SZMDETAIL AS \"Name\" , U.SZMOBILENO AS \"Mobile_No\"  FROM AMSBACKUP.TBLMASTER_DET M " +
                        " LEFT JOIN AMSBACKUP.TBLCRMGROUP G ON M.SZFCODE =G.SZFCODE  " +
                        " LEFT JOIN MILCOMMON.TBLUSERS U ON G.SZLOGINNAME= U.SZLOGINNAME " +
                        " WHERE M.SZMCODE='CRDO'  AND M.SZFCODE='" + strOffCode + "' ";

                    //  OracleCommand dbCommand1 = new OracleCommand(strSQL, con);
                    DataTable dsCus = new DataTable();

                    OracleCommand dbCommand = new OracleCommand(strSQL, con);
                    OracleDataAdapter da = new OracleDataAdapter(dbCommand);
                    da.Fill(dsCus);

                    List<Detail15> DetailList = new List<Detail15>();

                    if (dsCus.Rows.Count > 0)
                    {
                        foreach (DataRow row2 in dsCus.Rows)
                        {
                            var Detail15 = new Detail15()
                            {
                                Name = row2["Name"].ToString(),
                                Mobile_No = row2["Mobile_No"].ToString(),

                            };
                            DetailList.Add(Detail15);
                        }
                        pay.Detail = DetailList;
                    }
                    else
                    {
                        APIModel apiModel = new APIModel { Type = 0, Message = "Fail", Result = null };
                        strResult = new JavaScriptSerializer().Serialize(apiModel);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return pay;

        }


        //apI NO 16
        public APINO16Header getCustomerGuaranteedContractIDsByCustomerNumber(string customer_id)
        {
            string strSQL = "";
            APINO16Header contractList = new APINO16Header() { customer_id = customer_id };

            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {
                    DataTable dsSch = new DataTable();

                    strSQL = "SELECT  G.SZCONTRACTNO AS Contracts FROM AMSBACKUP.VW_GUARANTOR G " +
                      " WHERE G.SZCUSTCODE ='" + customer_id + "'   ";

                    OracleCommand dbCommand2 = new OracleCommand(strSQL, con);
                    OracleDataAdapter da2 = new OracleDataAdapter(dbCommand2);
                    da2.Fill(dsSch);


                    List<string> contractDetailsList = new List<string>();
                    if (dsSch.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsSch.Rows)
                        {
                            contractDetailsList.Add(row["Contracts"].ToString());
                        }
                    }

                    contractList.contracts = contractDetailsList;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return contractList;

        }


        /// <summary>
        /// API API NO 17
        public APINO17Header getVehicleDetailsByContractNumber(string contract_number)
        {
            string strResult = "";
            string strSQL = "";
            APINO17Header pay = new APINO17Header();

            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {

                    strSQL = "SELECT registration_number,vehicle_class "+
                        " ,  make,model,manufacture_des,"+
                        "  color  FROM AMSBACKUP.VW_3CXSECURITY M WHERE M.SZCONTRACTNO='" + contract_number + "'";

                    DataTable dsDet = new DataTable();

                    OracleCommand dbCommand = new OracleCommand(strSQL, con);
                    OracleDataAdapter da = new OracleDataAdapter(dbCommand); 
                    da.Fill(dsDet);


                    List<Detail17> detailsList = new List<Detail17>();

                    if (dsDet.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsDet.Rows)
                        {
                            var detail = new Detail17()
                            {
                                registration_number = row["registration_number"].ToString(),
                                vehicle_class = row["vehicle_class"].ToString(),
                                make = row["make"].ToString(),
                                model = (row["model"].ToString()),
                                manufacture_des = (row["manufacture_des"].ToString()),
                                color = (row["color"].ToString()),

                            };
                            detailsList.Add(detail);
                        }
                    }

                    pay.contract_number = contract_number;

                    pay.Detail_Ledger = detailsList;

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return pay;
        }

        // API API TEST NO 1
        public API_TEST01Header getBankCodeList()
        {
            string strResult = "";
            string strSQL = "";
            API_TEST01Header bankList = new API_TEST01Header();


            using (OracleConnection con = new OracleConnection(db))
            {
                con.Open();
                try
                {
                    strSQL = "SELECT B.SZBANKCODE AS BankCode , B.SZBANKNAME  AS  Name FROM MILCOMMON.TBLBANKS B";

                    DataTable dsCus = new DataTable();
                    //DataTable dsSch = new DataTable();

                    OracleCommand dbCommand = new OracleCommand(strSQL, con);
                    OracleDataAdapter da = new OracleDataAdapter(dbCommand);
                    da.Fill(dsCus);
                    List<Detail_T2> detailsList = new List<Detail_T2>();

                    if (dsCus.Rows.Count > 0)
                    {
                        foreach (DataRow row in dsCus.Rows)
                        {
                            var detail = new Detail_T2()
                            {
                                BankCode = row["BankCode"].ToString(),
                                BankName = row["Name"].ToString(),
                            };
                            detailsList.Add(detail);
                        }
                    }

                    bankList.Detail = detailsList;

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return bankList;

        }



        private string QryBulder(string strQry, string strQryAdd)
        {
            if (strQry.IndexOf("WHERE") == -1)
                strQry += " WHERE " + strQryAdd;
            else
                strQry += " AND " + strQryAdd;

            return strQry;
        }

    }
}