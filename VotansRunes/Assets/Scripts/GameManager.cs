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

        GameState = GameState.Menu;
    }

    public GameState GameState;

    public GameObject MenuPanel;
    public GameObject PausePanel;

    public TextMeshProUGUI Score;

    public HeartsController Hearts;

    private void Start()
    {
        Hearts.Restart();
        GameState = GameState.Play;
    }

    public void TakeDamage()
    {
        if (Hearts.TakeDamage())
            GameState = GameState.GameOver;
    }

    public void Pause()
    {
        GameState = GameState.Pause;

    }

    public void ToMenu()
    {
        GameState = GameState.Menu;

    }
}
