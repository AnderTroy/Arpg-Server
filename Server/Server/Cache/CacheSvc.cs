/****************************************************
	文件：Cache.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/09 18:49   	
	功能：数据缓存层
*****************************************************/
using System.Collections.Generic;
using PEProtocol;
using Server.Net;
using Server.MySQL;
public class CacheSvc
{
    private static CacheSvc _instance = null;
    public static CacheSvc Instance => _instance ?? (_instance = new CacheSvc());

    private DbMgr DbMgr;//引用数据库
    private readonly Dictionary<string, ServerSession> onLineAcctDic = new Dictionary<string, ServerSession>();
    private readonly Dictionary<ServerSession, PlayerData> onLineSessionDic = new Dictionary<ServerSession, PlayerData>();
    public void Init()
     {
         DbMgr = DbMgr.Instance;
         PeRoot.Log("CacheSvc Init Done.||数据缓存层.");
    }
    /// <summary>
    /// 判断账号是否在线
    /// </summary>
    public bool IsAcctOnline(string acct)
    {
        return onLineAcctDic.ContainsKey(acct);
    }
    /// <summary>
    /// 获取玩家账号密码
    /// </summary>
    public PlayerData GetPlayerData(string acct, string pass)
    {
        return DbMgr.QueryPlayerData(acct,pass);//从数据库中查找
    }
    /// <summary>
    /// 缓存玩家账号数据
    /// </summary>
    public void AcctOnLine(string acct, ServerSession session, PlayerData playerData)
    {
        onLineAcctDic.Add(acct, session);
        onLineSessionDic.Add(session, playerData);
    }
    /// <summary>
    /// 取得在线玩家
    /// </summary>
    public List<ServerSession> GetOnLineServerSessions()
    {
        List<ServerSession> list = new List<ServerSession>();
        foreach (var item in onLineSessionDic)
        {
            list.Add(item.Key);
        }
        return list;
    }
    public ServerSession GetOnLineServerSessions(int id)
    {
        ServerSession session = null;
        foreach (var item in onLineSessionDic)
        {
            session=item.Key;
        }
        return session;
    }
    /// <summary>
    /// 更新玩家账号数据
    /// </summary>
    public bool UpdatePlayerData(int id, PlayerData playerData)
    {
        return DbMgr.UpDatePlayerData(id, playerData);
    }
    /// <summary>
    /// 判断名字是否已经使用
    /// </summary>
    public bool IsNameExist(string name)
    {
        return DbMgr.QueryNameData(name);
    }
    /// <summary>
    /// 获取玩家游戏数据
    /// </summary>
    public PlayerData GetPlayerDataBySession(ServerSession session)
    {
        return onLineSessionDic.TryGetValue(session, out PlayerData playerData) ? playerData : null;
    }
    public Dictionary<ServerSession,PlayerData> GetOnLine()
    {
        return onLineSessionDic;
    }
    /// <summary>
    /// 玩家下线处理
    /// </summary>
    public void AcctOffLine(ServerSession session)
    {
        foreach (var item in onLineAcctDic)
        {
            if (item.Value == session)
            {
                onLineAcctDic.Remove(item.Key);
                break;
            }
        }
        bool success = onLineSessionDic.Remove(session);
        PeRoot.Log("Offline Result: SessionID:" + session.sessionId + "  " + success);
    }
}
