/****************************************************
	文件：ResCfgsSvc.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/18 9:19   	
	功能：配置数据服务
*****************************************************/
using System;
using System.Xml;
using System.Collections.Generic;
using PEProtocol;


public class ResCfgSvc
{
    private static ResCfgSvc _instance = null;
    public static ResCfgSvc Instance { get; } = _instance ?? (_instance = new ResCfgSvc());

    public void Init()
    {
        PeRoot.Log("CfgSvc Init Done.||数据配置.",LogType.Info);
        InitGuideCfgData();
        InitStrongCfg();
        InitTaskCfgData();
        InitMapCfgData();
    }

    #region Init GuideCfg
    private readonly Dictionary<int, GuideCfg> guideDictionary = new Dictionary<int, GuideCfg>();
    private void InitGuideCfgData()
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load("D:\\UNITY\\DarkGod\\Assets\\Resources\\ResCfgs\\guide.xml");

        XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("root")?.ChildNodes;

        for (int i = 0; i < xmlNodeList?.Count; i++)
        {
            XmlElement xmlElement = xmlNodeList[i] as XmlElement;

            if (xmlElement?.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int id = Convert.ToInt32(xmlElement.GetAttributeNode("ID")?.InnerText);
            GuideCfg mc = new GuideCfg
            {
                Id = id
            };

            foreach (XmlElement e in xmlNodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "coin":
                        mc.Coin = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        mc.Exp = int.Parse(e.InnerText);
                        break;
                }
            }
            guideDictionary.Add(id, mc);
        }
        PeRoot.Log("GuideCfg Init Done.||引导任务数据配置.");
    }

    public GuideCfg GetGuideData(int id)
    {
        GuideCfg agc = null;
        if (guideDictionary.TryGetValue(id, out agc))
        {
            return agc;
        }
        return null;
    }
    #endregion

    #region Init StrongCfg
    private readonly Dictionary<int, Dictionary<int, StrongCfg>> strongDictionary = new Dictionary<int, Dictionary<int, StrongCfg>>();
    private void InitStrongCfg()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(@"D:\UNITY\DarkGod\Assets\Resources\ResCfgs\strong.xml");

        XmlNodeList nodLst = doc.SelectSingleNode("root")?.ChildNodes;

        for (int i = 0; i < nodLst?.Count; i++)
        {
            XmlElement ele = nodLst[i] as XmlElement;

            if (ele != null && ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            if (ele != null)
            {
                int id = Convert.ToInt32(ele.GetAttributeNode("ID")?.InnerText);
                StrongCfg strongCfg = new StrongCfg
                {
                    Id = id
                };

                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    int val = int.Parse(e.InnerText);
                    switch (e.Name)
                    {
                        case "pos":
                            strongCfg.Pos = val;
                            break;
                        case "starlv":
                            strongCfg.StartLevel = val;
                            break;
                        case "addhp":
                            strongCfg.AddHp = val;
                            break;
                        case "addhurt":
                            strongCfg.AddHurt = val;
                            break;
                        case "adddef":
                            strongCfg.AddDefense = val;
                            break;
                        case "minlv":
                            strongCfg.MinLevel = val;
                            break;
                        case "coin":
                            strongCfg.Coin = val;
                            break;
                        case "crystal":
                            strongCfg.Crystal = val;
                            break;
                    }
                }
                if (strongDictionary.TryGetValue(strongCfg.Pos, out var dic))
                {
                    dic.Add(strongCfg.StartLevel, strongCfg);
                }
                else
                {
                    dic = new Dictionary<int, StrongCfg> {{strongCfg.StartLevel, strongCfg}};
                    strongDictionary.Add(strongCfg.Pos, dic);
                }
            }
        }
        PeRoot.Log("StrongCfg Init Done.||强化数据配置.");
    }
    public StrongCfg GetStrongCfg(int pos, int starlv)
    {
        StrongCfg strongCfg = null;
        if (strongDictionary.TryGetValue(pos, out var dic))
        {
            if (dic.ContainsKey(starlv))
            {
                strongCfg = dic[starlv];
            }
        }
        return strongCfg;
    }
    #endregion

    #region Init TaskCfg
    private readonly Dictionary<int, TaskRewardCfg> taskDictionary = new Dictionary<int, TaskRewardCfg>();
    private void InitTaskCfgData()
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load("D:\\UNITY\\DarkGod\\Assets\\Resources\\ResCfgs\\taskreward.xml");

        XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("root")?.ChildNodes;

        for (int i = 0; i < xmlNodeList?.Count; i++)
        {
            XmlElement xmlElement = xmlNodeList[i] as XmlElement;

            if (xmlElement?.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int id = Convert.ToInt32(xmlElement.GetAttributeNode("ID")?.InnerText);
            TaskRewardCfg taskCfg = new TaskRewardCfg
            {
                Id = id
            };

            foreach (XmlElement e in xmlNodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "coin":
                        taskCfg.Coin = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        taskCfg.Exp = int.Parse(e.InnerText);
                        break;
                    case "count":
                        taskCfg.Count = int.Parse(e.InnerText);
                        break;
                    case "diamond":
                        taskCfg.Diamond = int.Parse(e.InnerText);
                        break;
                }
            }
            taskDictionary.Add(id, taskCfg);
        }
        PeRoot.Log("TaskRewardCfg Init Done.||日常任务数据配置.");
    }

    public TaskRewardCfg GetTaskData(int id)
    {
        if (taskDictionary.TryGetValue(id, out var agc))
        {
            return agc;
        }
        return null;
    }
    #endregion

    #region Init MapCfg
    private readonly Dictionary<int, MapCfg> mapDictionary = new Dictionary<int, MapCfg>();
    private void InitMapCfgData()
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load("D:\\UNITY\\DarkGod\\Assets\\Resources\\ResCfgs\\map.xml");

        XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("root")?.ChildNodes;

        for (int i = 0; i < xmlNodeList?.Count; i++)
        {
            XmlElement xmlElement = xmlNodeList[i] as XmlElement;

            if (xmlElement?.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int id = Convert.ToInt32(xmlElement.GetAttributeNode("ID")?.InnerText);
            MapCfg mapCfg = new MapCfg
            {
                Id = id
            };

            foreach (XmlElement e in xmlNodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "power":
                        mapCfg.Power = int.Parse(e.InnerText);
                        break;
                    case "coin":
                        mapCfg.Coin = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        mapCfg.Exp = int.Parse(e.InnerText);
                        break;
                    case "crystal":
                        mapCfg.Crystal = int.Parse(e.InnerText);
                        break;
                }
            }
            mapDictionary.Add(id, mapCfg);
        }
        PeRoot.Log("MapCfg Init Done.||地图数据配置.");
    }

    public MapCfg GetMapData(int id)
    {
        if (mapDictionary.TryGetValue(id, out var agc))
        {
            return agc;
        }
        return null;
    }
    #endregion
}

public class BaseData<T>
{
    public int Id;
}
public class GuideCfg : BaseData<GuideCfg>
{
    public int Coin;
    public int Exp;
}
public class StrongCfg : BaseData<StrongCfg>
{
    public int Pos;
    public int StartLevel;
    public int AddHp;
    public int AddHurt;
    public int AddDefense;
    public int MinLevel;
    public int Coin;
    public int Crystal;
}

public class TaskRewardCfg : BaseData<TaskRewardCfg>
{
    public int Count;
    public int Exp;
    public int Coin;
    public int Diamond;
}

public class TaskRewardData : BaseData<TaskRewardData>
{
    public int Prangs;
    public bool Tasked;
}
public class MapCfg : BaseData<MapCfg>
{
    public int Power;
    public int Coin;
    public int Exp;
    public int Crystal;
}
