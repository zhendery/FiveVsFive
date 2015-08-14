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
    public class ChessBoard
    {
        Chess[] chesses;
        public bool isMyTurn { get; set; }
        public ChessBoard()
        {
            chesses = new Chess[10];
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
        }

        public Chess getChess(int index)
        {
            if (index > -1)
                return chesses[index];
            else
                return null;
        }

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
        public int[] getCanGo(int index)
        {
            List<int> list = new List<int>();
            if (index > -1)
            {
                Chess selected = chesses[index];
                int dirCount = selected.x % 2 == selected.y % 2 ? 8 : 4;//一奇一偶的只可走上下左右

                for (int i = 0; i < dirCount; ++i)
                {
                    Chess newC = new Chess(selected);
                    for (int step = 1; step < 5; ++step)
                    {
                        newC += directions[i];
                        if (newC.outBoard() || hasChess(newC))
                            break;
                        else
                        {
                            int countEn = getCount(false);
                            switch (countEn)
                            {

                                case 3://不可同时挑与夹

                                    break;
                                case 2://不可挑

                                    break;
                                case 1://不可夹

                                    break;
                            }

                            list.Add(newC.y * 5 + newC.x);
                        }
                    }
                }
            }
            return list.ToArray();
        }

        bool hasChess(Chess newC)
        {
            for (int i = 0; i < 10; ++i)
            {
                if (chesses[i].x == newC.x && chesses[i].y == newC.y)
                    return true;
            }
            return false;
        }

        public void moveChess(int index,int x, int y)
        {
            if (index > -1)
            {
                chesses[index].x = x;
                chesses[index].y = y;
            }
        }

        public void changeChessOwner(int index)
        {
            if (index > -1)
            {
                chesses[index].isMine = !chesses[index].isMine;
            }
        }

        public static Chess[] directions = { 
                 new Chess(0,1),new Chess(0,-1),new Chess(-1,0),new Chess(1,0),//上下左右
                 new Chess(1,1),new Chess(-1,-1),new Chess(-1,1),new Chess(1,-1)//斜着
                                             };
    }
}
