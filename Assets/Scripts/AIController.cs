namespace FiveVsFive
{
    public class AIController
    {
        static AIController ai;
        public static AIController getInstance()
        {
            if (ai == null)
                ai = new AIController();
            return ai;
        }

        int[][] chessBoard;
        public AIController()
        {
            chessBoard = new int[5][];
            for (int i = 0; i < 5; ++i)
                chessBoard[i] = new int[5];
        }


        public void reset(int[][] c)
        {
            for (int i = 0; i < 5; ++i)
                for (int j = 0; j < 5; ++j)
                    chessBoard[i][j] = c[i][j];
        }

        public void move()
        {


            Chess10.getInstance().isMyTurn = true;//ai走完了
        }
    }
}