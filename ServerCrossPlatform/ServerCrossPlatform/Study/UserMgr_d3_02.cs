using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ymck;

namespace ServerCrossPlatform.Study
{
    internal class UserMgr_d3_02:Singleton<UserMgr_d3_02>
    {
        List<User> user;
        Dictionary<string, User> dicNameUser = new Dictionary<string, User>();

        public User GetUser(string name)
        { 
            throw new NotImplementedException();
        }

        public void AddUser(User user)
        {
        }
    }
}
