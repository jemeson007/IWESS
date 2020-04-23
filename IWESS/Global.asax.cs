//using IWEBIZ.Logging;
using IWESS.Models;
using IWNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace IWESS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            CIWNetInit.InitIWNet();
        }

        internal class CIWNetInit
        {
            /// <summary>
            /// NOTE: This call expects APP.CONFIG and .IWPKG files to be in same location as DLL
            /// </summary>
            public static void InitIWNet()
            {
                try
                {
                    if (IWGlobals.AppInstance != null)
                        return;   //Already initialized

                    string DataSource;
                    string SSUid;
                    string SSPwd;
                    string SSCatalog;
                    string AssemblyVersion;
                    string appName = string.Empty;
                    IWGlobals.InitializeApp(appName, "InfoWARE ESS Project",
                        out DataSource, out SSUid, out SSPwd, out SSCatalog, out AssemblyVersion);
                    IWNet.Common.Logging.LogToFile("Returned from IWGlobals.InitializeApp...");
                    IWGlobals.EnableEventLogging = true;
                    IWGlobals.AppInstance.LogOnUser = "SYSTEM";
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
        }

        //protected void Application_Error()
        //{
        //    var ex = Server.GetLastError();
        //    //log the error!
        //    IWEBIZ.Logging.Logger.LogError(ex.ToString());
        //    Response.Redirect("~/Operation/Index");
        //}


        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception TheError = Server.GetLastError();
            //Logger.LogError(TheError.ToString()); // Make sure to use this in every code that you write

            if (!CSessionManager.IsWWWSessionExists())
            {
                Response.Redirect("~/Operation/Index");
                return;
            }

            if (TheError is HttpException)
            {
                Response.Redirect("~/Error/Index");
                return;
            }
            else
            {
                Response.Redirect("~/Error/Index");
                return;
            }

        }
    }
    
}
