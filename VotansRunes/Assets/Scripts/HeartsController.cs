using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartsController : MonoBehaviour
{
    private void Start()
    {
        Restart();
    }

    public void Restart()
    {
        _currentHP = _maxHP;

        for (int i = 0; i < Hearts.Length; i++)
            Hearts[i].sprite = FullHeart;
    }

    private int _currentHP;
    private const int _maxHP = 3;

    public SpriteRenderer[] Hearts;
    public Sprite FullHeart;
    public Sprite EmptyHeart;

    public bool TakeDamage()
    {
        _currentHP--;

        for (int i = 0; i < Hearts.Length; i++)
        {
            Hearts[i].sprite = i + 1 < _currentHP ? FullHeart : EmptyHeart;
        }

        return _currentHP <= 0;
    }
}
