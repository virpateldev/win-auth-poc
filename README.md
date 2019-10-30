# angular-windows-auth

In  Web.config file, add this section:
```
<system.web>
     <authentication mode="Windows" />
    <authorization>
      <allow verbs="OPTIONS" users="*"/>
      <deny users="?" />
    </authorization>  
  </system.web>
  ```
  Enable Cors:
  ```
  public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            string origin = "http://localhost:4200/";

            EnableCorsAttribute cors = new EnableCorsAttribute("http://localhost:4200", "*", "GET,POST") { SupportsCredentials=true};

            // Web API configuration and services
            config.EnableCors(cors);
            
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
  ```
Create own implementation of authorize attribute:
Nlog can be added to project using steps at https://www.c-sharpcorner.com/article/how-to-implement-nlog-in-webapi/
```
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
```
Helper Class
```
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using NLog;

    public static class helper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string getDomain(string identityName)
        {
            string s = identityName;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(0, stop) : string.Empty;
        }

        private  static string getLogin(string identityName)
        {
            string s = identityName;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(stop + 1, s.Length - stop - 1) : string.Empty;
        }
        public static bool isValidUser(string userInfo)
        {
            var userName = getLogin(userInfo);
            var domainName = getDomain(userInfo);
            var groupToCheck = System.Configuration.ConfigurationManager.AppSettings["groupName"];
            var domainToCheck = System.Configuration.ConfigurationManager.AppSettings["domainName"];
            logger.Info("Request Details" + Environment.NewLine + userInfo);
            logger.Info("Group and Domain Setting from config" + Environment.NewLine + " group to check--> " + groupToCheck + " domain to check--> " + domainToCheck);

            try
            {
                // set up domain context
                logger.Info("set up domain context" + Environment.NewLine + domainToCheck);
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainToCheck);
                logger.Info("domain context found" + Environment.NewLine + ctx);
                if (ctx != null)
                {
                    logger.Info("domain found" + Environment.NewLine + ctx);
                }
                else
                {
                    logger.Info("domain not found" + Environment.NewLine );
                }
                // find a user
                logger.Info("find a user " + Environment.NewLine + userName);
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, userName);
                if (user != null)
                {
                    logger.Info("user found" + Environment.NewLine + user);
                }
                else
                {
                    logger.Info("user not found" + Environment.NewLine );
                }
                 // find the group in question
                logger.Info("find a the group " + Environment.NewLine + groupToCheck);
                GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupToCheck);
                if (group != null)
                {
                    logger.Info("group found" + Environment.NewLine + group);
                }
                else
                {
                    logger.Info("group not found" + Environment.NewLine );
                }
                if (HttpContext.Current.User.Identity.IsAuthenticated && user != null && group != null)
                {
                    // Verify that the user is in the given AD group (if any)
                    logger.Info("checking if user is IsAuthenticated" + Environment.NewLine + HttpContext.Current.User.Identity.IsAuthenticated);
                    logger.Info(" check if user is member of that group" + Environment.NewLine + user.IsMemberOf(group));
                    // check if user is member of that group
                    if (user.IsMemberOf(group))
                    {
                        logger.Info(" user is Authenticated" + Environment.NewLine + userInfo);
                        return true;
                    }
                }
                logger.Info("Authenticated is failed" + Environment.NewLine + $"not authenticated: {userInfo},username: {userName} ,domainName: {domainName},group: {groupToCheck}");
                return false;
            }
            catch (Exception e)
            {
                logger.Info("exception occured " + Environment.NewLine + e);
                return false;

            }

        }
    }

```
In  Web.config file, add this section:
```
<appSettings>
    <add key="groupName" value="admin" />
    <add key="domainName" value="test" />
</appSettings>
```
set this attribute for controller:
```
[CustomWindowsAuthorize]
    //[Authorize]
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        [System.Web.Mvc.HttpGet()]       
        public async Task<HttpResponseMessage> getUser()
        {
 
            var domainToCheck = System.Configuration.ConfigurationManager.AppSettings["domainName"];
            // Verify that the user is in the given AD group (if any)
            var context = new PrincipalContext(
                                  ContextType.Domain,
                                  domainToCheck);

            var userPrincipal = UserPrincipal.FindByIdentity(
                                   context,
                                   IdentityType.SamAccountName,
                                   User.Identity.Name);

            var userDetails = new
            {
             EmailAddress=userPrincipal.EmailAddress,
             displayName = userPrincipal.Name,
             ldapName = User.Identity.Name
            };
           return Request.CreateResponse(HttpStatusCode.OK, $"{userDetails}");
        }

    }
    
```

# **IIS Settings:**
Under IIS, all of these seems to be solved under the Authentication icon.

Now go into the features of Authentication:

Enable **Windows Authentication**, then Right-Click to set the **Providers**.

NTLM needs to be **FIRST!**

![GitHub Logo](/images/auth0.JPG)
Next, check that under Advanced Settings... the Extended Protection is Accept and Enable Kernel-mode authentication is CHECKED:
![GitHub Logo](/images/auth1.JPG)

