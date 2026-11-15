using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using Ymck;
using ServerCrossPlatform.Study;
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

        public void OnUserRegReq(WebSocketBehaviorEx<Session> sender, UserRegReq req)
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
            sender.SendPBO(nm2.ToByteArray());
        }
            

        /// <summary>
        /// d3-01
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="req"></param>
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


        /*Custom
      public void OnUserRegReq(WebSocketBehaviorEx_d2_04 sender,UserRegReq req)
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

          using (var ms2 = new MemoryStream())
          {
              ProtoBuf.Serializer.Serialize(ms2, nm2);
              byte[] byPB = ms2.ToArray();
              sender.SendEx(byPB);
          }
      }

      public void OnUserRegReq_d3_02(WebSocketBehaviorEx_d2_04 sender, UserRegReq req)
      {
          NetMsg nm2 = new NetMsg();
          nm2.Resp = new RespMsg();
          nm2.Resp.UserReg = new UserRegResp();


          if (UserMgr_d3_02.Instance.GetUser(req.Name) != null)
          {
              nm2.Resp.UserReg.Ok = false;
              nm2.Resp.UserReg.ErrMsg = "User exist";
          }
          else {
              User user= new User()
              {
                  Id = Guid.NewGuid().ToString(),
                  Name = req.Name,
                  Pwd = req.Pwd,
              };
              UserMgr_d3_02.Instance.AddUser(user);

              nm2.Resp.UserReg.Ok = true;
              nm2.Resp.UserReg.User = user;
          }

          using (var ms2 = new MemoryStream())
          {
              ProtoBuf.Serializer.Serialize(ms2, nm2);
              byte[] byPB = ms2.ToArray();
              sender.SendEx(byPB);
          }
      }
      */

    }
}
