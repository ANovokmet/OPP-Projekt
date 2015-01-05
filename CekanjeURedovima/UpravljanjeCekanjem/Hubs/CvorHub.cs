using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using UpravljanjeCekanjem.Models;
namespace UpravljanjeCekanjem
{
    public class CvorHub : Hub
    {
        public void Pokreni_refresh(String vel, String boj)  
        {
            //System.Diagnostics.Debug.WriteLine("" + poruka);
            Clients.All.pokreni(vel + "px", "#" + boj);
            Osvjezi_postavku(0, vel);
            Osvjezi_postavku(1, boj);
        }

        public void Pokreni_flash()
        {
            Clients.All.flash();
        }

        public void Osvjezi_screen_tip()
        {
            using (var db = new DataBaseEntities())
            {
                var tipovi = from c in db.TipTiketa
                             where c.ponudjena == true
                             select c;

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
                        System.Diagnostics.Debug.WriteLine(a.tip+prviBroj.First());
                        Clients.All.updatered(a.tip, prviBroj.First().ToString());//trenutni broj
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(a.tip+0);
                        Clients.All.updatered(a.tip, "0"); 
                    }
                }
            }
            
        }

        public void Osvjezi_tipove()
        {
            System.Diagnostics.Debug.WriteLine("osvjezi tipove");
            
            using (var db = new DataBaseEntities())
            {
                var tipovi = from c in db.TipTiketa
                                           where c.ponudjena == true
                                           select c.tip;
                var opisi = from c in db.TipTiketa
                             where c.ponudjena == true
                             select c.opis;

                if (tipovi.Any())
                {
                    Clients.All.dohvati_tipove(tipovi, opisi);
                }
            }
        }

        public void Osvjezi_postavku(int id, string value)
        {
            using (var db = new DataBaseEntities())
            {
                Postavke postavka;
                postavka = db.Postavke.Where(s => s.Identifikator == id).FirstOrDefault<Postavke>();// ili db.Postavke.FirstOrDefault(c => c.Identifikator == id); 
                if (postavka != null)
                {
                    postavka.vrijednost = value;
                    db.Entry(postavka).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    postavka = new Postavke();
                    postavka.Identifikator = id;
                    postavka.naziv = "font";
                    postavka.vrijednost = value;
                    db.Postavke.Add(postavka);
                }
                db.SaveChanges();
            }
        }

        public void reset_brojaca(String tip)
        {
            //System.Diagnostics.Debug.WriteLine(""+tip);
            Global.semafor.WaitOne();
            Global.rjecnik[tip] = 1;
            Global.semafor.Release();
        }

        public void Promijeni_salter(string šalter, string username)
        {
            using (var db = new DataBaseEntities())
            {
                Korisnik korisnik;
                korisnik = db.Korisnik.FirstOrDefault(c => c.userName == username);
                korisnik.šalter = šalter;
                db.SaveChanges();
            }
        }

        public void Next_client(string šalter)
        {
            using (var db = new DataBaseEntities())
            {
                Tiket tiket;
                var tiketi = from t in db.Tiket
                        where t.vrijemeDolaska == null
                        where šalter.Equals(t.tip)
                        orderby t.vrijemeIzdavanja ascending
                        select t;
                var neobrađeni = from t in db.Tiket
                                 where t.vrijemeDolaska != null
                                 where šalter.Equals(t.tip)
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
                db.SaveChanges();
            }
        }
    }
}