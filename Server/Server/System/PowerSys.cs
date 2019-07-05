/****************************************************
	文件：PowerSys.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/22 16:06   	
	功能：体力恢复系统
*****************************************************/
using System.Collections.Generic;
using PEProtocol;
using Server.Net;

namespace Server.System
{
    class PowerSys
    {
        private static PowerSys _instance;
        public static PowerSys Instance => _instance ?? (_instance = new PowerSys());
        private CacheSvc _cacheSvc;
        private TimeSvc _timeSvc;
        public void Init()
        {
            _cacheSvc = CacheSvc.Instance;
            _timeSvc = TimeSvc.Instance;

            TimeSvc.Instance.AddTimeTask(CalcPowerAdd, PeRoot.PowerAddSpace, PETimeUnit.Minute, 0);
            PeRoot.Log("PowerSys Init Done.||体力恢复系统.");
        }

        private void CalcPowerAdd(int timeId)
        {
            PeRoot .Log("All Online Player Calc Power Incress....");
            NetMsg netMsg = new NetMsg
            {
                cmd = (int) Command.PshPower
            };
            netMsg.PshPower = new PshPower();

            Dictionary<ServerSession, PlayerData> onLineDictionary = _cacheSvc.GetOnLine();
            foreach (var item in onLineDictionary)
            {
                PlayerData playerData = item.Value;
                ServerSession session = item.Key;

                int powerMax = PeRoot.GetPowerLimit(playerData.Level);
                if (playerData.Power>=powerMax)
                {
                    continue;
                }
                else
                {
                    playerData.Power += PeRoot.PowerAddCount;
                    playerData.Time = _timeSvc.GetNowTime();
                    if (playerData.Power>powerMax)
                    {
                        playerData.Power = powerMax;
                    }
                }

                if (!_cacheSvc.UpdatePlayerData(playerData.Id,playerData))
                {
                    netMsg.err = (int) ErrorCode.UpdateDbError;
                }
                else
                {
                    netMsg.PshPower.Power = playerData.Power;
                    session.SendMsg(netMsg);
                }
            }
        }
    }
}
