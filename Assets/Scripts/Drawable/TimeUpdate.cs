using UnityEngine;
using System.Collections;
using System;

public class TimeUpdate : MonoBehaviour
{
    UILabel timeLabel;
    void Start()
    {
        timeLabel = this.GetComponent<UILabel>();
        InvokeRepeating("updateTime", 0, 60);
    }
    void updateTime()
    {
        timeLabel.text = DateTime.Now.ToString("HH:mm");
    }
}
