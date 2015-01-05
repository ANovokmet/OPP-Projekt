using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

public static class Global
{
    public static Semaphore semafor = new Semaphore(1, 1);
    public static Dictionary<String, int> rjecnik = new Dictionary<String, int>();
}
