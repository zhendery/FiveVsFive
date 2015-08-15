using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

namespace FiveVsFive
{
    public class LanServer : LanManager<LanServer>
    {
        public void startServer()
        {
            state = false;

            if (myIp == null)
                getMyIp();
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(new IPEndPoint(myIp, Const.PORT));
            sock.Listen(1);
            sock.BeginAccept(SocketConnected, sock);

            connectTH = new Thread((ThreadStart)tcpListen);
            connectTH.Start();

        }

        RuleController rule;
        void SocketConnected(IAsyncResult ar)
        {
            sock = (ar.AsyncState as Socket).EndAccept(ar);
            //LocalServer.Instance = this;
            //rule = new RuleController();
            state = true;
        }

        void tcpListen()
        {
            Socket sockCon = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sockCon.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            IPEndPoint broad = new IPEndPoint(IPAddress.Broadcast, Const.PORT);
            byte[] conn = Encoding.ASCII.GetBytes("connect");

            while (!state)
            {
                sockCon.SendTo(conn, broad);
                Thread.Sleep(50);
            }
            sockCon.Close();
            sockCon = null;

            rule.newTurn();
            recTH = new Thread((ThreadStart)recieveMessage);
            recTH.Start();
        }

        protected override void handleMessage(byte action, int[] indexs)
        {
            switch (action)
            {
                case Const.UP_CHESS:
                    //rule.upChess(9 - indexs[0]);
                    break;
                case Const.MOVE_CHESS:
//                    rule.moveChess(24 - indexs[0], false);
                    break;
                case Const.YOUR_TURN:
                    //Chess10.getInstance().isMyTurn = true;
                    break;
                case Const.OVER_CHESS:
                    foreach (int index in indexs)
                     ;//   rule.changeOwner(9-index);
                    break;
            }
        }
        public void yourTurn()
        {
            sendMessage(Const.YOUR_TURN);
        }
        public void upChess(int index)
        {
            //rule.upChess(index);
        }
        public void showTips(int[] indexs)
        {
            sendMessage(Const.UP_CHESS, indexs);
        }
        public void move(int pos)
        {
            //rule.moveChess(pos, true);
        }
        public void move(int index,int pos)
        {
            sendMessage(Const.MOVE_CHESS, new int[]{index,pos});
        }
        public void moveEnd() {
            sendMessage(Const.MOVE_END);
        }
        public void changeOner(int[] indexs)
        {
            foreach (int index in indexs)
                ;// rule.changeOwner(index);
            sendMessage(Const.OVER_CHESS, indexs);
        }

        public string getWlanIP()
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

    public class LanManager<T> where T : new()
    {
        private static T lanManager;
        public static T getInstance()
        {
            if (lanManager == null)
                lanManager = new T();
            return lanManager;
        }

        protected Socket sock;
        protected bool state;
        public bool State { get { return state; } }
        protected Thread connectTH, recTH;

        protected IPAddress myIp;
        protected void getMyIp()
        {
            IPAddress[] adds = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (IPAddress ip in adds)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    myIp = ip;
        }

        public void sendMessage(byte action)
        {
            sendMessage(action, 0);
        }
        public void sendMessage(byte action, int index)//index or pos
        {
            sendMessage(action, new int[] {index});
        }
        public void sendMessage(byte action, int[] indexs)
        {
            byte[] msg = new byte[indexs.Length * 4 + 5];
            msg[0] = action;
            Array.Copy(BitConverter.GetBytes(indexs.Length), 0, msg, 1, 4);//第一位表示长度
            for (int i = 1; i < indexs.Length + 1; ++i)
                Array.Copy(BitConverter.GetBytes(indexs[i-1]), 0, msg, i * 4 + 1, 4);
            //封装加密
            sock.Send(msg);

        }

        protected virtual void handleMessage(byte action, int[] indexs) { }
        public void recieveMessage()
        {
            while (true)
            {
                if (sock != null && sock.Available > 0)//有消息
                {
                    byte[] rec = new byte[Const.MAX_MSG_LEN];
                    sock.Receive(rec);
                    //解密验证 解封
                    int len = BitConverter.ToInt32(rec, 1);
                    int[] content = new int[len];
                    for (int i = 1; i < len + 1; ++i)
                        content[i - 1] = BitConverter.ToInt32(rec, i * 4 + 1);
                    this.handleMessage(rec[0], content);
                }
                Thread.Sleep(50);
            }
        }

        protected string getEecodeIp(string ip)
        {
            StringBuilder sb = new StringBuilder();
            string[] ipAdd = ip.Split('.');
            for (int i = 0; i < 4; ++i)
            {
                int num = Int32.Parse(ipAdd[i]);
                char c1 = (char)(num / 128);
                char c2 = (char)(num % 128);
                sb.Append(c1).Append(c2);
            }
            return sb.ToString();
        }
        protected string getDecodeIp(string ip)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 4; ++i)
            {
                int num = ip[i * 2] * 128 + ip[i * 2 + 1];
                sb.Append(num).Append('.');
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        public void stopServer()
        {
            connectTH.Abort();
            recTH.Abort();
            sock.Close();
            connectTH = null;
            recTH = null;
            sock = null;
        }
        ~LanManager()
        {
            stopServer();
        }
    }
}