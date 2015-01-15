using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using UpravljanjeCekanjem.Models;

namespace UpravljanjeCekanjem.Controllers
{
    [CustomAuthorize(Roles = "službenik")]
    public class ClerkController : Controller
    {
        //
        // GET: /Clerk/

        DataBaseEntities db = new DataBaseEntities();

        public ActionResult Index()
        {
            string user = User.Identity.Name;
            var šalteri = from c in db.Korisnik
                          where c.userName.Equals(user)
                          select c.šalter;
            string šalter = šalteri.First();
            var tiketi = from t in db.Tiket
                         where t.vrijemeDolaska != null
                         where šalter.Equals(t.tip)
                         orderby t.vrijemeIzdavanja descending
                         select t;
            int tiket = 0;
            if (tiketi.Any())
            {
                if (tiketi.First().obrađeno == false)
                {
                    tiket = tiketi.First().redniBroj;
                }
            }
            var tipovi = from a in db.TipTiketa
                         select a.tip;
            IEnumerable<SelectListItem> tipoviEnum = tipovi.ToList().Select(x =>
                                                        new SelectListItem()
                                                        {
                                                            Text = x.ToString()
                                                        });
            ViewData["šalter"] = šalter;
            ViewData["tiket"] = tiket.ToString();
            ViewData["tipovi"] = tipoviEnum;

            

            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var tip in tipovi)
            {
                items.Add(new SelectListItem { Text = tip, Value = tip });
            }
            ViewBag.TipoviSaltera = items;



            return View();
        }

        public ActionResult NextClient(string salter)
        {
            System.Diagnostics.Debug.WriteLine("NextClient" + salter);
            using (var db = new DataBaseEntities())
            {
                Tiket tiket;
                var tiketi = from t in db.Tiket
                             where t.vrijemeDolaska == null
                             where salter.Equals(t.tip)
                             orderby t.vrijemeIzdavanja ascending
                             select t;
                var neobrađeni = from t in db.Tiket
                                 where t.vrijemeDolaska != null
                                 where salter.Equals(t.tip)
                                 where t.obrađeno == false
                                 orderby t.vrijemeDolaska descending
                                 select t;
                if (neobrađeni.Any())
                {
                    neobrađeni.First().obrađeno = true;
                }
                if (tiketi.Any())
                {
                    tiket = tiketi.First();
                    tiket.vrijemeDolaska = DateTime.Now;
                }
                //Osvjezi_screen_vrijeme(šalter);
                db.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        public ActionResult PromijeniSalter(string TipoviSaltera)
        {
            string username = User.Identity.Name;
            System.Diagnostics.Debug.WriteLine("PromjeniSalter" + TipoviSaltera + username);
            using (var db = new DataBaseEntities())
            {
                Korisnik korisnik;
                korisnik = db.Korisnik.FirstOrDefault(c => c.userName == username);
                korisnik.šalter = TipoviSaltera;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}
