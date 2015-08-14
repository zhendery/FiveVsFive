using System.Collections.Generic;
using System.Threading;
namespace FiveVsFive
{
    public class RuleController
    {
        ChessBoard board;
        bool meFisrt;

        public RuleController(ChessBoard board)
        {
            meFisrt = false;
            this.board = board;
        }
        public void newTurn()
        {
            meFisrt = !meFisrt;
            board.reset();

            board.isMyTurn = meFisrt;
        }
        public void moveChess(int pos, bool myself)
        {
            Chess10 chess10 = Chess10.getInstance();
            if (lastSelected != -1)
            {
                int y = pos / 5, x = pos % 5;
                Chess c = chess10.getChess(lastSelected);
                chessBoard[c.x][c.y] = -1;
                chessBoard[x][y] = lastSelected;
                LocalServer.Instance.move(lastSelected, pos);

                if (myself)
                    chess10.isMyTurn = false;

                chess10.moveChess(lastSelected, x, y);
                new Thread((ThreadStart)(delegate { moveEnd(myself); })).Start();
            }
        }

        public void moveEnd(bool myself)
        {
            while (true)
            {
                int[] jaRes = ja(lastSelected);
                if (jaRes.Length > 0)
                    LocalServer.Instance.changeOner(jaRes);

                int[] tiaoRes = tiao(lastSelected);
                if (tiaoRes.Length > 0)
                    LocalServer.Instance.changeOner(tiaoRes);

                if (jaRes.Length == 0 && tiaoRes.Length == 0)
                    break;

                Thread.Sleep(1000);
            }

            if (myself)
                LocalServer.Instance.yourTurn();
            else
                LocalServer.Instance.moveEnd();
            lastSelected = -1;
        }

        int[] ja(int index)
        {
            List<int> res = new List<int>();
            Chess c = Chess10.getInstance().getChess(index);
            for (int i = 0; i < 8; ++i)
            {
                Chess c2 = c + directions[i] * 2,
                    c1 = c + directions[i];

                if (c2.outBoard() || c1.outBoard() || chessBoard[c2.x][c2.y] == -1 || chessBoard[c1.x][c1.y] == -1)
                    continue;

                if (Chess10.getInstance().getChess(chessBoard[c2.x][c2.y]).isMine == c.isMine
                    && Chess10.getInstance().getChess(chessBoard[c1.x][c1.y]).isMine != c.isMine)

                    res.Add(chessBoard[c1.x][c1.y]);
            }
            return res.ToArray();
        }

        int[] tiao(int index)
        {
            List<int> res = new List<int>();
            Chess c = Chess10.getInstance().getChess(index);
            for (int i = 0; i < 8; i += 2)
            {
                Chess cL = c + directions[i],
                    cR = c + directions[i + 1];

                if (cL.outBoard() || cR.outBoard() || chessBoard[cL.x][cL.y] == -1 || chessBoard[cR.x][cR.y] == -1)
                    continue;

                if (Chess10.getInstance().getChess(chessBoard[cL.x][cL.y]).isMine != c.isMine
                    && Chess10.getInstance().getChess(chessBoard[cR.x][cR.y]).isMine != c.isMine)
                {
                    res.Add(chessBoard[cL.x][cL.y]);
                    res.Add(chessBoard[cR.x][cR.y]);
                }
            }
            return res.ToArray();
        }
        void getCanGo(int index, ref List<int> list)
        {
            Chess10 chess10 = Chess10.getInstance();
            if (index > -1)
            {
                list.Clear();
                Chess selected = chess10.getChess(index);
                int dirCount = selected.x % 2 == selected.y % 2 ? 8 : 4;//一奇一偶的只可走上下左右

                for (int i = 0; i < dirCount; ++i)
                {
                    Chess newC = selected;
                    for (int step = 1; step < 5; ++step)
                    {
                        newC += directions[i];
                        if (newC.outBoard() || chessBoard[newC.x][newC.y] != -1)
                            break;
                        else
                            list.Add(newC.y * 5 + newC.x);
                    }
                }
                int countEn = chess10.getCount(false);
                switch (countEn)
                {

                    case 3://不可同时挑与夹

                        break;
                    case 2://不可挑

                        break;
                    case 1://不可夹

                        break;
                }
            }
        }

        public void changeOwner(int index)
        {
            Chess10.getInstance().changeChessOwner(index);
        }

        bool isWin()
        {
            return false;
        }
    }
}