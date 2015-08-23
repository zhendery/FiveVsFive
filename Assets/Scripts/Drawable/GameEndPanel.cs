using UnityEngine;
using System.Collections;
using FiveVsFive;

public class GameEndPanel : MonoBehaviour
{
    Transform blurPanel,toShow,particle;
    void Start()
    {
        blurPanel = transform.FindChild("blurPanel");
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
                if(Global.client.gameRes==GameRes.ME_WIN){
                   // toShow=transform.FindChild("");
                  //  particle = transform.FindChild("");
                }
                blurPanel.gameObject.SetActive(true);
                //toShow.gameObject.SetActive(true);
                isShow = true;
            }
        }
        else
        {
            if (isShow)
            {
                blurPanel.gameObject.SetActive(false);
                //toShow.gameObject.SetActive(false);
                //particle.gameObject.SetActive(false);
                isShow = false;
            }
        }
    }

}
