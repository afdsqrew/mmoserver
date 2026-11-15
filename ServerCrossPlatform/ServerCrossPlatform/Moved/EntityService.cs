using Microsoft.EntityFrameworkCore;
using ServerCrossPlatform.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ymck;


namespace ServerCrossPlatform.Test
{
    class EntityService : Singleton<EntityService>
    {
        public YmckMmoModelContainer Model = null;

        public void Init()
        {
            const string conn =
                "Server=127.0.0.1;Port=3306;Database=ymckmmo;Uid=root;Pwd=12345678;SslMode=none;CharSet=utf8mb4";
            var serverVersion = new MySqlServerVersion(new Version(5, 5, 20));
            //新
            DbContextOptions  options = new DbContextOptionsBuilder<YmckMmoModelContainer>().UseMySql(conn, serverVersion).Options;
            Model = new YmckMmoModelContainer(options);
            Model.Database.EnsureCreated();
        }

        public void Destroy()
        {
            Model.SaveChanges();
        }
    }
}
