using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UpravljanjeCekanjem.Models;

namespace UpravljanjeCekanjem.Controllers
{
    public class HomeClientController : Controller
    {
        //
        // GET: /HomeClient/
        DataBaseEntities db = new DataBaseEntities();

        public ActionResult Index()
        {
            var db = new DataBaseEntities();

            var tipovi = from c in db.TipTiketa
                            where c.ponudjena == true
                            select c;
            

            return View(tipovi);
        }

        public ActionResult IzdajTiket(string tip)
        {
            Tiket tiket = new Tiket();
            tiket.redniBroj = 1;
            tiket.tip = tip;
            tiket.vrijemeIzdavanja = DateTime.Now;
            try
            {
                db.Tiket.Add(tiket);
                db.SaveChanges();
            }
            catch
            {
                
            }

            return RedirectToAction("Index");
        }
    }
}
