namespace FiveVsFive
{
    public class Const
    {
        public const int PORT = 57257;
        public const int MAX_MSG_LEN = 64;

        public const byte CONNECT = 0,
            DISCONNECT = 1,

            GAME_START = 11,
            UP_CHESS = 12,
            MOVE_CHESS = 13,
            OVER_CHESS = 14,
            MOVE_END=15,

            YOUR_TURN=31,
            GAME_WIN = 32,
            GAME_LOSE = 32;
    }
}