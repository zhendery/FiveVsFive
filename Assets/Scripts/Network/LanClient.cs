using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System;
using System.Net.NetworkInformation;

namespace FiveVsFive
{
    public class LanClient
    {
        public LanClient()
        {
            isRunning = false;

            //游戏相关初始化
            whoseTurn = GameState.NO_TURN;
            gameRes = GameRes.NO_WIN;
            isGaming = false;
            myColor = false;
            board = Global.board;

            string name = UnityEngine.PlayerPrefs.GetString(Names.keyName, "");
            int logo = UnityEngine.PlayerPrefs.GetInt(Names.keyLogo, -1);
            if ("".Equals(name) || logo==-1)
            {
                Information info = Names.getRandomName();
                UnityEngine.PlayerPrefs.SetString(Names.keyName, info.name);
                UnityEngine.PlayerPrefs.SetInt(Names.keyLogo, info.logo);
                myLogo = info.logo;
                myName = info.name;
            }
            else
            {
                myLogo = logo;
                myName = name;
            }
            int audio = UnityEngine.PlayerPrefs.GetInt(Names.keyAudio, -1);
            if (audio == -1)
            {
                UnityEngine.PlayerPrefs.SetInt(Names.keyAudio, 1);
                audioOn = true;
            }
            else
                audioOn = audio == 1;
        }

        #region 和游戏内容相关的，包括各种收到来自服务端的Action后应该怎么处理，以及微量数据

        public bool audioOn;
        public int myLogo, yourLogo = -1;
        public string myName, yourName = "";
        public GameState whoseTurn;
        public GameRes gameRes;
        public bool isGaming, myColor;//myColor表示我是黑还是白，黑棋先走
        ChessBoard board;

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
                    bool isMyTurn = msg.readBool();
                    newTurn(isMyTurn);
                    break;
                case Const.UP_CHESS:
                    board.chessUp = msg.readInt();
                    break;
                case Const.MOVE_CHESS:
                    int pos = msg.readInt();
                    board.moveChess(pos);
                    break;
                case Const.YOUR_TURN://就是轮到我了
                    whoseTurn = GameState.MY_TURN;
                    break;
                case Const.END_GAME://有人赢了
                    isGaming = false;
                    whoseTurn = GameState.NO_TURN;
                    gameRes = (GameRes)msg.readInt();//将以int形式发送过来的比赛结果存入gameRes以供检测
                    break;

                case Const.DISCONNECT://有人掉线了，或者有人退出了isRunning = false;
                    if (client != null)
                    {
                        client.Close();
                        client = null;
                    }
                    Global.setSceneOld(GameScenes.WELCOME);//返回欢迎界面
                    isGaming = isRunning = false;
                    break;
            }
            msg.Close();
        }
        void newTurn(bool isMyTurn)
        {
            this.whoseTurn = isMyTurn ? GameState.MY_TURN : GameState.YOUR_TURN;
            this.gameRes = GameRes.NO_WIN;
            myColor = isMyTurn;
            board.reset();
            isGaming = true;
        }

        public void again()
        {
            ByteArray msg = new ByteArray();
            msg.write(Const.NEW_TURN);
            sendMsg(msg);
        }
        public void upChess(int index)
        {
            board.chessUp = index;
            ByteArray msg = new ByteArray();
            msg.write(Const.UP_CHESS);
            msg.write(index);
            sendMsg(msg);
        }
        public void move(int pos)
        {
            //走完便自封，直到server发来消息解封
            whoseTurn = GameState.NO_TURN;
            board.moveChess(pos);

            ByteArray msg = new ByteArray();
            msg.write(Const.MOVE_CHESS);
            msg.write(pos);
            sendMsg(msg);
        }
        public void yourTurn()
        {
            if (whoseTurn == GameState.NO_TURN)
            {
                whoseTurn = GameState.YOUR_TURN;
                Thread.Sleep(1000);
                ByteArray msg = new ByteArray();
                msg.write(Const.YOUR_TURN);
                sendMsg(msg);
            }
        }
        #endregion

        #region “网络”相关都在这里，想折起来，和游戏相关的分开
        public bool isRunning;
        Socket client;

        public void startLocal()//纯本地，与lcoalServer通讯
        {
            start("127.0.0.1");
        }
        //public void startServer(bool wlan)//server端的客户端，与自己的ip通讯
        //{
        //    if (wlan)
        //        start(IPManager.getWlanIP());
        //    else
        //        start(IPManager.getLanIP());
        //}
        public void startServer()//server端的客户端，与自己的ip通讯
        {
            start(IPManager.getLanIP());
        }
        public void startLan()//局域网远程端的客户端，首先检测正在广播的服务端，然后根据其ip进行连接
        {
            StateObject state = new StateObject();
            state.sock.BeginReceiveFrom(state.buffer, 0, state.buffer.Length, SocketFlags.None,
                ref state.senderRemote, new AsyncCallback(
                    delegate(IAsyncResult iar)//检测到正在广播的服务端
                    {
                        state = (StateObject)iar.AsyncState;
                        try
                        {
                            state.sock.EndReceiveFrom(iar, ref state.senderRemote);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            return;
                        }
                        state.sock.Close();//关闭检测广播的sock

                        IPAddress ipAdd = ((IPEndPoint)state.senderRemote).Address;//取出服务端的ip,进行连接
                        start(ipAdd.ToString());
                    }
            ), state);
        }

        public void startWlan(string ip)//广域网的远程客户端
        {
            start(ip);
        }
        void start(string ip)//客户端与服务器连接的入口，所有start均从此入口进入
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endP = new IPEndPoint(IPAddress.Parse(ip), Const.PORT);
            client.BeginConnect(endP, new AsyncCallback(connected), client);
        }
        void sendMsg(ByteArray msg)
        {
            byte[] bits = msg.encode();
            client.Send(bits);
            msg.Close();
        }
        public void close()
        {
            ByteArray msg = new ByteArray();
            msg.write(Const.DISCONNECT);
            sendMsg(msg);
            if (client != null)
            {
                client.Close();
                client = null;
            }
            isGaming = isRunning = false;
        }
        protected void connected(IAsyncResult iar)
        {
            client = (Socket)iar.AsyncState;
            try
            {
                client.EndConnect(iar);
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
                if (client.Available > 0)
                {
                    byte[] buffer = new byte[client.Available];
                    client.Receive(buffer);
                    ByteArray msg = new ByteArray();
                    msg.decode(buffer);
                    handleMsg(msg);
                }
                Thread.Sleep(50);
            }
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        #endregion

    }


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
}