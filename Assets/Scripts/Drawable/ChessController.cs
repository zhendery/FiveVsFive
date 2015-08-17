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

        const int BLOCK_WID = 215;

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

        static float[] index2Pos = { -2.5f, -1.3f, 0f, 1.3f, 2.5f };
        public void FixedUpdate()//自动刷新
        {
            if (RuleController.instance.isGaming)
            {
                ChessBoard board = ChessBoard.instance;
                Chess c = null;
                for (int i = 0; i < 10; ++i)
                {
                    c = board.getChess(i);
                    if (c.isMoved())
                    {
                        StartCoroutine(moveAction(c.x, c.y, i));
                        board.checkJaTiao(i);
                    }
                    if (c.isOvered())
                    {
                        StartCoroutine(overAction(c.isMine, i));
                        board.checkJaTiao(i);
                    }
                    c.setOld(c);
                }
                showTips();
                checkEndGame();
            }
        }

        IEnumerator moveAction(int x, int y, int index)
        {
            Vector3 newPos = new Vector3(index2Pos[x], index2Pos[y]);
            Vector3 velocity = (newPos - chesses[index].localPosition) / 10.0f;
            WaitForSeconds time = new WaitForSeconds(0.01f);
            for (int i = 0; i < 10; ++i)//0.1秒移动
            {
                chesses[index].Translate(velocity, Space.World);
                yield return time;
            }
        }
        IEnumerator overAction(bool isMine, int index)
        {
            Vector3 newEuler = (!(isMine && RuleController.instance.meFisrt)) ? Vector3.zero : v_pi;
            Vector3 velocity = (newEuler - chesses[index].eulerAngles) / 10.0f;
            WaitForSeconds time = new WaitForSeconds(0.01f);
            for (int i = 0; i < 10; ++i)//0.1秒旋转
            {
                chesses[index].Rotate(velocity);
                yield return time;
            }
        }

        static Vector3[] v_up = { new Vector3(0, 0, -1f), new Vector3(0, 0, -0.5f), new Vector3(0, 0, -0.3f), new Vector3(0, 0, -0.2f) };

        public void upChess(int index)
        {
            if (index < 0 || index > 9)
                return;
            LanClient.instance.upChess(index);
            int selected = ChessBoard.instance.selected;
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
                ChessBoard.instance.selected = index;
            }
            else//此棋子已经抬起，则将其放下
            {
                int upIndex = (int)((Vector2)chesses[selected].localPosition).magnitude;
                bool white = chesses[selected].eulerAngles.magnitude < 1;
                chesses[selected].Translate(white ? -v_up[upIndex] : v_up[upIndex]);
                //playAudio 放下
                ChessBoard.instance.selected = -1;
            }
        }


        void showTips()
        {
            for (int i = 0; i < 25; ++i)
                tips[i].alpha = 0;
            foreach (int index in ChessBoard.instance.getCanGo())
                tips[index].alpha = 1;
        }

        Chess oldChess;
        void OnMouseDown()
        {
            ChessBoard board = ChessBoard.instance;
            if (RuleController.instance.isMyTurn == GameState.MY_TURN)
            {
                centerScreen = new Vector2(Screen.width / 2, Screen.height / 2);//实际无需
                aspect = 720f / Screen.height;//实际无需

                float minDis = 40f;
                int index = -1;
                for (int i = 0; i < 10; ++i)
                {
                    if (board.getChess(i).isMine)
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
                    if (board.getChess(index).isMine)//点击棋子并可操纵
                        upChess(index);
                }
                else if (ChessBoard.instance.selected > -1)//点击空白并且已有选择棋子，则移动
                {
                    int pos = -1;
                    minDis = 40f;
                    foreach (int item in board.getCanGo())
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
                        oldChess = new Chess(ChessBoard.instance.getChess(ChessBoard.instance.selected));
                        LanClient.instance.move(pos);
                    }
                }
            }
        }

        public void checkEndGame()
        {
            if (RuleController.instance.gameRes != GameRes.NO_WIN)
            {
                UILabel lable = GameObject.Find("Label").GetComponent<UILabel>();
                lable.text = RuleController.instance.gameRes == GameRes.ME_WIN ? "你赢了" : "对手赢了";
            }
        }

        Vector2 getChessBoardPos(int pos)
        {
            int y = pos / 5 - 2, x = pos % 5 - 2;
            return new Vector2(x * 80, y * 80);
        }

    }

}