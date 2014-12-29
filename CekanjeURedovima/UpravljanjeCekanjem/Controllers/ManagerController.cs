using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UpravljanjeCekanjem.Controllers
{
    public class ManagerController : Controller
    {
        //
        // GET: /Manager/

        public ActionResult Index()
        {
            if (Request.Cookies["currentuser"] == null)
            {
                return RedirectToAction("Login", "HomeUser");
            }
            if (Request.Cookies["currentuser"]["razinaprava"].Equals("sluĹľbenik"))
            {
                return RedirectToAction("Index", "Clerk");
            }
            return View();
        }

    }
}
