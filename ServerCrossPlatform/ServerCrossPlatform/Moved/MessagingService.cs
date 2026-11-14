using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Ymck
{
	class MessagingService : WebSocketBehaviorEx<Session>
	{
		protected override void OnMessage(MessageEventArgs e)
		{
            Console.WriteLine("---server OnMessage1");
            Console.WriteLine(e.Data);

			// json:
			//NetMsg netMsg = JsonMapper.ToObject<NetMsg>(e.Data);
			//MsgProcessor.Process(this, netMsg);

			// pb:
			using (var ms = new MemoryStream(e.RawData))
			{
                Console.WriteLine("---server OnMessage2");
                NetMsg netMsg = NetMsg.Parser.ParseFrom(e.RawData);
				if (netMsg == null)
				{
                    Console.WriteLine("---server OnMessage2 \t\t\t\tif (netMsg == null)\r\n");
                }
                MsgProcessor<Session, WebSocketBehaviorEx<Session>>.Instance.Process(this, netMsg); // 接收消息
				MsgProcessor<Session, WebSocketBehaviorEx<Session>>.Instance.HandleAllMsgs(); // 处理消息
			}
            Console.WriteLine("---server OnMessage3");

            //this.Send($"From server: {e.Data}");
        }
    }
}
