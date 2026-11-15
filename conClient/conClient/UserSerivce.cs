using conClient;
using Google.Protobuf;
using LitJson;
using System;
using WebSocketSharp;

namespace Ymck
{
    public class UserService_d3_01 : Singleton<UserService_d3_01>
    {
        public void Init()
        {
            MsgProcessor_001.RegHandler<UserRegResp>(OnUserRegReq);
        }

        public void OnUserRegReq(WebSocketBehaviorEx_001 sender, UserRegResp resp)
        {
            Console.WriteLine($"RESP:{JsonMapper.ToJson(resp)}");
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
