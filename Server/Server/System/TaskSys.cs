/****************************************************
	文件：TaskSys.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/24 8:33   	
	功能：日常任务奖励系统
*****************************************************/

using PEProtocol;
using Server.Net;

namespace Server.System
{
    class TaskSys
    {
        private static TaskSys _instance;
        public static TaskSys Instance => _instance ?? (_instance = new TaskSys());
        private CacheSvc _cacheSvc;
        private ResCfgSvc _resCfgSvc;
        public void Init()
        {
            _cacheSvc = CacheSvc.Instance;
            _resCfgSvc = ResCfgSvc.Instance;
            PeRoot.Log("TaskSys Init Done.||日常任务奖励系统.");
        }

        public void RequestTaskReward(MsgPack msgPack)
        {
            RequestTask data = msgPack.Msg.RequestTask;
            NetMsg netMsg = new NetMsg
            {
                cmd = (int)Command.ResponseTask
            };
            PlayerData playerData = _cacheSvc.GetPlayerDataBySession(msgPack.Session);

            TaskRewardCfg taskCfg = _resCfgSvc.GetTaskData(data.TaskId);
            TaskRewardData taskData = CalcTaskRewardData(playerData, data.TaskId);

            if (taskData.Prangs == taskCfg.Count && !taskData.Tasked)
            {
                playerData.Coin += taskCfg.Coin;
                PeRoot.CalcExp(playerData, taskCfg.Exp);
                playerData.Diamond += taskCfg.Diamond;
                taskData.Tasked = true;
                CalcTaskArray(playerData, taskData);

                if (!_cacheSvc.UpdatePlayerData(playerData.Id, playerData))
                {
                    netMsg.err = (int)ErrorCode.UpdateDbError;
                }
                else
                {
                    ResponseTask responseTask = new ResponseTask
                    {
                        Coin = playerData.Coin,
                        Level = playerData.Level,
                        Diamond = playerData.Diamond,
                        Exp = playerData.Exp,
                        TaskArray = playerData.TaskArray,
                    };
                    netMsg.ResponseTask = responseTask;
                }
            }
            else
            {
                netMsg.err = (int) ErrorCode.ClientDataError;
            }

            msgPack.Session.SendMsg(netMsg);
        }

        public TaskRewardData CalcTaskRewardData(PlayerData playerData, int id)//解析数据
        {
            TaskRewardData data = null;
            foreach (var t in playerData.TaskArray)
            {
                string[] taskInfo = t.Split('|');
                if (int.Parse(taskInfo[0]) == id)
                {
                    data = new TaskRewardData
                    {
                        Id = int.Parse(taskInfo[0]),
                        Prangs = int.Parse(taskInfo[1]),
                        Tasked = taskInfo[2].Equals("1"),
                    };
                    break;
                }
            }
            return data;
        }

        public void CalcTaskArray(PlayerData playerData, TaskRewardData taskData)//更新任务进度数据
        {
            string result = taskData.Id + "|" + taskData.Prangs + "|" + (taskData.Tasked ? 1 : 0);
            int index = -1;
            for (int i = 0; i < playerData.TaskArray.Length; i++)
            {
                string[] taskInfo = playerData.TaskArray[i].Split('|');
                if (int.Parse(taskInfo[0]) == taskData.Id)
                {
                    index = i;
                    break;
                }
            }
            playerData.TaskArray[index] = result;
        }

        public void CalcTaskPrangs(PlayerData playerData, int taskId)
        {
            TaskRewardData taskData = CalcTaskRewardData(playerData, taskId);
            TaskRewardCfg taskCfg = _resCfgSvc.GetTaskData(taskId);

            if (taskData.Prangs<taskCfg.Count)
            {
                taskData.Prangs += 1;
                CalcTaskArray(playerData, taskData);

                ServerSession session = _cacheSvc.GetOnLineServerSessions(playerData.Id);
                session?.SendMsg(new NetMsg
                {
                    cmd=(int)Command.PshTask,
                    PshTask=new PshTask
                    {
                        TaskArray=playerData.TaskArray
                    }
                });
            }
        }

        public PshTask GetTaskPrangs(PlayerData playerData, int taskId)
        {
            TaskRewardData taskData = CalcTaskRewardData(playerData, taskId);
            TaskRewardCfg taskCfg = _resCfgSvc.GetTaskData(taskId);

            if (taskData.Prangs < taskCfg.Count)
            {
                taskData.Prangs += 1;
                CalcTaskArray(playerData, taskData);

                return new PshTask
                {
                    TaskArray = playerData.TaskArray
                };
            }
            else
            {
                return null;
            }
        }
    }
}
