/****************************************************
	文件：BattleSys.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/26 15:50   	
	功能：副本战斗系统
*****************************************************/

using PEProtocol;
using Server.Net;

namespace Server.System
{
    class BattleSys
    {
        private static BattleSys _instance;
        public static BattleSys Instance => _instance ?? (_instance = new BattleSys());
        private CacheSvc _cacheSvc;
        private ResCfgSvc _resCfgSvc;
        public void Init()
        {
            _cacheSvc = CacheSvc.Instance;
            _resCfgSvc = ResCfgSvc.Instance;
            PeRoot.Log("BattleSys Init Done.||副本战斗系统.");
        }

        public void RequestBattle(MsgPack msgPack)
        {
            RequestBattle data = msgPack.Msg.RequestBattle;

            NetMsg netMsg = new NetMsg
            {
                cmd = (int) Command.ResponseBattle
            };
            PlayerData playerData = _cacheSvc.GetPlayerDataBySession(msgPack.Session);
            int power = _resCfgSvc.GetMapData(data.BattleId).Power;
            if (playerData.Battle<data.BattleId)
            {
                netMsg.err = (int) ErrorCode.ClientDataError;
            }
            else if (playerData.Power<power)
            {
                netMsg.err = (int) ErrorCode.LackPower;
            }
            else
            {
                playerData.Power -= power;
                if (_cacheSvc.UpdatePlayerData(playerData.Id,playerData))
                {
                    ResponseBattle responseBattle = new ResponseBattle
                    {
                        BattleId = data.BattleId,
                        Power = playerData.Power,
                    };
                    netMsg.ResponseBattle = responseBattle;
                }
                else
                {
                    netMsg.err = (int) ErrorCode.UpdateDbError;
                }
            }
            msgPack.Session.SendMsg(netMsg);
        }

        public void RequestBattleEnd(MsgPack msgPack)
        {
            RequestBattleEnd data = msgPack.Msg.RequestBattleEnd;

            NetMsg netMsg = new NetMsg
            {
                cmd = (int)Command.ResponseBattleEnd
            };

            //校验战斗是否合法
            if (data.IsWin)
            {
                if (data.CostTime>0&&data.RestHp>0)
                {
                    //获取战斗副本相应的奖励
                    MapCfg mapCfg = _resCfgSvc.GetMapData(data.BattleId);
                    PlayerData playerData = _cacheSvc.GetPlayerDataBySession(msgPack.Session);

                    TaskSys.Instance.CalcTaskPrangs(playerData, 2);
                    playerData.Coin += mapCfg.Coin;
                    playerData.Crystal += mapCfg.Crystal;

                    PeRoot.CalcExp(playerData, mapCfg.Exp);

                    if (playerData.Battle==data.BattleId)
                    {
                        playerData.Battle += 1;
                    }

                    if (!_cacheSvc.UpdatePlayerData(playerData.Id,playerData))
                    {
                        netMsg.err = (int) ErrorCode.UpdateDbError;
                    }
                    else
                    {
                        ResponseBattleEnd battleEnd = new ResponseBattleEnd
                        {
                            IsWin = data.IsWin,
                            Battle = data.BattleId,
                            CostTime = data.CostTime,
                            RestHp = data.RestHp,


                            Coin = playerData.Coin,
                            Level = playerData.Level,
                            Exp = playerData.Exp,
                            Crystal = playerData.Crystal,
                            BattleId = playerData.Battle,
                        };
                        netMsg.ResponseBattleEnd = battleEnd;
                    }
                }
            }
            else
            {
                netMsg.err = (int) ErrorCode.ClientDataError;
            }
            msgPack.Session.SendMsg(netMsg);
        }
    }
}
