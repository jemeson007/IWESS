using IWNet;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace IWESS.Models
{
    public class CDataConversion
    {
        public List<CUser> User(DataTable data)
        {
            var user = new List<CUser>();
            foreach(var row in data.Select())
            {
                if(data.Rows.Count == 1 )
                {
                    user.Add(new CUser
                    {
                        UserID = Common.ConvertEx.ToString(row["UserID"]),
                        Fullname =  Common.ConvertEx.ToString(row["Fullname"]),
                        LastLogonDate = Common.ConvertEx.ToDateTime(row["LastLogonDate"]).ToString(Common.DateHelper.FMT_DT),
                        LastLogonMachine = Common.ConvertEx.ToString(row["LastLogonMachine"]),
                        IsAdmin = Common.ConvertEx.ToBoolean(row["IsAdmin"])
                });
                }
            }
            return user;
        }
        public List<CEmployeeInfo> EmployeeInfo(DataTable data)
        {
            var info = new List<CEmployeeInfo>();
            foreach (var row in data.Select())
            {
                if (data.Rows.Count == 1)
                {
                    info.Add(new CEmployeeInfo
                    {
                        EmpID = Common.ConvertEx.ToString(row["EmpID"]),
                        Name = Common.ConvertEx.ToString(row["Name"]),
                        Title = Common.ConvertEx.ToString(row["Title"]),
                        Gender = Common.ConvertEx.ToString(row["Gender"]),
                        City = Common.ConvertEx.ToString(row["City"]),
                        State = Common.ConvertEx.ToString(row["State"]),
                        Country = Common.ConvertEx.ToString(row["Country"]),
                        Email = Common.ConvertEx.ToString(row["Email1"]),
                        Phone = Common.ConvertEx.ToString(row["Phone1"]),
                        Birthday = Common.ConvertEx.ToDateTime(row["Birthday"]).ToString(Common.DateHelper.FMT_DT),
                        Nationality = Common.ConvertEx.ToString(row["Nationality"])
                    });
                }
            }
            return info;
        }
        public List<CJobHistory> JobHistory(DataTable data)
        {
            var history = new List<CJobHistory>();
            foreach(var row in data.Select())
            {
                history.Add(new CJobHistory
                {
                    JobTitle = Common.ConvertEx.ToString(row["JobTitle"]),
                    Manager = Common.ConvertEx.ToString(row["Manager"]),
                    Dept = Common.ConvertEx.ToString(row["Dept"]),
                    UpdateType = Common.ConvertEx.ToString(row["UpdateType"])
                });
            }
            return history;
        }
        public List<CBankInfo> BankInfo(DataTable data)
        {
            var bank = new List<CBankInfo>();
            foreach(var row in data.Select())
            {
                bank.Add(new CBankInfo {
                    BankName = Common.ConvertEx.ToString(row["BankName"]),
                    AcctNumber = Common.ConvertEx.ToString(row["AcctNumber"]),
                    AccountName = Common.ConvertEx.ToString(row["AccountName"])
                });
            }
            return bank;
        }

        public List<CPayStubList> SalaryDetails(DataTable data)
        {
            var salary = new List<CPayStubList>();
            foreach(var row in data.Select())
            {
                salary.Add(new CPayStubList
                {
                    ValueDate = Common.ConvertEx.ToDateTime(row["ValueDate"]).ToString(Common.DateHelper.FMT_DT),
                    AdjToEarnings = Common.FormatEx.FormatCurrency(Common.ConvertEx.ToDouble(row["AdjToEarnings"])),
                    TaxableEarnings = Common.FormatEx.FormatCurrency(Common.ConvertEx.ToDouble(row["TaxableEarnings"])),
                    NetPay = Common.FormatEx.FormatCurrency(Common.ConvertEx.ToDouble(row["NetPay"])),
                    BasePayAmount = Common.FormatEx.FormatCurrency(Common.ConvertEx.ToDouble(row["BasePayAmount"]))
                });
            }
            return salary;
        }
        public List<CAddition> Addition(DataTable data)
        {
            var ad = new List<CAddition>();

            foreach(var row in data.Select())
            {
                ad.Add(new CAddition
                {
                   LinkTitle = Common.ConvertEx.ToString(row["LinkTitle"]),
                    HyperLink = Common.ConvertEx.ToString(row["HyperLink"])
                });
            }
            return ad;
        }

        public List<CQualification> Qualification(DataTable data)
        {
            var qualify = new List<CQualification>();
            foreach(var row in data.Select())
            {
                qualify.Add(new CQualification {
                    StartDate = Common.ConvertEx.ToDateTime(row["StartDate"]).ToString(Common.DateHelper.FMT_DT),
                    EndDate = Common.ConvertEx.ToDateTime(row["EndDate"]).ToString(Common.DateHelper.FMT_DT),
                    Qualification = Common.ConvertEx.ToString(row["Qualification"]),
                    QualificationType = Common.ConvertEx.ToString(row["QualificationType"]),
                    UpdateType = Common.ConvertEx.ToString(row["UpdateType"]),
                    Comments = Common.ConvertEx.ToString(row["Comments"]),
                });
            }
            return qualify;
        }
        public List<CTimeOffHistory> TimeOffHistory(DataTable data)
        {
            var time = new List<CTimeOffHistory>();
            foreach(var row in data.Select())
            {
                time.Add(new CTimeOffHistory {
                    ID = Common.ConvertEx.ToString(row["ID#"]),
                    EMPID = Common.ConvertEx.ToString(row["EmpID"]),
                    TimeOffType = Common.ConvertEx.ToString(row["TimeOffType"]),
                    StartDate = Common.ConvertEx.ToDateTime(row["StartDate"]).ToString(Common.DateHelper.FMT_DT),
                    ProposedEndDate = Common.ConvertEx.ToDateTime(row["ProposedEndDate"]).ToString(Common.DateHelper.FMT_DT),
                    AccuralYear = Common.ConvertEx.ToString(row["AccrualYear"]),
                    ActualEndDate = Common.ConvertEx.ToDateTime(row["ActualEndDate"]).ToString(Common.DateHelper.FMT_DT),
                    Balance = Common.ConvertEx.ToString(row["Balance"]),
                    UpdateType = Common.ConvertEx.ToString(row["UpdateType"]),
                    Comments = Common.ConvertEx.ToString(row["Comments"]),
                    TxnDate = Common.ConvertEx.ToDateTime(row["TxnDate"]).ToString(Common.DateHelper.FMT_DT),
                });
            }
            return time;
        }


        public List<CNetPay> NetPayWord(DataTable data)
        {
            var payment = new List<CNetPay>();
            foreach (var row in data.Select())
            {
                payment.Add(new CNetPay
                {
                    NetPay = Common.ConvertEx.ToString(row["NetPayinWord"])
                });
            }
            return payment;
        }



        public List<CGrossEarnings> GrossEarnings(DataTable data)
        {
            var GrossPay = new List<CGrossEarnings>();
            foreach (var row in data.Select())
            {
                GrossPay.Add(new CGrossEarnings
                {
                    Classification = Common.ConvertEx.ToString(row["Classification"]),
                    EmpID = Common.ConvertEx.ToString(row["EmpID"]),
                    PayLevel = Common.ConvertEx.ToString(row["PayLevel"]),
                    Name = Common.ConvertEx.ToString(row["Name"]),
                    Description = Common.ConvertEx.ToString(row["Description"]),
                    Amount = Common.FormatEx.FormatSTD(Common.ConvertEx.ToDouble(row["Amount"]))
                });
            }
            return GrossPay;
        }


        public List<CEmployeePayInfo> EmployeePay(DataTable data)
        {
            var info = new List<CEmployeePayInfo>();
            foreach (var row in data.Select())
            {
                if (data.Rows.Count == 1)
                {
                    info.Add(new CEmployeePayInfo
                    {
                        Name = Common.ConvertEx.ToString(row["Name"]),
                        BankName = Common.ConvertEx.ToString(row["BANKNAME"]),
                        AcctNumber = Common.ConvertEx.ToString(row["AcctNumber"]),
                        Description = Common.ConvertEx.ToString(row["Description"]),
                        EmpID = Common.ConvertEx.ToString(row["EmpID"]),
                        FirstName = Common.ConvertEx.ToString(row["FirstName"]),
                        LastName = Common.ConvertEx.ToString(row["LastName"]),
                        MiddleName = Common.ConvertEx.ToString(row["MiddleName"]),
                        Designature = Common.ConvertEx.ToString(row["DESIGNATURENAME"]),
                        Department = Common.ConvertEx.ToString(row["DEPARTMENT"])

                    });
                }
            }
            return info;
        }

        public List<CSurbordinates> GetSurbordinates(DataTable data)
        {
            var subordinates = new List<CSurbordinates>();
            foreach (var row in data.Select())
            {
                subordinates.Add(new CSurbordinates
                {
                    EmpID = Common.ConvertEx.ToString(row["EmpID"]),
                    EmpName = Common.ConvertEx.ToString(row["Name"]),
                    EmpManager = Common.ConvertEx.ToString(row["Manager"])
                });

            }
            return subordinates;
        }

        public List<CAnnualTakenLeave> AnnualLeaveTaken(DataTable data)
        {
            var leave = new List<CAnnualTakenLeave>();
            foreach (var row in data.Select())
            {
                leave.Add(new CAnnualTakenLeave
                {
                    NumberAnnualLeave = Common.ConvertEx.ToString(row["NumberOfDaysTaken"])
                });
            }
            return leave;
        }


        public List<CTimeOffType> TimeOffType(DataTable data)
        {
            var type = new List<CTimeOffType>();
            foreach(var row in data.Select())
            {
                type.Add(new CTimeOffType {
                    ID = Common.ConvertEx.ToString(row["ID#"]),
                    Name = Common.ConvertEx.ToString(row["Name"]),
                    Description = Common.ConvertEx.ToString(row["Description"])
                });
            }

            return type;
        }


        public List<CViewCommittment> Committment(DataTable data)
        {
            var distinctRows = data.AsEnumerable()
                     .GroupBy(row => row.Field<string>("CommittmentID"))
                     .Select(g => g.First());

            //var distinctRows = data.AsEnumerable().DistinctBy(row => row.Field<string>("CommittmentID"));
            var type = new List<CViewCommittment>();
            foreach (var row in distinctRows)
            {
                type.Add(new CViewCommittment
                {
                    Question = Common.ConvertEx.ToString(row["Committment"]),
                    BusinessArea = Common.ConvertEx.ToString(row["BusinessArea"]),
                    SubArea = Common.ConvertEx.ToString(row["SubArea"]),
                    CommitmentID = Common.ConvertEx.ToString(row["CommittmentID"]),
                    Score = Common.ConvertEx.ToString(row["Score"]),
                    Credit = Common.ConvertEx.ToString(row["Credits"])
                });
            }
            return type;
        }


        public List<CViewSurveys> Surveys(DataTable data)
        {
            var type = new List<CViewSurveys>();
            foreach (var row in data.Select())
            {
                type.Add(new CViewSurveys
                {
                    EffectiveDate = Common.ConvertEx.ToDateTime(row["EffectiveDate"]).ToString(Common.DateHelper.FMT_DTTM),
                    DateSurveyTaken = Common.ConvertEx.ToDateTime(row["DateSurveyTaken"]).ToString(Common.DateHelper.FMT_DTTM),
                    Comments = Common.ConvertEx.ToString(row["Comments"]),
                    RejectionReasons= Common.ConvertEx.ToString(row["RejectionReasons"]),
                    SurveyURL = Common.ConvertEx.ToString(row["SurveyURL"]),
                    SurveyTaken = Common.ConvertEx.ToString(row["SurveyTaken"])
                });
            }
            return type;
        }


        public List<CViewAppraisals> Appraisals(DataTable data)
        {
            var type = new List<CViewAppraisals>();
            foreach (var row in data.Select())
            {
                type.Add(new CViewAppraisals
                {
                    EmpID = Common.ConvertEx.ToString(row["EmpID"]),
                    EffectiveDate = Common.ConvertEx.ToDateTime(Common.ConvertEx.ToString(row["EffectiveDate"])).ToString(Common.DateHelper.FMT_DTTM),
                    AppraisalURL = Common.ConvertEx.ToString(row["AppraisalURL"]),
                    AppraisalTaken = Common.ConvertEx.ToString(row["AppraisalTaken"])
                });
            }
            return type;
        }
    }

}