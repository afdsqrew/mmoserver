using Google.Protobuf;
using LitJson;
using Model;
using ServerCrossPlatform.Study;
using ServerCrossPlatform.Test;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Ymck
{
    public class UserService_002 : WebSocketBehavior
    {
        /// <summary>
        /// d1-03
        /// </summary>
        /// <param name="e"></param>
        protected void OnMessage_002(MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
            this.Send($"from server {e.Data}");
        }

        /// <summary>
        /// d1-04
        /// </summary>
        /// <param name="e"></param>
        protected void OnMessage_003(MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
            NetMsg nm = JsonMapper.ToObject<NetMsg>(e.Data);

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

            string s = JsonMapper.ToJson(nm2);
            this.Send(s);
        }

        /*
        protected void OnMessage_d2_04(MessageEventArgs e)
        {
            //Console.WriteLine(e.Data);
            //NetMsg nm = JsonMapper.ToObject<NetMsg>(e.Data);
            using (var ms = new MemoryStream(e.RawData))
            {
                NetMsg nm = ProtoBuf.Serializer.Deserialize<NetMsg>(ms);
                MsgProcessor_02_04.Process(this, nm);
               
            }
        }
        

        protected void OnMessage_d2_02(MessageEventArgs e)
        {
            Console.WriteLine("---OnMessage_d2_02---");
            Console.WriteLine(e.Data);

            //NetMsg nm = JsonMapper.ToObject<NetMsg>(e.Data);
            using (var ms = new MemoryStream(e.RawData))
            {
                NetMsg nm = ProtoBuf.Serializer.Deserialize<NetMsg>(ms);
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

                using (var ms2 = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(ms2,nm);
                    byte[] byPB = ms2.ToArray();
                    this.Send(byPB);
                }
            }
        }


   


        */

        protected override void OnMessage(MessageEventArgs e)
        {
            OnMessage_Custom(e);
        }

        protected void OnMessage_Custom(MessageEventArgs e)
        {
            Console.WriteLine("---OnMessage_d2_02---");
            Console.WriteLine(e.Data);

            //NetMsg nm = JsonMapper.ToObject<NetMsg>(e.Data);
            using (var ms = new MemoryStream(e.RawData))
            {
                NetMsg nm = NetMsg.Parser.ParseFrom(ms);
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
    class Program
    {
        /// <summary>
        /// d1-02-网络通信原理与面试问题
        /// </summary>
        static void Main_001()
        {
            WebSocketServer svr = new WebSocketServer("ws://localhost:8888");
            svr.Start();
            Console.WriteLine("server start");
            Console.ReadLine();
            svr.Stop();
            Console.WriteLine("svr.Stop");
        }

        /// <summary>
        /// d1-03 高性能网络框架
        /// </summary>
        static void Main_002()
        {
            WebSocketServer svr = new WebSocketServer("ws://127.0.0.1:8888");
            svr.AddWebSocketService<UserService_002>("/UserService");
            svr.Start();
            Console.WriteLine("111");
            Console.ReadLine();
            svr.Stop();
            Console.WriteLine("svr.Stop");
        }

        /// <summary>
        /// d2-04
        /// </summary>
        static void Main_003()
        {
            UserService_001.Instance.Init();
            WebSocketServer svr = new WebSocketServer("ws://localhost:8888");
            svr.AddWebSocketService<MessaingService_003>("/MessagingService");
            svr.Start();
            Console.WriteLine("111");
            Console.ReadLine();
            svr.Stop();
            Console.WriteLine("svr.Stop");
        }

        /// <summary>
        /// d2-04
        /// </summary>
        static void Main_004()
        {
            UserService_001.Instance.Init();
            WebSocketServer svr = new WebSocketServer("ws://localhost:8888");
            svr.AddWebSocketService<MessaingService_003>("/MessagingService");
            svr.Start();
            Console.WriteLine("111");
            Console.ReadLine();
            svr.Stop();
            Console.WriteLine("svr.Stop");
        }

        /// <summary>
        /// d1-02
        /// </summary>
        static void Main_005()
        {
            Logger.Init();
            Logger.Error("66666666");

            UserService_001.Instance.Init();
            WebSocketServer svr = new WebSocketServer("ws://localhost:8888");
            svr.AddWebSocketService<MessaingService_003>("/MessagingService");
            svr.Start();
            Console.WriteLine("111");
            Console.ReadLine();
            svr.Stop();
            Console.WriteLine("svr.Stop");
        }

        /// <summary>
        /// d2
        /// </summary>
        static void Main_006()
        {
            Logger.Init();
            Logger.Error("66666666");
            EntityService_001.Instance.Init();

            UserService_001.Instance.Init();
            WebSocketServer svr = new WebSocketServer("ws://localhost:8888");
            svr.AddWebSocketService<MessaingService_003>("/MessagingService");
            svr.Start();
            Console.WriteLine("111");
            Console.ReadLine();
            svr.Stop();
            EntityService_001.Instance.OnDestroy();
            Console.WriteLine("svr.Stop");
        }



        static void Main(string[] args)
        {
            Main_006();
            //return;
            Logger.Init();

            Utils.Init();

            EntityService.Instance.Init();

            WebSocketServer svr = new WebSocketServer("ws://localhost:8888");

            svr.AddWebSocketService<MessagingService>("/MessagingService");

            UserService.Instance.Init();

            svr.Start();

            Console.WriteLine("Server started...");

            Console.ReadLine();

            svr.Stop();

            EntityService.Instance.Destroy();

            Console.WriteLine("Server stopped...");
        }
    }
}
