using IWESS.Models;
using IWESS.ViewModel;
using IWNet;
using IWNet.DataAccess;
using IWNet.Utils.JSON;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Mvc;
using System.Web.Security;

namespace IWESS.Controllers
{
    public class OperationController : Controller
    {
        private readonly CFunctionCall _functionCall;
        public OperationController()
        {
            _functionCall = new CFunctionCall();
            //CSessionManager.IWSessionID = "Token67890";
        }

        private readonly CDataConversion _data = new CDataConversion();
        //private int _logged;
        // GET: Operation
        public ActionResult Index()
        {
            if (!CSessionManager.IsIWSessionExists())
            {
                ViewBag.Status = CSessionManager.LastError;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(VMLogin model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            try
            {
                // string url = string.Format("{0}/{1}/{2}/{3}/{4}"
                //, ConfigurationManager.AppSettings["URL"]
                //, "ESSLogin"
                //, ConfigurationManager.AppSettings["Token"]
                //, model.Username
                //, model.Password);
                // string json_data = Common.HTMLHelper.GetHtmlPage(url);
                // Out_ResultD result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
                // if (result.StatusID == IWAPIEnum_APIStatus.NoError)
                // {
                //     Session["SessionID"] = result.OutValue.ToString();
                //     var dataTable = CJSONHelper.ConvertToDatatable(result.DataTable);
                //     Session["UserID"] = model.Username;
                //     Session["UserInfo"] = _data.User(dataTable);
                //     return RedirectToAction("Dashboard");
                // }
                // else
                // {
                //     ViewBag.Status = "Invalid Credentials";
                //     return View(model);
                // }

                _functionCall.AppLogin(model.Username, model.Password);
                var usr = _functionCall.ESSLogin(CSessionManager.IWSessionID, model.Username, model.Password);
                if (usr.IsExist)
                {
                    FormsAuthentication.SetAuthCookie(usr.UserID, true);
                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Dashboard", "Operation");


                    //}

                }
                else
                {
                    //CSessionManager.LastError = "Invalid Credentials!";
                    CSessionManager.LastError = usr.StatusMessage;
                    ViewBag.Status = CSessionManager.LastError;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Status = ex.Message + " Contact the Administrator";
                return View();
            }

            return View();
        }

        public ActionResult Dashboard(VMLogin model)
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");
            var data = GetData("HRESSGetEmpInfo", CSessionManager.IWSessionID, CSessionManager.LoggedInUser.UserID);
            var info = new CEmployeeInfo();
            foreach (DataRow row in data.Rows)
            {

                info.EmpID = Common.ConvertEx.ToString(row["EmpID"]);
                info.Name = Common.ConvertEx.ToString(row["Name"]);
                info.Title = Common.ConvertEx.ToString(row["Title"]);
                info.Gender = Common.ConvertEx.ToString(row["Gender"]);
                info.City = Common.ConvertEx.ToString(row["City"]);
                info.State = Common.ConvertEx.ToString(row["State"]);
                info.Country = Common.ConvertEx.ToString(row["Country"]);
                info.Email = Common.ConvertEx.ToString(row["Email1"]);
                info.Phone = Common.ConvertEx.ToString(row["Phone1"]);
                info.Birthday = Common.ConvertEx.ToDateTime(row["Birthday"]).ToString(Common.DateHelper.FMT_DT);
                info.Nationality = Common.ConvertEx.ToString(row["Nationality"]);
            };

            var lastLogonDate = GetCommandDataParam("ESS_006", CSessionManager.IWSessionID, info.EmpID);
            string LogDate = null;
            foreach (DataRow row in lastLogonDate.Rows)
            {
                LogDate = Common.ConvertEx.ToDateTime(row["LastLogonDate"]).ToString(Common.DateHelper.FMT_DT);
            }

            string TodayDate = Common.ConvertEx.ToDateTime(DateTime.Today.ToShortDateString()).ToString(Common.DateHelper.FMT_DT);

            if (LogDate == TodayDate)
            {
                ViewBag.Message = true;
            }
            else
            {
                ViewBag.Message = false;
            }
            return View();
        }
        public ActionResult EmployeeInfo()
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");
            try
            {
                ViewData["Info"] = _data.EmployeeInfo(GetData("HRESSGetEmpInfo",  CSessionManager.IWSessionID, CSessionManager.LoggedInUser.UserID));
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }
        public ActionResult JobInfo()
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");
            try
            {
                ViewData["Info"] = _data.JobHistory(GetData("HRESSGetJobHistory", CSessionManager.IWSessionID, (string)Session["UserID"]));
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }
        public ActionResult BankInfo()
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");
            try
            {
                ViewData["Info"] = _data.BankInfo(GetData("HRESSGetBankInfo", CSessionManager.IWSessionID, (string)Session["UserID"]));
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public ActionResult Addition()
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");

            try
            {
                ViewData["ESSLNKS"] = _data.Addition(GetCommandData("ESS_0012", CSessionManager.IWSessionID));

                return View();
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
           
        }

        public ActionResult PayStubList()
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");
            try
            {

                var data = GetData("HRESSGetEmpInfo", CSessionManager.IWSessionID, CSessionManager.LoggedInUser.UserID);
                //var empID = _data.EmployeeInfo(GetData("HRESSGetEmpInfo", CSessionManager.IWSessionID, CSessionManager.LoggedInUser.UserID))
                var info = new CEmployeeInfo();
                foreach (DataRow row in data.Rows)
                {

                    info.EmpID = Common.ConvertEx.ToString(row["EmpID"]);
                    info.Name = Common.ConvertEx.ToString(row["Name"]);
                    info.Title = Common.ConvertEx.ToString(row["Title"]);
                    info.Gender = Common.ConvertEx.ToString(row["Gender"]);
                    info.City = Common.ConvertEx.ToString(row["City"]);
                    info.State = Common.ConvertEx.ToString(row["State"]);
                    info.Country = Common.ConvertEx.ToString(row["Country"]);
                    info.Email = Common.ConvertEx.ToString(row["Email1"]);
                    info.Phone = Common.ConvertEx.ToString(row["Phone1"]);
                    info.Birthday = Common.ConvertEx.ToDateTime(row["Birthday"]).ToString(Common.DateHelper.FMT_DT);
                    info.Nationality = Common.ConvertEx.ToString(row["Nationality"]);
                };

                ViewBag.EmpID = info.EmpID;
                ViewData["Info"] = _data.SalaryDetails(GetData("HRESSGetPayStubList", CSessionManager.IWSessionID, (string)Session["EmpID"]));
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }


        public ActionResult PayStubDetails(string EmpDate, string empID)
        {
            if(!CSessionManager.IsLoggedIn())
            {
                return RedirectToAction("Index");
            }
            try
            {
                ViewBag.PayDate = EmpDate;
                var param = IWServices.CreateParameters(EmpDate, empID);
                ViewData["NetPay"] = _data.NetPayWord(GetCommandDataParam("P_00062", CSessionManager.IWSessionID, param));
                ViewData["GrossDeductions"] = _data.GrossEarnings(GetCommandDataParam("P_00064", CSessionManager.IWSessionID, param));
                ViewData["GrossEarnings"] = _data.GrossEarnings(GetCommandDataParam("P_00063", CSessionManager.IWSessionID, param));
                ViewData["EmpPay"] = _data.EmployeePay(GetCommandDataParam("P_00065", CSessionManager.IWSessionID, param));
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }

        }

        public ActionResult Qualify()
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");
            try
            {
                ViewData["Info"] = _data.Qualification(GetData("HRESSGetQualifications", CSessionManager.IWSessionID, (string)Session["UserID"]));
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }
        public ActionResult History()
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");
            try
            {

                var usr = _functionCall.GetEmployeeInfo(CSessionManager.IWSessionID);
                var employee = new CEmployeeInfo();
                foreach (DataRow rw in usr.Rows)
                {
                    employee.EmpID = rw["EmpID"].ToString();
                    employee.Name = rw["Name"].ToString();
                    employee.Email = rw["Email1"].ToString();
                    employee.City = rw["City"].ToString();
                    employee.Country = rw["Country"].ToString();
                    employee.Birthday = rw["Birthday"].ToString();
                    employee.Gender = rw["Gender"].ToString();
                    employee.Nationality = rw["Nationality"].ToString();
                    employee.Title = rw["Title"].ToString();
                    employee.Phone = rw["Phone1"].ToString();
                    employee.State = rw["State"].ToString();
                    
                }
                var EmpID = employee.EmpID;
                ViewData["History"] = _data.TimeOffHistory(GetCommandDataParam("ESS_001", CSessionManager.IWSessionID, EmpID));
                ViewData["AnnualLeave"] = _data.AnnualLeaveTaken(GetCommandDataParam("ESS_004", CSessionManager.IWSessionID, EmpID));
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }

        }
        public ActionResult MyRequests()
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");
            else
            {
                var usr = _functionCall.GetEmployeeInfo(CSessionManager.IWSessionID);
                var employee = new CEmployeeInfo();
                foreach (DataRow rw in usr.Rows)
                {
                    employee.EmpID = rw["EmpID"].ToString();
                    employee.Name = rw["Name"].ToString();
                    employee.Email = rw["Email1"].ToString();
                    employee.City = rw["City"].ToString();
                    employee.Country = rw["Country"].ToString();
                    employee.Birthday = rw["Birthday"].ToString();
                    employee.Gender = rw["Gender"].ToString();
                    employee.Nationality = rw["Nationality"].ToString();
                    employee.Title = rw["Title"].ToString();
                    employee.Phone = rw["Phone1"].ToString();
                    employee.State = rw["State"].ToString();

                }
                var EmpID = employee.EmpID;
                ViewData["AnnualLeave"] = _data.AnnualLeaveTaken(GetCommandDataParam("ESS_004", CSessionManager.IWSessionID, EmpID));
                ViewData["Type"]= _data.TimeOffType( GetCommandData("ESS_002", CSessionManager.IWSessionID));
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyRequests(VMRequestTO request)
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");
            try
            {
                ViewData["Type"] = _data.TimeOffType(GetCommandData("ESS_002", CSessionManager.IWSessionID));
                if (!ModelState.IsValid)
                {
                    return View(request);
                }
                if ( request.EndDate < request.StartDate)
                {
                    ViewBag.Error = "Start Date is less than Current date or End Date is less than StartDate";
                    return View(request);
                }
                var usr = _functionCall.GetEmployeeInfo(CSessionManager.IWSessionID);
                var employee = new CEmployeeInfo();
                foreach (DataRow rw in usr.Rows)
                {
                    employee.EmpID = rw["EmpID"].ToString();
                    employee.Name = rw["Name"].ToString();
                    employee.Email = rw["Email1"].ToString();
                    employee.City = rw["City"].ToString();
                    employee.Country = rw["Country"].ToString();
                    employee.Birthday = rw["Birthday"].ToString();
                    employee.Gender = rw["Gender"].ToString();
                    employee.Nationality = rw["Nationality"].ToString();
                    employee.Title = rw["Title"].ToString();
                    employee.Phone = rw["Phone1"].ToString();
                    employee.State = rw["State"].ToString();

                }
                var EmpID = employee.EmpID;

                //var param = IWServices.CreateParameters("-1", CSessionManager.IWSessionID, EmpID, request.Type, Common.FormatEx.FormatDT(request.StartDate), Common.FormatEx.FormatDT(request.EndDate), "", "", "", "Pending", request.Comment, "", "0");
                var param = IWServices.CreateParameters("-1", EmpID, request.Type, Common.FormatEx.FormatDT(request.StartDate), Common.FormatEx.FormatDT(request.EndDate), DateTime.Now.Year.ToString(), "", "", "Pending", request.Comment, "", "0");

                var done = GetCommandDataParam("ESS_003", CSessionManager.IWSessionID, param);
                //if (!string.IsNullOrEmpty(done.ToString()))
                //{
                //    ViewBag.ErrorMessage = "Sorry your request cannot be processed at this time, please contact HR";
                //    return View("MyRequests");
                //}
                return RedirectToAction("History");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public ActionResult PerformanceAppraisal()
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");

            return View();
        }


        public ActionResult PerfAppraisal()
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");

            return View();
        }

        public ActionResult ViewCommittments()
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");
            CEmployeePoints model = new CEmployeePoints();
            try
            {
                var usr = _functionCall.GetEmployeeInfo(CSessionManager.IWSessionID);
                var employee = new CEmployeeInfo();
                foreach (DataRow rw in usr.Rows)
                {
                    employee.EmpID = rw["EmpID"].ToString();

                }
                var EmpID = employee.EmpID;
                var GetUsr = GetCommandDataParam("P_00050",CSessionManager.IWSessionID, EmpID);
                var GetID = new CEmployeeID();
                foreach (DataRow rw in GetUsr.Rows)
                {
                    GetID.ID = rw["ID#"].ToString();

                }

                var MyEmpID = GetID.ID;
                ViewBag.MyEmpID = MyEmpID;
                Session["MyEmpID"] = MyEmpID;
                string AppraisalPeriodStartDate = "";
                string AppraisalPeriodEndDate = "";
                ViewBag.Commitments = GetCommandDataParam("P_00041", CSessionManager.IWSessionID, MyEmpID);
                ViewBag.EmployeeGP = _functionCall.GetEmployeeGP(CSessionManager.IWSessionID, MyEmpID, AppraisalPeriodStartDate, AppraisalPeriodEndDate);
                ViewBag.EmployeeCGPA = _functionCall.GetEmployeeCGPA(CSessionManager.IWSessionID, MyEmpID);
                ViewData["Surbodinates"] = _data.GetSurbordinates(GetCommandDataParam("ESS_005", CSessionManager.IWSessionID, EmpID));
                model.StartDate = string.Format("{0}-01-01", DateTime.Today.Year);
                model.EndDate = string.Format("{0}-12-31", DateTime.Today.Year);


            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ViewCommittments(CEmployeePoints model)
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");

            try
            {
                var usr = _functionCall.GetEmployeeInfo(CSessionManager.IWSessionID);
                var employee = new CEmployeeInfo();
                foreach (DataRow rw in usr.Rows)
                {
                    employee.EmpID = rw["EmpID"].ToString();

                }
                var EmpID = employee.EmpID;
                ViewBag.EmpID = EmpID;
                //ViewData["EmpID"] = EmpID;
                var GetUsr = GetCommandDataParam("P_00050", CSessionManager.IWSessionID, model.EmpID);
                var GetID = new CEmployeeID();
                foreach (DataRow rw in GetUsr.Rows)
                {
                    GetID.ID = rw["ID#"].ToString();

                }

                var MyEmpID = GetID.ID;
                ViewBag.MyEmpID = MyEmpID;
                Session["MyEmpID"] = MyEmpID;
                //var Committments = GetCommandDataParam("P_00041", CSessionManager.IWSessionID, MyEmpID);
                ViewData["Committments"] = _data.Committment(GetCommandDataParam("P_00041", CSessionManager.IWSessionID, MyEmpID));
                ViewBag.Commitments = GetCommandDataParam("P_00041", CSessionManager.IWSessionID, MyEmpID);

                ViewBag.EmployeeGP = _functionCall.GetEmployeeGP(CSessionManager.IWSessionID, MyEmpID, model.StartDate, model.EndDate);
                ViewBag.EmployeeCGPA = _functionCall.GetEmployeeCGPA(CSessionManager.IWSessionID, MyEmpID);
                //var param = IWServices.CreateParameters(EmpID);
                ViewData["Surbodinates"] = _data.GetSurbordinates(GetCommandDataParam("ESS_005", CSessionManager.IWSessionID, EmpID));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
            return View(model);
        }

        public ActionResult ViewSurvey()
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");

            try
            {
                var usr = _functionCall.GetEmployeeInfo(CSessionManager.IWSessionID);
                var employee = new CEmployeeInfo();
                foreach (DataRow rw in usr.Rows)
                {
                    employee.EmpID = rw["EmpID"].ToString();

                }
                var EmpID = employee.EmpID;
                var GetUsr = GetCommandDataParam("P_00050", CSessionManager.IWSessionID, EmpID);
                var GetID = new CEmployeeID();
                foreach (DataRow rw in GetUsr.Rows)
                {
                    GetID.ID = rw["ID#"].ToString();

                }

                var MyEmpID = GetID.ID;

                ViewData["Surveys"] = _data.Surveys(GetCommandDataParam("P_00048", CSessionManager.IWSessionID, MyEmpID));
                ViewData["Surbodinates"] = _data.GetSurbordinates(GetCommandDataParam("ESS_005", CSessionManager.IWSessionID, EmpID));

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }

            return View();
        }


        [HttpPost]
        public ActionResult ViewSurvey(string empID)
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");

            try
            {
                var usr = _functionCall.GetEmployeeInfo(CSessionManager.IWSessionID);
                var employee = new CEmployeeInfo();
                foreach (DataRow rw in usr.Rows)
                {
                    employee.EmpID = rw["EmpID"].ToString();

                }
                var EmpID = employee.EmpID;
                var GetUsr = GetCommandDataParam("P_00050", CSessionManager.IWSessionID, empID);
                var GetID = new CEmployeeID();
                foreach (DataRow rw in GetUsr.Rows)
                {
                    GetID.ID = rw["ID#"].ToString();

                }

                var MyEmpID = GetID.ID;

                ViewData["Surveys"] = _data.Surveys(GetCommandDataParam("P_00048", CSessionManager.IWSessionID, MyEmpID));
                ViewData["Surbodinates"] = _data.GetSurbordinates(GetCommandDataParam("ESS_005", CSessionManager.IWSessionID, EmpID));

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }

            return View();
        }

        public ActionResult ViewAppraisals()
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");

            try
            {
                var usr = _functionCall.GetEmployeeInfo(CSessionManager.IWSessionID);
                var employee = new CEmployeeInfo();
                foreach (DataRow rw in usr.Rows)
                {
                    employee.EmpID = rw["EmpID"].ToString();

                }
                var EmpID = employee.EmpID;
                var GetUsr = GetCommandDataParam("P_00050", CSessionManager.IWSessionID, EmpID);
                var GetID = new CEmployeeID();
                foreach (DataRow rw in GetUsr.Rows)
                {
                    GetID.ID = rw["ID#"].ToString();

                }

                var MyEmpID = GetID.ID;

                ViewData["Appraisals"] = _data.Appraisals(GetCommandDataParam("P_00055", CSessionManager.IWSessionID, MyEmpID));
                ViewData["Surbodinates"] = _data.GetSurbordinates(GetCommandDataParam("ESS_005", CSessionManager.IWSessionID, EmpID));

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }

            return View();
        }


        [HttpPost]
        public ActionResult ViewAppraisals(string empID)
        {
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");

            try
            {
                var usr = _functionCall.GetEmployeeInfo(CSessionManager.IWSessionID);
                var employee = new CEmployeeInfo();
                foreach (DataRow rw in usr.Rows)
                {
                    employee.EmpID = rw["EmpID"].ToString();

                }
                var EmpID = employee.EmpID;
                var GetUsr = GetCommandDataParam("P_00050", CSessionManager.IWSessionID, empID);
                var GetID = new CEmployeeID();
                foreach (DataRow rw in GetUsr.Rows)
                {
                    GetID.ID = rw["ID#"].ToString();

                }

                var MyEmpID = GetID.ID;

                ViewData["Appraisals"] = _data.Appraisals(GetCommandDataParam("P_00055", CSessionManager.IWSessionID, MyEmpID));
                ViewData["Surbodinates"] = _data.GetSurbordinates(GetCommandDataParam("ESS_005", CSessionManager.IWSessionID, EmpID));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }

            return View();
        }

        [HttpGet]
        public ActionResult ViewComment(int id)
        {
            if(!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");
            var assignee = Session["MyEmpID"];

            //var MyEmpID = GetID.ID;
            ViewBag.details = _functionCall.GetCommitmentResponseDetails(assignee.ToString(), id);

            return View();
        }

        public ActionResult CancelRequest(string RID, string Type, DateTime StartDate, DateTime EndDate, string Comments)
        {
            //_logged = Logged();
            //if (_logged == 0)
            //{
            //    return RedirectToAction("Index");
            //}
            if (!CSessionManager.IsLoggedIn())
                return RedirectToAction("Index");

            if (string.IsNullOrEmpty(RID) || string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(StartDate.ToString()))
                return RedirectToAction("History");

            //var param = IWServices.CreateParameters(RID, (string)Session["UserID"], "",
            //                                        Common.FormatEx.FormatDT(StartDate),
            //                                        Common.FormatEx.FormatDT(EndDate), "", "", "",
            //                                        "Pending", Comments, "", "1");
            var param = IWServices.CreateParameters(RID, CSessionManager.IWSessionID, "",
                                                    Common.FormatEx.FormatDT(StartDate),
                                                    Common.FormatEx.FormatDT(EndDate), "", "", "",
                                                    "Pending", Url.Encode(Comments), "", "1");
            var done = GetCommandDataParam("ESS_003", CSessionManager.IWSessionID, param);
            return RedirectToAction("History");

        }
        private int Logged()
        {
            if (string.IsNullOrEmpty(CSessionManager.IWSessionID) || string.IsNullOrEmpty((string)Session["UserID"]))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }


        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }

        private DataTable GetData(string link, string SessionID, string UserID)
        {
            string url = string.Format("{0}/{1}/{2}"
             , ConfigurationManager.AppSettings["URL"]
             , link
             , SessionID);

            string json_data = Common.HTMLHelper.GetHtmlPage(url);
            var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
            return CJSONHelper.ConvertToDatatable(result.DataTable);
        }
        private DataTable GetCommandDataParam (string functionId, string sessionId, string parameter)
        {
          string  _fullurl = string.Format("{0}/PGetData2/{1}/?FunctionID={2}&Params={3}"
                     , ConfigurationManager.AppSettings["URL"]
                     , sessionId
                     , functionId
                     , parameter);

            string  _returnedData = Common.HTMLHelper.GetHtmlPage(_fullurl);
            var data = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(_returnedData);
            var dataTableValue = CJSONHelper.ConvertToDatatable(data.DataTable);
            return dataTableValue;
        }
        private DataTable GetCommandData(string functionId, string sessionId)
        {
            string _fullurl = string.Format("{0}/PGetData/{1}/?FunctionID={2}"
                     , ConfigurationManager.AppSettings["URL"]
                     , sessionId
                     , functionId);

            string _returnedData = Common.HTMLHelper.GetHtmlPage(_fullurl);
            var data = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(_returnedData);
            var dataTableValue = CJSONHelper.ConvertToDatatable(data.DataTable);
            return dataTableValue;
        }



    }
}