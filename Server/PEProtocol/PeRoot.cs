/****************************************************
	文件：PeRoot.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/09 10:57   	
	功能：客户端服务端共用工具类
*****************************************************/
using PENet;

namespace PEProtocol
{
    /// <summary>
    /// 错误提示类型
    /// </summary>
    public enum LogType
    {
        Log,
        LogWarning,
        LogError,
        Info
    }
    public class PeRoot
    {
        public const int PowerAddSpace = 5;//分钟
        public const int PowerAddCount = 2;
        /// <summary>
        /// 打印信息
        /// </summary>
        public static void Log(string msg = "", LogType logType = LogType.Log)
        {
            LogLevel level = (LogLevel) logType;
            PETool.LogMsg(msg, level);
        }
        /// <summary>
        /// 战斗力计算公式
        /// </summary>
        public static int GetFightByProps(PlayerData playerData)
        {
            return playerData.Level * 100 + playerData.Ad + playerData.Ap + playerData.AdDefense + playerData.ApDefense;
        }
        /// <summary>
        /// 最大体力值计算公式
        /// </summary>
        public static int GetPowerLimit(int level)
        {
            return ((level - 1) / 10) * 150 + 150;
        }
        /// <summary>
        /// 经验值随等级变化，需求公式
        /// </summary>
        public static int GetExpUpValByLv(int level)
        {
            return 100 * level * level;
        }

        public static void CalcExp(PlayerData playerData, int addExp)
        {
            int curtLv = playerData.Level;
            int curtExp = playerData.Exp;
            int addRestExp = addExp;
            while (true)
            {
                int upNeedExp = PeRoot.GetExpUpValByLv(curtLv) - curtExp;
                if (addRestExp >= upNeedExp)
                {
                    curtLv += 1;
                    curtExp = 0;
                    addRestExp -= upNeedExp;
                }
                else
                {
                    playerData.Level = curtLv;
                    playerData.Exp = curtExp + addRestExp;
                    break;
                }
            }
        }
    }
}
