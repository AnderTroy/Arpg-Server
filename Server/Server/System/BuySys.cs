/****************************************************
	文件：BuySys.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/22 11:40   	
	功能：购买系统
*****************************************************/

using PEProtocol;
using Server.Net;

namespace Server.System
{
    class BuySys
    {
        private static BuySys _instance;
        public static BuySys Instance => _instance ?? (_instance = new BuySys());
        private CacheSvc _cacheSvc;
        public void Init()
        {
            _cacheSvc = CacheSvc.Instance;
            PeRoot.Log("BuySys Init Done.||购买业务系统.");
        }

        public void RequestBuy(MsgPack msgPack)
        {
            RequestBuy data = msgPack.Msg.RequestBuy;
            NetMsg netMsg = new NetMsg
            {
                cmd = (int) Command.ResponseBuy
            };

            PlayerData playerData = _cacheSvc.GetPlayerDataBySession(msgPack.Session);
            if (playerData.Diamond<data.DiamondPay)
            {
                netMsg.err = (int) ErrorCode.LockDiamond;
            }
            else
            {
                playerData.Diamond -= data.DiamondPay;
                PshTask pshTask = null;
                switch (data.Type)
                {
                    case 0:
                        playerData.Power += 100;
                        pshTask= TaskSys.Instance.GetTaskPrangs(playerData, 4);
                        break;
                    case 1:
                        playerData.Coin += 1000;
                        pshTask= TaskSys.Instance.GetTaskPrangs(playerData, 5);
                        break;
                }

                if (!_cacheSvc.UpdatePlayerData(playerData.Id, playerData))
                {
                    netMsg.err = (int)ErrorCode.UpdateDbError;
                }
                else
                {
                    ResponseBuy responseBuy = new ResponseBuy
                    {
                        Type = data.Type,
                        Diamond = playerData.Diamond,
                        Coin = playerData.Coin,
                        Power = playerData.Power,
                    };

                    //并包处理
                    netMsg.ResponseBuy = responseBuy;

                    netMsg.PshTask = pshTask;
                }
            }
            msgPack.Session.SendMsg(netMsg);
        }
    }
}
