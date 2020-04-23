using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace IWESS
{
    public static class WWWParameters
    {
        public static string GetAppsetting(string Key) { return ConfigurationManager.AppSettings[Key]; }
        public static string CustStmt_CreditColHeader
        {
            get
            {
                string t = GetAppsetting("CUSTSMT_CreditColHeader");
                t = string.IsNullOrWhiteSpace(t) ? "Credit" : t;
                return t;
            }
        }
        public static string CustStmt_DebitColHeader
        {
            get
            {
                string t = GetAppsetting("CUSTSMT_DebitColHeader");
                t = string.IsNullOrWhiteSpace(t) ? "Debit" : t;
                return t;
            }
        }
        public static bool CustStmt_OrderASC { get { return IWNet.Common.IsParamTrueOrYes(GetAppsetting("CS_ORDERASC")); } } //Customer statement is order DESC by default
        public static bool ShowHomePagePIEChartAsPerAssetClass { get { return IWNet.Common.IsParamTrueOrYes(GetAppsetting("ShowHomePagePIEChartAsPerAssetClass")); } } //True=PerSector is default; False = PerAssetClass
        public static bool CustStmt_ShowTxnDate { get { return IWNet.Common.IsParamTrueOrYes(GetAppsetting("CUSTSMT_ShowTxnDate")); }
        }
    }
}