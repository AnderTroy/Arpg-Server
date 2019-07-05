/****************************************************
	文件：StrongSys.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/19 21:15   	
	功能：强化系统
*****************************************************/
using PEProtocol;
using Server.Net;

namespace Server.System
{
    class StrongSys
    {
        private static StrongSys _instance;
        public static StrongSys Instance => _instance ?? (_instance = new StrongSys());
        private CacheSvc CacheSvc;
        public void Init()
        {
            CacheSvc = CacheSvc.Instance;
            PeRoot.Log("StrongSys Init Done.||强化业务系统.");
        }

        public void RequestStrong(MsgPack msgPack)
        {
            RequestStrong data = msgPack.Msg.RequestStrong;
            NetMsg netMsg = new NetMsg
            {
                cmd = (int) Command.RspStrong
            };
            PlayerData playerData = CacheSvc.GetPlayerDataBySession(msgPack.Session);
            int curtStartLevel = playerData.StrongArray[data.pos];
            StrongCfg nextStrong = ResCfgSvc.Instance.GetStrongCfg(data.pos, curtStartLevel + 1);

            if (playerData.Level<nextStrong.MinLevel)
            {
                netMsg.err = (int) ErrorCode.LackLevel;
            }
            else if (playerData.Coin < nextStrong.Coin)
            {
                netMsg.err = (int) ErrorCode.LackCoin;

            }
            else if (playerData.Crystal < nextStrong.Crystal)
            {
                netMsg.err = (int)ErrorCode.LackCrystal;
            }
            else
            {
                TaskSys.Instance.CalcTaskPrangs(playerData, 3);
                playerData.Coin -= nextStrong.Coin;
                playerData.Crystal -= nextStrong.Crystal;
                playerData.StrongArray[data.pos] += 1;

                playerData.Hp += nextStrong.AddHp;
                playerData.Ad += nextStrong.AddHurt;
                playerData.AdDefense += nextStrong.AddDefense;
                playerData.ApDefense += nextStrong.AddDefense;
            }
            if (!CacheSvc.UpdatePlayerData(playerData.Id,playerData))
            {
                netMsg.err = (int) ErrorCode.UpdateDbError;
            }
            else
            {
                netMsg.ResponseStrong = new ResponseStrong
                {
                    Coin = playerData.Coin,
                    Crystal = playerData.Crystal,
                    Hp = playerData.Hp,
                    Ad = playerData.Ad,
                    Ap = playerData.Ap,
                    AdDefense = playerData.AdDefense,
                    ApDefense = playerData.ApDefense,
                    StrongArray = playerData.StrongArray,
                };
            }
            msgPack.Session.SendMsg(netMsg);
        }
    }
}
