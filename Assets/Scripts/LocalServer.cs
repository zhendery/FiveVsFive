using System.Threading;
namespace FiveVsFive
{
    public interface IHost
    {
        void upChess(int index);
        void move(int pos);
        void move(int index,int pos);
        void moveEnd();
        void showTips(int[] indexs);
        void changeOner(int[] indexs);
        void yourTurn();

    }

    public class LocalServer : IHost
    {
        public static IHost Instance;
        static LocalServer server;
        public static LocalServer getInstance()
        {
            if (server == null)
                server = new LocalServer();
            return server;
        }

        RuleController rule;
        public LocalServer()
        {
            rule = new RuleController();
        }
        public void startServer()
        {
            Instance = this;
            rule.newTurn();
            AIController.getInstance().reset(rule.getChessBoard());
        }

        public void yourTurn()
        {
            AIController.getInstance().move();
        }
        public void upChess(int index)
        {
            rule.upChess(index);
        }
        public void showTips(int[] indexs) { }
        public void move(int pos)
        {
            rule.moveChess(pos,true);
        }
        public void move(int index, int pos) { }
        public void moveEnd() { }
        public void changeOner(int[] indexs)
        {
            foreach(int index in indexs)
                rule.changeOwner(index);
        }
    }

}