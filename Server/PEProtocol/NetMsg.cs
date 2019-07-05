/****************************************************
	文件：NetMsg.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/09 10:40
	功能：网络消息定义
*****************************************************/
using System;
using PENet;

namespace PEProtocol
{
    /// <summary>
    /// 序列化功能，消息体支持多层嵌套
    /// 序列化 (Serialization)是将对象的状态信息转换为可以存储或传输的形式的过程(字节流||二进制)
    /// 在序列化期间，对象将其当前状态写入到临时或持久性存储区。
    /// 以后，可以通过从存储区中读取或反序列化对象的状态，重新创建该对象。
    /// </summary>
    [Serializable]
    public class NetMsg : PEMsg
    {
        public RequestLogin RequestLogin;
        public ResponseLogin ResponseLogin;

        public RequestName RequestName;
        public ResponseName ResponseName;

        public RequestGuide RequestGuide;
        public ResponseGuide ResponseGuide;

        public RequestStrong RequestStrong;
        public ResponseStrong ResponseStrong;

        public SendChat SendChat;
        public PshChat PshChat;

        public RequestBuy RequestBuy;
        public ResponseBuy ResponseBuy;
        public PshPower PshPower;

        public RequestTask RequestTask;
        public ResponseTask ResponseTask;
        public PshTask PshTask;

        public RequestBattle RequestBattle;
        public ResponseBattle ResponseBattle;

        public RequestBattleEnd RequestBattleEnd;
        public ResponseBattleEnd ResponseBattleEnd;
    }

    #region Login
    // 请求 账号密码request,response
    [Serializable]
    public class RequestLogin
    {
        public string Acct;
        public string Pass;
    }
    /// 回应 玩家信息
    [Serializable]
    public class ResponseLogin
    {
        public PlayerData PlayerData;
    }
    #endregion

    #region CreatName
    // 请求 创建名字
    [Serializable]
    public class RequestName
    {
        public string Name;
    }
    // 回应 创建名字
    [Serializable]
    public class ResponseName
    {
        public string Name;
    }
    #endregion

    #region Guide
    [Serializable]
    public class RequestGuide
    {
        public int GuideId;
    }
    [Serializable]
    public class ResponseGuide
    {
        public int GuideId;
        public int Coin;
        public int Level;
        public int Exp;
    }
    #endregion

    #region Strong
    [Serializable]
    public class RequestStrong
    {
        public int pos;
    }
    [Serializable]
    public class ResponseStrong
    {
        public int Coin;
        public int Crystal;
        public int Hp;
        public int Ad;
        public int Ap;
        public int AdDefense;
        public int ApDefense;
        public int[] StrongArray;
    }
    #endregion

    #region Chat
    [Serializable]
    public class SendChat
    {
        public string Chat;
    }
    [Serializable]
    public class PshChat
    {
        public string Name;
        public string Chat;
    }
    #endregion

    #region Buy
    [Serializable]
    public class RequestBuy
    {
        public int Type;
        public int DiamondPay;
    }
    [Serializable]
    public class ResponseBuy
    {
        public int Type;
        public int Diamond;
        public int Coin;
        public int Power;
    }
    [Serializable]
    public class PshPower
    {
        public int Power;
    }
    #endregion

    #region Task
    [Serializable]
    public class RequestTask
    {
        public int TaskId;
    }
    [Serializable]
    public class ResponseTask
    {
        public int Coin;
        public int Diamond;
        public int Level;
        public int Exp;
        public string[] TaskArray;//任务进度
    }
    [Serializable]
    public class PshTask
    {
        public string [] TaskArray;
    }
    #endregion

    #region Battles
    [Serializable]
    public class RequestBattle
    {
        public int BattleId;
    }
    [Serializable]
    public class ResponseBattle
    {
        public int BattleId;
        public int Power;
    }
    [Serializable]
    public class RequestBattleEnd
    {
        public int BattleId;
        public bool IsWin;
        public int RestHp;
        public int CostTime;
    }
    [Serializable]
    public class ResponseBattleEnd
    {
        public int BattleId;
        public bool IsWin;
        public int RestHp;
        public int CostTime;

        public int Coin;
        public int Exp;
        public int Crystal;
        public int Level;
        public int Battle;
    }
    #endregion

    #region PlayerData
    [Serializable]
    public class PlayerData
    {
        public int Id;
        public string Name;
        public int Level;
        public int Exp;//经验
        public int Power;//体力
        public int Vip;

        public int Coin;//金币
        public int Diamond;//钻石
        public int Crystal;

        public int Hp;
        public int Ad;
        public int Ap;
        public int AdDefense;//护甲
        public int ApDefense;//魔抗
        public int Dodge;//闪避概率
        public int Pierce;//穿透比率
        public int Critical;//暴击概率

        public int GuideId;
        public int[] StrongArray;
        public long Time;//离线时间

        public string[] TaskArray;
        public int Battle;
    }
    #endregion
    // 错误码
    public enum ErrorCode
    {
        None,
        AcctIsOnline,//账号已经上线
        WrongPass,//密码错误
        NameIsExist,//名字已存在
        ClientDataError,//客户端数据异常
        UpdateDbError,//更新游戏数据失败
        ServerDataError,//服务器数据异常

        LackLevel,
        LackCoin,
        LackCrystal,
        LockDiamond,
        LackPower,
    }
    /// 命令提示 ID
    public enum Command
    {
        None,
        //登录相关 100
        RequestLogin = 101,
        ResponseLogin = 102,
        RequestName = 103,
        ResponseName = 104,

        //主城相关 200
        ReqGuide = 201,
        RspGuide = 202,

        ReqStrong = 203,
        RspStrong = 204,

        SendChat = 205,
        PshChat = 206,

        RequestBuy=207,
        ResponseBuy=208,
        PshPower=209,

        RequestTask = 210,
        ResponseTask = 211,
        PshTask = 212,

        RequestBattle = 301,
        ResponseBattle = 302,

        RequestBattleEnd = 303,
        ResponseBattleEnd = 304,
    }
    /// 服务器地址，端口号
    public class IpCfg
    {
        //public const string SrvIp = "192.168.2.1";
        public const int SrvPort = 17666;
    }
}
