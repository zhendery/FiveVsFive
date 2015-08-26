using System;
using System.Collections.Generic;
using System.Threading;
namespace FiveVsFive
{
    class AIController
    {
        LocalServer server;
        public AIController(LocalServer server)
        {
            this.server = server;
            Information info = Names.getRandomName();
            Global.client.yourName = info.name;
            Global.client.yourLogo = info.logo;
        }

        public void move()
        {
            ChessBoard board = Global.board.copyBoard(true);//这样AI的棋子就是true了

            int yourCount = board.getCount(false);
            ShouldGo shouldGo = new ShouldGo();

            if (yourCount > 3)
            {
                foreach (int index in board.getChesses(true))//对我方的每一颗棋
                {
                    foreach (int pos in board.getCanGo(index))
                    {
                        ChessBoard newBoard = board.copyBoard(false);
                        newBoard.moveChess(index, pos % 5, pos / 5);//假走
                        newBoard.check3(index);//检测夹挑

                        int yourCountNow = newBoard.getCount(false);
                        shouldGo.setGravity(index, pos, yourCount - yourCountNow);//将夹挑完对方棋子的减量存入
                    }
                }
            }
            else
            {
                switch (yourCount)
                {
                    case 3:

                        break;
                    case 2:

                        break;
                    case 1:

                        break;
                }
            }
            int[] res = shouldGo.getMax();
            if (res == null)
                res = getRandomGo();

            Global.board.selected = res[0];

            Global.client.whoseTurn = GameState.NO_TURN;
            Global.board.moveChess(res[1]);
        }

        int[] getRandomGo()
        {
            int[] res = new int[2];
            ChessBoard board = Global.board;
            Random ran = new Random(DateTime.Now.Millisecond);
            int[] myChesses = board.getChesses(false);
            int[] canGo = new int[0];
            do
            {
                res[0] = myChesses[ran.Next(myChesses.Length)];
                canGo = board.getCanGo();
            }
            while (canGo.Length == 0);
            res[1] = canGo[ran.Next(canGo.Length)];

            return res;
        }

    }

    class ShouldGo
    {
        Dictionary<int, Dictionary<int, int>> shouldGo = new Dictionary<int, Dictionary<int, int>>();//棋子的索引，棋子应该走的权重
        public void setGravity(int index, int pos, int gravity)
        {
            if (!shouldGo.ContainsKey(index))
                shouldGo.Add(index, new Dictionary<int, int>());

            if (!shouldGo[index].ContainsKey(pos))
                shouldGo[index].Add(pos, gravity);
            else
                shouldGo[index][pos] = gravity;
        }

        public int[] getMax()
        {
            int[] max = new int[2];//0表示index, 1表示pos
            max[0] = -1;
            int maxGravity = -100;
            foreach (var s in shouldGo)
            {
                foreach (var item in s.Value)
                {
                    if (item.Value > maxGravity)
                    {
                        max[0] = s.Key;
                        max[1] = item.Key;
                        maxGravity = item.Value;
                    }
                }
            }
            return max[0] == -1 ? null : max;
        }
    }
}