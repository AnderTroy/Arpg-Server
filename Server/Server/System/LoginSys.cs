/****************************************************
	文件：LoginSys.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/09 22:04   	
	功能：登入业务系统
*****************************************************/
using PEProtocol;
using Server.Net;

namespace Server.System
{
    class LoginSys
    {
        private static LoginSys _instance;
        public static LoginSys Instance => _instance ?? (_instance = new LoginSys());
        private CacheSvc _cacheSvc;
        private TimeSvc _timeSvc;
        public void Init()
        {
            _cacheSvc = CacheSvc.Instance;
            _timeSvc = TimeSvc.Instance;
            PeRoot.Log("LoginSys Init Done.||登入业务系统.");
        }

        #region 请求 上线获取游戏数据
        public void ReqLogin(MsgPack pack)
        {
            RequestLogin data = pack.Msg.RequestLogin;
            //账号是否上线
            NetMsg netMsg = new NetMsg
            {
                cmd = (int)Command.ResponseLogin
            };
            if (_cacheSvc.IsAcctOnline(data.Acct))
            {
                //己上线：返回错误信息
                netMsg.err = (int)ErrorCode.AcctIsOnline;
            }
            else
            {
                //账号不在线 || 账号是否已经存在
                PlayerData playerData = _cacheSvc.GetPlayerData(data.Acct, data.Pass);
                if (playerData == null)
                {
                    //账号存在，密码错误
                    netMsg.err = (int)ErrorCode.WrongPass;
                }
                else
                {
                    #region 计算离线玩家体力值
                    int power = playerData.Power;
                    long timeNow = _timeSvc.GetNowTime();
                    long milliseconds = timeNow - playerData.Time;
                    int addPower = (int)(milliseconds / (1000 * 60 * PeRoot.PowerAddSpace)) * PeRoot.PowerAddCount;
                    if (addPower > 0)
                    {
                        int powerMax = PeRoot.GetPowerLimit(playerData.Level);
                        if (playerData.Power < powerMax)
                        {
                            playerData.Power += addPower;
                            if (playerData.Power > powerMax)
                            {
                                playerData.Power = powerMax;
                            }
                        }
                    }
                    if (power != playerData.Power)
                    {
                        _cacheSvc.UpdatePlayerData(playerData.Id, playerData);
                    }
                    #endregion
                    netMsg.ResponseLogin = new ResponseLogin
                    {
                        PlayerData = playerData
                    };
                    //缓存账号数据
                    _cacheSvc.AcctOnLine(data.Acct, pack.Session, playerData);
                }
            }
            pack.Session.SendMsg(netMsg);
        }
        #endregion

        #region 请求是否可以使用该名字
        public void ReqRename(MsgPack pack)
        {
            RequestName data = pack.Msg.RequestName;
            NetMsg msg = new NetMsg
            {
                cmd = (int)Command.ResponseName
            };
            if (_cacheSvc.IsNameExist(data.Name))
            {
                //名字是否已经存在
                //存在：返回错误码
                msg.err = (int)ErrorCode.NameIsExist;
            }
            else
            {
                //不存在：更新缓存，以及数据库，再返回给客户端
                PlayerData playerData = _cacheSvc.GetPlayerDataBySession(pack.Session);
                playerData.Name = data.Name;
                if (!_cacheSvc.UpdatePlayerData(playerData.Id, playerData))
                {
                    msg.err = (int)ErrorCode.UpdateDbError;
                }
                else
                {
                    msg.ResponseName = new ResponseName
                    {
                        Name = data.Name
                    };
                }
            }
            pack.Session.SendMsg(msg);
        }
        #endregion

        /// <summary>
        /// 玩家下线处理
        /// </summary>
        public void ClearOfflineData(ServerSession session)
        {
            _cacheSvc.AcctOffLine(session);
        }

        public void ClearOffLineData(ServerSession session)
        {
            PlayerData playerData = _cacheSvc.GetPlayerDataBySession(session);
            if (playerData!=null)
            {
                playerData.Time = _timeSvc.GetNowTime();
                if (!_cacheSvc.UpdatePlayerData(playerData.Id,playerData))
                {
                    PeRoot.Log("Update offline time error", LogType.LogError);
                }
                _cacheSvc.AcctOffLine(session);
            }
        }
    }
}
