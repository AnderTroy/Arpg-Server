/****************************************************
	文件：ChatSys.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/21 0:35   	
	功能：聊天系统
*****************************************************/
using System.Collections.Generic;
using PEProtocol;
using Server.Net;

namespace Server.System
{
    class ChatSys
    {
        private static ChatSys _instance;
        public static ChatSys Instance => _instance ?? (_instance = new ChatSys());
        private CacheSvc _cacheSvc;
        public void Init()
        {
            _cacheSvc = CacheSvc.Instance;
            PeRoot.Log("ChatSys Init Done.||聊天业务系统.");
        }

        public void SendChat(MsgPack pack)
        {
            SendChat data = pack.Msg.SendChat;
            PlayerData playerData = _cacheSvc.GetPlayerDataBySession(pack.Session);

            TaskSys.Instance.CalcTaskPrangs(playerData, 6);
            NetMsg netMsg = new NetMsg
            {
                cmd = (int) Command.PshChat,
                PshChat = new PshChat
                {
                    Name = playerData.Name,
                    Chat = data.Chat
                }
            };

            List<ServerSession> list = _cacheSvc.GetOnLineServerSessions();
            byte[] bytes = PENet.PETool.PackNetMsg(netMsg);
            foreach (var t in list)
            {
                t.SendMsg(bytes);
            }
        }
        
    }
}
