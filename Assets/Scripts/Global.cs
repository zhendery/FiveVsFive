namespace FiveVsFive
{
    public enum GameScenes
    {
        WELCOME,
        CHOOSE_FRIEND,
        SEARCHING_FRIEND,
        JOIN_FRIEND,
        SETTING,
        HELP,
        GAME_SCENE
    };
    public class Global
    {
        public static ChessBoard board = new ChessBoard();//每个设备总会有且只有一个棋盘（不含虚拟棋盘）
        public static LanClient client = new LanClient();//每个设备总会有且只有一个客户端

        public static GameScenes oldScene, gameScene;
        public static void setSceneOld(GameScenes newScene)
        {
            oldScene = gameScene;
            gameScene = newScene;
        }
    }
}