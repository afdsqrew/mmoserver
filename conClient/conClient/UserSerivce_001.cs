using conClient;
using Google.Protobuf;
using LitJson;
using System;
using WebSocketSharp;

namespace Ymck
{
    /// <summary>
    /// d3_01
    /// </summary>
    public class UserService_001 : Singleton<UserService_001>
    {
        public void Init()
        {
            MsgProcessor_002.RegHandler<UserRegResp>(OnUserRegReq);
        }

        public void OnUserRegReq(WebSocketBehaviorEx_001 sender, UserRegResp resp)
        {
            Console.Write("d3 01 OnBack:" + resp.User.Name);

        }

        public void DoUserReg(string name,string pwd)
        {
            var nm = new NetMsg();
            nm.Req = new ReqMsg();
            nm.Req.UserReg = new UserRegReq();
            nm.Req.UserReg.Name = name;
            nm.Req.UserReg.Pwd = pwd;

            Program.clientSocket.Send(nm.ToByteArray());  
        }
    }
}
