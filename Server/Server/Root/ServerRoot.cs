/****************************************************
	文件：ServerRoot.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/09 10:30   	
	功能：服务器初始化
*****************************************************/
using PEProtocol;
using Server.Net;
using Server.System;
using Server.MySQL;
namespace Server.Root
{
    class ServerRoot
    {
        private static ServerRoot _instance = null;
        public static ServerRoot Instance => _instance ?? (_instance = new ServerRoot());

        public void Init()
        {
            //服务层
            NetSvc.Instance.Init();
            //数据层
            DbMgr.Instance.Init();
            //缓存层
            CacheSvc.Instance.Init();
            //计时器
            TimeSvc.Instance.Init();
            //配置层
            ResCfgSvc.Instance.Init();
            //业务系统层
            PeRoot.Log("ServerRoot Init Done.||业务系统:", LogType.Info);
            LoginSys.Instance.Init();
            GuideSys.Instance.Init();
            StrongSys.Instance.Init();
            ChatSys.Instance.Init();
            BuySys.Instance.Init();
            PowerSys.Instance.Init();
            TaskSys.Instance.Init();
            BattleSys.Instance.Init();
            PeRoot.Log("等待客户端连接",LogType.Info);
        }
        //实时处理客户端发出的请求
        public void Update()
        {
            NetSvc.Instance.Update();
            TimeSvc.Instance.Update();
        }
        private int sessionId = 0;
        /// <summary>
        /// 记录上线的玩家 ID
        /// </summary>
        public int GetSessionId()
        {
            if (sessionId == int.MaxValue)
            {
                sessionId = 0;
            }
            return sessionId += 1;
        }
        /// <summary>
        /// 删除下线的玩家 ID
        /// </summary>
        public int GetSessionIdOff()
        {
            if (sessionId == 0)
            {
                sessionId = 0;
            }
            return sessionId -= 1;
        }
    }
}
