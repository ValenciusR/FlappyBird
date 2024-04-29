using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreWindow : MonoBehaviour
{
    private Text scoreText;
    private Text highscoreText;
    private void Awake()
    {
        scoreText = transform.Find("ScoreText").GetComponent<Text>();
        highscoreText = transform.Find("HighScoreText").GetComponent<Text>();

        //Debug.Log(scoreText);
    }

    private void Start()
    {
        highscoreText.text = "HIGHSCORE: " + Score.GetHighScore().ToString();
    }

    private void Update()
    {
        scoreText.text = Level.GetInstance().GetPipesPassedSpawned().ToString();
    }
}
