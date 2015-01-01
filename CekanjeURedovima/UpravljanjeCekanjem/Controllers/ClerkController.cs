using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;

namespace UpravljanjeCekanjem.Controllers
{
    [CustomAuthorize(Roles = "službenik")]
    public class ClerkController : Controller
    {
        //
        // GET: /Clerk/

        public ActionResult Index()
        {
            return View();
        }

    }
}
