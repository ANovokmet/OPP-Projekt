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

        public void Pokreni_flash(string tip)
        {
            System.Diagnostics.Debug.WriteLine("flash:" + tip);
            Clients.All.flash(tip);
        }

        public void prikazi_vrijeme(bool show)
        {
            Osvjezi_postavku(2, show == true ? "1" : "0");
            Clients.All.showvrijeme(show);
        }

        public void prikazi_izdane(bool show)
        {
            Osvjezi_postavku(3, show == true ? "1" : "0");
            Clients.All.showizdani(show);
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
                    var tiketi = from t in db.Tiket
                                where t.vrijemeDolaska != null
                                && t.vrijemeIzdavanja.Day == DateTime.Now.Day
                                && t.vrijemeIzdavanja.Month == DateTime.Now.Month
                                && t.vrijemeIzdavanja.Year == DateTime.Now.Year
                                && a.tip.Equals(t.tip)
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


                    System.Diagnostics.Debug.WriteLine(a.tip + tiket.ToString());
                    Clients.All.updatered(a.tip, tiket.ToString());//trenutni broj
                    
                    
                }
            }
            
        }

        public void Osvjezi_screen_vrijeme(string red)
        {
            System.Diagnostics.Debug.WriteLine("update vrijeme za "+red);
            double value = Global.get_avg_vrijeme_cekanja(red);
            Clients.All.updatevrijeme(red, value.ToString());
        }

        public void osvjezi_izdane(string red)
        {
            using (var db = new DataBaseEntities())
            {
                var count = (from t in db.Tiket
                             where t.vrijemeIzdavanja.Day == DateTime.Now.Day
                             && t.vrijemeIzdavanja.Month == DateTime.Now.Month
                             && t.vrijemeIzdavanja.Year == DateTime.Now.Year
                             && red.Equals(t.tip)
                             orderby t.vrijemeIzdavanja descending
                             select t).Count();
                Clients.All.refreshizdani(red, count.ToString());
            }
        }

        public void Osvjezi_tipove()
        {
            
            
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
                    System.Diagnostics.Debug.WriteLine("osvjezi tipove");
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
            System.Diagnostics.Debug.WriteLine("cvor"+tip);
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
                Osvjezi_screen_vrijeme(šalter);
                db.SaveChanges();
            }
        }
    }
}