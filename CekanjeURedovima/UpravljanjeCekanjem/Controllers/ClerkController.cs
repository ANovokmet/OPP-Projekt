using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;

namespace UpravljanjeCekanjem.Controllers
{
    public class ClerkController : Controller
    {
        //
        // GET: /Clerk/

        public ActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            ViewBag.Role = claimsIdentity.FindFirst(ClaimTypes.Role).Value;

            if (ViewBag.Role.Equals("nadzornik "))
            {
                return RedirectToAction("Index", "Manager");
            }
            return View();
        }

    }
}
