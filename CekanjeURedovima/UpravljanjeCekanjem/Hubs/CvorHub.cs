using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace UpravljanjeCekanjem
{
    public class CvorHub : Hub
    {
        public void Pokreni_refresh(String poruka)   //dodati potrebne parametre, spremiti u bazu, dodati dohvat iz baze pri reloadanju
        {
            System.Diagnostics.Debug.WriteLine("" + poruka);
            Clients.All.pokreni(poruka+"px");
        }
    }
}