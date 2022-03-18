using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameEndController : MonoBehaviour
{
    public TextMeshProUGUI Score;

    public void ShowScore(int score)
    {
        Score.text = score.ToString();  
    }

    public void Play()
    {
        GameManager.instance.Play();
        gameObject.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
