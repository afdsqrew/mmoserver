using Google.Protobuf;
using WebSocketSharp;
using Ymck;

namespace ServerCrossPlatform.Study
{
    /// <summary>
    /// d2-04
    /// </summary>
    class MessaingService_002 : WebSocketBehaviorEx_001
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            OnMessage002(e);
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

        protected void OnMessage002(MessageEventArgs e)
        {
            NetMsg nm = NetMsg.Parser.ParseFrom(e.RawData);
            //这里不知道源码
            MsgProcessor_001.Process(this, nm);
            //MsgProcessor<Session, WebSocketBehaviorEx<Session>>.Instance.Process(this, nm); // 接收消息
        }
    }
}
