using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Security.Principal;
using System.Security.Claims;

namespace UpravljanjeCekanjem
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs args)
        {
            if (Context.User != null)
            {
                
                string[] rolesArray = new string[1];

                var claimsIdentity = User.Identity as ClaimsIdentity;
                rolesArray[0] = claimsIdentity.FindFirst(ClaimTypes.Role).Value;

                //System.Diagnostics.Debug.WriteLine(Context.User.Identity.Name+rolesArray[0]);
                GenericPrincipal gp = new GenericPrincipal(Context.User.Identity, rolesArray);
                Context.User = gp;
            }
        }
    }


}