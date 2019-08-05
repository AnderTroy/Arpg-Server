/****************************************************
	文件：NetSvc.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/09 10:39   	
	功能：网络服务
*****************************************************/
using PENet;
using PEProtocol;
using System.Collections.Generic;
using System.Net.Sockets;
using Server.System;
using System.Net;

namespace Server.Net
{
    public class NetSvc
    {
        private static NetSvc _instance = null;
        public static NetSvc Instance => _instance ?? (_instance = new NetSvc());

        public static readonly string Obj = "lock";
        private readonly Queue<MsgPack> msgPackQueue = new Queue<MsgPack>();
        public void Init()
        {
            //自动获取本机IP地址
            string name = Dns.GetHostName();
            string ipName = "";
            IPAddress[] iPAddress = Dns.GetHostAddresses(name);
            foreach (IPAddress ipa in iPAddress)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipName = ipa.ToString();
                }
            }
            PESocket<ServerSession, NetMsg> server = new PESocket<ServerSession, NetMsg>();//创建Socket服务器
            server.StartAsServer(ipName, IpCfg.SrvPort);//打印小提示，服务器启动成功
            PeRoot.Log("NetSvc Init Done.||网络服务.", LogType.Info);
        }
        /// <summary>
        /// 添加对象消息
        /// </summary>
        public void AddMsgQueue(ServerSession session, NetMsg msg)
        {
            lock (Obj)
                msgPackQueue.Enqueue(new MsgPack(session, msg));
        }
        /// <summary>
        /// 判断是否存在消息，存在就取出
        /// </summary>
        public void Update()
        {
            lock (Obj)
            {
                if (msgPackQueue.Count>0)
                {
                    MsgPack pack = msgPackQueue.Dequeue();
                    HandOutMsg(pack);
                }
            }
        }
        /// <summary>
        /// 取出数据
        /// </summary>
        public void HandOutMsg(MsgPack pack)
        {
            switch ((Command)pack.Msg.cmd)
            {
                case Command.RequestLogin:
                    LoginSys.Instance.ReqLogin(pack);
                    break;
                case Command.RequestName:
                    LoginSys.Instance.ReqRename(pack);
                    break;
                case Command.ReqGuide:
                    GuideSys.Instance.RequestGuide(pack);
                    break;
                case Command.ReqStrong:
                    StrongSys.Instance.RequestStrong(pack);
                    break;
                case Command.SendChat:
                    ChatSys.Instance.SendChat(pack);
                    break;
                case Command.RequestBuy:
                    BuySys.Instance.RequestBuy(pack);
                    break;
                case Command.RequestTask:
                    TaskSys.Instance.RequestTaskReward(pack);
                    break;
                case Command.RequestBattle:
                    BattleSys.Instance.RequestBattle(pack);
                    break;
                case Command.RequestBattleEnd:
                    BattleSys.Instance.RequestBattleEnd(pack);
                    break;
            }
        }
    }
    /// <summary>
    /// 打包消息
    /// </summary>
    public class MsgPack
    {
        public ServerSession Session;
        public NetMsg Msg;
        //构造出一个消息
        public MsgPack(ServerSession session, NetMsg msg)
        {
            Session = session;
            Msg = msg;
        }
    }
}
