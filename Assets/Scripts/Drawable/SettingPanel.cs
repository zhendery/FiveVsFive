using UnityEngine;
using System.Collections;
using FiveVsFive;

public class SettingPanel : MonoBehaviour
{

    public UIEventListener[] buttons;
    UISprite[] backs;
    UIInput nameInput;
    string myName;
    int myLogo, audioOn;

    void Awake()
    {
        for (short i = 0; i < buttons.Length; ++i)
            buttons[i].onClick = onClick;

        backs = new UISprite[6];
        for (short i = 0; i < 6; ++i)
            backs[i] = buttons[i].transform.FindChild("back").GetComponent<UISprite>();

        nameInput = transform.FindChild("myName").GetComponent<UIInput>();
    }
    void OnEnable()
    {
        myName = PlayerPrefs.GetString(Names.keyName);
        myLogo = PlayerPrefs.GetInt(Names.keyLogo);
        audioOn = PlayerPrefs.GetInt(Names.keyAudio, 0);

        setShow();
    }
    void onClick(GameObject button)
    {
        //playAudio
        switch (button.name)
        {
            case "commit":
                if ("".Equals(nameInput.value))
                {
                    //提示
                    return;
                }
                myName = nameInput.value;

                PlayerPrefs.SetString(Names.keyName, myName);
                PlayerPrefs.SetInt(Names.keyLogo, myLogo);
                PlayerPrefs.SetInt(Names.keyAudio, audioOn);

                Global.client.myLogo = myLogo;
                Global.client.myName = myName;

                Global.setSceneOld(GameScenes.WELCOME);
                gameObject.SetActive(false);
                break;
            case "cancle":
                Global.setSceneOld(GameScenes.WELCOME);
                gameObject.SetActive(false);
                break;
            case "random":
                myName = Names.getRandomName().name;
                setShow();
                break;
            default://是头像
                myLogo = int.Parse(button.name.Substring(4))-1;
                setShow();
                break;
        }
    }

    Color selectedColor = new Color(1, 0.41f, 0.71f);

    void setShow()
    {
        nameInput.value = myName;

        for (short i = 0; i < 6; ++i)
            backs[i].color = Color.white;
        backs[myLogo].color = selectedColor;
        
        //设置声音开关
    }
}
