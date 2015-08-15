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
        bool isRunning;
        Socket sock;
        public static LanClient instance = new LanClient();
        public LanClient()
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            isRunning = false;
        }
        public void start(string ip)
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
                    msg.write(buffer);
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
            byte action = msg.readByte();
            switch (action)
            {
                case Const.NEW_TURN:
                    RuleController.instance.newTurn();
                    break;
                case Const.MOVE_CHESS:
                    int pos = msg.readInt();
                    ChessBoard.instance.moveChess(pos);
                    break;
            }
        }

        public void newTurn()
        {
            ByteArray msg = new ByteArray();
            msg.write(Const.NEW_TURN);
            sendMsg(msg);
        }

        public void yourTurn()
        {
            //sendMessage(Const.YOUR_TURN);
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
        public void moveEnd()
        {
            yourTurn();
        }
        void sendMsg(ByteArray msg)
        {
            byte[] bits = msg.encode();
            sock.Send(bits);
        }
        public void changeOner(int[] indexs)
        {
            // foreach (int index in indexs)
            //    chessBoard.changeChessOwner(index);
            //sendMessage(Const.OVER_CHESS, indexs);
        }
    }
}