using Ymck;
using Google.Protobuf;

namespace ServerCrossPlatform.Study
{
    /// <summary>
    /// d2-04
    /// </summary>
    class UserService_001: Singleton<UserService_001>
    {
        public void Init()
        {
           MsgProcessor_002.RegHandler<UserRegReq>(OnUserRegReq_001);           //d3-01

        }

        /// <summary>
        /// d3-02
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="req"></param>
        public void OnUserRegReq_002(WebSocketBehaviorEx_001 sender, UserRegReq req)
        {
            NetMsg nm2 = new NetMsg();
            nm2.Resp = new RespMsg();
            nm2.Resp.UserReg = new UserRegResp();

            if (UserMgr_001.Instance.GetUser(req.Name) != null)
            {
                nm2.Resp.UserReg.Ok = false;
                nm2.Resp.UserReg.ErrMsg = "user exisit";
            }
            else 
            {
                var newUser = new User()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = req.Name,
                    Pwd = req.Pwd,
                };

                UserMgr_001.Instance.AddUser(newUser);
                nm2.Resp.UserReg.Ok = true;
                nm2.Resp.UserReg.User = newUser;
            }

            sender.SendEx(nm2.ToByteArray());
        }

        public void OnUserRegReq_001(WebSocketBehaviorEx_001 sender, UserRegReq req)
        {
            NetMsg nm2 = new NetMsg();
            nm2.Resp = new RespMsg();
            nm2.Resp.UserReg = new UserRegResp();
            nm2.Resp.UserReg.Ok = true;
            nm2.Resp.UserReg.User = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Name = req.Name,
                Pwd = req.Pwd,
            };
            sender.SendEx(nm2.ToByteArray());
        }
    }
}
