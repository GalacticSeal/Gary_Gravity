using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    string timeTxt = "Time: 00:00";
    private float levelTime = 0f;
    private float startTime = 0f; //starting time of level compared to runtime
    [SerializeField] private TMP_Text timer;

    void Start()
    {
        startTime = Time.time;
    }

    void Update() {
        levelTime = Time.time-startTime;
        int minutes = (int) levelTime/60;
        int seconds = (int) levelTime % 60;

        if((levelTime % 60) >= 10) { //for formatting purposes
            timeTxt = "Time: "+minutes+":"+seconds;
        } else {
            timeTxt = "Time: "+minutes+":0"+seconds;
        }
        timer.text = timeTxt;
    }

    public void RestartTimer() {
        startTime = Time.time;
        levelTime = 0f;
    }
}
