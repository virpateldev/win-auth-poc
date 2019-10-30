using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace sme.Controllers
{
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


}
