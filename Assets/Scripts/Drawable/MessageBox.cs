using UnityEngine;
using System.Collections;

namespace FiveVsFive
{
    public delegate void MessageBoxCallBack();
    struct MsgInfo
    {
        public bool show;
        public string message;
        public string yes;
        public MessageBoxCallBack yesCall;
        public string no;
        public MessageBoxCallBack noCall;
    }
    public class MessageBox : MonoBehaviour
    {
        public UILabel msg;
        public UILabel yesBtn, noBtn;

        Transform messageBox,blurPanel;
        void Start()
        {
            blurPanel = transform.FindChild("blurPanel");
            messageBox = transform.FindChild("message");
        }
        Vector2 leftPos = new Vector2(-220, -60),centerPos=new Vector2(0,-60);

        bool isShow;
        static MsgInfo info;

        public static void show(string message, string yes, MessageBoxCallBack yesCall, string no, MessageBoxCallBack noCall)
        {
            info = new MsgInfo
            {
                show = true,
                message = message,
                yes = yes,
                yesCall = yesCall,
                no = no,
                noCall = noCall
            };
        }

        void FixedUpdate()
        {
            if (!isShow)
            {
                if (info.show)
                {
                    isShow = true;
                    blurPanel.gameObject.SetActive(true);
                    messageBox.gameObject.SetActive(true);

                    msg.text = info.message;
                    yesBtn.text = info.yes;
                    yesBtn.GetComponent<UIEventListener>().onClick = delegate(GameObject bt)
                    {
                        info.yesCall();
                        hide(bt);
                    };
                    if (info.no==null)
                    {
                        noBtn.gameObject.SetActive(false);
                        yesBtn.transform.localPosition = centerPos;
                    }
                    else
                    {
                        noBtn.text = info.no;
                        noBtn.gameObject.SetActive(true);
                        yesBtn.transform.localPosition = leftPos;

                        if (info.noCall == null)
                            noBtn.GetComponent<UIEventListener>().onClick = hide;
                        else
                            noBtn.GetComponent<UIEventListener>().onClick = delegate(GameObject bt)
                            {
                                info.noCall();
                                hide(bt);
                            };
                    }
                }
            }
        }
        void hide(GameObject bt)
        {
            messageBox.gameObject.SetActive(false);
            blurPanel.gameObject.SetActive(false);
            info.show = false;
            isShow = false;
        }
    }
}