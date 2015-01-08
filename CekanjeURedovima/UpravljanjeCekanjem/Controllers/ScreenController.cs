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

            Dictionary<string, Tuple<int, double, int>> redovi = new Dictionary<string, Tuple<int, double, int>>();

            using(var db = new DataBaseEntities())
            {
                var tipovi = from c in db.TipTiketa
                             select c;

                var postavke = db.Postavke.Where(s => s.Identifikator.Equals(0)).FirstOrDefault<Postavke>();
                ViewBag.vel = postavke.vrijednost;
                postavke = db.Postavke.Where(s => s.Identifikator.Equals(1)).FirstOrDefault<Postavke>();
                ViewBag.boja = postavke.vrijednost;
                postavke = db.Postavke.Where(s => s.Identifikator.Equals(2)).FirstOrDefault<Postavke>();
                ViewBag.showvrijeme = postavke.vrijednost;
                postavke = db.Postavke.Where(s => s.Identifikator.Equals(3)).FirstOrDefault<Postavke>();
                ViewBag.showizdani = postavke.vrijednost;

                foreach (TipTiketa a in tipovi)
                {
                    
                    var tiketi = from t in db.Tiket
                                 where t.vrijemeDolaska != null
                                 && t.vrijemeIzdavanja.Day == DateTime.Now.Day
                                 && t.vrijemeIzdavanja.Month == DateTime.Now.Month
                                 && t.vrijemeIzdavanja.Year == DateTime.Now.Year
                                 && a.tip.Equals(t.tip)
                                 orderby t.vrijemeIzdavanja descending
                                 select t;

                    var count = (from t in db.Tiket where
                                 t.vrijemeIzdavanja.Day == DateTime.Now.Day
                                 && t.vrijemeIzdavanja.Month == DateTime.Now.Month
                                 && t.vrijemeIzdavanja.Year == DateTime.Now.Year
                                 && a.tip.Equals(t.tip)
                                 select t).Count();

                    int tiket = 0;
                    if (tiketi.Any())
                    {
                        if (tiketi.First().obrađeno == false)
                        {
                            tiket = tiketi.First().redniBroj;
                        }
                    }
                    tiketi.Count();
                    
                    redovi.Add(a.tip, new Tuple<int, double, int>(tiket, Global.get_avg_vrijeme_cekanja(a.tip), count ));//trenutni broj
                    
                }
            }


            return View(redovi);
        }

    }
}
