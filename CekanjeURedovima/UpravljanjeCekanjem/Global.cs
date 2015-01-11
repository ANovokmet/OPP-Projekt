using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using UpravljanjeCekanjem.Models;

public static class Global
{
    static UpravljanjeCekanjem.DataBaseEntities db = new UpravljanjeCekanjem.DataBaseEntities();
    public static Semaphore semafor = new Semaphore(1, 1);
    public static Dictionary<String, int> rjecnik = new Dictionary<String, int>();
    static System.Timers.Timer t;

    public static void refresh_dataset()
    {
        List<String> redovi = (from x in db.TipTiketa
                               where x.ponudjena == true
                               select x.tip).ToList<String>();
        Global.semafor.WaitOne();
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
            foreach (string key in Global.rjecnik.Keys.ToList())
            {
                if (!redovi.Contains(key))
                {
                    Global.rjecnik.Remove(key);
                }
            }
        }
        System.Diagnostics.Debug.WriteLine("zadnje osvj " + Global.rjecnik.Keys);
        Global.semafor.Release();
    }

    public static double get_avg_vrijeme_cekanja(string tip)
    {
        var posluzeni =
                from x in db.Tiket
                where x.tip.Equals(tip) && x.vrijemeDolaska != null 
                && x.vrijemeIzdavanja.Day == DateTime.Now.Day
                && x.vrijemeIzdavanja.Month == DateTime.Now.Month
                && x.vrijemeIzdavanja.Year == DateTime.Now.Year
                select x;

        if (posluzeni.Any())//ocekivano vrijeme se treba moci prikazati i na /screen/, bolje da se izracunava tu a drugdje dohvaća
        {
            TimeSpan duration;
            double total = 0;
            foreach (var a in posluzeni)
            {
                duration = (DateTime)a.vrijemeDolaska - a.vrijemeIzdavanja;
                total += duration.TotalSeconds;
            }
            return Math.Round((total / posluzeni.Count()), 2); 
        }
        else
        {
            return 0;
        }
    }
    public static void set_auto_reset()
    {
        int vrijeme_sada = (int) DateTime.Now.TimeOfDay.TotalSeconds;
        int vrijeme, reset_sekunde;
        List<String> postavka =
                (from x in db.Postavke
                where x.naziv.Equals("autoreset")
                 select x.vrijednost).ToList<String>();
        if (postavka.Count == 0)
        {
            Postavke a = new Postavke();
            a.naziv = "autoreset";
            a.Identifikator = 4;
            a.vrijednost = "25200";
            db.Postavke.Add(a);
            db.SaveChanges();
            vrijeme = 25200;
        }
        else
        {
            vrijeme = Convert.ToInt32(postavka.ElementAt(0));
        }
        if (vrijeme - vrijeme_sada < 0)
        {
            reset_sekunde = 24*60*60 - (vrijeme_sada - vrijeme);
        }
        else if (vrijeme - vrijeme_sada == 0)
        {
            reset_sekunde = 24 * 60 * 60;
            Global.semafor.WaitOne();
            foreach (string key in Global.rjecnik.Keys.ToList())
            {
                Global.rjecnik[key] = 1;
            }
            Global.semafor.Release();
        }
        else
        {
            reset_sekunde = vrijeme - vrijeme_sada;
        }
        Global.t = new System.Timers.Timer(reset_sekunde*1000);
        t.Elapsed += new System.Timers.ElapsedEventHandler(timer_event);
        t.Enabled = true;
        System.Diagnostics.Debug.WriteLine("vrijeme " + reset_sekunde);
    }

    static void timer_event(Object sender, System.Timers.ElapsedEventArgs e)
    {
        Global.semafor.WaitOne();
        foreach (string key in Global.rjecnik.Keys.ToList())
        {
            Global.rjecnik[key] = 1;
        }
        Global.semafor.Release();
        Global.t = new System.Timers.Timer(24*60*60*1000);
        t.Elapsed += new System.Timers.ElapsedEventHandler(timer_event);
        t.Enabled = true;
    }
}