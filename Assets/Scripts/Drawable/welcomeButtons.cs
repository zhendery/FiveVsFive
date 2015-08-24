using UnityEngine;
using System.Collections;
using System.Threading;
using System;

namespace FiveVsFive
{
    public class WelcomeButtons : MonoBehaviour
    {
        public UIEventListener[] buttons;
        UIPlaySound clickSound;
        public TweenPosition boardTween;

        void Start()
        {
            for (short i = 0; i < buttons.Length; ++i)
                buttons[i].onClick = onClick;

            for (short i = 4; i < 6; ++i)
                buttons[i].GetComponent<UISprite>().alpha = 0;

            Global.gameScene = GameScenes.WELCOME;
            Global.setSceneOld(GameScenes.WELCOME);

            clickSound = transform.GetComponent<UIPlaySound>();
        }

        void onClick(GameObject button)
        {
            if (Global.client.audioOn)
                clickSound.Play();
            switch (button.name)
            {
                case "local"://挑战电脑 -->直接开始游戏
                    Server server = new LocalServer();
                    server.start();

                    Global.client.startLocal();
                    Global.setSceneOld(GameScenes.GAME_SCENE);
                    StartCoroutine(waitForStart());
                    break;
                case "friend"://挑战朋友 -->选择哪种朋友
                    Global.setSceneOld(GameScenes.CHOOSE_FRIEND);
                    break;
                case "server"://搜寻附近朋友 -->进入搜寻界面
                    Global.setSceneOld(GameScenes.SEARCHING_FRIEND);

                    server = new LanServer();
                    server.start();

                    Global.client.startServer();
                    StartCoroutine(waitForStart());
                    break;
                case "client"://加入附近朋友 -->开启局域网客户端
                    Global.client.startLan();
                    Global.setSceneOld(GameScenes.JOIN_FRIEND);

                    StartCoroutine(waitForStart());
                    break;
                case "help"://挑战须知 -->游戏帮助界面
                    Debug.Log("hepl");
                    break;
                case "setting"://设置用户头像和昵称
                    Global.setSceneOld(GameScenes.SETTING);
                    break;
            }
        }

        void hideShowWels(bool hide)
        {
            Transform startUI = transform.parent;
            TweenPosition welTop = startUI.FindChild("welTop").GetComponent<TweenPosition>();
            TweenPosition welDown = startUI.FindChild("welDown").GetComponent<TweenPosition>();

            if (hide)
            {
                welTop.PlayForward();
                welDown.PlayForward();
                boardTween.PlayForward();
                if (Global.client.audioOn)
                    welTop.transform.GetComponent<UIPlaySound>().Play();
            }
            else
            {
                welTop.PlayReverse();
                welDown.PlayReverse();
                boardTween.PlayReverse();
            }
        }
        delegate void CallFunc();
        IEnumerator showButtons()
        {
            WaitForSeconds time = new WaitForSeconds(0.05f);
            short s1 = 0, s2 = 0, e1 = 0, e2 = 0;

            switch (Global.oldScene)
            {
                case GameScenes.WELCOME:
                    s1 = 0; e1 = 4;
                    if (Global.gameScene == GameScenes.CHOOSE_FRIEND)
                    { s2 = 4; e2 = 6; }
                    break;


                case GameScenes.CHOOSE_FRIEND:
                    s1 = 4; e1 = 6;
                    if (Global.gameScene == GameScenes.WELCOME)
                    { s2 = 0; e2 = 4; }
                    break;


                case GameScenes.GAME_SCENE:
                    s1 = 4; e1 = 6;
                    if (Global.gameScene == GameScenes.WELCOME)
                    {
                        s2 = 0; e2 = 4;
                        //因为从游戏界面回到欢迎界面，所以要合起来
                        hideShowWels(false);
                    }
                    break;

                case GameScenes.SETTING:
                    if (Global.gameScene == GameScenes.WELCOME)
                    { s2 = 0; e2 = 4; }
                    break;
                case GameScenes.SEARCHING_FRIEND:
                case GameScenes.JOIN_FRIEND:
                    if (Global.gameScene == GameScenes.GAME_SCENE)
                        showFriend();
                    break;
            }

            if (Global.gameScene == GameScenes.SEARCHING_FRIEND
                || Global.gameScene == GameScenes.JOIN_FRIEND)
            {//只可能是从选朋友界面到的
                showFriend();//
                s1 = 4; e1 = 6;
            }

            Global.oldScene = Global.gameScene;

            for (int i = s1; i < e1; ++i)
            {
                hideShowBtn(buttons[i].transform, true);//将隐藏的按钮们
                yield return time;
            }
            for (int i = s2; i < e2; ++i)
            {
                hideShowBtn(buttons[i].transform, false);//将显示的按钮们
                yield return time;
            }
            if (Global.gameScene == GameScenes.GAME_SCENE)
                hideShowWels(true);
            if (Global.gameScene == GameScenes.SETTING)
            {
                Transform startUI = transform.parent;
                startUI.FindChild("settingPanel").gameObject.SetActive(true);
            }
        }
        void showFriend()
        {
            GameObject showFriend = transform.FindChild("showFriend").gameObject;
            string title = "", tip = "";
            if (Global.gameScene == GameScenes.SEARCHING_FRIEND)
            {
                title = "正在搜索附近好友，请稍后...";
                tip = "“接收挑战”";
            }
            else if (Global.gameScene == GameScenes.JOIN_FRIEND)
            {
                title = "正在加入附近好友，请稍后...";
                tip = "发起挑战";
            }
            else
            {
                showFriend.SetActive(false);
                return;
            }
            tip = "请保证您和您的好友处在同一网段中，并请您的好友依次选择“挑战好友”--" + tip;
            showFriend.SetActive(true);
            UILabel titleLabel = showFriend.transform.FindChild("title").GetComponent<UILabel>(),
                tipLabel = showFriend.transform.FindChild("tip").GetComponent<UILabel>();

            titleLabel.text = title;
            tipLabel.text = tip;
        }
        void hideShowBtn(Transform btn, bool hide)
        {
            TweenRotation rotate = btn.GetComponent<TweenRotation>();
            TweenAlpha alpha = btn.GetComponent<TweenAlpha>();

            if (hide)
            {
                rotate.PlayReverse();
                alpha.PlayReverse();
            }
            else
            {
                rotate.PlayForward();
                alpha.PlayForward();
            }
        }

        IEnumerator waitForStart()
        {
            while (!Global.client.isGaming)//表示未开局
                yield return null;

            //连接了，打开欢迎画面
            Global.setSceneOld(GameScenes.GAME_SCENE);
        }

        void Update()
        {
            //监听back键
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (Global.client.audioOn)
                    clickSound.Play();
                switch (Global.gameScene)//表示当前场景
                {
                    case GameScenes.WELCOME:
                        Application.Quit();
                        break;
                    case GameScenes.CHOOSE_FRIEND://回到欢迎画面
                        Global.setSceneOld(GameScenes.WELCOME);
                        break;
                    case GameScenes.GAME_SCENE:
                        //出现离开提示{}
                        Global.client.close();
                        Global.setSceneOld(GameScenes.WELCOME);
                        break;
                    case GameScenes.SETTING:
                        transform.parent.FindChild("settingPanel").gameObject.SetActive(false);
                        Global.setSceneOld(GameScenes.WELCOME);
                        break;
                    default:
                        Debug.Log("没有这个场景");
                        break;
                }
            }
        }
        void FixedUpdate()
        {
            if (Global.gameScene != Global.oldScene)//场景变换，内容跟着变化
                StartCoroutine(showButtons());
        }
    }
}
