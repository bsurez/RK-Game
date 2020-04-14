﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour {

    GameController controller;
    public float roundTime = 30.0f;
    public float actionTime = 10.0f;
    public float countDownTime = 3.0f;
    TimerText mainTimer;    public float roundTimeLeft; float actionTimeLeft;
    public float countDownTimerLeft;
    bool roundTimeHasStarted, timerIsOver;
    bool countDownTimerIsOver;



	// Use this for initialization
	void Start () {
        controller = GetComponent<GameController>();

        roundTimeLeft = roundTime;
        actionTimeLeft = actionTime;

        roundTimeHasStarted = false;
        timerIsOver = false;

        startCountDownTimer();
	}
	
	// Update is called once per frame
	void Update () {

        countDownTimer();

        if (roundTimeHasStarted && !timerIsOver)
        {
            roundTimeLeft -= Time.deltaTime;

            actionTimeLeft -= Time.deltaTime;

            if (roundTimeLeft<=0 || actionTimeLeft <= 0)
            {
                controller.GameOver();
                timerIsOver = true;
            }
        }
    }

    void startRoundTimer()
    {
        roundTimeHasStarted = true;
    }

    public void restartActionTimer()
    {
        actionTimeLeft = actionTime;
    }

    public void restartTimers()
    {
        roundTimeLeft = roundTime;
        restartActionTimer();
        timerIsOver = false;
    }

    public float getActionTimeLeft()
    {
        return actionTimeLeft;
    }

    public void startCountDownTimer()
    {
        countDownTimerIsOver = false;
        countDownTimerLeft = countDownTime;
        GetComponentInChildren<CountDownTimer>().activate();
    }

    public void countDownTimer()
    {
        if (countDownTimerLeft <= 0 && !controller.gameplayHasStarted)
        {

            controller.startGameplay();
            startRoundTimer();
            GetComponentInChildren<CountDownTimer>().deactivate();
        }
        else
        {
            countDownTimerLeft -= Time.deltaTime;
        }
    }

    public float getCountdownTimerLeft()
    {
        return countDownTimerLeft;
    }
}