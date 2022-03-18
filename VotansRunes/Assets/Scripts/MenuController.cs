using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
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
