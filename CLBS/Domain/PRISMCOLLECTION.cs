namespace CLBS
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using CLBS.Domain;

    public partial class PRISMCOLLECTION : DbContext
    {
        
        public PRISMCOLLECTION(): 
            base("name=PRISMCOLLECTION")
        {
        }

        public virtual DbSet<CLBSData> CLBSDatas { get; set; }
        public virtual DbSet<STANDARDData> STANDARDDatas { get; set; }
        public virtual DbSet<PrismCol> PrismColss { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<STANDARDData>()
                .HasMany(e => e.CLBSDatas)
                .WithRequired(e => e.STANDARDData)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<User>();
                //.HasMany(e => e.CLBSDatas)
                //.WithOptional(e => e.User)
                //.HasForeignKey(e => e.CreateBy);

            modelBuilder.Entity<User>();
                //.HasMany(e => e.STANDARDDatas)
                //.WithOptional(e => e.User)
                //.HasForeignKey(e => e.CreateBy);

            modelBuilder.Entity<CLBSData>()
               .HasOptional(s => s.PrismCol)
               .WithRequired(ad => ad.CLBSData)
               .WillCascadeOnDelete(true);


        }

        public System.Data.Entity.DbSet<CLBS.Models.Users> vUsers { get; set; }

        //public System.Data.Entity.DbSet<CLBS.Domain.PrismCol> PrismCols { get; set; }
    }
}
