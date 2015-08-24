using UnityEngine;
using System.Collections;
using FiveVsFive;

public class SettingPanel : MonoBehaviour
{

    public UIEventListener[] buttons;
    UISprite[] backs;
    UIInput nameInput;
    UIToggle audioButton;
    string myName;
    int myLogo, audioOn;
    UIPlaySound clickSound;

    void Awake()
    {
        for (short i = 0; i < buttons.Length; ++i)
            buttons[i].onClick = onClick;

        backs = new UISprite[6];
        for (short i = 0; i < 6; ++i)
            backs[i] = buttons[i].transform.FindChild("back").GetComponent<UISprite>();

        nameInput = transform.FindChild("myName").GetComponent<UIInput>();
        audioButton = transform.FindChild("audioOn").GetComponent<UIToggle>();

        clickSound = transform.GetComponent<UIPlaySound>();
    }
    void OnEnable()
    {
        myName = PlayerPrefs.GetString(Names.keyName);
        myLogo = PlayerPrefs.GetInt(Names.keyLogo)-1;
        audioOn = PlayerPrefs.GetInt(Names.keyAudio, 0);

        setShow();
    }
    void onClick(GameObject button)
    {
        if (Global.client.audioOn)
            clickSound.Play();
        switch (button.name)
        {
            case "commit":
                if ("".Equals(nameInput.value))
                {
                    //提示
                    return;
                }
                myName = nameInput.value;
                audioOn = audioButton.value ? 1 : 0;

                PlayerPrefs.SetString(Names.keyName, myName);
                PlayerPrefs.SetInt(Names.keyLogo, myLogo+1);
                PlayerPrefs.SetInt(Names.keyAudio, audioOn);

                Global.client.myLogo = myLogo+1;
                Global.client.myName = myName;
                Global.client.audioOn = audioButton.value;

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

        audioButton.value = audioOn == 1;
    }
}
