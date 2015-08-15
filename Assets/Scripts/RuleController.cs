using System.Collections.Generic;
using System.Threading;
namespace FiveVsFive
{
    public class RuleController
    {
        public static RuleController instance = new RuleController();

        ChessBoard board;
        public bool meFisrt,isMyTurn,isGaming;
        public RuleController()
        {
            this.board = ChessBoard.instance;
            meFisrt = false;
            isGaming = false;
        }

        public void newTurn()
        {
            isGaming = true;
            meFisrt = !meFisrt;
            board.reset();

            isMyTurn = meFisrt;
        }
        //int[] ja(int index)
        //{
        //    List<int> res = new List<int>();
        //    Chess c = board.getChess(index);
        //    for (int i = 0; i < 8; ++i)
        //    {
        //        Chess c2 = c + ChessBoard.directions[i] * 2,
        //            c1 = c + directions[i];

        //        if (c2.outBoard() || c1.outBoard() || chessBoard[c2.x][c2.y] == -1 || chessBoard[c1.x][c1.y] == -1)
        //            continue;

        //        if (Chess10.getInstance().getChess(chessBoard[c2.x][c2.y]).isMine == c.isMine
        //            && Chess10.getInstance().getChess(chessBoard[c1.x][c1.y]).isMine != c.isMine)

        //            res.Add(chessBoard[c1.x][c1.y]);
        //    }
        //    return res.ToArray();
        //}

        //int[] tiao(int index)
        //{
        //    List<int> res = new List<int>();
        //    Chess c = Chess10.getInstance().getChess(index);
        //    for (int i = 0; i < 8; i += 2)
        //    {
        //        Chess cL = c + directions[i],
        //            cR = c + directions[i + 1];

        //        if (cL.outBoard() || cR.outBoard() || chessBoard[cL.x][cL.y] == -1 || chessBoard[cR.x][cR.y] == -1)
        //            continue;

        //        if (Chess10.getInstance().getChess(chessBoard[cL.x][cL.y]).isMine != c.isMine
        //            && Chess10.getInstance().getChess(chessBoard[cR.x][cR.y]).isMine != c.isMine)
        //        {
        //            res.Add(chessBoard[cL.x][cL.y]);
        //            res.Add(chessBoard[cR.x][cR.y]);
        //        }
        //    }
        //    return res.ToArray();
        //}
        

        bool isWin()
        {
            return false;
        }
    }
}