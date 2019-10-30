using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using NLog;

namespace sme.Controllers
{
    public class CustomWindowsAuthorize: System.Web.Http.AuthorizeAttribute
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {

            //Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            var identity = HttpContext.Current.User.Identity; 
            string method = HttpContext.Current.Request.HttpMethod;
            logger.Info("Request Method" + Environment.NewLine + method);
            if (method != "OPTIONS" && identity.Name != string.Empty)
            {
                if (identity == null &&
                HttpContext.Current != null)
                {
                    identity = HttpContext.Current.User.Identity;
                }
                logger.Info("IdentityName" + Environment.NewLine + identity.Name);
                logger.Info("IsAuthenticated" + Environment.NewLine + identity.IsAuthenticated);
                if (identity != null && identity.IsAuthenticated)
                {
                    //check for role from ad group.
                    return helper.isValidUser(identity.Name);
                }
                return false;
            }
            else
            {
                logger.Info("Name is empty from request" + Environment.NewLine + identity.Name);
            }
            return base.IsAuthorized(actionContext);
            //return true;
        }
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            //Code to handle unauthorized request
            actionContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
                Content = new StringContent("You are unauthorized to access this resource")
            };
        }

    }
}