/****************************************************
	文件：ServerStart.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/09 10:27   	
	功能：服务器入口
*****************************************************/

using System.Threading;

namespace Server.Root
{
    class ServerStart
    {
        static void Main(string[] args)
        {
            ServerRoot.Instance.Init();//启动服务器
            while (true)//避免服务器直接退出
            {
                //实时处理客户端发出的请求
                ServerRoot.Instance.Update();
                Thread.Sleep(20);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
