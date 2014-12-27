namespace UpravljanjeCekanjem
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using UpravljanjeCekanjem.Models;

    public partial class DataBaseEntities : DbContext
    {
        public DataBaseEntities()
            : base("name=DataBaseEntities")
        {
        }

        public virtual DbSet<Korisnik> Korisnik { get; set; }
        public virtual DbSet<Postavke> Postavke { get; set; }
        public virtual DbSet<Tiket> Tiket { get; set; }
        public virtual DbSet<TipTiketa> TipTiketa { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Korisnik>()
                .Property(e => e.razinaPrava)
                .IsFixedLength();

            modelBuilder.Entity<TipTiketa>()
                .HasMany(e => e.Tiket)
                .WithRequired(e => e.TipTiketa)
                .WillCascadeOnDelete(false);
        }
    }
}
