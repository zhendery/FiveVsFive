using System;
namespace FiveVsFive
{
    struct Information
    {
        public int logo;
        public string name;
    }
    class Names
    {
        public static string keyName = "myName",
            keyLogo = "myLogo",
            keyAudio = "audioOn";

        static string[] names = {"流氓也是一种气质","帅死一条街","降妞十巴掌","厌学症晚期","冷眼旁观装逼狗",
                "人潮拥挤咋没挤死你","一见你就笑","风吹散了的回忆","驴是的念来过倒","oo兜兜冇餹ヾ","忘了你亡了我",
                "风吹花成雨","余生多指教施主","你的贞操掉了","喝着脉动割动脉","许一世天荒地老","星期⑧娶你","你是我的生死劫",
                "情话烫心","深情不如久伴","年少总有装逼梦","上帝哋寵児","别低头、除非地上有钱","路还长你别皱眉","据说名字长了会拖网速",
                "嗯哼嗯哼蹦叉叉","是深爱还是深碍","陌然浅笑°","是人是狗慢慢瞅","╰朕射迩无罪╯","封心锁爱只为你","嘴唇゛愛上煙﹏",
                "若要我死，你必先亡","天然呆萌无药可医","月亮是我踹弯的","拿回忆下酒","不痛不痒不在乎丶","冰是睡着的水",
                "电费太贵发光太累","温一盏杏花酒","开着拖拉机唱情歌" 
                                };

        public static Information getRandomName()
        {
            Random ran = new Random(DateTime.Now.Millisecond);
            int name = ran.Next(names.Length),
                logo = ran.Next(6) + 1;
            Information info =new Information { logo=logo, name=names[name] };
            return info;
        }
    }
}