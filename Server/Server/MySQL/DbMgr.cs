/****************************************************
	文件：DbMgr.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/11 11:00   	
	功能：数据库管理层
*****************************************************/
using System;
using MySql.Data.MySqlClient;
using PEProtocol;
using Server.Net;

namespace Server.MySQL
{
    class DbMgr
    {
        private static DbMgr _instance = null;
        public static DbMgr Instance => _instance ?? (_instance = new DbMgr());
        private MySqlConnection connection;//数据库连接

        public void Init()
        {
            PeRoot.Log("DbMgr Init Done.||数据库管理层.");
            connection = new MySqlConnection(
                "server=localhost;port = 3306;User = root;password=1329524041;Database=mysqldrakgod;");
            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                PeRoot.Log(e.ToString(), LogType.LogError);
            }
        }

        #region 玩家上线
        public PlayerData QueryPlayerData(string acct, string pass)
        {
            bool isNew = true;
            PlayerData playerData = null;
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand command = new MySqlCommand("select * from drakgod where acct=@acct", connection);
                command.Parameters.AddWithValue("acct", acct);
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isNew = false;
                    string _pass = reader.GetString("pass");
                    if (_pass.Equals(pass))
                    {
                        ////密码正确，返回玩家数据
                        playerData = new PlayerData
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Level = reader.GetInt32("level"),
                            Exp = reader.GetInt32("exp"),
                            Power = reader.GetInt32("power"),
                            Vip = reader.GetInt32("vip"),

                            Coin = reader.GetInt32("coin"),
                            Diamond = reader.GetInt32("diamond"),
                            Crystal = reader.GetInt32("crystal"),

                            Hp = reader.GetInt32("hp"),
                            Ad = reader.GetInt32("ad"),
                            Ap = reader.GetInt32("ap"),
                            AdDefense = reader.GetInt32("addefense"),
                            ApDefense = reader.GetInt32("apdefense"),
                            Dodge = reader.GetInt32("dodge"),
                            Pierce = reader.GetInt32("pierce"),
                            Critical = reader.GetInt32("critical"),

                            GuideId = reader.GetInt32("guideid"),
                            Time = reader.GetInt64("time"),
                            Battle=reader.GetInt32("battle"),
                            //TODO
                        };

                        #region Strong
                        string[] strongArray = reader.GetString("strong").Split('#');
                        int[] strongArr = new int[6];
                        for (int i = 0; i < strongArray.Length; i++)
                        {
                            if (strongArray[i] == "")
                            {
                                continue;
                            }
                            if (int.TryParse(strongArray[i], out int starLevel))
                            {
                                strongArr[i] = starLevel;
                            }
                            else
                            {
                                PeRoot.Log("Parse Strong Data Error", LogType.LogError);
                            }
                        }
                        playerData.StrongArray = strongArr;
                        #endregion

                        #region Task
                        string[] taskArray = reader.GetString("task").Split('#');
                        playerData.TaskArray = new string[6];
                        for (int i = 0; i < taskArray.Length; i++)
                        {
                            if (taskArray[i]=="")
                            {
                                continue;
                            }
                            else if (taskArray[i].Length>=5)
                            {
                                playerData.TaskArray[i] = taskArray[i];
                            }
                            else
                            {
                                throw new Exception("DataError.");
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                PeRoot.Log(e.ToString(), LogType.LogError);
            }
            finally
            {
                reader?.Close();
                if (isNew)
                {
                    //不存在账号数据，创建新的默认账号数据，并返回
                    playerData = new PlayerData
                    {
                        Id = -1,
                        Name = "",
                        Level = 1,
                        Exp = 0,
                        Power = 150,
                        Vip = 0,

                        Coin = 5000,
                        Diamond = 500,
                        Crystal = 500,

                        Hp = 2000,
                        Ad = 100,
                        Ap = 100,
                        AdDefense = 40,
                        ApDefense = 40,
                        Dodge = 5,//闪避概率
                        Pierce = 5,//穿透比率
                        Critical = 5,//暴击概率
                        GuideId = 1001,
                        StrongArray = new int[6],
                        Time = TimeSvc.Instance.GetNowTime(),
                        TaskArray=new string[6],
                        Battle=10001,
                        //TODO
                    };
                    for (int i = 0; i < playerData.TaskArray.Length; i++)
                    {
                        playerData.TaskArray[i] = (i + 1) + "|0|0";
                    }
                    playerData.Id = InsertNewAcctData(acct, pass, playerData);
                }
            }
            return playerData;
        }
        #endregion

        #region 创建新的玩家游戏数据
        /// <summary>
        /// 创建新的玩家游戏数据
        /// </summary>
        public int InsertNewAcctData(string acct, string pass, PlayerData playerData)
        {
            int id = -1;
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                    "insert into drakgod set acct=@acct,pass =@pass,name=@name,level=@level,exp=@exp,power=@power,vip=@vip,coin=@coin,diamond=@diamond,crystal=@crystal,hp = @hp, ad = @ad, ap = @ap," +
                    " addefense = @addefense, apdefense = @apdefense, dodge = @dodge, pierce = @pierce, critical = @critical," +
                    "guideid=@guideid,strong=@strong,time=@time,task=@task,battle=@battle", connection);
                cmd.Parameters.AddWithValue("acct", acct);
                cmd.Parameters.AddWithValue("pass", pass);
                cmd.Parameters.AddWithValue("name", playerData.Name);
                cmd.Parameters.AddWithValue("level", playerData.Level);
                cmd.Parameters.AddWithValue("exp", playerData.Exp);
                cmd.Parameters.AddWithValue("power", playerData.Power);
                cmd.Parameters.AddWithValue("vip", playerData.Vip);

                cmd.Parameters.AddWithValue("coin", playerData.Coin);
                cmd.Parameters.AddWithValue("diamond", playerData.Diamond);
                cmd.Parameters.AddWithValue("crystal", playerData.Crystal);

                cmd.Parameters.AddWithValue("hp", playerData.Hp);
                cmd.Parameters.AddWithValue("ad", playerData.Ad);
                cmd.Parameters.AddWithValue("ap", playerData.Ap);
                cmd.Parameters.AddWithValue("addefense", playerData.AdDefense);
                cmd.Parameters.AddWithValue("apdefense", playerData.ApDefense);
                cmd.Parameters.AddWithValue("dodge", playerData.Dodge);
                cmd.Parameters.AddWithValue("pierce", playerData.Pierce);
                cmd.Parameters.AddWithValue("critical", playerData.Critical);

                cmd.Parameters.AddWithValue("guideid", playerData.GuideId);

                string strongInfo = "";
                foreach (var t in playerData.StrongArray)
                {
                    strongInfo += t;
                    strongInfo += "#";
                }
                cmd.Parameters.AddWithValue("strong", strongInfo);
                cmd.Parameters.AddWithValue("time", playerData.Time);

                string taskInfo = "";
                foreach (var t in playerData.TaskArray)
                {
                    taskInfo += t;
                    taskInfo += "#";
                }
                cmd.Parameters.AddWithValue("task", taskInfo);
                cmd.Parameters.AddWithValue("battle", playerData.Battle);
                //TODO
                cmd.ExecuteNonQuery();
                id = (int)cmd.LastInsertedId;
            }
            catch (Exception e)
            {
                PeRoot.Log(e.ToString(), LogType.LogError);
            }
            return id;

        }
        #endregion

        #region 更新玩家游戏数据
        /// <summary>
        /// 更新玩家游戏数据
        /// </summary>
        public bool UpDatePlayerData(int id, PlayerData playerData)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                    "update drakgod set name=@name,level=@level,exp=@exp,power=@power,vip=@vip,coin=@coin,diamond=@diamond,crystal=@crystal,hp = @hp, ad = @ad, ap = @ap, " +
                    "addefense = @addefense, apdefense = @apdefense, dodge = @dodge, pierce = @pierce, critical = @critical," +
                    "guideid=@guideid, strong=@strong,time=@time,task=@task,battle=@battle where id =@id", connection);
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("name", playerData.Name);
                cmd.Parameters.AddWithValue("level", playerData.Level);
                cmd.Parameters.AddWithValue("exp", playerData.Exp);
                cmd.Parameters.AddWithValue("power", playerData.Power);
                cmd.Parameters.AddWithValue("vip", playerData.Vip);

                cmd.Parameters.AddWithValue("coin", playerData.Coin);
                cmd.Parameters.AddWithValue("diamond", playerData.Diamond);
                cmd.Parameters.AddWithValue("crystal", playerData.Crystal);

                cmd.Parameters.AddWithValue("hp", playerData.Hp);
                cmd.Parameters.AddWithValue("ad", playerData.Ad);
                cmd.Parameters.AddWithValue("ap", playerData.Ap);
                cmd.Parameters.AddWithValue("addefense", playerData.AdDefense);
                cmd.Parameters.AddWithValue("apdefense", playerData.ApDefense);
                cmd.Parameters.AddWithValue("dodge", playerData.Dodge);
                cmd.Parameters.AddWithValue("pierce", playerData.Pierce);
                cmd.Parameters.AddWithValue("critical", playerData.Critical);

                cmd.Parameters.AddWithValue("guideid", playerData.GuideId);

                string strongInfo = "";
                foreach (var t in playerData.StrongArray)
                {
                    strongInfo += t;
                    strongInfo += "#";
                }
                cmd.Parameters.AddWithValue("strong", strongInfo);
                cmd.Parameters.AddWithValue("time", playerData.Time);

                string taskInfo = "";
                foreach (var t in playerData.TaskArray)
                {
                    taskInfo += t;
                    taskInfo += "#";
                }
                cmd.Parameters.AddWithValue("task", taskInfo);
                cmd.Parameters.AddWithValue("battle", playerData.Battle);
                //TODO
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                PeRoot.Log("Update PlayerData Error:" + e, LogType.LogError);
            }
            return true;
        }
        #endregion

        #region 是否存在名字
        public bool QueryNameData(string name)
        {
            bool exist = false;
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from drakgod where name= @name", connection);
                cmd.Parameters.AddWithValue("name", name);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    exist = true;
                }
            }
            catch (Exception e)
            {
                PeRoot.Log("Query Name State Error:" + e, LogType.LogError);
            }
            finally
            {
                reader?.Close();
            }
            return exist;
        }
        #endregion

    }
}