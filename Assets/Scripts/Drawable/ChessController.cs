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

        public GameObject chessPre, tipPre;
        public Transform tipsParent;

        const int BLOCK_WID = 215;
        float aspect;

        void Start()
        {
            aspect = 720f / Screen.height;
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
                    trans.localPosition = new Vector2(i * BLOCK_WID, j * BLOCK_WID - 105);
                    trans.localScale = Vector3.one;
                    tips[index] = trans.GetComponent<UISprite>();
                    tips[index].alpha = 0;
                }
            }
        }

        static float[] index2Pos = { -2.5f, -1.3f, 0f, 1.3f, 2.5f };
        public void FixedUpdate()//自动刷新
        {
            if (Global.client != null && Global.client.isGaming)
            {
                if (Global.board.chessUp > -1 && Global.board.chessUp < 10)
                    upChess();

                Chess c = null;
                for (int i = 0; i < 10; ++i)
                {
                    c = Global.board.getChess(i);
                    if (c.isMoved())
                        moveAction(c.x, c.y, i);
                    if (c.isOvered())
                        overAction(c.isMine, i);
                    c.setOld();
                }
            }
            checkEndGame();
        }

        void moveAction(int x, int y, int index)
        {
            showTips();
            Vector3 newPos = new Vector3(index2Pos[x], index2Pos[y]);
            TweenPosition move = chesses[index].GetComponent<TweenPosition>();
            move.from = chesses[index].localPosition;
            move.to = newPos;
            move.ResetToBeginning();
            move.PlayForward();
        }
        void overAction(bool isMine, int index)
        {
            Vector3 newEuler = (isMine ^ Global.client.myColor) ? Vector3.zero : v_pi;//^是异或(不同为true)，不许再改了~~
            TweenRotation rotate = chesses[index].GetComponent<TweenRotation>();
            rotate.from = chesses[index].eulerAngles;
            rotate.to = newEuler;
            rotate.ResetToBeginning();
            rotate.PlayForward();
        }

        static Vector3[] v_up = { new Vector3(0, 0, -1f), new Vector3(0, 0, -0.5f), new Vector3(0, 0, -0.3f), new Vector3(0, 0, -0.2f) };

        public void upChess()
        {
            int index = Global.board.chessUp;
            if (index < 0 || index > 9)
                return;
            int selected = Global.board.selected;
            if (index != selected)//此棋子未抬起，则将其抬起
            {
                int upIndex = (int)((Vector2)chesses[index].localPosition).magnitude;
                bool white = chesses[index].eulerAngles.magnitude < 1;
                chesses[index].Translate(white ? v_up[upIndex] : -v_up[upIndex]);
                //playAudio 抬起
                if (selected > -1)//如果原来有抬起的，将其放下
                {
                    upIndex = (int)((Vector2)chesses[selected].localPosition).magnitude;
                    white = chesses[selected].eulerAngles.magnitude < 1;
                    chesses[selected].Translate(white ? -v_up[upIndex] : v_up[upIndex]);
                    //playAudio 放下
                }
                Global.board.selected = index;
            }
            else//此棋子已经抬起，则将其放下
            {
                int upIndex = (int)((Vector2)chesses[selected].localPosition).magnitude;
                bool white = chesses[selected].eulerAngles.magnitude < 1;
                chesses[selected].Translate(white ? -v_up[upIndex] : v_up[upIndex]);
                //playAudio 放下
                Global.board.selected = -1;
            }
            Global.board.chessUp = -1;
            showTips();
        }


        public void showTips()
        {
            for (int i = 0; i < 25; ++i)
                tips[i].alpha = 0;
            foreach (int index in Global.board.getCanGo())
                tips[index].alpha = 1;
        }

        public Camera camerUI;
        void OnMouseDown()
        {
            if (Global.client.whoseTurn == GameState.MY_TURN)
            {
                float minDis = 32f;
                int index = -1;
                for (int i = 0; i < 10; ++i)
                {
                    if (Global.board.getChess(i).isMine)
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
                ///////////////  index 表示点击地方  是棋子还是空白

                if (index > -1 && index < 10)
                {
                    if (Global.board.getChess(index).isMine)//点击棋子并可操纵
                        Global.client.upChess(index);
                }
                else if (Global.board.selected > -1)//点击空白并且已有选择棋子，则移动
                {
                    int pos = -1;
                    minDis = 32f;
                    foreach (int item in Global.board.getCanGo())
                    {
                        Vector2 itemPos = camerUI.WorldToScreenPoint(tips[item].transform.position);
                        float tempDis = Vector2.Distance(Input.mousePosition, itemPos) * aspect;
                        if (tempDis < minDis)
                        {
                            minDis = tempDis;
                            pos = item;
                        }
                    }
                    if (pos > -1)
                        Global.client.move(pos);
                }
            }
        }
        void checkEndGame()
        {
            if (Global.client!=null && Global.client.gameRes != GameRes.NO_WIN)
            {
                string result = Global.client.gameRes == GameRes.ME_WIN ? "你赢了" : "你输了";
                Debug.Log(result);
            }
        }

    }

}