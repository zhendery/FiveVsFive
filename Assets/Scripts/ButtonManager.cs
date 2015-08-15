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
                    string localIP="127.0.0.1";
                    LocalServer server = new LocalServer();
                    server.start(localIP);

                    LanClient.instance.start(localIP);
                    LanClient.instance.newTurn();
                    break;
                case "server":
                    LanServer.getInstance().startServer();
                    break;
                case "client":
                    LanClient.instance.start("");
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
