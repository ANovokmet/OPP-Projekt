using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UpravljanjeCekanjem.Models;

namespace UpravljanjeCekanjem.Controllers
{
    public class HomeUserController : Controller
    {
        //
        // GET: /HomeUser/

        public ActionResult Index()
        {
            List<Korisnik> korisnici = new List<Korisnik>();
            using (var db = new DataBaseEntities())
            {
                korisnici = db.Korisnik.ToList();
            }
            return View(korisnici);
        }

    }
}
