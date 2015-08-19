using UnityEngine;
using System.Collections;
using FiveVsFive;

public class WhoseTurn : MonoBehaviour
{

    public TweenRotation arrow;
    public UISprite myLogo, yourLogo;
    public UILabel myName, yourName;
    TweenScale chess1, chess2;
    TweenRotation chess1R, chess2R;
    void Start()
    {
        whose = GameState.NO_TURN;
        chess1 = transform.FindChild("chess1").GetComponent<TweenScale>();
        chess2 = transform.FindChild("chess2").GetComponent<TweenScale>();
        chess1R = transform.FindChild("chess1").GetComponent<TweenRotation>();
        chess2R = transform.FindChild("chess2").GetComponent<TweenRotation>();
    }

    void setHead()
    {
        if (Global.client.myColor)
        {
            chess1R.PlayForward();
            chess2R.PlayReverse();
        }
        else
        {
            chess1R.PlayReverse();
            chess2R.PlayForward();
        }
        StartCoroutine(setLogos());
    }

    IEnumerator setLogos()
    {
        while ("".Equals(Global.client.myName))
            yield return null;
        myLogo.spriteName = "" + Global.client.myLogo;
        myName.text = Global.client.myName;

        while ("".Equals(Global.client.yourName))
            yield return null;
        yourLogo.spriteName = "" + Global.client.yourLogo;
        yourName.text = Global.client.yourName;
    }

    bool isGaming = false;
    GameState whose;
    void Update()
    {
        if (Global.client!=null && Global.client.isGaming)
        {
            if (!isGaming)
            {
                setHead();
                isGaming = true;
            }
            if (Global.client.whoseTurn != whose)//游戏方变了
            {
                if (Global.client.whoseTurn == GameState.MY_TURN)
                {
                    chess1.style = UITweener.Style.PingPong;
                    chess1.PlayForward();
                    chess2.style = UITweener.Style.Once;
                    chess2.PlayReverse();

                    arrow.PlayForward();
                }
                else
                {
                    chess2.style = UITweener.Style.PingPong;
                    chess2.PlayForward();
                    chess1.style = UITweener.Style.Once;
                    chess1.PlayReverse();

                    arrow.PlayReverse();
                }
                whose = Global.client.whoseTurn;
            }

            else//没有在游戏中
                isGaming = false;
        }
    }
}
