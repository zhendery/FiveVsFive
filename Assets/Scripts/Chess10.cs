using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace FiveVsFive
{
    public class Chess
    {
        public int x, y;
        public bool isMine;
        public Chess(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Chess(Chess c)
        {
            this.x = c.x;
            this.y = c.y;
        }
        public Chess(int x, int y, bool isMine)
        {
            this.x = x;
            this.y = y;
            this.isMine = isMine;
        }
        public static Chess operator +(Chess a, Chess b)
        {
            return new Chess(a.x + b.x, a.y + b.y, a.isMine);
        }
        public static Chess operator *(Chess a, int aspet)
        {
            return new Chess(a.x * aspet, a.y * aspet, a.isMine);
        }
        public bool outBoard()
        {
            return this.x > 4 || this.x < 0 || this.y < 0 || this.y > 4;
        }
    }
    public class Chess10
    {
        static Chess10 chess10;
        public static Chess10 getInstance()
        {
            if (chess10 == null)
                chess10 = new Chess10();
            return chess10;
        }

        Chess[] chesses;
        public Chess10()
        {
            chesses = new Chess[10];
            showIndexs = new List<int>();
        }

        public void reset()
        {
            isMyTurn = false;

            int index = 0;
            int i = 0;
            for (i = 0; i < 5; ++i)
                chesses[index++] = new Chess(i, 0, true);
            for (i = 0; i < 5; ++i)
                chesses[index++] = new Chess(i, 4, false);
            selectedIndex = -1;
            showIndexs.Clear();

            ChessController.Action = ChessController.ChessAction.RESET;
        }

        public Chess getChess(int index)
        {
            if (index > -1)
                return chesses[index];
            else
                return null;
        }

        public bool isMyTurn { get; set; }

        public Chess[] getChesses(bool my)
        {
            List<Chess> temp = new List<Chess>();
            foreach (Chess c in chesses)
                if (c.isMine == my)
                    temp.Add(c);

            return temp.ToArray();
        }

        public int getCount(bool my)
        {
            int count = 0;
            foreach (Chess c in chesses)
                if (c.isMine == my)
                    count++;
            return count;
        }

        public int selectedIndex;
        public List<int> showIndexs;
        public void upChess()
        {
            selectedIndex = showIndexs[0];
            showIndexs.RemoveAt(0);
            ChessController.Action = ChessController.ChessAction.UP_CHESS;
        }

        public void moveChess(int index,int x, int y)
        {
            chesses[index].x = x;
            chesses[index].y = y;

            showIndexs.Clear();
            selectedIndex = -1;
            ChessController.Action = ChessController.ChessAction.REFRESH;

            //Thread.Sleep(1000);//动棋时间
        }

        public void changeChessOwner(int index)
        {
            if (index > -1)
            {
                chesses[index].isMine = !chesses[index].isMine;
                ChessController.Action = ChessController.ChessAction.REFRESH;
            }
        }

    }
}
