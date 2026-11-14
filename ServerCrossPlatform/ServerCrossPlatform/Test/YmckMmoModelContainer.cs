using Microsoft.EntityFrameworkCore;

namespace ServerCrossPlatform.Test
{
    public partial class YmckMmoModelContainer : DbContext
    {
        /*
        public YmckMmoModelContainer()
            : base("name=YmckMmoModelContainer")
        {
        }
        */

        /*@@@@@
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }
        */

        public YmckMmoModelContainer(DbContextOptions options):base(options)
        {
        }
       /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseSqlServer
        }*/

        public virtual DbSet<DbUser> DbUserSet { get; set; }
        public virtual DbSet<DbPlayer> DbPlayerSet { get; set; }
        public virtual DbSet<DbCharacter> DbCharacterSet { get; set; }
    }
}
