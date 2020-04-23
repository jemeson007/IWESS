using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace IWEBIZ.Logging
{
    [Flags]
    public enum LogLevel
    {
        Info = 1,
        Warning = 2,
        Error = 4,
    }

    //public enum IWEnum_APICaller
    //{
    //    WP8_42 = 0, //Old 4.2 version of WP8 used 0 but going forward, this should now belong to 'IWERP'
    //    IwT = 1, //InfoWARE Terminal
    //    WP8 = 2,
    //    AND = 3,
    //    iPhone = 4,
    //    iPad = 5,
    //    Wn8 = 6,
    //    ERP = 7,     //InfoWARE ERP
    //    MDC = 8,    //Market Data Client
    //    PAPI = 9,      //InfoWARE Platform API
    //    EBIZ = 10      //InfoWARE mobile or desktop EBiz
    //}
    public class Logger
    {
        static Logger()
        {
            // Initiate config of Loglevel 
            int i = 0;
            int.TryParse(ConfigurationManager.AppSettings["LogLevel"], out i);

            switch (i)
            {
                case 0:
                    Logger.level = LogLevel.Error;
                    break;
                case 1:
                    Logger.level = LogLevel.Warning | LogLevel.Error;
                    break;
                case 2:
                    Logger.level = LogLevel.Info | LogLevel.Warning | LogLevel.Error;
                    break;
                default:
                    Logger.level = LogLevel.Error;
                    break;
            }


            // Initiate config of log switch
            int.TryParse(ConfigurationManager.AppSettings["LogSwitch"], out i);

            Logger.isKeepLog = (i != 0) ? true : false;

        }

        private static LogLevel level = LogLevel.Error;

        private static bool isKeepLog = true;

        private static string configLogDir = string.Empty;

        private static string LogDir
        {
            get
            {
                if (string.IsNullOrEmpty(Logger.configLogDir))
                {
                    string dir = ConfigurationManager.AppSettings["LogFileDir"];

                    if (!Directory.Exists(dir))
                    {
                        dir = Path.Combine(HttpRuntime.AppDomainAppPath, "Log");
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }
                    Logger.configLogDir = dir;
                }

                return Logger.configLogDir;
            }
        }

        public static void LogInfo(string message)
        {
            if (!Logger.isKeepLog)
                return;

            if ((Logger.level & LogLevel.Info) == LogLevel.Info)
            {
                string filename = string.Format(CLogStrings.Log_TraceFileNameFormat, DateTime.Now);
                Logger.WriteLog(filename, string.Format(CLogStrings.Log_BodyFormat, DateTime.Now.ToString(), CLogStrings.Log_Info, message));
            }
        }
        public static void LogInfo(string APIName, string message)
        {
            if (!Logger.isKeepLog)
                return;

            //if ((Logger.level & LogLevel.Info) == LogLevel.Info)
            {
                string filename = string.Format(CLogStrings.Log_TraceFileNameFormat, DateTime.Now);
                Logger.WriteLog(filename, string.Format(CLogStrings.Log_BodyFormat, DateTime.Now.ToString(), CLogStrings.Log_Info, APIName, message));
            }
        }

        public static void LogWarning(string message)
        {
            if (!Logger.isKeepLog)
                return;

            if ((Logger.level & LogLevel.Warning) == LogLevel.Warning)
            {
                string filename = string.Format(CLogStrings.Log_WarningFileNameFormat, DateTime.Now);
                Logger.WriteLog(filename, string.Format(CLogStrings.Log_BodyFormat, DateTime.Now.ToString(), CLogStrings.Log_Warning, message));

            }
        }

        public static void LogError(string message)
        {
            if (!Logger.isKeepLog)
                return;

            if ((Logger.level & LogLevel.Error) == LogLevel.Error)
            {
                string filename = string.Format(CLogStrings.Log_ErrorFileNameFormat, DateTime.Now);
                Logger.WriteLog(filename, string.Format(CLogStrings.Log_BodyFormat, DateTime.Now.ToString(), CLogStrings.Log_Error, message));
            }
        }

        private static void WriteLog(string filename, string logMessage)
        {
            string fullPath = Path.Combine(Logger.LogDir, filename);

            try
            {
                using (StreamWriter sw = new StreamWriter(fullPath, true))
                {
                    sw.WriteLine(logMessage);
                }
            }
            catch
            {
                Logger.isKeepLog = false;  // If error happen, disable the log and send email to admin.
                // [TBD] Send mail to admin
            }
        }
        //[Obsolete]  //Remove after completion of conversion to Platform API code from MD code
        public static void LogActivity(int Source, string WSFName, int PIN, int ClientToken, int FunctionID, string Params)
        {
            LogActivity(Source, WSFName, PIN, ClientToken, FunctionID, Params, string.Empty, string.Empty);
        }
        public static void LogActivity(string WSFName, string SessionID, string SecID, string Params, string SecRights, string Message)
        {
            string senderIPAddress = GetCallerIPAddress();
            const string fmtstring = "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}";
            string filename = string.Format(CLogStrings.Log_ActivityFileNameFormat, DateTime.Now);
            string logMessage = string.Format(fmtstring, WSFName, SessionID, SecID, Params, SecRights, senderIPAddress, DateTime.Now, Message);
            if (!File.Exists(Path.Combine(Logger.LogDir, filename)))
            {
                string header = string.Format(fmtstring, "FnName", "SessionID", "SecID", "Params", "SecRights", "senderIPAddress", "DateTime", "ExtraInfo");
                WriteLogV2(header, filename);
            }
            WriteLogV2(logMessage, filename);
        }
        public static string GetCallerIPAddress()
        {
            //string senderIPAddress = "LOCALHOST";
            string senderIPAddress = HttpContext.Current.Request.UserHostAddress;

            //if (OperationContext.Current != null)
            //{
            //    RemoteEndpointMessageProperty clientEndpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            //    senderIPAddress = clientEndpoint.Address;
            //}

            return senderIPAddress;
        }

        //public static void LogActivity(string WSFName, CRequestPkg Req)
        //{
        //    LogActivity(Req.Src, WSFName, Req.Pin, Req.ClientToken, Req.FunctionID, Req.Params, Req.SQL, Req.DeviceID);
        //}
        public static void LogActivity(int Source, string WSFName, int PIN, int ClientToken, int FunctionID, string Params, string SQL, string DeviceID)
        {
            string device = Source.ToString(); //string device = DeviceType(Source);
            if (!string.IsNullOrWhiteSpace(SQL))
                SQL = SQL.Replace("\n", " ").Replace("\r", " ").Replace("\t", " ");    //Remove all carrage return/line feed xters so sql sits on single line and looks good when log file is loaded into excel etc

            //RemoteEndpointMessageProperty clientEndpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            //string senderIPAddress = clientEndpoint.Address;
            string senderIPAddress = HttpContext.Current.Request.UserHostAddress;

            string filename = string.Format(CLogStrings.Log_ActivityFileNameFormat, DateTime.Now);
            string logMessage = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}", device, WSFName, PIN, ClientToken, FunctionID, Params, DeviceID, SQL, senderIPAddress, DateTime.Now);
            if (!File.Exists(Path.Combine(Logger.LogDir, filename)))
            {
                string header = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}", "Device", "FnName", "PIN", "ClientToken", "FunctionID", "Params", "DeviceID", "SQL", "senderIPAddress", "DateTime");
                WriteLogV2(header, filename);
            }
            WriteLogV2(logMessage, filename);
        }

        //private static string DeviceType(int Source)
        //{
        //    string device;  // = "Unk";
        //    switch (Source)
        //    {
        //        case 0:
        //            device = "WP8";
        //            break;
        //        case 1:
        //            device = "Wn8";
        //            break;
        //        case 2:
        //            device = "iOS";
        //            break;
        //        case 3:
        //            device = "And";
        //            break;
        //        case 4:
        //            device = "BB";
        //            break;
        //        case 5:
        //            device = "MDC"; //Market data client
        //            break;
        //        case 6:
        //            device = "IwT"; //InfoWARE Terminal
        //            break;
        //        case 7:
        //            device = "ERP"; //InfoWARE ERP
        //            break;
        //        default:
        //            device = string.Format("Unk{0}", Source);
        //            break;
        //    }
        //    return device;
        //}
        //private static string DeviceType(int Source)
        //{
        //    string device;  // = "Unk";

        //    if (Enum.IsDefined(typeof(IWEnum_APICaller), Source))
        //        device = ((IWEnum_APICaller)Source).ToString();
        //    else
        //        device = string.Format("Unk{0}", Source);

        //    return device;
        //}

        public static void WriteLogV2(string logMessage, string LogFileName)
        {
            string filename = LogFileName;
            string fullPath = Path.Combine(Logger.LogDir, filename);

            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(fullPath, true))
                {
                    sw.WriteLine(logMessage);
                }
            }
            catch
            {
                Logger.isKeepLog = false;  // If error happen, disable the log and send email to admin.
                // [TBD] Send mail to admin
            }
        }
    }

    public class CLogStrings
    {
        #region Log Strings

        //public static string Log_BodyFormat = "\r\n---------------------------------------\r\nLog Time:{0}\r\nLog Type:{1}\r\nAPIName:{2}\r\n{3}\r\n---------------------------------------";
        public static string Log_BodyFormat = "\r\n---------------------------------------\r\nLog Time:{0}\r\nLog Type:{1}\r\nAPIName:{2}\r\n---------------------------------------";
        public static string Log_Error = "Error";
        public static string Log_Warning = "Warning";
        public static string Log_Info = "Trace";

        public static string Log_TraceFileNameFormat = "IWLOGTrace_{0:yy_MM_dd}.LOG";
        public static string Log_WarningFileNameFormat = "IWLOGWarning_{0:yy_MM_dd}.LOG";
        public static string Log_ErrorFileNameFormat = "IWLOGErrors_{0:yy_MM_dd}.LOG";
        public static string Log_ActivityFileNameFormat = "IWLOGAct_{0:yy_MM_dd}.LOG";

        public static string FormatLog(string source, string detail, params object[] pars)
        {
            string template = string.Format("{0}\r\nSource:{1}", detail, source);

            if (pars.Length > 0)
            {
                template = string.Format("{0}\r\n{1}", template, "Pamaters:");
                string tmp = string.Empty;
                for (int i = 0; i < pars.Length; i++)
                {
                    tmp = string.Format("  Paramter[{0}]:{1}", i, pars[i]);
                    template = string.Format("{0}\r\n{1}", template, tmp);
                }

            }
            return string.Format(template, pars);
        }

        #endregion

        #region System Message

        public static string Sys_Error = "The request was refused\r\n  Unexpected error occured on the server, please contact admin for more help.";

        #endregion

        //#region Validation Message

        //public static string Val_Error_ClientNotDefined = "The request was refused\r\n  The Client is not defined on server.";
        //public static string Val_Error_InActive = "The request was refused\r\n  The Client is disabled on server.";
        //public static string Val_Error_Expir = "The request was refused\r\n  The service of this client is expired.";

        //#endregion

        //#region Function Def message

        //public static string FunDef_Error_FunctionNotDefined = "The request was refused\r\n  The Function is not defined on server.";
        //public static string FunDef_Error_NoPara = "The request was refused\r\n  The Function is defined with paramater on server.";
        //public static string FunDef_Error_IncorPara = "The request was refused\r\n  The Function is defined without paramater on server.";

        //#endregion

    }
}