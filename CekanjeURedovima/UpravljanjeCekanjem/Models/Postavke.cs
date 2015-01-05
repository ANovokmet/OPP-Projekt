namespace UpravljanjeCekanjem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Postavke")]
    public partial class Postavke
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Identifikator { get; set; }

        [Required]
        [StringLength(50)]
        public string naziv { get; set; }

        public String vrijednost { get; set; }
    }
}
