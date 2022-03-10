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
    public List<Sprite> BallSprites;

    public Transform SpawningPorition;
    public BallController Spawn()
    {
        var newBall = Instantiate(BallPrefab, SpawningPorition);
        int color = Random.Range(0, BallSprites.Count - 1);
        newBall.Set((Color)color, BallSprites[color]);
        newBall.SetState(BallState.Active);

        return newBall;
    }
}
