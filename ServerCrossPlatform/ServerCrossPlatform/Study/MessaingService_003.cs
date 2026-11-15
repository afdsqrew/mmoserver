using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using Ymck;

namespace ServerCrossPlatform.Study
{
    /// <summary>
    /// d2_04
    /// </summary>
    class MessaingService_003 : WebSocketBehaviorEx_001
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            OnMessage003(e);
        }

        protected void OnMessage003(MessageEventArgs e)
        {
            NetMsg nm = NetMsg.Parser.ParseFrom(e.RawData);
            //这里不知道源码
            MsgProcessor_002.Process(this, nm);
        }
    }
}
