using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System;

namespace FiveVsFive
{
    public class LanClient
    {
        public bool isRunning;
        Socket sock;
        public static LanClient instance = new LanClient();
        public LanClient()
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            isRunning = false;
        }

        public void startLocal()//纯本地
        {
            start("127.0.0.1");
        }
        public void startWithIP(string ip)//含服务端的本地，或者手动输入ip
        {
            start(ip);
        }

        public int myLogo=1, yourLogo;
        public string myName="zhendery", yourName;
        public void startLan()//局域网客户端，首先开始广播自己
        {
            new Thread((ThreadStart)sendBroadcast).Start();
        }

        void sendBroadcast()
        {
            Socket sockCon = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sockCon.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            IPEndPoint broad = new IPEndPoint(IPAddress.Broadcast, Const.PORT);

            ByteArray msg = new ByteArray();
            msg.write(myLogo);//头像索引
            msg.write(myName);//名字
            byte[] msgBits = msg.encode();

            bool state = false;
            while (!state)
            {
                sockCon.SendTo(msgBits, broad);
                Thread.Sleep(50);
                if (sockCon.Available > 0)//边广播边接收，收到对方消息
                {
                    //解析头像等 与 ip
                    ByteArray msgRec = new ByteArray();
                    byte[] recBits = new byte[sockCon.Available];
                    msgRec.decode(recBits);
                    yourLogo = msgRec.readInt();
                    yourName = msgRec.readString();

                    startWithIP(msgRec.readString());
                    msgRec.Close();

                    state = true;
                }
            }
            sockCon.Close();
            sockCon = null;
        }
        void start(string ip)
        {
            IPEndPoint endP = new IPEndPoint(IPAddress.Parse(ip), Const.PORT);
            sock.BeginConnect(endP, new AsyncCallback(connected), sock);
        }
        protected void connected(IAsyncResult iar)
        {
            sock = (Socket)iar.AsyncState;
            try
            {
                sock.EndConnect(iar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            isRunning = true;

            newTurn();
            new Thread((ThreadStart)recieveMsg).Start();
        }
        void recieveMsg()
        {
            while (isRunning)
            {
                if (sock.Available > 0)
                {
                    byte[] buffer = new byte[sock.Available];
                    sock.Receive(buffer);
                    ByteArray msg = new ByteArray();
                    msg.decode(buffer);
                    handleMsg(msg);
                }
                Thread.Sleep(50);
            }
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }
        void tryConnect()
        {
            try
            {
                Socket sockConnect = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sockConnect.Bind(new IPEndPoint(IPAddress.Any, Const.PORT));
                byte[] rec = new byte[Const.MAX_MSG_LEN];
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint senderRemote = (EndPoint)sender;
                sockConnect.ReceiveFrom(rec, ref senderRemote);

                sockConnect.Close();
                sockConnect = null;

                //sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //sock.Connect(new IPEndPoint(((IPEndPoint)senderRemote).Address, Const.PORT));

                //Chess10.getInstance().reset();
                // recTH = new Thread((ThreadStart)recieveMessage);
                //recTH.Start();

                // state = true;

            }
            catch { }
        }
        void handleMsg(ByteArray msg)
        {
            ByteArray newMsg = new ByteArray();
            byte action = msg.readByte();
            switch (action)
            {
                case Const.CONNECT:
                    newMsg.write(Const.CONNECT);
                    newMsg.write(myLogo);
                    newMsg.write(myName);
                    sendMsg(newMsg);
                    break;
                case Const.NEW_TURN:
                    RuleController.instance.reset();
                    break;
                case Const.MOVE_CHESS:
                    int pos = msg.readInt();
                    ChessBoard.instance.moveChess(pos);
                    break;
                case Const.YOUR_TURN:
                    RuleController.instance.yourTurn();
                    break;
            }
            msg.Close();
        }

        public void newTurn()
        {
            ByteArray msg = new ByteArray();
            msg.write(Const.NEW_TURN);
            sendMsg(msg);
        }

        public void yourTurn()
        {
            ByteArray msg = new ByteArray();
            msg.write(Const.YOUR_TURN);
            sendMsg(msg);
        }
        public void upChess(int index)
        {
            ByteArray msg = new ByteArray();
            msg.write(Const.UP_CHESS);
            msg.write(index);
            sendMsg(msg);
        }
        public void move(int pos)
        {
            ByteArray msg = new ByteArray();
            msg.write(Const.MOVE_CHESS);
            msg.write(pos);
            sendMsg(msg);
        }
        void sendMsg(ByteArray msg)
        {
            byte[] bits = msg.encode();
            sock.Send(bits);
            msg.Close();
        }
        public void changeOner(int[] indexs)
        {
            // foreach (int index in indexs)
            //    chessBoard.changeChessOwner(index);
            //sendMessage(Const.OVER_CHESS, indexs);
        }
    }
}