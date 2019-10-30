using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using NLog;

namespace sme.Controllers
{
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
}