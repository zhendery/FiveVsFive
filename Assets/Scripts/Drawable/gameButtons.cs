using UnityEngine;
using System.Collections;
using FiveVsFive;

public class GameButtons : MonoBehaviour {

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
                
                default:
                    Debug.Log("no button!!");
                    break;
            }
        }
}
