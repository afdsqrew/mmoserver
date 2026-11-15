using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using Ymck;
using ServerCrossPlatform.Study;
using Google.Protobuf;
using WebSocketSharp;

namespace ServerCrossPlatform.Study
{
    /// <summary>
    /// 协议序列化d203  接收消息不处理消息
    /// </summary>
    class MessaingService_001: WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {

        }


        protected void OnMessage001(MessageEventArgs e)
        {
            NetMsg nm = NetMsg.Parser.ParseFrom(e.RawData);

            NetMsg nm2 = new NetMsg();
            nm2.Resp = new RespMsg();
            nm2.Resp.UserReg = new UserRegResp();
            nm2.Resp.UserReg.Ok = true;
            nm2.Resp.UserReg.User = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Name = nm.Req.UserReg.Name,
                Pwd = nm.Req.UserReg.Pwd,
            };

            this.Send(nm2.ToByteArray());
        }
    }
}
