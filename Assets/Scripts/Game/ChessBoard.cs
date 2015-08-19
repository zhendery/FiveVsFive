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
            oldMine = false;
        }
        public Chess(int x, int y)
            : this()
        {
            this.x = this.oldX = x;
            this.y = this.oldY = y;
        }
        public Chess(Chess c)
        {
            this.x = c.x;
            this.y = c.y;
            this.isMine = c.isMine;
            this.oldX = c.oldX;
            this.oldY = c.oldY;
            this.oldMine = c.oldMine;
        }
        public Chess(int x, int y, bool isMine)
        {
            this.x =this.oldX = x;
            this.y = this.oldY = y;
            this.isMine = this.oldMine = isMine;
        }
        public void setOldPos()
        {//将自己的信息设旧(即存到old当中)
            oldY = this.y;
            oldX = this.x;
        }
        public void setOld()
        {//将自己的信息设旧(即存到old当中)
            setOldPos();
            setOldMine();
        }
        public void setOldMine()
        {
            oldMine = this.isMine;
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
        public int selected,chessUp;
        int[][] locations;//表示25个棋点,值为chesses索引

        public ChessBoard()
        {
            chesses = new Chess[10];
            locations = new int[5][];
            for (short i = 0; i < 5; ++i)
                locations[i] = new int[5];
        }
        public ChessBoard(ChessBoard board)
            : this()//复制棋盘，供假走的时候参考
        {
            for (short i = 0; i < 10; ++i)
                chesses[i] = new Chess(board.chesses[i]);
            for (short i = 0; i < 5; ++i)
                for (short k = 0; k < 5; ++k)
                    locations[i][k] = board.locations[i][k];
        }
        public void reset()
        {
            short i = 0;
            for (i = 0; i < 5; ++i)
            {
                chesses[i] = new Chess(2, 2, false);//首先反置，并置错位，使得绘图可以检测出
                chesses[i + 5] = new Chess(2, 2, true);

                changeChessOwner(i);//开始翻转
                changeChessOwner(i + 5);
                moveChess(i, i, 0);//开始移动
                moveChess(i + 5, i, 4);


                for (short k = 0; k < 5; ++k)//全部置零(-1 表示无棋子)
                    locations[i][k] = -1;
            }
            for (i = 0; i < 5; ++i)//将0和4排填上棋子
            {
                locations[i][0] = i;
                locations[i][4] = i + 5;
            }
            selected = -1;
            chessUp = -1;
        }

        public Chess getChess(int index)
        {
            return index > -1 && index < 10 ? chesses[index] : null;
        }
        public Chess getChess(int x, int y)
        {
            return x > -1 && x < 5 && y > -1 && y < 5 ? chesses[locations[x][y]] : null;
        }
        public int[] getChesses(bool my)
        {
            List<int> temp = new List<int>();
            for (int i = 0; i < 10; ++i)
                if (chesses[i].isMine == my)
                    temp.Add(i);

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
            return getCanGo(selected);
        }
        public int[] getCanGo(int index)//此函数尽量少用，在规则判断可以用
        {
            List<int> list = new List<int>();
            if (index > -1 && index < 10)
            {
                Chess selectedChess = chesses[index];
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
                            ChessBoard newBoard = new ChessBoard(this);//复制当前棋盘
                            newBoard.moveChess(index, newC.x, newC.y);//假走

                            //对方的回合就要判断我方棋子数量
                            int countEn = newBoard.getCount(LanClient.instance.whoseTurn == GameState.YOUT_TURN);
                            bool canGoIreg = true;
                            switch (countEn)
                            {
                                case 3://不可同时挑与夹
                                    int[] jaRes = newBoard.ja(index);
                                    int[] tiaoRes = newBoard.tiao(index);
                                    if (jaRes.Length == 1 && tiaoRes.Length == 2)
                                        canGoIreg = false;
                                    break;
                                case 2://不可挑
                                    tiaoRes = newBoard.tiao(index);
                                    if (tiaoRes.Length == 2)
                                        canGoIreg = false;
                                    break;
                                case 1://不可夹
                                    jaRes = newBoard.ja(index);
                                    if (jaRes.Length == 1)
                                        canGoIreg = false;
                                    break;
                            }

                            if (canGoIreg)
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
                chesses[selected].setOldPos();
                chesses[selected].x = pos % 5;
                chesses[selected].y = pos / 5;
                locations[chesses[selected].x][chesses[selected].y] = selected;
                selected = -1;

                //走完便自封，直到server发来消息解封
                LanClient.instance.whoseTurn = GameState.NO_TURN;
            }
        }
        public void moveChess(int index, int x, int y)//供假走测试用
        {
            if (index > -1 && index < 10)
            {
                locations[chesses[index].x][chesses[index].y] = -1;
                chesses[index].setOldPos();
                chesses[index].x = x;
                chesses[index].y = y;
                locations[chesses[index].x][chesses[index].y] = index;
            }
        }

        public void changeChessOwner(int index)
        {
            if (index > -1 && index < 10)
            {
                chesses[index].setOldMine();
                chesses[index].isMine = !chesses[index].isMine;
            }
        }
        public void checkJaTiao(int index)
        {
            int[] tiaoRes = tiao(index);
            foreach (int r in tiaoRes)
                changeChessOwner(r);

            int[] jaRes = ja(index);
            foreach (int r in jaRes)
                changeChessOwner(r);

        }

        int[] ja(int index)
        {
            List<int> res = new List<int>();
            Chess c = chesses[index];
            for (int i = 0; i < 8; ++i)
            {
                Chess c2 = c + ChessBoard.directions[i] * 2,//八个方向上的第2颗棋子，应是同色
                    c1 = c + directions[i];//第1颗棋子，应是异色，两个条件同时满足，则形成“夹”

                if (c2.outBoard() || c1.outBoard() || locations[c2.x][c2.y] == -1 || locations[c1.x][c1.y] == -1)
                    continue;

                if (chesses[locations[c2.x][c2.y]].isMine == c.isMine
                    && chesses[locations[c1.x][c1.y]].isMine != c.isMine)

                    res.Add(locations[c1.x][c1.y]);//被夹的棋子索引
            }
            return res.ToArray();
        }

        int[] tiao(int index)
        {
            List<int> res = new List<int>();
            Chess c = chesses[index];
            for (int i = 0; i < 8; i += 2)
            {
                Chess cL = c + directions[i],
                    cR = c + directions[i + 1];

                if (cL.outBoard() || cR.outBoard() || locations[cL.x][cL.y] == -1 || locations[cR.x][cR.y] == -1)
                    continue;

                if (chesses[locations[cL.x][cL.y]].isMine != c.isMine
                    && chesses[locations[cR.x][cR.y]].isMine != c.isMine)
                {
                    res.Add(locations[cL.x][cL.y]);
                    res.Add(locations[cR.x][cR.y]);
                }
            }
            return res.ToArray();
        }

        public static Chess[] directions = { 
                 new Chess(0,1),new Chess(0,-1),new Chess(-1,0),new Chess(1,0),//上下左右
                 new Chess(1,1),new Chess(-1,-1),new Chess(-1,1),new Chess(1,-1)//斜着
                                             };
    }
}
