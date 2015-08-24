using UnityEngine;
using System.Collections;
using FiveVsFive;

public class GameEndPanel : MonoBehaviour
{
    Transform blurPanel;
    UISprite againBtn, animation,res;
    void Start()
    {
        blurPanel = transform.FindChild("blurPanel");
        againBtn=transform.FindChild("again").GetComponent<UISprite>();
        animation = transform.FindChild("animation").GetComponent<UISprite>();
        res = transform.FindChild("res").GetComponent<UISprite>();

        againBtn.GetComponent<UIEventListener>().onClick = again;
    }
    void again(GameObject button)
    {
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
                }
                animation.spriteName = win + ran;
                res.spriteName = win;
                animation.GetComponent<UISpriteAnimation>().namePrefix = win + ran;
                againBtn.spriteName = "again_" + win;

                blurPanel.gameObject.SetActive(true);
                res.gameObject.SetActive(true);
                animation.gameObject.SetActive(true);
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
                animation.gameObject.SetActive(false);
                againBtn.gameObject.SetActive(false);
                isShow = false;
            }
        }
    }

}
