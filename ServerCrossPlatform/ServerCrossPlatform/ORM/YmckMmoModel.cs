using Microsoft.EntityFrameworkCore;
using ServerCrossPlatform.Test;
using System.ComponentModel.DataAnnotations;

namespace Model
{ 
    public class YmckMmoModel:DbContext
    {

        public YmckMmoModel()
        { 
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /*
            const string conn =
     "Server=127.0.0.1;Port=3306;Database=orm;Uid=root;Pwd=12345678;SslMode=none;CharSet=utf8mb4";
            */
            
            const string conn =
     "Server=168.110.47.81;Port=3306;Database=orm;Uid=root;Pwd=Root@123456;SslMode=none;CharSet=utf8mb4";
           
            var serverVersion = new MySqlServerVersion(new Version(5, 5, 20));
            //新
            optionsBuilder.UseMySql(conn, serverVersion);
        }



        public virtual DbSet<User> Users { get; set; }
    }

    public class User
    {
        [Key]
        public string Name { get; set; }
        public string Pwd { get; set; }
    }
}
