using System;
using System.Threading;
namespace FiveVsFive
{
    public class AIController
    {
        public static AIController instance=new AIController();
        ChessBoard board;
        public AIController()
        {
            board = ChessBoard.instance;
        }

        public void move()
        {
            Random ran = new Random(DateTime.Now.Millisecond);
            int[] myChesses = board.getChesses(false);

            board.selected = myChesses[ ran.Next(myChesses.Length)];
            int[] canGo = board.getCanGo();

            int step = canGo[ran.Next(canGo.Length)];
            board.moveChess(step);

            LanClient.instance.yourTurn();//AI走完了
        }
    }
}