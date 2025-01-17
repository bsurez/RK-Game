﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivalModeController : MonoBehaviour
{

    int currentAvailableNumber;
    SurvivalModeClickableItem[] clickableObjects;
    CenterItem centerItem;
    SurvivalModeTimerController timerController;
    GameOverMenu gameOverMenu;
    SoundController soundController;
    SoundTrackController soundTrackController;
    SurvivalScoreSavingController survivalScoreSavingController;



    public bool gameplayHasStarted = false;
    public bool gameplayHasFinished = false;
    //int pointsPerCorrect = 100;
    //int pointsPerCenter = 500;
    int secondsToAddPerRound = 2;


    // Use this for initialization
    void Start()
    {

        timerController = GetComponent<SurvivalModeTimerController>();
        soundController = GetComponent<SoundController>();

        centerItem = GetComponentInChildren<CenterItem>();
        clickableObjects = GetComponentsInChildren<SurvivalModeClickableItem>();
        gameOverMenu = GetComponentInChildren<GameOverMenu>();

        soundTrackController = GetComponentInChildren<SoundTrackController>();

        survivalScoreSavingController = GetComponent<SurvivalScoreSavingController>();

        //highscore = PlayerPrefs.GetInt("HighScore");

        soundTrackController.startSoundtrack();
    }

    void setInitialPositions(List<int> positions)
    {
        for (int i = 0; i < clickableObjects.Length; i++)
        {
            clickableObjects[i].SetPositionText(positions[i]);
            if (positions[i] == 0)
            {
                clickableObjects[i].Activate();
            }
        }
    }

    void deactivateAllItems()
    {
        for (int i = 0; i < clickableObjects.Length; i++)
        {
            if (clickableObjects[i].isActiveAndEnabled)
            {
                clickableObjects[i].Deactivate();
            }
        }
    }

    List<int> getNewPositions()
    {
        List<int> filledPositions = new List<int>();

        for (int i = 0; i < clickableObjects.Length; i++)
        {
            bool filled = false;

            while (!filled)
            {
                int intToAdd = Random.Range(0, clickableObjects.Length);

                if (filledPositions.Count <= 0)
                {
                    filledPositions.Add(intToAdd);
                    filled = true;
                }
                else
                {
                    bool canBeAdded = true;
                    for (int j = 0; j < filledPositions.Count; j++)
                    {
                        if (intToAdd == filledPositions[j])
                        {
                            canBeAdded = false;
                        }
                    }
                    if (canBeAdded)
                    {
                        filledPositions.Add(intToAdd);
                        filled = true;
                    }
                }
            }
        }
        return filledPositions;
    }

    public void clickedPosition(int position)
    {
        deactivateItem(position);

        //addScore(pointsPerCorrect);

        soundController.playAudio(position);

        if (position + 1 >= clickableObjects.Length)
        {
            centerItem.activate();
        }
        else
        {
            activateNextItem(position);
        }
    }

    void activateNextItem(int currentItem)
    {
        int nextItem = currentItem + 1;

        for (int i = 0; i < clickableObjects.Length; i++)
        {
            if (clickableObjects[i].GetPosition() == nextItem)
            {
                clickableObjects[i].Activate();
            }
        }
    }

    void deactivateItem(int item)
    {
        for (int i = 0; i < clickableObjects.Length; i++)
        {
            if (clickableObjects[i].GetPosition() == item)
            {
                clickableObjects[i].Deactivate();
            }
        }
    }

    public void GameOver()
    {
        gameplayHasFinished = true;
        deactivateAllItems();
        if (timerController.getTotalTime() > PlayerPrefs.GetFloat("LongestSurvivalGame"))
        {
            PlayerPrefs.SetFloat("LongestSurvivalGame", timerController.getTotalTime());
            gameOverMenu.triggerGameOverMenuSurvival(timerController.getTotalTime(), timerController.getTotalTime());
        }
        else
        {
            gameOverMenu.triggerGameOverMenuSurvival(timerController.getTotalTime(), PlayerPrefs.GetFloat("LongestSurvivalGame"));
        }
        soundTrackController.triggerFadeOut();
       
    }

    void roundCompleted()
    {
        GetComponentInChildren<MenuAnimations>().playRoundCompletedAnim();
        timerController.addTimeToMainTimer(secondsToAddPerRound);
        setInitialPositions(getNewPositions());
    }

    public void roundOver()
    {
        GetComponentInChildren<MenuAnimations>().playErrorAnim();

   
        resetGameplay();

        if (centerItem.isActive())
        {
            centerItem.deactivate();
        }

    }




    public void startGameplay()
    {
        gameplayHasStarted = true;
        setInitialPositions(getNewPositions());
        



        ///////////////////////////////////////////
        //CODE PORTION TO SEND INFO TO THE TUTORIAL

        if (isFirstTimeSurvivalGame())
        {
            timerController.pauseTimer();
            GetComponentInChildren<TutorialScript>().startTutorial();
        }

        ///////////////////////////////////////////

    }

    public void restartGame()
    {
        ///////////////////////////////////////////
        // RESTARTING THE CURRENT SCENE
        //////////////////////////////////////////

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void centerItemClicked()
    {
        centerItem.deactivate();
        soundController.playRoundSound();
        roundCompleted();

        ///////////////////////////////////////////
        //CODE PORTION TO SEND INFO TO THE TUTORIAL

        if (isFirstTimeSurvivalGame())
        {
            GetComponentInChildren<TutorialScript>().centerItemClicked();
        }

        ///////////////////////////////////////////
    }

    void resetGameplay()
    {
        deactivateAllItems();
        setInitialPositions(getNewPositions());
    }
  

    bool isFirstTimeSurvivalGame()
    {
        int isFirstTime = PlayerPrefs.GetInt("isFirstTimeSurvival");
        if (isFirstTime != 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void saveCurrentScore()
    {
        survivalScoreSavingController.SaveSurvivalScore((int)timerController.getTotalTime());
    }
    private void OnApplicationQuit()
    {
        if (gameplayHasFinished)
        {
            saveCurrentScore();
        }
    }

}
