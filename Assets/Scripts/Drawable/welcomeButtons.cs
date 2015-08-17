using UnityEngine;
using System.Collections;
using System.Threading;
using System;

namespace FiveVsFive
{
    public class welcomeButtons : MonoBehaviour
    {

        public UIEventListener[] buttons;
        Transform welTop, welDown, startUI;

        void Start()
        {
            for (short i = 0; i < buttons.Length; ++i)
                buttons[i].onClick = onClick;

            for (short i = 3; i < 7; ++i)
                buttons[i].GetComponent<UISprite>().alpha = 0.01f;

            startUI = transform.parent;
            welTop = startUI.FindChild("welTop");
            welDown = startUI.FindChild("welDown");

            gameScene = GameScenes.WELCOME;
            setSceneOld(GameScenes.WELCOME);
        }

        void onClick(GameObject button)
        {
            //playAudio
            switch (button.name)
            {
                case "local"://挑战电脑 -->直接开始游戏

                    LocalServer localServer = new LocalServer();
                    localServer.start();

                    LanClient.instance.startLocal();
                    StartCoroutine(whenStart());
                    break;
                case "friend"://挑战朋友 -->选择哪种朋友
                    setSceneOld(GameScenes.CHOOSE_FRIEND);
                    StartCoroutine(showButtons(null));
                    break;
                case "search"://搜寻附近朋友 -->进入搜寻界面
                    setSceneOld(GameScenes.SEARCHING_FRIEND);
                    StartCoroutine(showButtons(null));

                    LanServer lanServer = new LanServer();
                    lanServer.start();

                    LanClient.instance.startLan();
                    break;
                case "wlan"://挑战远程朋友 -->开启远程服务，出现分享信息与按钮，等待加入
                    break;
                case "help"://挑战须知 -->游戏帮助界面
                    Debug.Log("hepl");
                    break;
                case "clientLan"://加入附近朋友 -->开启局域网客户端
                    //LanClient.instance.start("");
                    break;
                case "clientWlan"://加入远程朋友 -->文本框，输入确定后开启远程客户端
                    break;
                case "commitWlan"://确定
                    //LanClient.instance.start("");
                    break;
                case "confirmFriend"://确定朋友
                    break;
                default:
                    Debug.Log("no button!!");
                    break;
            }
        }

        void hideShowWel()
        {
            StartCoroutine(hideShowWels());
        }
        IEnumerator hideShowWels()
        {//要在0.5秒内移动掉960 ，也就是在50步移动掉
            Vector3 velocityTop = new Vector3(0,
                gameScene == GameScenes.WELCOME ?  //说明从其他场景返回欢迎屏幕
                -0.06f : 0.06f, 0);//不是9.6，因为ui有一个比例 0.003125
            WaitForSeconds time = new WaitForSeconds(0.01f);
            for (int i = 0; i < 50; ++i)
            {
                welTop.Translate(velocityTop, Space.Self);
                welDown.Translate(-velocityTop, Space.Self);
                yield return time;
            }
        }
        delegate void CallFunc();
        GameScenes oldScene, gameScene;
        IEnumerator showButtons(CallFunc call)
        {
            WaitForSeconds time = new WaitForSeconds(0.05f);
            short s1 = 0, s2 = 0, e1 = 0, e2 = 0;

            if (oldScene == GameScenes.CHOOSE_FRIEND && gameScene == GameScenes.WELCOME)
            {//从选朋友界面返回
                s1 = 3; e1 = 7; s2 = 0; e2 = 3;
            }
            if (oldScene == GameScenes.WELCOME && gameScene == GameScenes.CHOOSE_FRIEND)
            {//从欢迎界面到选朋友界面
                s1 = 0; e1 = 3; s2 = 3; e2 = 7;
            }
            if (oldScene == GameScenes.GAME_SCENE && gameScene == GameScenes.WELCOME)
            {//从游戏界面离开
                s1 = e1 = 0; s2 = 0; e2 = 3;
            }
            if (oldScene == GameScenes.WELCOME && gameScene == GameScenes.GAME_SCENE)
            {//从游戏界面离开
                s1 = 0; e1 = 3; s2 = e2 = 0;
            }
            if (gameScene == GameScenes.SEARCHING_FRIEND)
            {//只可能是从选朋友界面到的
                s1 = 3; e1 = 7; s2 = e2 = 0;
            }

            for (int i = s1; i < e1; ++i)
            {
                StartCoroutine(hideShowBtn(buttons[i].transform, true));//将隐藏的按钮们
                yield return time;
            }
            for (int i = s2; i < e2; ++i)
            {
                StartCoroutine(hideShowBtn(buttons[i].transform, false));//将显示的按钮们
                yield return time;
            }
            if (call != null)//回调函数，可以在动画结束后做些什么，比如显示其他东西
                call();
        }

        IEnumerator hideShowBtn(Transform btn, bool hide)
        {
            Vector3 velocity = new Vector3(0, hide ? 3 : -3, 0);
            float alpha = hide ? -0.03f : 0.03f;
            WaitForSeconds time = new WaitForSeconds(0.01f);
            for (int i = 0; i < 30; ++i)
            {
                btn.Rotate(velocity);
                btn.GetComponent<UISprite>().alpha += alpha;
                yield return time;
            }
        }

        IEnumerator whenStart()
        {
            while (!LanClient.instance.isRunning)
                yield return null;

            //连接了，打开欢迎画面
            setSceneOld(GameScenes.GAME_SCENE);
            StartCoroutine(showButtons(hideShowWel));
        }

        void Update()
        {
            //监听back键
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                switch (gameScene)//表示当前场景
                {
                    case GameScenes.WELCOME:
                        Application.Quit();
                        break;
                    case GameScenes.CHOOSE_FRIEND://回到欢迎画面
                        setSceneOld(GameScenes.WELCOME);
                        StartCoroutine(showButtons(null));
                        break;
                    default:
                        Debug.Log("没有这个场景");
                        break;
                }
            }
        }
        public void setSceneOld(GameScenes newScene)
        {
            oldScene = gameScene;
            gameScene = newScene;
        }
    }
    public enum GameScenes
    {
        WELCOME,
        CHOOSE_FRIEND,
        SEARCHING_FRIEND,
        INPUT_IP,
        GAME_SCENE
    };
}
