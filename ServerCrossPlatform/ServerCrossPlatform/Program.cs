using Google.Protobuf;
using LitJson;
using log4net.Config;
using Microsoft.Extensions.Logging;
using ServerCrossPlatform.Study;
using ServerCrossPlatform.Test;
using System;
using System.IO;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using Ymck;
using Logger = Ymck.Logger;

namespace Ymck
{
    public class UserService_d1_03 : WebSocketBehavior
    {
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


        protected void OnMessage_d1_04(MessageEventArgs e)
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

        protected void OnMessage_d1_03(MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
            this.Send($"from server {e.Data}");
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
        static void Main_d1_03_2()
        {
            WebSocketServer svr = new WebSocketServer("ws://localhost:8888");
            svr.AddWebSocketService<UserService_d1_03>("/UserService");
            svr.Start();
            Console.WriteLine("111");
            Console.ReadLine();
            svr.Stop();
            Console.WriteLine("svr.Stop");
        }

        static void Main_d1_03()
        {
            WebSocketServer svr = new WebSocketServer("ws://127.0.0.1:8888");
            svr.AddWebSocketService<UserService_d1_03>("/UserService");
            svr.Start();
            Console.WriteLine("111");
            Console.ReadLine();
            svr.Stop();
            Console.WriteLine("svr.Stop");
        }

        /// <summary>
        /// 第一次期——d0
        /// </summary>
        static void Main_d2_04()
        {
            UserService_d2_04.Instance.Init();

            WebSocketServer svr = new WebSocketServer("ws://localhost:8888");
            svr.Start();
            Console.WriteLine("111");
            Console.ReadLine();
            svr.Stop();
            Console.WriteLine("svr.Stop");
        }

        static void Main(string[] args)
        {
            Main_d1_03();
            return;
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
