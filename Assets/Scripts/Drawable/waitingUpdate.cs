using UnityEngine;
using System.Collections;

public class waitingUpdate : MonoBehaviour {

    UISprite sprite;
	void Start () {
        sprite = transform.GetComponent<UISprite>();
        StartCoroutine(hide());
	}

    IEnumerator hide()
    {
        while (sprite.fillAmount > 0)
        {
            sprite.fillAmount -= 0.01f;
            yield return null;
        }
        sprite.invert = true;
        StartCoroutine(show());
    }
    IEnumerator show()
    {
        while (sprite.fillAmount <1)
        {
            sprite.fillAmount += 0.01f;
            yield return null;
        }
        sprite.invert = false;
        StartCoroutine(hide());
    }
}
