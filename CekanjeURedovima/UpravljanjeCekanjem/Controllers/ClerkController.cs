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
                         orderby t.vrijemeDolaska descending
                         select t;
            /*var neposluženi = from t in db.Tiket
                              where t.vrijemeDolaska == null
                              where šalter.Equals(t.tip)
                              orderby t.vrijemeIzdavanja ascending
                              select t;*/
            int tiket = 0;
            if (tiketi.Any())
            {
                if(tiketi.First().obrađeno == false)
                    tiket = tiketi.First().redniBroj;
            }
            var tipovi = from a in db.TipTiketa
                         select a.tip;
            IEnumerable<SelectListItem> tipoviEnum = tipovi.ToList().Select(x =>
                                                        new SelectListItem()
                                                        {
                                                            Text = x.ToString()
                                                        });
            ViewData["šalter"] = šalter;
            ViewData["tiket"] = tiket;
            ViewData["tipovi"] = tipoviEnum;
            return View();
        }

    }
}
