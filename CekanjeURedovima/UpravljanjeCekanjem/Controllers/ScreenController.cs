using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UpravljanjeCekanjem.Controllers
{
    public class ScreenController : Controller
    {
        //
        // GET: /Screen/

        public ActionResult Index()
        {
            ViewBag.broj = "BROJ KOJI FLASHA";
            return View();
        }

    }
}
