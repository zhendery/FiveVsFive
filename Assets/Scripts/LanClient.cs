using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace FiveVsFive
{
    public class LanClient : LanManager<LanClient>, IHost //半个rule
    {
        public void startClient()//局域网
        {
            state = false;
            LocalServer.Instance = this;
            connectTH = new Thread((ThreadStart)tryConnect);
            connectTH.Start();
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

                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(new IPEndPoint(((IPEndPoint)senderRemote).Address, Const.PORT));

                Chess10.getInstance().reset();
                recTH = new Thread((ThreadStart)recieveMessage);
                recTH.Start();

                state = true;

            }
            catch { }
        }
        protected override void handleMessage(byte action, int[] indexs)
        {
            switch (action)
            {
                case Const.UP_CHESS:
                    List<int> canGoList = Chess10.getInstance().showIndexs;
                    canGoList.Clear();
                    if (indexs[0] == -1)
                        canGoList.Add(-1);
                    else
                    {
                        foreach (int index in indexs)
                            canGoList.Add(24 - index);
                        canGoList[0] -= 15;
                    }
                    Chess10.getInstance().upChess();
                    break;
                case Const.MOVE_CHESS:
                    int lastSelected = 9 - indexs[0], pos = 24 - indexs[1];
                    int y = pos / 5, x = pos % 5;
                    Chess10.getInstance().moveChess(lastSelected,x,y);
                    break;
                case Const.MOVE_END:
                    moveEnd();
                    break;
                case Const.YOUR_TURN:
                    Chess10.getInstance().isMyTurn = true;
                    break;
                case Const.OVER_CHESS:
                    foreach (int index in indexs)
                        Chess10.getInstance().changeChessOwner(9 - index);
                    break;
            }
        }

        public void yourTurn()
        {
            sendMessage(Const.YOUR_TURN);
        }
        public void upChess(int index)
        {
            sendMessage(Const.UP_CHESS, index);
        }
        public void showTips(int[] indexs)
        {
            sendMessage(Const.UP_CHESS, indexs);
        }
        public void move(int pos)
        {
            Chess10.getInstance().isMyTurn = false;
            sendMessage(Const.MOVE_CHESS, pos);
        }
        public void move(int index, int pos) { }
        public void moveEnd()
        {
            yourTurn();
        }
        public void changeOner(int[] indexs)
        {
            // foreach (int index in indexs)
            //    chessBoard.changeChessOwner(index);
            sendMessage(Const.OVER_CHESS, indexs);
        }
    }
}