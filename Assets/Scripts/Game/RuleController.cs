using System.Collections.Generic;
using System.Threading;
namespace FiveVsFive
{
    class RuleController
    {
        Server server;
        bool meFisrt;

        GameState whoseTurn;

        public RuleController(Server server)
        {
            meFisrt = false;
            this.server = server;
        }

        public void reset()
        {
            meFisrt = !meFisrt;
            whoseTurn = meFisrt ? GameState.MY_TURN : GameState.YOUT_TURN;

            server.newTurn(whoseTurn);
        }
        public GameRes yourTurn()
        {
            GameState oldState = whoseTurn;
            Thread.Sleep(400);

            GameRes gameRes = checkIsWin(oldState);//在一方走完  夹挑飞完之后判断输赢

            if (gameRes == GameRes.NO_WIN)//如果没有人赢，则游戏继续，换成对手的回合
                whoseTurn = oldState == GameState.YOUT_TURN ? GameState.MY_TURN : GameState.YOUT_TURN;

            return gameRes;
        }

        GameRes checkIsWin(GameState whoseTurn)
        {
            ChessBoard board = Global.board;
            GameRes res = GameRes.NO_WIN, resPre = GameRes.NO_WIN;
            bool yourTurn = false;
            if (whoseTurn == GameState.MY_TURN)
            {
                resPre = GameRes.ME_WIN;
                yourTurn = false;
            }
            else if (whoseTurn == GameState.YOUT_TURN)
            {
                resPre = GameRes.YOU_WIN;
                yourTurn = true;
            }

            //判断输赢
            //1、没子的情况
            if (board.getChesses(yourTurn).Length == 0)//yourTurn在我方回合时是false，此时只需判断对方棋子看他是否输
                res = resPre;
            //2、对方无棋可走时
            bool canGo = false;
            foreach (int index in board.getChesses(yourTurn))
            {
                if (board.getCanGo(index).Length > 0)
                {
                    canGo = true;
                    break;
                }
            }
            if (!canGo)
                res = resPre;
            //判断结束，如果有人赢就终止游戏

            return res;
        }
    }
    public enum GameRes { NO_WIN, ME_WIN, YOU_WIN };
    public enum GameState { NO_TURN, MY_TURN, YOUT_TURN };
}