using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UpravljanjeCekanjem.Models;

namespace UpravljanjeCekanjem.Controllers
{
    public class HomeUserController : Controller
    {
        //
        // GET: /HomeUser/
        public ActionResult Index()
        {
            if (Request.Cookies["currentuser"] == null)
            {
                return View((object)"");
            }
            else
            {
                return View((object)Request.Cookies["currentuser"]["username"]);
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Request.Cookies["currentuser"] == null)
            {
                return View();
            }
            else if (Request.Cookies["currentuser"]["razinaprava"].Equals("nadzornik"))
            {
                return RedirectToAction("Index", "Manager");
            }
            else
            {
                return RedirectToAction("Index", "Clerk");
            }
        }

        [HttpPost]
        public ActionResult Login(Korisnik user)
        {
            using (var db = new DataBaseEntities())
            {
                Korisnik korisnik = (from u in db.Korisnik
                                        where u.userName.Equals(user.userName) &&
                                    u.lozinka.Equals(user.lozinka)
                                select u).FirstOrDefault();

                if (korisnik != null)
                {

                    string CookieName = "currentuser";
                    HttpCookie myCookie = Request.Cookies[CookieName] ?? new HttpCookie(CookieName);
                    myCookie.Values["username"] = korisnik.userName;
                    myCookie.Values["razinaprava"] = korisnik.razinaPrava;
                    myCookie.Expires = DateTime.Now.AddDays(1);//new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23,59,59);//možda do ponoći DateTime.Now
                    Response.Cookies.Add(myCookie);

                    if (korisnik.razinaPrava.Equals("nadzornik "))
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
                    ModelState.AddModelError("", "Login data is incorrect!");
                }
            }
            return View(user);
        }

        public ActionResult Logout()
        {
            if (Request.Cookies["currentuser"] != null)
            {
                HttpCookie myCookie = new HttpCookie("currentuser");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);
            }
            return RedirectToAction("Login", "HomeUser");
        }

    }
}
