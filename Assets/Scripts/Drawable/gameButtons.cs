using UnityEngine;
using System.Collections;
using FiveVsFive;

public class GameButtons : MonoBehaviour
{

    UIEventListener[] buttons;
    UIButton retractBtn;
    UIPlaySound clickSound;
    void Start()
    {
        UIEventListener[] btns = transform.GetComponentsInChildren<UIEventListener>();
        buttons = new UIEventListener[btns.Length];
        for (short i = 0; i < btns.Length; ++i)
        {
            buttons[i] = btns[i];
            buttons[i].onClick = onClick;
        }
        retractBtn = buttons[1].GetComponent<UIButton>();
        clickSound = transform.GetComponent<UIPlaySound>();
    }
    void onClick(GameObject button)
    {
        if (Global.client.audioOn)
            clickSound.Play();
        switch (button.name)
        {
            case "retract":
                Global.client.retract();
                break;
            case "defeated":
                MessageBox.show("您真要承认比对方怂吗？", "承认",
                            delegate()
                            {
                                //战绩 - 1;
                                Global.client.defeated();
                            }, "不服", null);
                break;
        }
    }
    void FixedUpdate()
    {
        retractBtn.isEnabled = (Global.client != null && Global.client.isGaming && Global.client.whoseTurn == GameState.MY_TURN);
    }
}
