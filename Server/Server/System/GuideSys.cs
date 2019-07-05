/****************************************************
	文件：GuideSys.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/18 9:09   	
	功能：引导任务业务系统
*****************************************************/
using PEProtocol;
using System.Xml;
using Server.Net;

namespace Server.System
{
    public class GuideSys
    {
        private static GuideSys _instance;
        public static GuideSys Instance => _instance ?? (_instance = new GuideSys());
        private CacheSvc _cacheSvc;
        private ResCfgSvc _resCfgSvc;
        public void Init()
        {
            _cacheSvc = CacheSvc.Instance;
            _resCfgSvc = ResCfgSvc.Instance;
            PeRoot.Log("GuideSys Init Done.||引导任务业务系统.");
        }

        public void RequestGuide(MsgPack pack)
        {
            RequestGuide data = pack.Msg.RequestGuide;
            NetMsg netMsg = new NetMsg
            {
                cmd = (int) Command.RspGuide
            };
            PlayerData playerData = _cacheSvc.GetPlayerDataBySession(pack.Session);
            GuideCfg guideCfg = _resCfgSvc.GetGuideData(data.GuideId);

            //更新引导Id
            if (playerData.GuideId==data.GuideId)
            {
                if (playerData.GuideId==1001)
                {
                    TaskSys.Instance.CalcTaskPrangs(playerData, 1);
                }
                playerData.GuideId += 1;

                //更新玩家数据
                playerData.Coin += guideCfg.Coin;
                PeRoot.CalcExp(playerData, guideCfg.Exp);
                if (!_cacheSvc.UpdatePlayerData(playerData.Id,playerData))
                {
                    netMsg.err = (int) ErrorCode.UpdateDbError;
                }
                else
                {
                    netMsg.ResponseGuide = new ResponseGuide
                    {
                        GuideId = playerData.GuideId,
                        Coin = playerData.Coin,
                        Level = playerData.Level,
                        Exp = playerData.Exp,
                    };
                }
            }
            else
            {
                netMsg.err = (int) ErrorCode.ServerDataError;
            }
            pack.Session.SendMsg(netMsg);
        }
    }
}
