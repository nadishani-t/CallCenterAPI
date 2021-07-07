using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WcfCallCenterAPI.Models;

namespace CallCenterAPI.Controllers
{
    public class HomeController : Controller
    {
        clsCommen obj = new clsCommen();
        public ActionResult Index()
        {
            return View();
        }

        //api no 01
        [HttpPost]
        public JsonResult RecoveryData(Root root)
        {
            // return Json(obj.getRecoveryData(root), JsonRequestBehavior.AllowGet);
            JsonResult result = Json(obj.getRecoveryData(root), JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = 86753090;
            return result;
        }

        //api no 02
        [HttpGet]
        public JsonResult getContractIDsByCustomerNumber(string customer_id)
        {
            return Json(obj.getContractIDsByCustomerNumber(customer_id), JsonRequestBehavior.AllowGet);
        }

        // API NO 03
        [HttpPost]
        public JsonResult getContractLedgersByContractNumbers(List<string> contracts)
        {
            return Json(obj.getContractLedgersByContractNumbers(contracts), JsonRequestBehavior.AllowGet);
        }

        // API NO 04
        [HttpGet]
        public JsonResult getCustomerDetailsByCustomerNumber(string customer_id)
        {
            return Json(obj.getCustomerDetailsByCustomerNumber(customer_id), JsonRequestBehavior.AllowGet);
        }

        // API NO 05
        [HttpGet]
        public JsonResult getPaymentScheduleByContractNumber(string contract_number)
        {
            return Json(obj.getPaymentScheduleByContractNumber(contract_number), JsonRequestBehavior.AllowGet);
        }

        // API NO 06
        [HttpGet]
        public JsonResult getArrearsBreakdownByContractNumber(string contract_number, string strUser)
        {
            return Json(obj.getArrearsBreakdownByContractNumber(contract_number, strUser), JsonRequestBehavior.AllowGet);
        }

        // API NO 07
        [HttpGet]
        public JsonResult getPDChequesByContractNumber(string contract_number)
        {
            return Json(obj.getPDChequesByContractNumber(contract_number), JsonRequestBehavior.AllowGet);
        }

        // API NO 08
        [HttpGet]
        public JsonResult getChequePaymentHistoryByContractNumber(string contract_number)
        {
            return Json(obj.getChequePaymentHistoryByContractNumber(contract_number), JsonRequestBehavior.AllowGet);
        }

        // API NO 09
        [HttpGet]
        public JsonResult getNotesByContractNumber(string contract_number)
        {
            return Json(obj.getNotesByContractNumber(contract_number), JsonRequestBehavior.AllowGet);
        }

        // api no 10
        [HttpGet]
        public JsonResult getCribStatusList()
        {
            return Json(obj.getCribStatusList(), JsonRequestBehavior.AllowGet);
        }

        //API NO 12
        [HttpGet]
        public JsonResult getNoteCodeList()
        {
            return Json(obj.getNoteCodeList(), JsonRequestBehavior.AllowGet);
        }

        //API NO 13
        [HttpPost]
        public JsonResult AddNoteToContract(Notes notes)
        {
            return Json(obj.AddNoteToContract(notes), JsonRequestBehavior.AllowGet);
        }

        // API NO 14
        [HttpGet]
        public JsonResult getBranchList()
        {
            return Json(obj.getBranchList(), JsonRequestBehavior.AllowGet);
        }


        //API NO 15
        [HttpGet]
        public JsonResult getCrdOffDetail(string strOffCode)
        {
            return Json(obj.getCrdOffDetail(strOffCode), JsonRequestBehavior.AllowGet);
        }




        //api no 16
        [HttpGet]
        public JsonResult getCustomerGuaranteedContractIDsByCustomerNumber(string customer_id)
        {
            return Json(obj.getCustomerGuaranteedContractIDsByCustomerNumber(customer_id), JsonRequestBehavior.AllowGet);
        }

        //API NO 17

        [HttpGet]
        public JsonResult getVehicleDetailsByContractNumber(string contract_number)
        {
            return Json(obj.getVehicleDetailsByContractNumber(contract_number), JsonRequestBehavior.AllowGet);
        }

        //API TEST NO 01
        [HttpGet]
        public JsonResult getBankCodeList()
        {
            return Json(obj.getBankCodeList(), JsonRequestBehavior.AllowGet);
        }

    }
}