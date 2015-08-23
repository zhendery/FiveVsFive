using UnityEngine;
using System.Collections;
using FiveVsFive;

public class SettingPanel : MonoBehaviour
{

    public UIEventListener[] buttons;

    void Start()
    {
        for (short i = 0; i < buttons.Length; ++i)
            buttons[i].onClick = onClick;
    }

    void onClick(GameObject button)
    {
        //playAudio
        switch (button.name)
        {
            case "commit":
                Debug.Log("commit");
                Global.setSceneOld(GameScenes.WELCOME);
                gameObject.SetActive(false);
                break;
            case "cancle":
                Debug.Log("cancle");
                Global.setSceneOld(GameScenes.WELCOME);
                gameObject.SetActive(false);
                break;
            case "random":
                Debug.Log("random");
                break;
        }
    }
    void Update()
    {

    }
}
