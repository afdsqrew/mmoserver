using LitJson;
using System;
using System.IO;
using System.Text;
using WebSocketSharp;
using Ymck;
using CommonCrossPlatform;
using Google.Protobuf;
namespace conClient
{
    internal class Program
    {
        /// <summary>
        /// d1-03-高性能网络通信框架
        /// </summary>
        static void Main_002()
        {
            using (WebSocket ws = new WebSocket("ws://localhost:8888/UserService"))
            {
                ws.OnMessage += GetWs_OnMessage_d1_03;
                ws.Connect();
                ws.Send("hello");
                Console.ReadLine();
            }

            Console.WriteLine("client end");
        }

        /// <summary>
        /// d1-04
        /// </summary>
        static void Main_003()
        {
            using (WebSocket ws = new WebSocket("ws://localhost:8888/UserService"))
            {
                ws.OnMessage += GetWs_OnMessage_d1_03;
                ws.Connect();

                var nm = new NetMsg();
                nm.Req = new ReqMsg();
                nm.Req.UserReg = new UserRegReq();
                nm.Req.UserReg.Name = "111";
                nm.Req.UserReg.Pwd = "222";

                string s = JsonMapper.ToJson(nm);

                ws.Send(s);
                Console.ReadLine();
            }

            Console.WriteLine("client end");
        }

        /// <summary>
        /// d202
        /// </summary>
        static void Main_004()
        {
            using (WebSocket ws = new WebSocket("ws://localhost:8888/UserService"))
            {
                ws.OnMessage += GetWs_OnMessage_d2_02;
                ws.Connect();

                var nm = new NetMsg();
                nm.Req = new ReqMsg();
                nm.Req.UserReg = new UserRegReq();
                nm.Req.UserReg.Name = Console.ReadLine();
                nm.Req.UserReg.Pwd = Console.ReadLine();

                ws.Send(nm.ToByteArray());
                
                Console.ReadLine();
            }

            Console.WriteLine("client end");
        }
        /// <summary>
        /// d204
        /// </summary>
        static void Main_005()
        {
            using (WebSocket ws = new WebSocket("ws://localhost:8888/MessagingService"))
            {
                ws.OnMessage += GetWs_OnMessage_d2_02;
                ws.Connect();
                Console.Write("111");
                var nm = new NetMsg();
                Console.Write("222");

                nm.Req = new ReqMsg();
                nm.Req.UserReg = new UserRegReq();
                nm.Req.UserReg.Name = "111";
                nm.Req.UserReg.Pwd = "222";
                ws.Send(nm.ToByteArray());
                Console.ReadLine();
            }
            Console.WriteLine("client end");
        }


        static void Main_d3_01()
        {
            using (WebSocket ws = new WebSocket("ws://localhost:8888/UserService"))
            {
                UserService_d3_01.Instance.Init();

                ws.OnMessage += GetWs_OnMessage_d2_02;
                ws.Connect();

                var nm = new NetMsg();
                nm.Req = new ReqMsg();
                nm.Req.UserReg = new UserRegReq();
                Console.WriteLine("write");
                nm.Req.UserReg.Name = Console.ReadLine();
                nm.Req.UserReg.Pwd = Console.ReadLine();

                ws.Send(nm.ToByteArray());

                Console.ReadLine();
            }

            Console.WriteLine("client end");
        }

        private static void GetWs_OnMessage_d3_01(object sender, MessageEventArgs e)
        {
            NetMsg nm = NetMsg.Parser.ParseFrom(e.RawData);
            Console.WriteLine($"RESP:{JsonMapper.ToJson(nm)}");
        }


        private static void GetWs_OnMessage_d2_02(object sender, MessageEventArgs e)
        {
            NetMsg nm = NetMsg.Parser.ParseFrom(e.RawData);
            Console.Write("OnBack:" + nm.Resp.UserReg.User.Name);
            //Console.WriteLine($"RESP:{JsonMapper.ToJson(nm)}");
            
        }

        public static WebSocket clientSocket = null;
   
        static void Main_d3_02()
        {
            using (clientSocket = new WebSocket("c"))
            {
                UserService_d3_01.Instance.Init();

                clientSocket.OnMessage += GetWs_OnMessage_d2_02;
                clientSocket.Connect();

                UserService_d3_01.Instance.DoUserReg("111","222");

                Console.ReadLine();
            }

            Console.WriteLine("client end");
        }










        private static void GetWs_OnMessage_d1_03(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        static void Main(string[] args)
        {
            Main_005();
            //Main_Current();
        }



        /// <summary>
        /// d2-02
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ws_OnMessage_004(object sender, MessageEventArgs e)
        {
            NetMsg nm = NetMsg.Parser.ParseFrom(e.RawData);
            Console.WriteLine($"RESP:{JsonMapper.ToJson(nm)}");
        }
    }
}
