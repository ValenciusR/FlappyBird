using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public static class Score
{

    public static void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
    }

    private static void Bird_OnDied(object sender, EventArgs e)
    {
        TrySetNewHighScore(Level.GetInstance().GetPipesPassedSpawned());
        //Debug.Log(GetHighScore());
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt("highscore");
    }

    public static bool TrySetNewHighScore(int score)
    {
        int currentHighScore = GetHighScore();
        if(score > currentHighScore) {
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void ResetHighscore()
    {
        PlayerPrefs.SetInt("highscore", 0);
        PlayerPrefs.Save();
    }
}
