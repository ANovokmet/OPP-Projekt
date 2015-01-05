using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UpravljanjeCekanjem.Models;

namespace UpravljanjeCekanjem.Controllers
{
    [AllowAnonymous]
    public class ScreenController : Controller
    {
        //
        // GET: /Screen/
        DataBaseEntities db = new DataBaseEntities();

        public ActionResult Index()//u viewu treba hrpa jquerija zbog signalR i realtime, ovo je za probu, prebacit u cvorhub
        {
            ViewBag.broj = "BROJ KOJI FLASHA";

            Dictionary<string, int> redovi = new Dictionary<string,int>();

            using(var db = new DataBaseEntities())
            {
                var tipovi = from c in db.TipTiketa
                             where c.ponudjena == true
                             select c;
                var postavke = db.Postavke.Where(s => s.Identifikator.Equals(0)).FirstOrDefault<Postavke>();
                ViewBag.vel = postavke.vrijednost;
                postavke = db.Postavke.Where(s => s.Identifikator.Equals(1)).FirstOrDefault<Postavke>();
                ViewBag.boja = postavke.vrijednost;

                foreach (TipTiketa a in tipovi)
                {
                    var prviBroj =
                        from x in db.Tiket
                        where x.tip.Equals(a.tip) && x.vrijemeDolaska == null
                        && x.vrijemeIzdavanja.Day == DateTime.Now.Day
                        && x.vrijemeIzdavanja.Month == DateTime.Now.Month
                        && x.vrijemeIzdavanja.Year == DateTime.Now.Year
                        orderby x.vrijemeIzdavanja ascending
                        select x.redniBroj;

                    if (prviBroj.Any())
                    {
                        redovi.Add(a.tip, prviBroj.First());//trenutni broj
                    }
                    else
                    {
                        redovi.Add(a.tip, 0);
                    }
                }
            }


            return View(redovi);
        }

    }
}
