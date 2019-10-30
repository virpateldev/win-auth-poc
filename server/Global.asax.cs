using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NLog;

namespace sme
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
       

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
 

            string[] allowedOrigin = new string[] { "http://localhost:4200" };
            var origin = HttpContext.Current.Request.Headers["Origin"];
            if (origin != null && allowedOrigin.Contains(origin))
            {
               // HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", origin);
                //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET,POST");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Credentials", "true");
            }
 
            /*if (Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.StatusCode = 200;
                var httpApplication = sender as HttpApplication;
                httpApplication.CompleteRequest();
            }
            */
            /*string httpOrigin = HttpContext.Current.Request.Params["HTTP_ORIGIN"] ?? HttpContext.Current.Request.Params["ORIGIN"] ?? "*";
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", httpOrigin);
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, X-Token");
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Credentials", "true");

            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.StatusCode = 200;
                var httpApplication = sender as HttpApplication;
                httpApplication.CompleteRequest();
            }
            */

        }

    }
}
