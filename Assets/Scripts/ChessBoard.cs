using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace FiveVsFive
{
    public class Chess
    {
        public int x, y, oldX, oldY;
        public bool isMine, oldMine;
        public Chess()
        {
            oldX = -1;
            oldY = -1;
            oldMine = false;
        }
        public Chess(int x, int y)
            : this()
        {
            this.x = x;
            this.y = y;
        }
        public Chess(Chess c)
            : this()
        {
            this.x = c.x;
            this.y = c.y;
        }
        public Chess(int x, int y, bool isMine)
            : this()
        {
            this.x = x;
            this.y = y;
            this.isMine = isMine;
        }
        public void setOld(Chess c)
        {
            oldY = c.y;
            oldX = c.x;
            oldMine = c.isMine;
        }
        public bool isMoved()
        {
            return x != oldX || y != oldY;//已经移动过了
        }
        public bool isOvered()
        {
            return isMine != oldMine;
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
        public static ChessBoard instance = new ChessBoard();
        Chess[] chesses;//表示10颗棋子
        public int selected;
        int[][] locations;//表示25个棋点,值为chesses索引

        public ChessBoard()
        {
            chesses = new Chess[10];
            locations = new int[5][];
            for (short i = 0; i < 5; ++i)
                locations[i] = new int[5];
        }

        public void reset()
        {
            short i = 0;
            for (i = 0; i < 5; ++i)
            {
                chesses[i] = new Chess(i, 0, true);
                chesses[i + 5] = new Chess(i, 4, false);

                for (short k = 0; k < 5; ++k)//全部置零(-1 表示无棋子)
                    locations[i][k] = -1;
            }
            for (i = 0; i < 5; ++i)//将0和4排填上棋子
            {
                locations[i][0] = i;
                locations[i][4] = i + 5;
            }
            selected = -1;
        }

        public Chess getChess(int index)
        {
            return index > -1 && index < 10 ? chesses[index] : null;
        }
        public Chess getChess(int x, int y)
        {
            return x > -1 && x < 5 && y > -1 && y < 5 ? chesses[locations[x][y]] : null;
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
        public int[] getCanGo()
        {
            List<int> list = new List<int>();
            if (selected > -1 && selected < 10)
            {
                Chess selectedChess = chesses[selected];
                int dirCount = selectedChess.x % 2 == selectedChess.y % 2 ? 8 : 4;//一奇一偶的只可走上下左右

                for (int i = 0; i < dirCount; ++i)
                {
                    Chess newC = new Chess(selectedChess);
                    for (int step = 1; step < 5; ++step)
                    {
                        newC += directions[i];
                        if (newC.outBoard() || locations[newC.x][newC.y] != -1)//出界了 || 已经有棋子了
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

        public void moveChess(int pos)
        {
            if (selected > -1 && selected < 10)
            {
                locations[chesses[selected].x][chesses[selected].y] = -1;
                chesses[selected].setOld(chesses[selected]);
                chesses[selected].x = pos % 5;
                chesses[selected].y = pos / 5;
                locations[chesses[selected].x][chesses[selected].y] = selected;
                selected = -1;
            }
        }

        public void changeChessOwner(int index)
        {
            if (index > -1 && index < 10)
                chesses[index].isMine = !chesses[index].isMine;
        }

        public static Chess[] directions = { 
                 new Chess(0,1),new Chess(0,-1),new Chess(-1,0),new Chess(1,0),//上下左右
                 new Chess(1,1),new Chess(-1,-1),new Chess(-1,1),new Chess(1,-1)//斜着
                                             };
    }
}
