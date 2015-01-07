using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

public static class Global
{
    static UpravljanjeCekanjem.DataBaseEntities db = new UpravljanjeCekanjem.DataBaseEntities();
    public static Semaphore semafor = new Semaphore(1, 1);
    public static Dictionary<String, int> rjecnik = new Dictionary<String, int>();

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
}
