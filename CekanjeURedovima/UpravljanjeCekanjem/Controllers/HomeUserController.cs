using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UpravljanjeCekanjem.Models;
using System.Security.Claims;

namespace UpravljanjeCekanjem.Controllers
{
    public class HomeUserController : Controller
    {
        //
        // GET: /HomeUser/
        public ActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            ViewBag.UserName = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            ViewBag.Role = claimsIdentity.FindFirst(ClaimTypes.Role).Value;
            
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manager");
            }

            var model = new LogInModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LogInModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            using (var db = new DataBaseEntities())
            {
                Korisnik korisnik = (from u in db.Korisnik
                                     where u.userName.Equals(model.UserName) &&
                                    u.lozinka.Equals(model.Password)
                                select u).FirstOrDefault();

                if (korisnik != null)
                {

                    var identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, korisnik.userName),
                        new Claim(ClaimTypes.Role, korisnik.razinaPrava)
                            },
                        "ApplicationCookie");

                    var ctx = Request.GetOwinContext();
                    var authManager = ctx.Authentication;
                    authManager.SignIn(identity);

                    if (korisnik.razinaPrava.Equals("nadzornik"))
                    {
                        return RedirectToAction("Index", "Manager");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Clerk");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Provjerite svoje podatke i pokušajte ponovno.");
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("index", "HomeUser");
        }

    }
}
