using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace IWESS.Models
{
    public class CUser
    {
        public string UserID { get; set; }
        public string Fullname { get; set; }
        public string LastLogonDate { get; set; }
        public string LastLogonMachine { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class CEmployeeInfo
    {
        public string EmpID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Nationality { get; set; }
        public string Birthday { get; set; }
    }
    public class CJobHistory
    {
        public string JobTitle { get; set; }
        public string Manager { get; set; }
        public string Dept { get; set; }
        public string UpdateType { get; set; }
    }
    public class CBankInfo
    {
        public string BankName { get; set; }
        public string AcctNumber { get; set; }
        public string AccountName { get; set; }
    }
    public class CPayStubList
    {
        public string ValueDate { get; set; }
        public string AdjToEarnings { get; set; }
        public string NetPay { get; set; }
        public string BasePayAmount { get; set; }
        public string TaxableEarnings { get; set; }
    }
    public class CQualification
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string QualificationType { get; set; }
        public string Qualification { get; set; }
        public string UpdateType { get; set; }
        public string Comments { get; set; }
    }

    public class CTimeOffHistory
    {
        public string ID { get; set; }
        public string EMPID { get; set; }
        public string TimeOffType { get; set; }
        public string StartDate { get; set; }
        public string ProposedEndDate { get; set; }
        public string AccuralYear { get; set; }
        public string ActualEndDate { get; set; }
        public string Balance { get; set; }
        public string UpdateType { get; set; }
        public string Comments { get; set; }
        public string TxnDate { get; set; }
    }

    public class CTimeOffType
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CAnnualTakenLeave
    {
        //public string ID { get; set; }
        public string NumberAnnualLeave { get; set; }
    }

    public class CViewCommittment
    {
        public string Question { get; set; }
        public string BusinessArea { get; set; }
        public string SubArea { get; set; }
        public string CommitmentID { get; set; }
        public string Score { get; set; }
        public string Credit { get; set; }
    }

    public class CEmployeePoints
    {
        //public string EmpID { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public string StartDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public string EndDate { get; set; }

        public string EmpID { get; set; }
    }


    public class CViewSurveys
    {
        public string DateSurveyTaken { get; set; }
        public string EffectiveDate { get; set; }
        public string Comments { get; set; }
        public string RejectionReasons { get; set; }
        public string SurveyURL { get; set; }
        public string SurveyTaken { get; set; }
    }


    public class CViewAppraisals
    {
        public string EffectiveDate { get; set; }
        public string EmpID { get; set; }
        public string AppraisalTaken { get; set; }
        public string AppraisalURL { get; set; }
    }

    public class CEmployeeID
    {
        public string ID { get; set; }
    }

    public class CSurbordinates
    {
        public string EmpID { get; set; }
        public string EmpName { get; set; }
        public string EmpManager { get; set; }

    }

    public class CEmployeePayInfo
    {
        public string Name { get; set; }
        public string AcctNumber { get; set; }
        public string BankCode { get; set; }
        public string Description { get; set; }
        public string BankName { get; set; }
        public string EmpID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string Designature { get; set; }
    }

    public class CGrossEarnings
    {
        public string Classification { get; set; }
        public string EmpID { get; set; }
        public string PayLevel { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Amount { get; set; }
    }

    public class CNetPay
    {
        public string NetPay { get; set; }
    }

    public class CAddition
    {
        public string LinkTitle { get; set; }
        public string HyperLink { get; set; }
    }
}