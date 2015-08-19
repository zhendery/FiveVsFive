using UnityEngine;
using System.Collections;

public class whoseTurn : MonoBehaviour {

    public TweenRotation arrow;
    TweenScale chess1, chess2;
	void Start () {
        chess1 = transform.FindChild("chess1").GetComponent<TweenScale>();
        chess2 = transform.FindChild("chess2").GetComponent<TweenScale>();
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
            arrow.PlayForward();
        if (Input.GetKeyDown(KeyCode.B))
            arrow.PlayReverse();


	}
}
