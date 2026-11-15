using System;
using System.Collections.Generic;
using System.Text;
using Ymck;

namespace ymck
{
    /// <summary>
    /// d3-02
    /// </summary>
    class UserMgr : Singleton<UserMgr>
    {
        List<User> users = new List<User>();
        Dictionary<string,User> dicNameUser = new Dictionary<string,User>();

        public User GetUser(string name)
        {
            return null;
        }
        public void AddUser(User user)
        { 
            
        }
    }
}
