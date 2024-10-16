using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public Text ScoreText;
    public Text TimeText;
    public Text gameOverScoreText;
    public Text gameOverBestText;
    private float time;
    private int score;
    private bool bGameOver;
    public Car car;
    public Animator gameOverAnimator;
    void Start()
    {
        score = 0;
        bGameOver = false;
    }

    void Update()
    {
        UpdateTimer();
    }

    void UpdateTimer()
    {
        time += Time.deltaTime;
        int timer = (int)time;
        int seconds = timer % 60;
        int minutes = timer / 60;
        TimeText.text = "Time:";
        if (minutes > 0) TimeText.text += minutes.ToString() + " : ";
        TimeText.text += seconds.ToString();
    }

    public void UpdateScore(int val)
    {
        score += val;
        ScoreText.text = "Score: " + score.ToString();
    }

    public void GameOver()
    {
        if (bGameOver)
        {
            return;
        }
        SetScore();
        gameOverAnimator.SetTrigger("GameOver");
        bGameOver = true;
        car.FallApart();
        foreach(var basicMovement in GameObject.FindObjectsOfType<BasicMovement>())
        {
            basicMovement.moveSpeed = 0;
            basicMovement.rotateSpeed = 0;
        }
        

    }

    void SetScore()
    {
        if(score > PlayerPrefs.GetInt("best"))
        {
            PlayerPrefs.SetInt("best", score);
        }
        gameOverScoreText.text = "Score:" + score.ToString();
        gameOverBestText.text = "Best: " + PlayerPrefs.GetInt("best");
    }
}
