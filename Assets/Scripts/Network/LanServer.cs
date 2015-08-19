using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

namespace FiveVsFive
{
    class LanServer : Server  //相当于两个client的中转站，以及负责与ruleController沟通
    {
        Socket client1, client2;
        public override void start()
        {
            new Thread((ThreadStart)
                delegate()//不断发送广播
                {
                    Socket sockCon = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    sockCon.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
#if UNITY_EDITOR
                    sockCon.Bind(new IPEndPoint(IPAddress.Parse(getLanIP()), 13000));
#endif
                    IPEndPoint broad = new IPEndPoint(IPAddress.Broadcast, Const.PORT);

                    byte[] msg = new byte[4];

                    while (!isRunning)//当两个客户端（含自己）都连接成功后，isRunning将变为true，即广播结束
                    {
                        sockCon.SendTo(msg, broad);
                        Thread.Sleep(50);
                    }

                    sockCon.Close();
                    Console.WriteLine("stop sending broadcast");
                }
            ).Start();//开始广播自己

            base.start(getLanIP());//启动tcp连接侦听
        }
        protected override void accepted(IAsyncResult iar)
        {
            sock = (Socket)iar.AsyncState;

            if (client1 == null)//本地客户端
            {
                client1 = sock.EndAccept(iar);
                sock.BeginAccept(new AsyncCallback(accepted), sock);
            }
            else if (client2 == null)//远程客户端
            {
                client2 = sock.EndAccept(iar);
                isRunning = true;

                new Thread((ThreadStart)recieveMsg).Start();
                ruleController.reset();//开始新游戏

                sock.Close();//联机状态中sock充当监听角色，监听完了即释放
                sock = null;
            }
        }
        public override void newTurn(GameState whoseTurn)
        {
            bool meFirst = whoseTurn == GameState.MY_TURN;
            ByteArray msg = new ByteArray();
            msg.write(Const.NEW_TURN);
            msg.write(meFirst);
            sendMsg(client1, msg);
            msg.Close();

            msg = new ByteArray();
            msg.write(Const.NEW_TURN);
            msg.write(!meFirst);
            sendMsg(client2, msg);
            msg.Close();
        }
        protected override void recieveMsg()
        {
            while (isRunning)
            {
                if (client1.Available > 0)
                {
                    byte[] buffer = new byte[client1.Available];
                    client1.Receive(buffer);
                    ByteArray msg = new ByteArray();
                    msg.decode(buffer);
                    handleMsg(msg, client1, client2);
                }
                if (client2.Available > 0)
                {
                    byte[] buffer = new byte[client2.Available];
                    client2.Receive(buffer);
                    ByteArray msg = new ByteArray();
                    msg.decode(buffer);
                    handleMsg(msg, client2, client1);
                }
                Thread.Sleep(50);
            }
            client1.Close();
            client2.Close();
        }
        protected void handleMsg(ByteArray msg, Socket from, Socket to)
        {
            byte action = msg.readByte();
            ByteArray newMsg = new ByteArray();
            switch (action)
            {
                case Const.UP_CHESS:
                    newMsg.write(Const.UP_CHESS);
                    //因为对手的棋盘和我是正好相反的，所以要取”互补数“  这里是index
                    newMsg.write(9 - msg.readInt());
                    sendMsg(to, newMsg);
                    break;
                case Const.MOVE_CHESS:
                    newMsg.write(Const.MOVE_CHESS);
                    //因为对手的棋盘和我是正好相反的，所以要取”互补数“  这里是pos
                    newMsg.write(24 - msg.readInt());
                    sendMsg(to, newMsg);

                    //走完了判断输赢
                    GameRes res = ruleController.yourTurn();
                    newMsg.Close();
                    newMsg = new ByteArray();
                    if (res == GameRes.NO_WIN)//如果没有人赢则向对手发送你走的指令
                    {
                        newMsg.write(Const.YOUR_TURN);
                        sendMsg(to, newMsg);
                    }
                    else//如果有人赢了，则向两位玩家发送谁赢了
                    {
                        newMsg.write(Const.END_GAME);
                        newMsg.write((int)res);//将比赛结果以int的形式发送给两方
                        sendMsg(to, newMsg);
                        sendMsg(from, newMsg);
                    }
                    break;
                case Const.DISCONNECT:
                    sendMsg(to, msg);
                    if (client1 != null)
                        client1.Close();//结束监听进程
                    if (client2 != null)
                        client2.Close();//结束监听进程
                    isRunning = false;//结束所有进程
                    break;
            }

            newMsg.Close();
            msg.Close();
        }
        protected void sendMsg(Socket sock, ByteArray msg)
        {
            byte[] bits = msg.encode();
            if (sock.Connected)
                sock.Send(bits);
            msg.Close();
        }

        public virtual string getLanIP()
        {
#if UNITY_EDITOR
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.Name == "WLAN")//判断是否是无线
                {
                    IPInterfaceProperties ip = adapter.GetIPProperties();     //IP配置信息
                    foreach (UnicastIPAddressInformation add in ip.UnicastAddresses)
                    {
                        if (add.Address.AddressFamily == AddressFamily.InterNetwork)
                            return add.Address.ToString();
                    }
                }
            }
#elif  UNITY_ANDROID
            IPAddress[] adds = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (IPAddress ip in adds)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
#else
            
#endif
            return null;
        }
    }
    class WlanServer : LanServer
    {
        public override void start()
        {
        }


        public override string getLanIP()
        {
            try
            {
                WebRequest wr = WebRequest.Create("http://www.ip138.com/ip2city.asp");
                Stream s = wr.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.Default);
                string all = sr.ReadToEnd(); //读取网站的数据

                int start = all.IndexOf("[") + 1;
                int end = all.IndexOf("]", start);
                string tempip = all.Substring(start, end - start);
                sr.Close();
                s.Close();
                return tempip;
            }
            catch
            {
                return null;
            }
        }

    }
}