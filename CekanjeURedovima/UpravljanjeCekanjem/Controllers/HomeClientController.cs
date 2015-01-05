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

            List<String> redovi = (from x in db.TipTiketa
                        select x.tip).ToList<String>();
            int broj_redova = redovi.Count;
            int broj_u_rj = Global.rjecnik.Count;
            if (broj_u_rj < broj_redova)
            {
                foreach (String a in redovi)
                {
                    if (!Global.rjecnik.ContainsKey(a))
                    {
                        Global.rjecnik.Add(a, 1);
                    }
                }
            }
            else if (broj_u_rj > broj_redova)
            {
                foreach (string key in Global.rjecnik.Keys)
                {
                    if (!redovi.Contains(key))
                    {
                        Global.rjecnik.Remove(key);
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("zadnje osvj "+Global.rjecnik.Keys);
            return View(tipovi);
        }

        public ActionResult IzdajTiket(string tip)//trebat će nekak primat informaciju od drugih controllera kad se treba resetat redni broj
        {
            Tiket tiket = new Tiket();

            List<String> redovi = (from x in db.TipTiketa
                                   select x.tip).ToList<String>();

            //opasan kod
            int broj_tiket;
            Global.semafor.WaitOne();
            Global.rjecnik.TryGetValue(tip, out broj_tiket);
            Global.rjecnik[tip] += 1;
            Global.semafor.Release();
            //kraj
            System.Diagnostics.Debug.WriteLine(broj_tiket);

            /*var zadnjiBroj = 
                from x in db.Tiket
                where x.tip.Equals(tip)
                orderby x.vrijemeIzdavanja descending
                select x.redniBroj;

            if (zadnjiBroj.Any())//zasad sve prek baze
            {
                if (zadnjiBroj.First() != 999) 
                { 
                    tiket.redniBroj = zadnjiBroj.First() + 1; 
                }
                else
                {
                    tiket.redniBroj = 1;
                }
            }
            else
            {
                tiket.redniBroj = 1;
            }*/
            tiket.redniBroj = broj_tiket;
            tiket.tip = tip;
            tiket.vrijemeIzdavanja = DateTime.Now;

            var posluzeni =
                from x in db.Tiket
                where x.tip.Equals(tip) && x.vrijemeDolaska != null //možda u bazi dodat CHECK dolazak>izdavanje
                && x.vrijemeIzdavanja.Day == DateTime.Now.Day
                && x.vrijemeIzdavanja.Month == DateTime.Now.Month
                && x.vrijemeIzdavanja.Year == DateTime.Now.Year
                select x;

            if (posluzeni.Any())//ocekivano vrijeme se treba moci prikazati i na /screen/, bolje da se izracunava negdje drugdje a tu dohvaca
            {
                TimeSpan duration;
                double total = 0;
                foreach (var a in posluzeni)
                {
                    duration = (DateTime)a.vrijemeDolaska - a.vrijemeIzdavanja;
                    total += duration.TotalSeconds;
                }
                ViewBag.ocekivano = DateTime.Now.AddSeconds(total / posluzeni.Count());
            }
            else
            {
                ViewBag.ocekivano = DateTime.Now;
            }

            var neposluzeni =
                from x in db.Tiket
                where x.tip.Equals(tip) && x.vrijemeDolaska == null //možda u bazi dodat CHECK dolazak>izdavanje
                && x.vrijemeIzdavanja.Day == DateTime.Now.Day
                && x.vrijemeIzdavanja.Month == DateTime.Now.Month
                && x.vrijemeIzdavanja.Year == DateTime.Now.Year
                && x.vrijemeIzdavanja < DateTime.Now
                select x;

            if (neposluzeni.Any())//trenutni broj u redu je samo tu potrebno prikazivati
            {
                ViewBag.brojuredu = neposluzeni.Count() + 1;
            }
            else
            {
                ViewBag.brojuredu = 1;
            }

            try
            {
                db.Tiket.Add(tiket);
                db.SaveChanges();
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Error!");
            }

            return View(tiket);
        }
    }
}