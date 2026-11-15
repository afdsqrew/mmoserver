using Microsoft.EntityFrameworkCore;
using ServerCrossPlatform.Test;
using Ymck;

namespace Model
{
    /// <summary>
    /// d2
    /// </summary>
    class EntityService_001:Singleton<EntityService_001>
    {
        public YmckMmoModel Model = null;
        public void Init()
        {
            
            Model = new YmckMmoModel();
            Model.Database.EnsureCreated();
        }

        public void OnDestroy()
        {
            Model.SaveChanges();
        }
    }
}
