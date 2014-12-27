namespace UpravljanjeCekanjem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tiket")]
    public partial class Tiket
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int redniBroj { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string tip { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime vrijemeIzdavanja { get; set; }

        public DateTime? vrijemeDolaska { get; set; }

        public virtual TipTiketa TipTiketa { get; set; }
    }
}
