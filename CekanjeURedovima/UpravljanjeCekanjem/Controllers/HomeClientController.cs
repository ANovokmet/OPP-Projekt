using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UpravljanjeCekanjem.Models;

namespace UpravljanjeCekanjem.Controllers
{
    [AllowAnonymous]
    public class HomeClientController : Controller
    {
        //
        // GET: /HomeClient/
        DataBaseEntities db = new DataBaseEntities();

        public ActionResult Index()
        {
            var tipovi = from c in db.TipTiketa
                            where c.ponudjena == true
                            select c;

            return View(tipovi);
        }

        public ActionResult IzdajTiket(string tip)
        {
            Tiket tiket = new Tiket();

            var zadnjiBroj = 
                from x in db.Tiket
                where x.tip.Equals(tip) && x.vrijemeIzdavanja.Day == DateTime.Now.Day
                 && x.vrijemeIzdavanja.Month == DateTime.Now.Month
                 && x.vrijemeIzdavanja.Year == DateTime.Now.Year
                orderby x.redniBroj descending
                select x.redniBroj;
            if (zadnjiBroj.Any())
            {
                tiket.redniBroj = zadnjiBroj.First() + 1;
            }
            else
            {
                tiket.redniBroj = 1;
            }
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
            ViewBag.tiket = tiket;
            return View(tiket);
        }
    }
}
