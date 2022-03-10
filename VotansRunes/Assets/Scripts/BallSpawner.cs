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
        var newBall = Instantiate(BallPrefab, SpawningPorition);
        Color color = (Color)Random.Range(0, BallSprites.Count - 1);
        newBall.Set(color, BallSprites[color]);

        return newBall;
    }
}
