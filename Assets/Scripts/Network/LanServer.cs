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
    class StateObject
    {
        public byte[] buffer;
        public EndPoint senderRemote;
        public Socket sock;

        public StateObject()
        {
            buffer = new byte[Const.MAX_MSG_LEN];
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            senderRemote = (EndPoint)sender;

            sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.Bind(new IPEndPoint(IPAddress.Any, Const.PORT));
        }
    }
    class LanServer : Server  //相当于两个client的中转站，以及负责与ruleController沟通
    {
        protected int myLogo;
        protected string myName;
        public void start()
        {
            base.start(getLanIP());//先连上本地client(local)

            StateObject state = new StateObject();//然后开始检测是否有其他客户端（正在广播）
            state.sock.BeginReceiveFrom(state.buffer, 0, state.buffer.Length, SocketFlags.Broadcast,
                ref state.senderRemote, new AsyncCallback(recieveBroadcast), state);
        }
        private void recieveBroadcast(IAsyncResult iar)//检测到正在广播的客户端
        {
            StateObject state = (StateObject)iar.AsyncState;
            state.sock.EndReceiveFrom(iar, ref state.senderRemote);

            ByteArray msg = new ByteArray();
            msg.decode(state.buffer);
            int logoI=msg.readInt();
            string name = msg.readString();//解析出其头像与昵称

            ByteArray newMsg = new ByteArray();
            msg.write(myLogo);
            msg.write(myName);
            msg.write(this.getLanIP());
            state.sock.Send(newMsg.encode());

            state.sock.Close();//关闭广播sock
        }
        protected Socket local=null;
        protected override void accepted(IAsyncResult iar)
        {
            Socket client = (Socket)iar.AsyncState;

            if (local == null)
            {
                local = client.EndAccept(iar);
                client.BeginAccept(new AsyncCallback(accepted), client);
            }
            else if (sock == null)
            {
                sock = client.EndAccept(iar);
                isRunning = true;
                new Thread((ThreadStart)recieveMsg).Start();
            }
        }
        protected override void recieveMsg()
        {
            while (isRunning)
            {
                if (local.Available > 0)
                {
                    byte[] buffer = new byte[local.Available];
                    local.Receive(buffer);
                    ByteArray msg = new ByteArray();
                    msg.decode(buffer);
                    handleMsg(msg,local,sock);
                }
                if (sock.Available > 0)
                {
                    byte[] buffer = new byte[sock.Available];
                    sock.Receive(buffer);
                    ByteArray msg = new ByteArray();
                    msg.decode(buffer);
                    handleMsg(msg,sock,local);
                }
                Thread.Sleep(50);
            }
            sock.Close();
            local.Close();
        }
        protected void handleMsg(ByteArray msg,Socket from,Socket to)
        {
            byte action = msg.readByte();
            ByteArray newMsg = new ByteArray();
            switch (action)
            {
                case Const.CONNECT:
                    if(from==local )
                    {
                        myLogo = msg.readInt();
                        myName = msg.readString();
                    }
                    break;
            }
            msg.Close();
        }
        public void yourTurn()
        {

        }
        public void upChess(int index)
        {

        }

        public virtual string getLanIP()
        {
            IPAddress[] adds = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (IPAddress ip in adds)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            return null;
        }
    }
    class WlanServer : LanServer
    {
        public void start()
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