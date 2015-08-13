using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FiveVsFive
{
    public class ChessController : MonoBehaviour
    {
        //const static
        public static Vector3 v_pi = new Vector3(0, 180, 0);

        Transform[] chesses;
        UISprite[] tips;

        Vector2 centerScreen;
        float aspect;

        public GameObject chessPre, tipPre;
        public Transform tipsParent;

        const int BLOCK_WID = 243;

        void Start()
        {
            chesses = new Transform[10];
            for (int i = 0; i < 10; ++i)
            {
                chesses[i] = (GameObject.Instantiate(chessPre) as GameObject).transform;
                chesses[i].parent = transform;
            }
            tips = new UISprite[25];
            for (int i = -2; i < 3; ++i)
            {
                for (int j = -2; j < 3; ++j)
                {
                    int index = (i + 2) + (j + 2) * 5;
                    Transform trans = (GameObject.Instantiate(tipPre) as GameObject).transform;
                    trans.parent = tipsParent;
                    trans.localPosition = new Vector2(i * BLOCK_WID, j * BLOCK_WID);
                    trans.localScale = Vector3.one;
                    tips[index] = trans.GetComponent<UISprite>();
                    tips[index].alpha = 0;
                }
            }
            centerScreen = new Vector2(Screen.width / 2, Screen.height / 2);
            aspect = 720 / Screen.height;
        }

        bool white = false;
        public bool White { get { return white; } }

        static float[] index2Pos = { -2.5f, -1.3f, 0f, 1.3f, 2.5f };

        public void reset()
        {
            white = !white;
            selectIndex = -1;
            refresh();
        }

        public void refresh()
        {
            Chess c = null;
            for (int i = 0; i < 10; ++i)
            {
                c = Chess10.getInstance().getChess(i);
                chesses[i].localPosition = new Vector3(index2Pos[c.x], index2Pos[c.y]);
                chesses[i].eulerAngles = (!(c.isMine ^ white)) ? Vector3.zero : v_pi;
            }
            selectIndex = -1;
            showTips();
        }


        static Vector3[] v_up = { new Vector3(0, 0, -1f), new Vector3(0, 0, -0.5f), new Vector3(0, 0, -0.3f), new Vector3(0, 0, -0.2f) };

        int selectIndex;
        public void upChess()
        {
            int index = Chess10.getInstance().selectedIndex;
            if (index > -1 )//抬起
            {
                int upIndex = (int)((Vector2)chesses[index].localPosition).magnitude;
                bool white = chesses[index].eulerAngles.magnitude < 1;
                chesses[index].Translate(white ? v_up[upIndex] : -v_up[upIndex]);
                //playAudio 抬起
                if (selectIndex > -1)
                {
                    upIndex = (int)((Vector2)chesses[selectIndex].localPosition).magnitude;
                    white = chesses[selectIndex].eulerAngles.magnitude < 1;
                    chesses[selectIndex].Translate(white ? -v_up[upIndex] : v_up[upIndex]);
                    //playAudio 放下
                }
            }
            else//放下
            {
                int upIndex = (int)((Vector2)chesses[selectIndex].localPosition).magnitude;
                bool white = chesses[selectIndex].eulerAngles.magnitude < 1;
                chesses[selectIndex].Translate(white ? -v_up[upIndex] : v_up[upIndex]);
                //playAudio 放下
                Chess10.getInstance().selectedIndex = selectIndex = -1;
            }
            selectIndex = index;
            showTips();
        }


        void showTips()
        {
            for (int i = 0; i < 25; ++i)
                tips[i].alpha = 0;
            foreach (int index in Chess10.getInstance().showIndexs)
                tips[index].alpha = 1;
        }

        void OnMouseDown()
        {
            Chess10 c = Chess10.getInstance();
            if (c.isMyTurn)
            {
                centerScreen = new Vector2(Screen.width / 2, Screen.height / 2);//实际无需
                aspect = 720f / Screen.height;//实际无需

                float minDis = 40f;
                int index = -1;
                for (int i = 0; i < 10; ++i)
                {
                    if (c.getChess(i).isMine)
                    {
                        Vector2 chessPos = Camera.main.WorldToScreenPoint(chesses[i].position);
                        float tempDis = Vector2.Distance(Input.mousePosition, chessPos) * aspect;
                        if (tempDis < minDis)
                        {
                            minDis = tempDis;
                            index = i;
                        }
                    }
                }
                ///////////////  index 表示点击地方是棋子还是空白

                if (index > -1)
                {
                    if (c.getChess(index).isMine)//点击棋子并可操纵
                        LocalServer.Instance.upChess(index);
                }
                else if (selectIndex > -1)//点击空白并且已有选择棋子，则移动
                {
                    int pos = -1;
                    minDis = 40f;
                    foreach (int item in Chess10.getInstance().showIndexs)
                    {
                        Vector2 touchPos = ((Vector2)Input.mousePosition - centerScreen) * aspect;
                        float tempDis = Vector2.Distance(touchPos, getChessBoardPos(item));
                        if (tempDis < minDis)
                        {
                            minDis = tempDis;
                            pos = item;
                        }
                    }
                    if (pos > -1)
                    {
                        LocalServer.Instance.move(pos);
                    }
                }
            }
        }

        Vector2 getChessBoardPos(int pos)
        {
            int y = pos / 5 - 2, x = pos % 5 - 2;
            return new Vector2(x * 80, y * 80);
        }

        public enum ChessAction
        {
            NONE,
            RESET,
            REFRESH,
            UP_CHESS,
            SHOW_TIPS,
        }
        public static ChessAction Action { get; set; }
        void Update()
        {
            switch (Action)
            {
                case ChessAction.NONE:
                    break;
                case ChessAction.UP_CHESS:
                    upChess();
                    break;
                case ChessAction.REFRESH:
                    refresh();
                    break;
                case ChessAction.RESET:
                    reset();
                    break;
            }
            Action = ChessAction.NONE;
        }

    }

}