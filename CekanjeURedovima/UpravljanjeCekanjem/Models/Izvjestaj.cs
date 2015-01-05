namespace UpravljanjeCekanjem.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;

    public class Izvjestaj
    {
        public DateTime key { get; set; }

        public double avg { get; set; }

        public double max { get; set; }

        public int count { get; set; }

    }
}
