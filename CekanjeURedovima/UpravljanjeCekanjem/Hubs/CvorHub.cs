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
        public void Pokreni_refresh(String vel, String boj)   //dodati potrebne parametre, spremiti u bazu, dodati dohvat iz baze pri reloadanju
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

        public void Osvjezi_tipove()
        {
            System.Diagnostics.Debug.WriteLine("osvjezi tipove");
            
            using (var db = new DataBaseEntities())
            {
                var tipovi = from c in db.TipTiketa
                                           where c.ponudjena == true
                                           select c.tip;
                if (tipovi.Any())
                {
                    Clients.All.dohvati_tipove(tipovi.ToList());
                }
            }
        }

        public void Osvjezi_postavku(int id, String value)
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

        public void Promijeni_salter(string šalter, string username)
        {
            using (var db = new DataBaseEntities())
            {
                Korisnik korisnik;
                korisnik = db.Korisnik.FirstOrDefault( c => c.userName == username);
                korisnik.šalter = šalter;
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
    }
}