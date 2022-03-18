using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public void Play()
    {
        GameManager.instance.Continue();
        gameObject.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
