﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FiveVsFive
{
    class Server
    {
        protected bool isRunning;
        protected Socket sock;

        public Server()
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void start(string ip)
        {
            IPEndPoint endP = new IPEndPoint(IPAddress.Parse(ip), Const.PORT);
            sock.Bind(endP);
            sock.Listen(1);

            sock.BeginAccept(new AsyncCallback(accepted), sock);
        }
        protected void accepted(IAsyncResult iar)
        {
            Socket client = (Socket)iar.AsyncState;
            sock = client.EndAccept(iar);
            isRunning = true;

            new Thread((ThreadStart)recieveMsg).Start();
        }
        protected void recieveMsg()
        {
            while (isRunning)
            {
                if (sock.Available > 0)
                {
                    byte[] buffer = new byte[sock.Available];
                    sock.Receive(buffer);
                    ByteArray msg = new ByteArray();
                    msg.write(buffer);
                    handleMsg(msg);
                }
                Thread.Sleep(50);
            }
            sock.Close();
        }
        protected virtual void handleMsg(ByteArray msg) { }
        protected void sendMsg(ByteArray msg)
        {
            byte[] bits = msg.encode();
            sock.Send(bits);
        }
    }

    class LocalServer : Server
    {
        protected override void handleMsg(ByteArray msg)
        {
            byte action = msg.readByte();
            ByteArray newMsg = new ByteArray();
            switch (action)
            {
                case Const.NEW_TURN://新一轮游戏
                    newMsg.write(Const.NEW_TURN);
                    sendMsg(msg);
                    break;
                //case Const.UP_CHESS: 不用处理，因为电脑不需要知道你抬起了哪一个棋子
                case Const.MOVE_CHESS:
                    sendMsg(msg);
                    break;
                case Const.YOUR_TURN:
                    sendMsg(msg);
                    Thread.Sleep(500);//让夹挑飞一会
                    if (RuleController.instance.isMyTurn==GameState.YOUT_TURN)
                        AIController.instance.move();
                    break;
            }
        }

    }
}
