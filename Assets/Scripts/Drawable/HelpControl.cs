using UnityEngine;
using System.Collections;
using FiveVsFive;
using System.Collections.Generic;

public class HelpControl : MonoBehaviour
{
    public GameObject chessPre;
    public Transform helpPanel,head;
    Transform[] chesses;

    UILabel helpMsg;

    Transform nextBtn;

    short step;

    #region 帮助文字
    string[] helpMsgs = {
                       "        五路夹挑是一款双人棋，开始每人各有五颗棋子，分布于两方的边上，如果对方没有棋子了或者对方无路可走，则获胜。",
                        //1

                        "        黑子先走，点击棋子将棋子抬起，点击要走的点，棋子将走到对应位置。棋子可以沿着所在的直线走任意步，除非被其他棋子挡住。",
                        //2

                        "        吃掉对方棋子的方法有两种，“夹”和“挑”。其中“夹”是指我方两颗棋子中间有一颗对方棋子，则对方棋子将变为我方棋子。",
                        //3

                        "        “挑”是指我方一颗棋子移动到对方两颗棋子中间，则对方棋子将变为我方棋子。",
                        //4

                        "        当然如果三颗棋子不在棋盘上的同一条轨道上，则不算夹或者挑。",//5

                        "        任何夹或者挑的效果都具有联动效果。",//6

                        "        最后一个规则就是当对方只剩3颗棋子的时候，你走的棋不能一次将对方三颗都消灭；对方剩2颗的时候，你不能挑对方两颗棋；对方剩1颗的时候，你不能夹对方一颗棋。",
                        //7

                        "        规则就这么多，请尽情发挥吧。"//8

                        };
    #endregion

    #region 开始布局
    Chess[] ch0 = {
                  new Chess(0,0,true),new Chess(1,0,true),new Chess(2,0,true),new Chess(3,0,true),new Chess(4,0,true),
                  new Chess(0,4,false),new Chess(1,4,false),new Chess(2,4,false),new Chess(3,4,false),new Chess(4,4,false),
                  };
    Chess[] ch1 = {
                  new Chess(1,1,true),new Chess(1,0,true),new Chess(2,0,true),new Chess(3,0,true),new Chess(2,2,true),
                  new Chess(0,4,false),new Chess(1,4,false),new Chess(2,4,false),new Chess(3,3,false),new Chess(0,0,false),
                  };
    Chess[] ch2 = {
                  new Chess(0,0,true),new Chess(1,0,true),new Chess(2,0,true),new Chess(3,0,true),new Chess(2,2,true),
                  new Chess(0,4,false),new Chess(1,4,false),new Chess(2,4,false),new Chess(3,4,false),new Chess(3,1,false),
                  };
    Chess[] ch3 = {
                  new Chess(0,0,true),new Chess(1,0,true),new Chess(2,0,true),new Chess(3,0,true),new Chess(2,1,true),
                  new Chess(0,4,false),new Chess(1,4,false),new Chess(4,2,false),new Chess(3,4,false),new Chess(4,4,false),
                  };
    Chess[] ch4 = {
                  new Chess(0,0,true),new Chess(1,0,true),new Chess(2,0,true),new Chess(3,0,true),new Chess(4,0,true),
                  new Chess(2,1,false),new Chess(1,4,false),new Chess(2,4,false),new Chess(3,4,false),new Chess(4,4,false),
                  };
    Chess[] ch5 = {
                  new Chess(0,0,true),new Chess(1,0,true),new Chess(2,0,true),new Chess(3,0,true),new Chess(1,4,true),
                  new Chess(0,4,false),new Chess(1,3,false),new Chess(2,2,false),new Chess(3,4,false),new Chess(4,4,false),
                  };
    Chess[] ch6 = {
                  new Chess(0,0,true),new Chess(1,0,true),new Chess(2,0,true),new Chess(3,0,true),new Chess(4,0,true),
                  new Chess(4,2,true),new Chess(0,3,false),new Chess(1,3,false),new Chess(2,3,false),new Chess(1,4,true),
                  };
    Chess[] ch7 = {
                 new Chess(0,0,true),new Chess(1,0,true),new Chess(2,0,true),new Chess(3,0,true),new Chess(4,0,true),
                  new Chess(0,2,true),new Chess(0,3,false),new Chess(1,3,true),new Chess(2,3,true),new Chess(1,4,true),
                  };
    #endregion

    #region 动画
    struct Anima
    {
        public short index;
        public bool moveOrOver;
        public short pos;
        public float delay;
    }

    Anima[] an0 = {
                  new Anima {index=0, moveOrOver=true,pos=0,delay=1.2f}//不动
                  };//开始

    Anima[] an1 = { 
                      new Anima {index=0, moveOrOver=true,pos=9,delay=0.8f},
                      new Anima {index=8, moveOrOver=true,pos=23,delay=0.8f},
                      new Anima {index=4, moveOrOver=true,pos=24,delay=1.2f}//走棋
                  };
    Anima[] an2 = {
                      new Anima {index=4, moveOrOver=true,pos=13,delay=0.4f},
                      new Anima {index=9, moveOrOver=false,delay=1.2f}//夹
                   };
    Anima[] an3 = {
                      new Anima {index=4, moveOrOver=true,pos=22,delay=0.4f},
                      new Anima {index=6, moveOrOver=false,delay=0},
                      new Anima {index=8, moveOrOver=false,delay=1.2f},//挑
                   };
    Anima[] an4 = { 
                      new Anima {index=1, moveOrOver=true,pos=11,delay=1.2f}//非共线
                  };
    Anima[] an5 = { 
                      new Anima {index=1, moveOrOver=true,pos=11,delay=0.4f},
                      new Anima {index=6, moveOrOver=false,delay=0.4f},
                      new Anima {index=5, moveOrOver=false,delay=0},
                      new Anima {index=7, moveOrOver=false,delay=1.2f}//联动
                   };
    Anima[] an6 = { 
                      new Anima {index=5, moveOrOver=true,pos=18,delay=1.2f}//三颗
                  };
    Anima[] an7 = {
                  new Anima {index=8, moveOrOver=true,pos=12,delay=0.8f},
                      new Anima {index=6, moveOrOver=true,pos=20,delay=0.8f},
                      new Anima {index=5, moveOrOver=true,pos=15,delay=1.2f}//一颗
                  };

    #endregion

    Chess[][] ches;
    Anima[][] anims;
    float index2Pos = 1.35f;
    void Awake()
    {
        #region 初始化数组

        ches = new Chess[8][];
        ches[0] = ch0;
        ches[1] = ch1;
        ches[2] = ch2;
        ches[3] = ch3;
        ches[4] = ch4;
        ches[5] = ch5;
        ches[6] = ch6;
        ches[7] = ch7;

        anims = new Anima[8][];
        anims[0] = an0;
        anims[1] = an1;
        anims[2] = an2;
        anims[3] = an3;
        anims[4] = an4;
        anims[5] = an5;
        anims[6] = an6;
        anims[7] = an7;
        #endregion

        helpMsg = transform.FindChild("helpMsg").GetComponent<UILabel>();
        nextBtn = transform.FindChild("next");
        nextBtn.GetComponent<UIEventListener>().onClick = iKnow;

        chesses = new Transform[10];
    }

    void iKnow(GameObject bt)
    {
        if (step > 7)
        {
            transform.gameObject.SetActive(false);
            helpPanel.gameObject.SetActive(false);
            return;
        }

        nextBtn.gameObject.SetActive(false);

        helpMsg.text = helpMsgs[step];
        helpMsg.gameObject.AddComponent<TypewriterEffect>();

        StartCoroutine(animate(step++));

        StartCoroutine(showBtn());
    }

    IEnumerator showBtn()
    {
        yield return new WaitForSeconds(2f);
        nextBtn.gameObject.SetActive(true);
    }

    IEnumerator animate(short times)
    {
        Dictionary<UITweener, float> animations = new Dictionary<UITweener, float>();//动画和延迟
        //按顺序将动画添加到列表中
        for (short i = 0; i < anims[times].Length; ++i)
        {
            Anima ani = anims[times][i];
            if (ani.moveOrOver)
            {
                int x = ani.pos % 5 - 2, y = ani.pos / 5 - 2;
                TweenPosition move = chesses[ani.index].GetComponent<TweenPosition>();
                move.from = new Vector3((ches[times][ani.index].x - 2) * index2Pos, (ches[times][ani.index].y - 2) * index2Pos - 0.6f);
                move.to = new Vector3(index2Pos * x, index2Pos * y - 0.6f);
                animations.Add(move, ani.delay);
            }
            else
            {
                TweenRotation rotate = chesses[ani.index].GetComponent<TweenRotation>();
                rotate.from = ches[times][ani.index].isMine ? ChessController.v_pi : Vector3.zero;
                rotate.to = ches[times][ani.index].isMine ? Vector3.zero : ChessController.v_pi;
                animations.Add(rotate, ani.delay);
            }
        }
        //然后开始无限循环重播
        while (times == step - 1)
        {
            //首先将每个动画还原
            foreach (var animation in animations)
                animation.Key.ResetToBeginning();

            //先排布棋子
            for (int i = 0; i < 10; ++i)
            {
                chesses[i].localPosition = new Vector3((ches[times][i].x - 2) * index2Pos, (ches[times][i].y - 2) * index2Pos - 0.6f);
                chesses[i].localEulerAngles = ches[times][i].isMine ? ChessController.v_pi : Vector3.zero;
            }


            yield return new WaitForSeconds(1);//暂停1秒后再开始动

            //然后播放每个动画
            foreach (var animation in animations)
            {
                if (times != step - 1)
                    yield break;
                animation.Key.PlayForward();
                yield return new WaitForSeconds(animation.Value);
            }
        }
    }

    //每次出现则显示
    void OnEnable()
    {
        head.gameObject.SetActive(false);
        Global.setSceneOld(GameScenes.HELP);
        helpPanel.gameObject.SetActive(true);
        for (int i = 0; i < 10; ++i)
        {
            chesses[i] = (GameObject.Instantiate(chessPre) as GameObject).transform;
            chesses[i].parent = helpPanel;
        }

        step = 0;
        iKnow(nextBtn.gameObject);
    }
    void OnDisable()
    {
        for (int i = 0; i < 10; ++i)
        {
            Destroy(chesses[i].gameObject);
            chesses[i] = null;
        }
        Global.setSceneOld(GameScenes.WELCOME);
        head.gameObject.SetActive(true);
    }
}
