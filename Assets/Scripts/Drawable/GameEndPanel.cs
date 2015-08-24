using UnityEngine;
using System.Collections;
using FiveVsFive;

public class GameEndPanel : MonoBehaviour
{
    Transform blurPanel;
    UISprite againBtn, animate, res;
    UIPlaySound clickSound,winLoseSound;
    public AudioClip[] winLoseAudio;
    void Start()
    {
        blurPanel = transform.FindChild("blurPanel");
        againBtn=transform.FindChild("again").GetComponent<UISprite>();
        animate = transform.FindChild("animation").GetComponent<UISprite>();
        res = transform.FindChild("res").GetComponent<UISprite>();

        againBtn.GetComponent<UIEventListener>().onClick = again;
        clickSound = transform.GetComponent<UIPlaySound>();
        winLoseSound = res.transform.GetComponent<UIPlaySound>();
    }
    void again(GameObject button)
    {
        if (Global.client.audioOn)
            clickSound.Play();
        Global.client.again();
    }

    bool isShow = false;
    void FixedUpdate()
    {
        if (Global.client == null)
            return;
        if (Global.client.gameRes != GameRes.NO_WIN)
        {
            if (!isShow)
            {
                int ran = Random.Range(1, 6);
                string win = "lose";
                if(Global.client.gameRes==GameRes.ME_WIN){
                    win = "win";
                    winLoseSound.audioClip = winLoseAudio[0];
                }
                else
                    winLoseSound.audioClip = winLoseAudio[1];

                winLoseSound.Play();

                animate.spriteName = win + ran;
                res.spriteName = win;
                animate.GetComponent<UISpriteAnimation>().namePrefix = win + ran;
                againBtn.spriteName = "again_" + win;

                blurPanel.gameObject.SetActive(true);
                res.gameObject.SetActive(true);
                animate.gameObject.SetActive(true);
                againBtn.gameObject.SetActive(true);
                isShow = true;
            }
        }
        else
        {
            if (isShow)
            {
                blurPanel.gameObject.SetActive(false);
                res.gameObject.SetActive(false);
                animate.gameObject.SetActive(false);
                againBtn.gameObject.SetActive(false);
                isShow = false;
            }
        }
    }

}
