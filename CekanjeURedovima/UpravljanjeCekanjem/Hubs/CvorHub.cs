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
        public void Pokreni_refresh(String poruka)   //dodati potrebne parametre, spremiti u bazu, dodati dohvat iz baze pri reloadanju
        {
            System.Diagnostics.Debug.WriteLine("" + poruka);
            Clients.All.pokreni(poruka+"px");
            Osvjezi_postavku(0, Convert.ToInt32(poruka));
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

        public void Osvjezi_postavku(int id, int value)
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
    }
}