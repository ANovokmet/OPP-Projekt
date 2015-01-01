namespace UpravljanjeCekanjem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TipTiketa")]
    public partial class TipTiketa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TipTiketa()
        {
            Tiket = new HashSet<Tiket>();
        }

        [Key]
        [StringLength(50)]
        public string tip { get; set; }

        [StringLength(50)]
        public string opis { get; set; }

        public bool ponudjena { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tiket> Tiket { get; set; }
    }
}
