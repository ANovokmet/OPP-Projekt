using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using UpravljanjeCekanjem.Models;

namespace UpravljanjeCekanjem.Controllers
{
    public class ManagerController : Controller
    {/*mozda samo jedan db = new entitie*/

        private DataBaseEntities db = new DataBaseEntities();
        // 
        // GET: /Manager/
        public ActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            ViewBag.Role = claimsIdentity.FindFirst(ClaimTypes.Role).Value;

            if (ViewBag.Role.Equals("službenik "))
            {
                return RedirectToAction("Index", "Clerk");
            }

            List<TipTiketa> tipovi;
            using (var db = new DataBaseEntities())
            {
                tipovi = db.TipTiketa.ToList();
            }

            return View(tipovi);
        }

        [HttpGet]
        public ActionResult Edit(string tip)
        {
            TipTiketa tipTiketa;
            using (var db = new DataBaseEntities())
            {
                tipTiketa = db.TipTiketa.Find(tip);
            }
            return View(tipTiketa);
        }
        [HttpPost]
        public ActionResult Edit(TipTiketa tipTiketa)
        {
            using (var db = new DataBaseEntities())
            {
                db.Entry(tipTiketa).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Delete(string tip)
        {
            TipTiketa tipTiketa = db.TipTiketa.Find(tip);
            db.TipTiketa.Remove(tipTiketa);
            db.SaveChanges();
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
    }
}
