using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UpravljanjeCekanjem.Controllers
{
    public class ClerkController : Controller
    {
        //
        // GET: /Clerk/

        public ActionResult Index()
        {
            if (Request.Cookies["currentuser"] == null)
            {
                return RedirectToAction("Login", "HomeUser");
            }
            if (Request.Cookies["currentuser"]["razinaprava"].Equals("nadzornik"))
            {
                return RedirectToAction("Index", "Manager");
            }
            return View();
        }

    }
}
