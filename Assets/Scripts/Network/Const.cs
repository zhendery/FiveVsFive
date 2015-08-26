namespace FiveVsFive
{
    public class Const
    {
        public const int PORT = 57257;
        public const int MAX_MSG_LEN = 64;
        //public const string BROADCAST_MSG = "FiveVsFive";

        public const byte CONNECT = 41,
            DISCONNECT = 42,

            GAME_START = 11,
            UP_CHESS = 12,
            MOVE_CHESS = 13,
            OVER_CHESS = 14,
            RETRACT_CHESS=15,//悔棋
            RETRACT_APPLY=16,//申请悔棋
            RETRACT_DISAGREE=17,//不同意悔棋

            NEW_TURN=20,

            YOUR_TURN=31,
            END_GAME = 32;
    }
}