namespace UpravljanjeCekanjem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Korisnik")]
    public partial class Korisnik
    {
        [Key]
        [StringLength(50)]
        public string userName { get; set; }

        [StringLength(50)]
        public string ime { get; set; }

        [StringLength(50)]
        public string prezime { get; set; }

        [Required]
        [StringLength(50)]
        public string lozinka { get; set; }

        [Required]
        [StringLength(10)]
        public string razinaPrava { get; set; }
    }
}
