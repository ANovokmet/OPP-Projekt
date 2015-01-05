using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using UpravljanjeCekanjem.Models;

namespace UpravljanjeCekanjem.Controllers
{
    [CustomAuthorize(Roles = "nadzornik")]
    public class ManagerController : Controller
    {/*mozda samo jedan db = new entitie*/

        private DataBaseEntities db = new DataBaseEntities();
        // 
        // GET: /Manager/
        
        public ActionResult Index()
        {
            List<TipTiketa> tipovi  = db.TipTiketa.ToList();

            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var tip in tipovi)
            {
                items.Add(new SelectListItem { Text = tip.tip, Value = tip.tip });
            }
            ViewBag.IzvjestajTip = items;

            List<SelectListItem> timespan = new List<SelectListItem>();
            timespan.Add(new SelectListItem { Text = "Dnevno", Value = "dnevno" });
            timespan.Add(new SelectListItem { Text = "Tjedno", Value = "tjedno" });
            timespan.Add(new SelectListItem { Text = "Mjesečno", Value = "mjesečno" });
            ViewBag.IzvjestajRaspon = timespan;
            




            return View(tipovi);
        }

        [HttpGet]
        public ActionResult Edit(string tip)
        {
            TipTiketa tipTiketa;
            tipTiketa = db.TipTiketa.Find(tip);
            return View(tipTiketa);
        }
        [HttpPost]
        public ActionResult Edit(TipTiketa tipTiketa)
        {
            db.Entry(tipTiketa).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(string tip)
        {
            TipTiketa tipTiketa = db.TipTiketa.Find(tip);
            try
            {
                db.TipTiketa.Remove(tipTiketa);
                db.SaveChanges();
            }
            catch
            {
                ModelState.AddModelError("", "Provjeriti postoje li tiketi vezani stranim ključem!");
                return View();
            }
            return RedirectToAction("Index", "Manager");
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(TipTiketa tipTiketa)
        {
            try
            {
                db.TipTiketa.Add(tipTiketa);
                db.SaveChanges();
            }
            catch
            {
                ModelState.AddModelError("", "Nevalja!");
                return View();
            }

            return RedirectToAction("Index", "Manager");
        }

        public ActionResult Report(string IzvjestajTip, string IzvjestajRaspon)
        {   
            ViewBag.tip = IzvjestajTip;
            ViewBag.raspon = IzvjestajRaspon;

            var posluzeni =//ne uzimam u obzir neponištene tikete, možda bih trebao, ali svi trebaju bit poništeni do kraja dana
                from x in db.Tiket
                where x.tip.Equals(IzvjestajTip) && x.vrijemeDolaska != null
                select x;

            switch (IzvjestajRaspon)
            {
                case "dnevno":
                    {
                        var izvjestaj = posluzeni.AsEnumerable().GroupBy(kvp => new DateTime(kvp.vrijemeIzdavanja.Year, kvp.vrijemeIzdavanja.Month, kvp.vrijemeIzdavanja.Day), kvp => (kvp.vrijemeDolaska.Value - kvp.vrijemeIzdavanja).TotalSeconds)
                            .Select(g => new Izvjestaj { key = g.Key, avg = g.Average(), max = g.Max(), count = g.Count() });
                        return View(izvjestaj);
                    }
                case "tjedno":
                    {
                        var izvjestaj = posluzeni.AsEnumerable().GroupBy(kvp => StartOfWeek(kvp.vrijemeIzdavanja), kvp => (kvp.vrijemeDolaska.Value - kvp.vrijemeIzdavanja).TotalSeconds)
                            .Select(g => new Izvjestaj { key = g.Key, avg = g.Average(), max = g.Max(), count = g.Count() });
                        return View(izvjestaj);
                    }
                case "mjesečno":
                    {
                        var izvjestaj = posluzeni.AsEnumerable().GroupBy(kvp => new DateTime(kvp.vrijemeIzdavanja.Year, kvp.vrijemeIzdavanja.Month, 1), kvp => (kvp.vrijemeDolaska.Value - kvp.vrijemeIzdavanja).TotalSeconds)
                            .Select(g => new Izvjestaj { key = g.Key, avg = g.Average(), max = g.Max(), count = g.Count() });
                        return View(izvjestaj);
                    }
                default:
                    return RedirectToAction("Index","Manager");
            }
        }

        public static DateTime StartOfWeek(DateTime dt)
        {
            int diff = dt.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }
    }


}
