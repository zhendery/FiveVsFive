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
        protected Socket sock, client1;
        protected RuleController ruleController;

        public Server()
        {
            ruleController = new RuleController(this);
        }
        public virtual void start()
        {

        }
        public void start(string ip)
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (ip == null)
                return;
            IPEndPoint endP = new IPEndPoint(IPAddress.Parse(ip), Const.PORT);
            sock.Bind(endP);
            sock.Listen(2);

            sock.BeginAccept(new AsyncCallback(accepted), sock);
        }
        protected virtual void accepted(IAsyncResult iar)
        {
            Socket sock = (Socket)iar.AsyncState;
            client1 = sock.EndAccept(iar);
            isRunning = true;

            new Thread((ThreadStart)recieveMsg).Start();

            ruleController.reset();//开始新游戏

            sock.Close();//联机状态中sock充当监听角色，监听完了即释放
            sock = null;
        }
        protected virtual void recieveMsg()
        {
            while (isRunning)
            {
                if (client1.Available > 0)
                {
                    byte[] buffer = new byte[client1.Available];
                    client1.Receive(buffer);
                    ByteArray msg = new ByteArray();
                    msg.decode(buffer);
                    handleMsg(msg);
                }
                Thread.Sleep(50);
            }
        }
        protected virtual void handleMsg(ByteArray msg) { }
        protected void sendMsg(ByteArray msg)
        {
            byte[] bits = msg.encode();
            client1.Send(bits);
            msg.Close();
        }
        public virtual void newTurn(GameState whoseTurn) { }
    }

    class LocalServer : Server
    {
        AIController ai;
        public LocalServer()
            : base()
        {
            ai = new AIController(this);
        }

        public override void start()
        {
            base.start("127.0.0.1");
        }
        protected override void handleMsg(ByteArray msg)
        {
            byte action = msg.readByte();
            ByteArray newMsg = new ByteArray();
            switch (action)
            {
                case Const.NEW_TURN:
                    ruleController.reset();//开始新游戏
                    break;
                case Const.YOUR_TURN:
                    GameRes res = ruleController.yourTurn();
                    if (res == GameRes.NO_WIN)//如果没有人赢则让ai走
                    {
                        Global.client.whoseTurn = ruleController.whoseTurn;
                        if (ruleController.whoseTurn == GameState.YOUR_TURN)
                            ai.move();
                        else
                        {
                            newMsg.write(Const.YOUR_TURN);
                            sendMsg(newMsg);
                        }
                    }
                    else//如果有人赢了，则告诉玩家发送谁赢了
                    {
                        newMsg.write(Const.END_GAME);
                        newMsg.write((int)res);//将比赛结果以int的形式发送给我
                        sendMsg(newMsg);
                    }
                    break;
                case Const.RETRACT_APPLY://本机收到申请则默认同意~直接回发悔棋指令
                    newMsg.write(Const.RETRACT_CHESS);
                    sendMsg(newMsg);
                    break;
                case Const.END_GAME:
                    newMsg.write(Const.END_GAME);
                    newMsg.write((int)GameRes.YOU_WIN);//发送对方赢的结果
                    sendMsg(newMsg);
                    break;
                case Const.DISCONNECT:
                    if (client1 != null)
                    {
                        client1.Close();
                        client1 = null;
                    }
                    break;
            }
            msg.Close();
        }
        public override void newTurn(GameState whoseTurn)
        {
            bool meFirst = whoseTurn == GameState.MY_TURN;
            ByteArray msg = new ByteArray();
            msg.write(Const.NEW_TURN);
            msg.write(meFirst);
            sendMsg(msg);
            msg.Close();

            if (!meFirst)
            {
                Thread.Sleep(1600);
                ai.move();
            }
        }
    }
}
