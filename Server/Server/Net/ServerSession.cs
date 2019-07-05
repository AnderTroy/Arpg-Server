/****************************************************
	文件：ServerSession.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/09 10:22   	
	功能：与客户端建立连接，完成通信，数据传输
*****************************************************/
using PENet;
using PEProtocol;
using Server.Root;
using Server.System;

namespace Server.Net
{
    public class ServerSession:PESession<NetMsg>
    {
        public int sessionId=0;
        protected override void OnConnected()
        {
            PeRoot.Log("SessionId: " + sessionId + " Client Connect...||与客户端建立连接...\n开始接收消息：");
            sessionId = ServerRoot.Instance.GetSessionId();
        }

        protected override void OnReciveMsg(NetMsg msg)
        {
            PeRoot.Log("SessionId: " + sessionId + " Client Req || 客户端发送消息：" + (Command)msg.cmd);
            NetSvc.Instance.AddMsgQueue(this, msg);
        }

        protected override void OnDisConnected()
        {
            LoginSys.Instance.ClearOfflineData(this);
            ServerRoot.Instance.GetSessionIdOff();
            PeRoot.Log("SessionId: " + sessionId +" Client DisConnect...||客户端断开连接...");
        }
    }
}
