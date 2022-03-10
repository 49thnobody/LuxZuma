using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public static BallSpawner instance;

    private void Awake()
    {
        instance = this;
    }

    public BallController BallPrefab;
    public Dictionary<Color, Sprite> BallSprites;

    public Transform SpawningPorition;
    public BallController Spawn()
    {
       return Instantiate(BallPrefab, SpawningPorition);
    }
}
