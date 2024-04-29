using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using UnityEngine.SceneManagement;
using CodeMonkey;
using System;


public class GameOverWIndow : MonoBehaviour
{
    private Text scoreText;
    private Text highscoreText;

    private void Awake()
    {
        scoreText = transform.Find("ScoreText").GetComponent<Text>();
        highscoreText = transform.Find("HighScoreText").GetComponent<Text>();
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
        Hide();
    }

    private void Bird_OnDied(object sender, EventArgs e)
    {
        scoreText.text = Level.GetInstance().GetPipesPassedSpawned().ToString();
        if(Level.GetInstance().GetPipesPassedSpawned() > Score.GetHighScore())
        {
            highscoreText.text = "NEW HIGHSCORE";
        }
        else
        {
            highscoreText.text = "HIGHSCORE: " + Score.GetHighScore();
        }
        
        Show();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    public void RestartBtn_OnClick()
    {
        Loader.Load(Loader.Scene.GameScene);
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
    }

    public void MainMenuBtn_OnClick()
    {
        Loader.Load(Loader.Scene.MainMenu);
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick); 
    }
}
