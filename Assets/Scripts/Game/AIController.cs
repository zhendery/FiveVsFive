using System;
using System.Threading;
namespace FiveVsFive
{
    class AIController
    {
        LocalServer server;
        public AIController(LocalServer server)
        {
            this.server = server;
            Information info=Names.getRandomName();
            Global.client.yourName = info.name;
            Global.client.yourLogo = info.logo;
        }

        public void move()
        {
            ChessBoard board = Global.board;
            //此处应为ai
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


            Global.client.whoseTurn = GameState.NO_TURN;
            board.moveChess(step);
        }
    }
}