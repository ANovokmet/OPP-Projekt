using System.Web.Mvc;
using System.Web;
using System.Web.Routing;

namespace UpravljanjeCekanjem
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "HomeUser", action = "Index" }));
                filterContext.Controller.TempData.Add("RedirectReason", "Unauthorized");
            }
        }
    }
}