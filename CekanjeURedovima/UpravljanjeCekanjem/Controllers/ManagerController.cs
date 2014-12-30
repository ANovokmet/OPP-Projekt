using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;

namespace UpravljanjeCekanjem.Controllers
{
    public class ManagerController : Controller
    {
        //
        // GET: /Manager/

        public ActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            ViewBag.Role = claimsIdentity.FindFirst(ClaimTypes.Role).Value;

            if (ViewBag.Role.Equals("službenik "))
            {
                return RedirectToAction("Index", "Clerk");
            }
            return View();
        }

    }
}
