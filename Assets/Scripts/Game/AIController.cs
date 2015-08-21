using System;
using System.Threading;
namespace FiveVsFive
{
    class AIController
    {
        LocalServer server;
        ChessBoard board;
        public AIController(LocalServer server)
        {
            board = Global.board;
            this.server = server;
        }

        public void move()
        {
            Random ran = new Random(DateTime.Now.Millisecond);
            int[] myChesses = board.getChesses(false);

            int[] canGo=new int[0];
            do
            {
                board.selected = myChesses[ran.Next(myChesses.Length)];
                canGo = board.getCanGo();
            }
            while (canGo.Length == 0);


            int step = canGo[ran.Next(canGo.Length)];
            board.moveChess(step);

           server.yourTurn() ;//AI走完了
        }
    }
}