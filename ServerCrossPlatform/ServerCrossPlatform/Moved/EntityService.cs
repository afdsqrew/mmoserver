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
            //Model = new YmckMmoModelContainer();
            //Model.Database.CreateIfNotExists();


            const string conn =
                "Server=127.0.0.1;Port=3306;Database=ymckmmo;Uid=root;Pwd=12345678;SslMode=none;CharSet=utf8mb4";
            //新
            DbContextOptions  options = new DbContextOptionsBuilder<YmckMmoModelContainer>().UseSqlServer(conn).Options;
            Model = new YmckMmoModelContainer(options);
        }

        public void Destroy()
        {
            Model.SaveChanges();
        }
    }
}
