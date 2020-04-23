using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using IWNet;
using IWNet.DataAccess;
using IWNet.Utils.JSON;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Net.Http;
using IWNetSvcs;

namespace IWESS.Models
{
    #region Enums
    public enum enumAPICustInfo
    {
        CSCSNumbers = 1,
        OpenOrders,
        ExecutedOrders,
        CancelledOrders,
        RejectedOrders,
        BioData,
        Portfolio,
        PortfolioTxns,
        CustomerBalances,
    }
    public enum IWEnum_TradeAction
    {
        Buy = 0,
        Sell
    }
    public enum IWEnum_OrderType
    {
        Market = 49,
        Limit = 50,
        //Stop = 51,
        //StopLimit = 52,
    }
    public enum IWEnum_TimeInForce
    {
        Day = 0,
        GoodTillCanceled = 1,
        ImmediateOrCancel = 3,
        FillOrKill = 4,
        GoodTillDate = 6
    }
    #endregion Enums
    public class CSessionManager
    {
        const string SESSIONID = "AppSessionID";
        const string LOGGEDINUSER = "LoggedInUser";
        const string LASTERROR = "LASTERROR";
        //public static HttpSessionStateBase WWWSession { get; set; }

        //public static object WWWSession(string SessionValID)
        //{
        //    return new CSessionManager()[SessionValID];
        //}
        //public object this[string Index]   // Indexer declaration
        //{
        //    get
        //    {
        //        return (HttpContext.Current != null && HttpContext.Current.Session != null) ?
        //            HttpContext.Current.Session[Index] : null;
        //    }

        //    set
        //    {
        //        if (HttpContext.Current != null && HttpContext.Current.Session != null)
        //            HttpContext.Current.Session[Index] = value;

        //    }
        //}

        public static void SetSessionValue(string name, object val)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
                HttpContext.Current.Session[name] = val;
        }
        public static object GetSessionValue(string name)
        {
            return (HttpContext.Current != null && HttpContext.Current.Session != null) ?
                HttpContext.Current.Session[name] : null;
        }
        public static string IWSessionID
        {
            get { return GetSessionValue(SESSIONID) as string; }
            set { SetSessionValue(SESSIONID, value); }
        }

        public static CLoggedInUser LoggedInUser
        {
            get { return GetSessionValue(LOGGEDINUSER) as CLoggedInUser; }
            internal set { SetSessionValue(LOGGEDINUSER, value); }
        }

        public static bool IsIWSessionExists() { return !string.IsNullOrWhiteSpace(IWSessionID); }
        public static bool IsWWWSessionExists() { return (HttpContext.Current != null && HttpContext.Current.Session != null); }
        public static string LastError
        {
            get { return GetSessionValue(LASTERROR) as string; }
            internal set { SetSessionValue(LASTERROR, value); }
        }

        public static bool IsLoggedIn()
        {
            if (!IsWWWSessionExists()) return false;
            if (!IsIWSessionExists()) return false;
            return (LoggedInUser != null);
        }
    }

    public class CLoggedInUser
    {
        public string UserID { get; set; }
        public string Fullname { get; set; }
        public string LastLogonDate { get; set; }
        public string LastLogonMachine { get; set; }
        public string IsAdmin { get; set; }
        public string CompanyCustAID { get; set; }
        public string CompanyName { get; set; }
        public bool IsExist { get; internal set; }
        public string Email { get; internal set; }
        public string Phone { get; internal set; }

        public string IsPasswordChangeRequired { get; internal set; }

        public string LockedOut { get; internal set; }

        public string StatusMessage { get; internal set; }
    }

    public class CFunctionCall
    {
        private readonly string m_url;
        private string m_fullurl;
        private string m_returnedData;

        public CFunctionCall()
        {
            m_url = ConfigurationManager.AppSettings["URL"];
        }
        #region Calls to Platform API Web Services
        private DataTable GetDataWithParameter(string functionId, string sessionId, string parameter)
        {
            m_fullurl = string.Format("{0}/PGetData2/{1}?FunctionID={2}&Params={3}"
                      , m_url
                      , sessionId
                      , functionId
                      , parameter);

            string json_data = Common.HTMLHelper.GetHtmlPage(m_fullurl);
            var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
            var tb = CJSONHelper.ConvertToDatatable(result.DataTable);

            return tb;
        }
        private DataTable GetDataWithoutParameter(string functionId, string sessionId)
        {
            m_fullurl = string.Format("{0}/PGetData/{1}?FunctionID={2}"
                      , m_url
                      , sessionId
                      , functionId);

            string json_data = Common.HTMLHelper.GetHtmlPage(m_fullurl);
            var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
            var tb = CJSONHelper.ConvertToDatatable(result.DataTable);

            return tb;
        }
        private Out_Result GetScalarWithoutParameter(string functionId, string sessionId)
        {
            m_fullurl = string.Format("{0}/PGetDataS/{1}?FunctionID={2}"
                      , m_url
                      , sessionId
                      , functionId);

            string json_data = Common.HTMLHelper.GetHtmlPage(m_fullurl);
            var result = CJSONHelper.JsonConvertDeserialise<Out_Result>(json_data);
            return result;
        }
        private Out_Result GetScalarWithParameter(string functionId, string sessionId, string parameter)
        {
            m_fullurl = string.Format("{0}/PGetDataS2/{1}?FunctionID={2}&Params={3}"
                      , m_url
                      , sessionId
                      , functionId
                      , parameter);

            string json_data = Common.HTMLHelper.GetHtmlPage(m_fullurl);
            var result = CJSONHelper.JsonConvertDeserialise<Out_Result>(json_data);
            return result;
        }

        private DataTable GetFunctionCallForResearchWithParameter(int functionId, string parameter)
        {
            return IWServices.CallMDVWsFunctionWithParameters(functionId, parameter);

        }

        private DataTable GetFunctionCallForResearchNoParameter(int functionId)
        {
            return IWServices.CallMDVWsFunctionWithNoParameters(functionId);

        }

        #endregion Calls to Platform API Web Services

        public string AppLogin(string user, string pass)
        {
            CSessionManager.IWSessionID = null;
            string token = ConfigurationManager.AppSettings["TOKEN"];
            string username = user;
            string password = pass;

            string apiUrl = string.Format("{0}/ESSLogin/{1}/{2}/{3}"
                , m_url
                , token
                , username
                , password);
            string json_data = Common.HTMLHelper.GetHtmlPage(apiUrl);
            var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
            if (result.StatusID == IWAPIEnum_APIStatus.NoError)
            {
                var id = result.OutValue;
                CSessionManager.IWSessionID = id;
            }
            return CSessionManager.IWSessionID;
        }

        public CLoggedInUser ESSLogin(string sessionId, string username, string password)
        {
            string token = ConfigurationManager.AppSettings["TOKEN"];
            var user = new CLoggedInUser();
            user.IsExist = false;
            user.StatusMessage = "";

            m_fullurl = string.Format("{0}/ESSLogin/{1}/{2}/{3}"
                        , m_url
                        , token
                        , username
                        , password);
            string json_data = Common.HTMLHelper.GetHtmlPage(m_fullurl);

            var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
            user.StatusMessage = result.StatusMessage;
            if (result.StatusID == IWAPIEnum_APIStatus.NoError)
            {
                var tb = CJSONHelper.ConvertToDatatable(result.DataTable);
                //user.LastLogonDate = Common.ConvertEx.ToDateTime(tb.Rows[0]["LastLogonDate"], "en-us").ToString(Common.DateHelper.FMT_DTTM);
                user.UserID = Common.ConvertEx.ToString(tb.Rows[0]["UserID"]);
                user.Fullname = Common.ConvertEx.ToString(tb.Rows[0]["Fullname"]);
                user.LastLogonDate = Common.ConvertEx.ToDateTime(tb.Rows[0]["LastLogonDate"]).ToString(Common.DateHelper.FMT_DTTM);
                user.LastLogonMachine = Common.ConvertEx.ToString(tb.Rows[0]["LastLogonMachine"]);
                user.IsAdmin = Common.ConvertEx.ToString(tb.Rows[0]["IsAdmin"]);


                user.IsExist = true;
                CSessionManager.LoggedInUser = user;
                //CSessionManager.IWSessionID = sessionId;

            }
            return user;
        }
        public CLoggedInUser Ebiz_Login(string sessionId, string username, string password)
        {
            var user = new CLoggedInUser();
            user.IsExist = false;
            user.StatusMessage = "";

            m_fullurl = string.Format("{0}/EBizLogin/{1}/{2}/{3}"
                        , m_url
                        , sessionId
                        , username
                        , password);

            string json_data = Common.HTMLHelper.GetHtmlPage(m_fullurl).Normalize();

            var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
            user.StatusMessage = result.StatusMessage;
            if (result.StatusID == IWAPIEnum_APIStatus.NoError)
            {
                var tb = CJSONHelper.ConvertToDatatable(result.DataTable);
                user.LastLogonDate = Common.ConvertEx.ToDateTime(tb.Rows[0]["LastLogonDate"], "en-us").ToString(Common.DateHelper.FMT_DTTM);
                user.UserID = Common.ConvertEx.ToString(tb.Rows[0]["UserID"]);
                user.Fullname = Common.ConvertEx.ToString(tb.Rows[0]["Fullname"]);
                user.LastLogonDate = Common.ConvertEx.ToDateTime(tb.Rows[0]["LastLogonDate"]).ToString(Common.DateHelper.FMT_DTTM);
                user.LastLogonMachine = Common.ConvertEx.ToString(tb.Rows[0]["LastLogonMachine"]);
                user.Email = Common.ConvertEx.ToString(tb.Rows[0]["Email"]);
                user.Phone = Common.ConvertEx.ToString(tb.Rows[0]["Phone"]);
                user.IsPasswordChangeRequired = Common.ConvertEx.ToString(tb.Rows[0]["IsPasswordChangeRequired"]);
                user.LockedOut = Common.ConvertEx.ToString(tb.Rows[0]["LockedOut"]);
                user.IsExist = true;
                CSessionManager.LoggedInUser = user;
                CSessionManager.IWSessionID = sessionId;
            }
            //user.StatusMessage = result.StatusMessage;
            //return user.StatusMessage;
            return user;
        }


        public DataTable GetEmployeeInfo(string sessionid)
        {
            var user = new CEmployeeInfo();
            m_fullurl = string.Format("{0}/HRESSGetEmpInfo/{1}"
                        , m_url
                        , sessionid);

            string json_data = Common.HTMLHelper.GetHtmlPage(m_fullurl);
            var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
            var tb = CJSONHelper.ConvertToDatatable(result.DataTable);
            return tb;

        }

        public CLoggedInUser Ebiz_LoginP(string sessionId, string username, string password)
        {
            var user = new CLoggedInUser();
            user.IsExist = false;
            user.StatusMessage = "";

            m_fullurl = string.Format("{0}/EBizLoginP/{1}?CustUserID={2}&PWD={3}"
                        , m_url
                        , sessionId
                        , username
                        , password);

            var req = WebRequest.Create(m_fullurl) as HttpWebRequest;
            req.Method = "POST";
            req.ContentType = "application/json";
            req.ContentLength = 0;
            StreamReader responseReader = new StreamReader(req.GetResponse().GetResponseStream());
            string responseData = responseReader.ReadToEnd();
            responseReader.Close();

            var resp = (HttpWebResponse)req.GetResponse();            

            var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(responseData);
            user.StatusMessage = result.StatusMessage;
            if (result.StatusID == IWAPIEnum_APIStatus.NoError)
            {
                var tb = CJSONHelper.ConvertToDatatable(result.DataTable);
                user.LastLogonDate = Common.ConvertEx.ToDateTime(tb.Rows[0]["LastLogonDate"], "en-us").ToString(Common.DateHelper.FMT_DTTM);
                user.UserID = Common.ConvertEx.ToString(tb.Rows[0]["UserID"]);
                user.Fullname = Common.ConvertEx.ToString(tb.Rows[0]["Fullname"]);
                user.LastLogonDate = Common.ConvertEx.ToDateTime(tb.Rows[0]["LastLogonDate"]).ToString(Common.DateHelper.FMT_DTTM);
                user.LastLogonMachine = Common.ConvertEx.ToString(tb.Rows[0]["LastLogonMachine"]);
                user.Email = Common.ConvertEx.ToString(tb.Rows[0]["Email"]);
                user.Phone = Common.ConvertEx.ToString(tb.Rows[0]["Phone"]);
                user.IsPasswordChangeRequired = Common.ConvertEx.ToString(tb.Rows[0]["IsPasswordChangeRequired"]);
                user.LockedOut = Common.ConvertEx.ToString(tb.Rows[0]["LockedOut"]);
                user.IsExist = true;
                CSessionManager.LoggedInUser = user;
                CSessionManager.IWSessionID = sessionId;
            }
            //user.StatusMessage = result.StatusMessage;
            //return user.StatusMessage;
            return user;
        }

        internal string Ebiz_MakeRequest(string SessionID, string CustAID, string Subject, string Message, string LoggedInUserEmail)
        {
            m_fullurl = string.Format("{0}/EBizMakeRequest/{1}?CustAID={2}&Subject={3}&Message={4}"
                        , m_url
                        , SessionID
                        , CustAID
                        , Subject
                        , Message
                        );
            string json_data = Common.HTMLHelper.GetHtmlPage(m_fullurl);
            var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
            return result.StatusMessage;

        }

        public string Ebiz_ChangePassword(string sessionId, string clientId, string currentPwd, string newpwd)
        {
            m_fullurl = string.Format("{0}/EBizPWDChg/{1}?CustAID={2}&CurrentPWD={3}&NewPWD={4}&PWDChangeRequired={5}"
                     , m_url
                     , sessionId
                     , clientId
                     , HttpUtility.UrlEncode(currentPwd)
                     , newpwd
                     , false);
            m_returnedData = Common.HTMLHelper.GetHtmlPage(m_fullurl);
            return m_returnedData;
        }

        public string Ebiz_ResetPassword(string sessionId, string clientId, string NewPwd, bool PWDChangeRequired)
       {
           m_fullurl = string.Format("{0}/EBizResetPWD/{1}?CustAID={2}&NewPWD={3}&PWDChangeRequired={4}"
                    , m_url
                    , sessionId
                    , clientId
                    , HttpUtility.UrlEncode(NewPwd)   //If NewPwd is empty string, API will autogenerate the password
                    , PWDChangeRequired);
           m_returnedData = Common.HTMLHelper.GetHtmlPage(m_fullurl);
           return m_returnedData;
        }

    public DataTable AccountOverview(string sessionID, string clientID)
        {
            return GetDataWithParameter("P_00001", sessionID, clientID);
        }
        public DataTable ListMutualFundProduct(string sessionID)
        {
            return GetDataWithoutParameter("P_00002", sessionID);
        }

        public DataTable SubscribedMutualFund(string sessionID, string clientID)
        {
            return GetDataWithParameter("P_00003", sessionID, clientID);
        }

        public DataTable PriceHistory(string sessionID, string fundCode)
        {
            return GetDataWithParameter("P_00004", sessionID, fundCode);
        }

        public DataTable MutualFundSubscriptionTransactionHistory(string sessionID, string parameter)
        {
            return GetDataWithParameter("P_00005", sessionID, parameter);
        }
        public DataTable MutualFundAccountStatement(string sessionID, string parameter)
        {
            return GetDataWithParameter("P_00006", sessionID, parameter);
        }

        public DataTable MfYearToDatePerformance(string sessionID, string fundCode)
        {
            return GetDataWithParameter("P_00007", sessionID, fundCode);
        }
        public DataTable MfHypotheticalGrowth(string sessionID, string fundCode)
        {
            return GetDataWithParameter("P_00008", sessionID, fundCode);
        }

        public DataTable MfSummaryDetails(string sessionID, string fundCode)
        {
            return GetDataWithParameter("P_00009", sessionID, fundCode);
        }
        public DataTable AccountDetailPlusBalance(string sessionID, string clientID)
        {
            return GetDataWithParameter("P_00010", sessionID, clientID);
        }
        public DataTable MfAverageAnnualTotalReturns(string sessionID, string parameter)
        {
            return GetDataWithParameter("P_00011", sessionID, parameter);
        }
        public DataTable MfTop10Holdings(string sessionID, string fundCode)
        {
            return GetDataWithParameter("P_00012", sessionID, fundCode);
        }
        public DataTable MfCumulativeTotalReturns(string sessionID, string fundCode)
        {
            return GetDataWithParameter("P_00013", sessionID, fundCode);
        }

        public DataTable PortfolioHolding(string sessionID, string parameter)
        {
            return GetDataWithParameter("P_00015", sessionID, parameter);
        }

        public DataTable PortfolioAnalysis(string sessionID, string clientID)
        {
            return GetDataWithParameter("P_00016", sessionID, clientID);
        }
        public DataTable ChgPwd(string sessionID, string param)
        {
            return GetDataWithParameter("P_00017", sessionID, param);
        }
        public DataTable ClientCurrentHoldings(string sessionID, string clientID)
        {
            return GetDataWithParameter("P_00018", sessionID, clientID);
        }

        internal dynamic OMS_PortfolioSummaryPerStock(string sessionID, string ClientID, DateTime ValueDate)
        {
            var param = IWServices.CreateParameters(
                ClientID
                , ValueDate.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                );
            return GetDataWithParameter("P_00019", sessionID, param);
        }

        public DataTable Fin_GetCustomerBalances(string SessionID, string ClientID, string LedgerType, DateTime ValueDate, double CCYRate)
        {
            var param = IWServices.CreateParameters(
                ClientID
                , LedgerType
                , ValueDate.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                , CCYRate.ToString()
                );
            DataTable tb = GetDataWithParameter(
                "P_00021"
                , CSessionManager.IWSessionID
                , param);

            return tb;
        }

        #region Financials/Accounts
        public DataTable Fin_GetGLTransactions(string SessionID, string MasterAID, string ClientID, DateTime StDate, DateTime EndDate, double CCYRate, bool IncludeReversals)
        {
            var param = IWServices.CreateParameters(
                MasterAID
                , ClientID
                , StDate.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                , EndDate.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                , CCYRate.ToString()
                , IncludeReversals ? "1" : "0"
                );
            //bool excludeReversals = IsParamTrueOrYes(GetAppsettingValue("CS_EXCLUDEREVERSALS")); //Reversals are included by default. This parameter specifically calls for exclusion
            DataTable tbTxns = GetDataWithParameter(
                "P_00022"
                , CSessionManager.IWSessionID
                , param);

            return tbTxns;
        }
        public double Fin_GetGLAccountBalance(string SessionID, string MasterAID, string ClientID, DateTime ValDate, string Ledger)
        {
            var param = IWServices.CreateParameters(
                MasterAID
                , ClientID
                , Ledger
                , ValDate.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                , "1"
                );
            var result = GetScalarWithParameter(
                "P_00023"   //FunctionID to get account balance
                , CSessionManager.IWSessionID
                , param);

            //double bal;
            if (result.StatusID == IWAPIEnum_APIStatus.NoError)
                return Common.ConvertEx.ToDouble(result.OutValue);
            else
                throw new InfoWAREException(result.StatusMessage);
        }
        public string Fin_GetDefaultLedger(string SessionID)
        {
            var result = GetScalarWithoutParameter("FIN_0000", CSessionManager.IWSessionID);

            if (result.StatusID == IWAPIEnum_APIStatus.NoError)
                return result.OutValue;
            else
                throw new InfoWAREException(result.StatusMessage);
        }
        public DataTable Fin_GetMasterAccountsForCustomer(string SessionID, string ClientID)
        {
            DataTable tb = GetDataWithParameter("FIN_0001", CSessionManager.IWSessionID, ClientID);
            return tb;
        }


        #endregion Financials/Accounts
        #region OMS Functions
        public DataTable OMS_TopGainers(string sessionID) { return GetDataWithoutParameter("OMS_0001", sessionID); }
        public DataTable OMS_TopLosers(string sessionID) { return GetDataWithoutParameter("OMS_0002", sessionID); }

        public DataTable OMS_GetMarketTopGainers()
        {
            return GetFunctionCallForResearchNoParameter(8001);

        }
        public DataTable OMS_GetMarketTopLosers()
        {
            return GetFunctionCallForResearchNoParameter(8002);
        }

        public DataTable GetPriceList()
        {
            return GetFunctionCallForResearchNoParameter(8063);
        }
        public DataTable OMS_CurrencyRates(string sessionID) { return GetDataWithoutParameter("P_CZTN_001", sessionID); }
        public DataTable OMS_GetTotalHoldings (string sessionID, string clientID, string LedgerType, DateTime AsAtDate, bool ShowAsPerAssetClass)
        {
            if (!Common.DateHelper.IsValidSQL2005Date(AsAtDate))
                AsAtDate = DateTime.Today;
            var param = IWServices.CreateParameters(
                clientID
                , string.IsNullOrWhiteSpace(LedgerType) ? "" : LedgerType
                , AsAtDate.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                , ShowAsPerAssetClass ? "1" : "0"
                );
            return GetDataWithParameter("OMS_0003", sessionID, param);
        }
        public DataTable OMS_GetCustomerStatement(string SessionID, string MasterAID, string ClientID, DateTime StDate, DateTime EndDate, double CCYRate, bool IncludeReversals, out double GLBalance)
        {
            DataRow rw;

            DataTable tb = new CDataTable();
            tb.Columns.Add(new DataColumn("Transaction Date", typeof(string)));
            tb.Columns.Add(new DataColumn("Value Date", typeof(string)));
            tb.Columns.Add(new DataColumn("Description", typeof(string)));
            tb.Columns.Add(new DataColumn(WWWParameters.CustStmt_DebitColHeader, typeof(string)));
            tb.Columns.Add(new DataColumn(WWWParameters.CustStmt_CreditColHeader, typeof(string)));
            tb.Columns.Add(new DataColumn("Balance", typeof(string)));
            tb.Columns.Add(new DataColumn("RowID", typeof(Int32)));

            DataTable tbTxns = Fin_GetGLTransactions(SessionID, MasterAID, ClientID, StDate, EndDate, CCYRate, IncludeReversals);

            double d;
            foreach(DataRow rwT in tbTxns.Rows)
            {
                rw = tb.NewRow();
                rw[0] = Common.ConvertEx.ToDateTime(rwT["TxnDate"]).ToString(Common.DateHelper.FMT_DTTM);  //Common.FormatEx.FormatDT(Common.ConvertEx.ToDateTime(rwT["TxnDate"]));
                rw[1] = Common.ConvertEx.ToDateTime(rwT["EffectiveDate"]).ToString(Common.DateHelper.FMT_DT);  //Common.FormatEx.FormatDT(Common.ConvertEx.ToDateTime(rwT["EffectiveDate"]));
                rw[2] = rwT["Description"].ToString();
                d = Common.ConvertEx.ToDouble(rwT["Amount"].ToString());

                if (d < 0)
                {
                    rw[3] = Common.FormatEx.FormatSTD(d * -1);
                    rw[4] = string.Empty;
                }
                else
                {
                    rw[3] = string.Empty;
                    rw[4] = Common.FormatEx.FormatSTD(d);
                }
                tb.Rows.Add(rw);
            }
            //Now compute running balance column
            string defaultCCY = Fin_GetDefaultLedger(SessionID);
            double bal = Fin_GetGLAccountBalance(SessionID, MasterAID, ClientID, EndDate, defaultCCY);
            GLBalance = bal;
            double totalBalance = 0;
            double averageDailyBalance = 0;
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                rw = tb.Rows[i];
                rw[5] = Common.FormatEx.FormatCurrency(bal);
                totalBalance += bal;

                double amt = ((string)rw[3]).Length != 0 ? IWNet.Common.ConvertEx.ToDouble(rw[3]) * -1 : IWNet.Common.ConvertEx.ToDouble(rw[4]);
                bal = bal - amt;
                rw[6] = tb.Rows.Count - i;
            }
            if (tb.Rows.Count > 0)
                averageDailyBalance = totalBalance / tb.Rows.Count;

            return tb;
        }
        public DataTable OMS_GetCustomerStatement_Sorted(string SessionID, string MasterAID, string ClientID, DateTime StDate, DateTime EndDate, double CCYRate, bool IncludeReversals, out double GLBalance)
        {
            DataTable tbtmp = OMS_GetCustomerStatement(SessionID, MasterAID, ClientID, StDate, EndDate, CCYRate, IncludeReversals, out GLBalance);

            bool orderASC = WWWParameters.CustStmt_OrderASC; //Customer statement is order DESC by default
            if (orderASC)
                tbtmp.DefaultView.Sort = "RowID ASC"; //Invert the sort order. We use the RowID column to ensure the balance column lines up properly (sorting by Value Date column does not guarantee row order if there are repeated dates values
            else
                tbtmp.DefaultView.Sort = "RowID DESC";

            DataTable tb = tbtmp.DefaultView.ToTable();
            tb.Columns.Remove("RowID");
            if (!WWWParameters.CustStmt_ShowTxnDate)
                tb.Columns.Remove("Transaction Date");

            return tb;
        }
        /// <summary>
        /// Accepts Customer# and Value Date as parameters
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public DataTable OMS_PortfolioSummaryPerSector(string sessionID, string ClientID, DateTime ValueDate)
        {
            var param = IWServices.CreateParameters(
                ClientID
                , ValueDate.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                );
            return GetDataWithParameter("P_00014", sessionID, param);
        }
        public DateTime OMS_MostRecentStockQuoteDate(string SessionID)
        {
            var result = GetScalarWithoutParameter("OMS_0004", CSessionManager.IWSessionID);

            if (result.StatusID == IWAPIEnum_APIStatus.NoError)
                return Common.ConvertEx.ToDateTime(result.OutValue);
            else
                throw new InfoWAREException(result.StatusMessage);
        }

        public DataTable OMS_Portfolio_Positions(string sessionID, string ClientID)
        {
            var param = IWServices.CreateParameters(ClientID);
            return GetDataWithParameter("OMS_0005", sessionID, param);
        }
        public Out_Result OMS_GetStockTradingAccountBalance(string sessionID, string ClientID)
        {
            var param = IWServices.CreateParameters(ClientID);
            return GetScalarWithParameter("OMS_0006", sessionID, param);
        }
        public DataTable OMS_GetStockList(string sessionID) { return GetDataWithoutParameter("OMS_0007", sessionID); }
        public Out_Result OMS_OpenOrdersBalance(string sessionID, string ClientID)
        {
            var param = IWServices.CreateParameters(ClientID);
            return GetScalarWithParameter("OMS_0008", sessionID, param);
        }
        public DataTable OMS_CSCSInfo(string sessionID, string ClientID)
        {
            return Cust_Info(sessionID, ClientID, enumAPICustInfo.CSCSNumbers);

            //m_fullurl = string.Format("{0}/CustInfo/{1}/{2}/{3}"
            //, m_url
            //, CSessionManager.IWSessionID
            //, CSessionManager.LoggedInUser.UserID
            //, (int)enumAPICustInfo.CSCSNumbers);
            //string json_data = Common.HTMLHelper.GetHtmlPage(m_fullurl);

            //DataTable tb = null;
            //var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
            //if (result.StatusID == IWAPIEnum_APIStatus.NoError)
            //    tb = CJSONHelper.ConvertToDatatable(result.DataTable);
            //return tb;
        }

        public int Cust_CreateNewAccountTemp(string sessionID, string title, string surname, string firstname, string othername, DateTime birthdate, string gender, string address, string city,
            string country, string state, string localGov, string email, string phone, string homePhone, string occupation, string company, string typeOfEmployee,
            string politicalResponse, string nameOfBank, string accountNumber, string bvn, string identityType, string IDNumber, DateTime dateOfExpiry, string kinFullname, string relationship, string contact,
            string phoneNumber, string nextofKinEmail)
        {
            var param = IWServices.CreateParameters(title, Common.StringEx.ReplaceSingleQuote(surname), Common.StringEx.ReplaceSingleQuote(firstname), othername, birthdate.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT), gender, Common.StringEx.ReplaceSingleQuote(address), city
                , country, state, localGov, email, phone, homePhone, Common.StringEx.ReplaceSingleQuote(occupation), Common.StringEx.ReplaceSingleQuote(company), typeOfEmployee
                , politicalResponse, nameOfBank, accountNumber, bvn, Common.StringEx.ReplaceSingleQuote(identityType), IDNumber, Common.ConvertEx.ToDateTime(dateOfExpiry).ToString(Common.DateHelper.FMT_DT), Common.StringEx.ReplaceSingleQuote(kinFullname), Common.StringEx.ReplaceSingleQuote(relationship), Common.StringEx.ReplaceSingleQuote(contact)
                , phoneNumber, nextofKinEmail);

            Out_Result ret = GetScalarWithParameter("P_00030", sessionID, param);
            if (ret.StatusID == IWAPIEnum_APIStatus.NoError)
                return Common.ConvertEx.ToInt(ret.OutValue);
            else
            {
                IWGlobals.LastBusObjError = new IWBusObj_Error(ret.StatusMessage);
                return -1;
            }
        }

        //public string CustAddFileTmp_OLD(string SessionID, int ParentID, string FileName, string FileType)
        //{
        //    m_fullurl = string.Format("CustAddFileTmp/{0}?CustAID={1}&FileName={2}&FileType={3}"
        //                , m_url
        //                , SessionID
        //                , ParentID
        //                , FileName
        //                , FileType);
        //    string json_data = Common.HTMLHelper.GetHtmlPage(m_fullurl);
        //    var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
        //    return result.StatusMessage;
        //}

        public bool Cust_AddFileTmp(string SessionID, int ParentID, string FileName, string FileType, HttpPostedFileBase file)
        {
            try
            {
                m_fullurl = string.Format("{0}/CustAddFileTmp/{1}?ParentID={2}&FileName={3}&FileType={4}"
                           , m_url
                           , SessionID
                           , ParentID.ToString()
                           , FileName
                           , FileType
                           );

                //Logger.LogActivity(m_fullurl);
                var fn = new CFunctionCall();
                var req = WebRequest.Create(m_fullurl) as HttpWebRequest;
                if (req == null)
                {
                    IWGlobals.LastBusObjError = new IWBusObj_Error("* Operation failed. You can not add null files");
                    return false;
                }

                req.Method = "POST";
                req.ContentType = file.ContentType; //"image/jpeg";
                var reqStream = req.GetRequestStream();
                var ms = new MemoryStream();

                file.InputStream.CopyTo(ms);

                var imageArray = ms.ToArray();
                reqStream.Write(imageArray, 0, imageArray.Length);
                reqStream.Close();
                //reqStream.Dispose();
                var resp = (HttpWebResponse)req.GetResponse();
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    IWGlobals.LastBusObjError = new IWBusObj_Error(resp.StatusDescription);
                    return false;
                }
                //string ln;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable OMS_OpenOrders(string sessionID, string ClientID) { return Cust_Info(sessionID, ClientID, enumAPICustInfo.OpenOrders); }
        public DataTable OMS_ExecutedOrders(string sessionID, string ClientID) { return Cust_Info(sessionID, ClientID, enumAPICustInfo.ExecutedOrders); }
        public DataTable OMS_CancelledOrders(string sessionID, string ClientID) { return Cust_Info(sessionID, ClientID, enumAPICustInfo.CancelledOrders); }
        public DataTable OMS_RejectedOrders(string sessionID, string ClientID) { return Cust_Info(sessionID, ClientID, enumAPICustInfo.RejectedOrders); }
        public DataTable OMS_CustomerBalances(string sessionID, string ClientID) { return Cust_Info(sessionID, ClientID, enumAPICustInfo.CustomerBalances); }
        public DataTable OMS_PortfolioTransaction(string SessionID, string ClientID) { return Cust_Info(SessionID, ClientID, enumAPICustInfo.PortfolioTxns); }

        //Newly added
        public DataTable OMS_TBills(string sessionID, string CustAID, DateTime ValueDate)
        {
            var param = IWServices.CreateParameters(
                CustAID
                , ValueDate.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                );
            return GetDataWithParameter("P_00024", sessionID, param);
        }

        public DataTable OMS_Bonds(string sessionID, string CustAID, DateTime ValueDate)
        {
            var param = IWServices.CreateParameters(
                CustAID
                , ValueDate.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                );
            return GetDataWithParameter("P_00025", sessionID, param);
        }

        //new functions P_00026
        public DataTable AvailableTBills(string sessionID)
        {
            return GetDataWithoutParameter("P_00026", sessionID);
        }

        public DataTable AvailableBonds(string sessionID)
        {
            return GetDataWithoutParameter("P_00027", sessionID);
        }

        internal DataTable BondsRequest(string SessionID, string CustAID, string SelectedBond, DateTime LastCouponDate, DateTime MaturityDate, double CouponRate, double IssuingPrice, double Consideration, int Tenure)
        {
            var param = IWServices.CreateParameters(
                CustAID
                , SelectedBond
                , Common.ConvertEx.ToDateTime(LastCouponDate).ToString(Common.DateHelper.FMT_DT)
                , Common.ConvertEx.ToDateTime(MaturityDate).ToString(Common.DateHelper.FMT_DT)
                , CouponRate.ToString()
                , IssuingPrice.ToString()
                , Tenure.ToString()
                , Consideration.ToString()
                );

            DataTable Breq = GetDataWithParameter(
                "P_00028"
                , CSessionManager.IWSessionID
                , param);

            return Breq;
        }


        internal DataTable TBillsRequest(string SessionID, string CustAID, string SelectedTBill, DateTime LastCouponDate, DateTime MaturityDate, double Rate, double Yield, double Consideration, int Tenure)
        {
            var param = IWServices.CreateParameters(
                CustAID
                , SelectedTBill
                , Common.ConvertEx.ToDateTime(LastCouponDate).ToString(Common.DateHelper.FMT_DT)
                , Common.ConvertEx.ToDateTime(MaturityDate).ToString(Common.DateHelper.FMT_DT)
                , Rate.ToString()
                , Yield.ToString()
                , Tenure.ToString()
                , Consideration.ToString()
                );

           DataTable TBreq = GetDataWithParameter(
                "P_00029"
                , CSessionManager.IWSessionID
                , param);

            return TBreq;
        }

        public DataTable BondsRequestHistory(string SessionID, string CustAID)
        {
            var param = IWServices.CreateParameters(CustAID);

            DataTable BReqHistory = GetDataWithParameter(
                "P_00031"
                , CSessionManager.IWSessionID
                , param);

            return BReqHistory;
        }

        public DataTable TBillsRequestHistory(string SessionID, string CustAID)
        {
            var param = IWServices.CreateParameters(CustAID);

            DataTable BReqHistory = GetDataWithParameter(
                "P_00032"
                , CSessionManager.IWSessionID
                , param);

            return BReqHistory;
        }

        public DataTable StockPrice(string SessionID, string StockCode)
        {
            var param = IWServices.CreateParameters(StockCode);

            DataTable StockPrice = GetDataWithParameter(
                "P_00036"
                , SessionID
                , param);
            return StockPrice;
        }


        public DataTable GetNetPay(string sessionID, string EmpDate, string EmpID)
        {
            var param = IWServices.CreateParameters(EmpDate, EmpID);
            return GetDataWithParameter("P_00062", sessionID, param);
        }

        internal DataTable OMS_TradeRequest(string SessionID, string CustAID, string CsCsNumber, string StkCode, int sTradeAction, double sUnits, int sOrderType, double sPriceLimit, int sTimeInForce, DateTime sDateLimit, DateTime sEffectiveDate, string Instructions, out string ErrMsg)
        {
            //m_fullurl = string.Format("{0}/TradeRequest/{1}/{2}/{3}"
            m_fullurl = string.Format("{0}/TradeRequest/{1}/?CustAID={2}&CsCsNumber={3}&StkCode={4}&sTradeAction={5}&sUnits={6}&sOrderType={7}&sPriceLimit={8}&sTimeInForce={9}&sDateLimit={10}&sEffectiveDate={11}&Instructions={12}"
            , m_url
            , CSessionManager.IWSessionID
            , CustAID
            , CsCsNumber
            , StkCode
            , sTradeAction
            , sUnits
            , sOrderType
            , sPriceLimit
            , sTimeInForce
            , sDateLimit.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
            , sEffectiveDate.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
            , Instructions);
            string json_data = Common.HTMLHelper.GetHtmlPage(m_fullurl);

            DataTable tb = null;
            ErrMsg = string.Empty;
            var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
            if (result.StatusID == IWAPIEnum_APIStatus.NoError)
                tb = CJSONHelper.ConvertToDatatable(result.DataTable);
            else
                ErrMsg = result.StatusMessage;

            return tb;
        }
        internal DataTable OMS_TradeRequest(string SessionID, int PIN, string CustAID, string CsCsNumber, string StkCode, int TradeAction, double Units, int OrderType, double PriceLimit, int TimeInForce, DateTime DateLimit, DateTime Effectivedate, string Instructions, out string ErrMsg)
        {
            m_fullurl = string.Format("{0}/PlaceTrade/{1}?SMSPIN={2}&CustAID={3}&CsCsNumber={4}&StkCode={5}&sTradeAction={6}&sUnits={7}&sOrderType={8}&sPriceLimit={9}&sTimeInForce={10}&sDateLimit={11}&sEffectiveDate={12}&Instructions={13}"
            , m_url
            , SessionID
            , PIN
            , CustAID
            , CsCsNumber
            , StkCode
            , (int)TradeAction
            , Units
            , (int)OrderType
            , PriceLimit
            , (int)TimeInForce
            , DateLimit.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
            , Effectivedate.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
            , Instructions);
            string json_data = Common.HTMLHelper.GetHtmlPage(m_fullurl);

            DataTable tb = null;
            ErrMsg = string.Empty;
            var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
            if (result.StatusID == IWAPIEnum_APIStatus.NoError)
                tb = CJSONHelper.ConvertToDatatable(result.DataTable);
            else
                ErrMsg = result.StatusMessage;

            return tb;
        }
        internal string OMS_CancelTrade(string SessionID, int TradeID)
        {
            m_fullurl = string.Format("{0}/CancelTrade/{1}?JbkID={2}"
            , m_url
            , SessionID
            , TradeID);
            string json_data = Common.HTMLHelper.GetHtmlPage(m_fullurl);

            const string HTMLEND = "</html>";   //Something weird, if there is an exception in the API, some html text is prepended so we detect and remove before conversion
            int idx = json_data.IndexOf(HTMLEND);
            if (idx > 0)
                json_data = json_data.Substring(idx + HTMLEND.Length).Replace("\n", "").Replace("\r", "");

            var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
            return result.StatusMessage;
        }
        public DataTable OMS_PurchaseContractNote_NSE(string SessionID, string CustAID, DateTime DateFrm, DateTime DateTo)
        {
            if (!Common.DateHelper.IsValidSQL2005Date(DateFrm))
                DateFrm = DateTime.Today;
            if (!Common.DateHelper.IsValidSQL2005Date(DateTo))
                DateTo = DateTime.Today;
            int Aggregate = 1;
            var param = IWServices.CreateParameters(
                CustAID
                , DateFrm.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                , DateTo.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                , Aggregate.ToString()
                );
            return GetDataWithParameter("OMS_0009", SessionID, param);
        }
        public DataTable OMS_SaleContractNote_NSE(string SessionID, string CustAID, DateTime DateFrm, DateTime DateTo)
        {
            if (!Common.DateHelper.IsValidSQL2005Date(DateFrm))
                DateFrm = DateTime.Today;
            if (!Common.DateHelper.IsValidSQL2005Date(DateTo))
                DateTo = DateTime.Today;
            int Aggregate = 1;
            var param = IWServices.CreateParameters(
                CustAID
                , DateFrm.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                , DateTo.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                , Aggregate.ToString()
                );
            return GetDataWithParameter("OMS_0010", SessionID, param);
        }
        public DataTable OMS_Certificates(string SessionID, string CustAID, DateTime DateFrm, DateTime DateTo)
        {
            if (!Common.DateHelper.IsValidSQL2005Date(DateFrm))
                DateFrm = DateTime.Today;
            if (!Common.DateHelper.IsValidSQL2005Date(DateTo))
                DateTo = DateTime.Today;
            var param = IWServices.CreateParameters(
                CustAID
                , DateFrm.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                , DateTo.ToString(Common.DateHelper.FMT_DT_LOCALEINDEPENDENT)
                );
            return GetDataWithParameter("OMS_0011", SessionID, param);
        }
        #endregion OMS Functions

        #region Customer info Functions
        private DataTable Cust_Info(string sessionID, string ClientID, enumAPICustInfo CustInfoFnID)
        {

            m_fullurl = string.Format("{0}/CustInfo/{1}/{2}/{3}"
            , m_url
            , CSessionManager.IWSessionID
            , CSessionManager.LoggedInUser.UserID
            , (int)CustInfoFnID);
            string json_data = Common.HTMLHelper.GetHtmlPage(m_fullurl);

            DataTable tb = null;
            var result = CJSONHelper.JsonConvertDeserialise<Out_ResultD>(json_data);
            if (result.StatusID == IWAPIEnum_APIStatus.NoError)
                tb = CJSONHelper.ConvertToDatatable(result.DataTable);
            return tb;
        }
        public DataTable Cust_BioDataInfo(string SessionID, string ClientID)
        {
            return Cust_Info(SessionID, ClientID, enumAPICustInfo.BioData);
        }
        #endregion Customer info Functions

        public void EmailSender(string custEmail, string fromEmailAddress, string fromEmailAddressName, string emailSubject, string emailMsg)
        {
            try
            {
                var mc = new IWMailClientEx(custEmail, fromEmailAddress, fromEmailAddressName, emailSubject, emailMsg);
                mc.IsBodyHtml = true;
                //mc.AttachmentFiles = attach;
                // mc.Bcc = m_bcc;
                mc.SendMailEx();
            }
            catch
            { }
        }

        public string GetEmployeeGP(string SessionId, string EmpId, string AppraisalPeriodStartDate, string AppraisalPeriodEndDate)
        {
            string stPeriod = (string.IsNullOrEmpty(AppraisalPeriodStartDate)) ? string.Format("{0}-01-01", DateTime.Today.Year) : AppraisalPeriodStartDate;
            string endPeriod = (string.IsNullOrEmpty(AppraisalPeriodEndDate)) ? string.Format("{0}-12-31", DateTime.Today.Year) : AppraisalPeriodEndDate;

            var param = IWServices.CreateParameters(EmpId, stPeriod, endPeriod);

            Out_Result ret = GetScalarWithParameter("ESS_009", SessionId, param);
            if (ret.StatusID == IWAPIEnum_APIStatus.NoError)
                return Common.ConvertEx.ToString(ret.OutValue);
            else
            {
                IWGlobals.LastBusObjError = new IWBusObj_Error(ret.StatusMessage);
                return ret.StatusMessage;
            }
        }

        public string GetEmployeeCGPA(string SessionId, string EmpId)
        {
            var param = IWServices.CreateParameters(EmpId);

            Out_Result ret = GetScalarWithParameter("ESS_008", SessionId, param);
            if (ret.StatusID == IWAPIEnum_APIStatus.NoError)
                return Common.ConvertEx.ToString(ret.OutValue);
            else
            {
                IWGlobals.LastBusObjError = new IWBusObj_Error(ret.StatusMessage);
                return ret.StatusMessage;
            }
        }

        public string GetCommitmentMeanScore(string SessionId, string CommitmentId,string EmpId)
        {
            var param = IWServices.CreateParameters(CommitmentId,EmpId);

            Out_Result ret = GetScalarWithParameter("ESS_0010", SessionId, param);
            if (ret.StatusID == IWAPIEnum_APIStatus.NoError)
                return Common.ConvertEx.ToString(ret.OutValue);
            else
            {
                IWGlobals.LastBusObjError = new IWBusObj_Error(ret.StatusMessage);
                return ret.StatusMessage;
            }
        }


        public DataTable GetCommitmentResponseDetails(string EmpID, int CommitmentID)
        {
            var param = IWServices.CreateParameters(
                EmpID.ToString(),
                CommitmentID.ToString()
                );
            DataTable tb = GetDataWithParameter(
                "ESS_0011"
                , CSessionManager.IWSessionID
                , param);

            return tb;
        }
    }
}
