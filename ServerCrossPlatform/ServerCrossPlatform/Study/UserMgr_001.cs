using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ymck;

namespace ServerCrossPlatform.Study
{
    /// <summary>
    /// d3-02
    /// </summary>
    internal class UserMgr_001:Singleton<UserMgr_001>
    {
        List<User> user;
        Dictionary<string, User> dicNameUser = new Dictionary<string, User>();

        public User GetUser(string name)
        {
            return null;
        }

        public void AddUser(User user)
        {
        }
    }
}
