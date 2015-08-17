using UnityEngine;
using System.Collections;

public class gameButtons : MonoBehaviour {

    UIEventListener[] buttons;
	void Start () {
        UIEventListener[] btns=transform.GetComponentsInChildren<UIEventListener>();
        buttons=new UIEventListener[btns.Length];
        for (short i = 0; i < btns.Length;++i )
        {
            buttons[i] = btns[i];
            buttons[i].onClick = onClick;
        }
	}
	void onClick(GameObject button)
        {
            //playAudio
            switch (button.name)
            {
                case "quit"://返回到开始界面
                    //出现离开提示
                    break;
                case "setting"://设置用户头像和昵称
                    Debug.Log("setting");
                    break;
                default:
                    Debug.Log("no button!!");
                    break;
            }
        }
}
