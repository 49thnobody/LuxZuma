using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    public GameState GameState;

    public MenuController MenuPanel;
    public PauseController PausePanel;
    public GameEndController GameOverPanel;

    public TextMeshProUGUI Score;
    private int _score;

    public HeartsController Hearts;

    private void Start()
    {
        Hearts.Restart();
        ToMenu();
    }

    public void TakeDamage()
    {
        if (Hearts.TakeDamage())
        {
            GameState = GameState.GameOver;
            GameOverPanel.gameObject.SetActive(true);
            GameOverPanel.ShowScore(_score);
        }
    }

    public void AddScore(int score)
    {
        _score += score;
        Score.text = _score.ToString();
    }

    public void Pause()
    {
        GameState = GameState.Pause;
        PausePanel.gameObject.SetActive(true);
    }

    public void ToMenu()
    {
        GameState = GameState.Menu;
        MenuPanel.gameObject.SetActive(true);
    }

    public void Play()
    {
        GameState = GameState.Play;
        _score = 0;
        Score.text = _score.ToString();

        
        RoadController.instance.Restart();
    }

    public void Continue()
    {
        GameState = GameState.Play;
        RoadController.instance.Continue();
    }
}
