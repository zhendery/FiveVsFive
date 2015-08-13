using UnityEngine;
using System.Collections;
using System.Threading;
using System;

namespace FiveVsFive
{
    public class ButtonManager : MonoBehaviour
    {

        public UIEventListener[] buttons;

        public UILabel label;

        void Start()
        {
            for (int i = 0; i < buttons.Length; ++i)
                buttons[i].onClick = onClick;
        }

        void onClick(GameObject button)
        {
            //playAudio
            switch (button.name)
            {
                case "reset":
                    label.text = "by myself";
                    LocalServer.getInstance().startServer();
                    break;
                case "server":
                    LanServer.getInstance().startServer();
                    break;
                case "client":
                    LanClient.getInstance().startClient();
                    break;
                case "quit":
                    Application.Quit();
                    break;
                case "setting":
                    Debug.Log("setting");
                    break;
                default:
                    Debug.Log("no button!!");
                    break;
            }
        }

    }
}
