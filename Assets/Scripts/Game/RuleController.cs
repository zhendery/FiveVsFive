using System.Collections.Generic;
using System.Threading;
namespace FiveVsFive
{
    public class RuleController
    {
        public static RuleController instance = new RuleController();

        ChessBoard board;
        public bool meFisrt, isGaming;
        public GameState isMyTurn;
        public GameRes gameRes;
        public RuleController()
        {
            this.board = ChessBoard.instance;
            meFisrt = false;
            isGaming = false;
        }

        public void reset()
        {
            isGaming = true;
            meFisrt = !meFisrt;
            board.reset();

            isMyTurn = meFisrt ? GameState.MY_TURN : GameState.YOUT_TURN;
            gameRes = GameRes.NO_WIN;
        }
        public void yourTurn()
        {
            GameState oldState = isMyTurn;
            Thread.Sleep(400);

            checkIsWin(oldState);//在夹挑飞完之后判断输赢

            isMyTurn = oldState == GameState.YOUT_TURN ? GameState.MY_TURN : GameState.YOUT_TURN;
        }

        void checkIsWin(GameState whoseTurn)
        {
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

            if (res != GameRes.NO_WIN)
            {
                isMyTurn = GameState.NO_TURN;
                gameRes = res;
            }
        }
    }
    public enum GameRes { NO_WIN, ME_WIN, YOU_WIN };
    public enum GameState { NO_TURN, MY_TURN, YOUT_TURN };
}