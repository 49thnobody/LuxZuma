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

    public Transform PlatformPorition;
    public BallController SpawnOnPlatform()
    {
        var newBall = Instantiate(BallPrefab, PlatformPorition);
        int color = Random.Range(0, BallSprites.Count - 1);
        newBall.Set((Color)color, BallSprites[color]);
        newBall.SetState(BallState.Active);

        return newBall;
    }

    public Transform RoadSpawnPosition;
    public BallController SpawnOnRoad()
    {
        var newBall = Instantiate(BallPrefab, RoadSpawnPosition);
        int color = Random.Range(0, BallSprites.Count - 1);
        newBall.Set((Color)color, BallSprites[color]);
        newBall.SetState(BallState.OnRoad);

        return newBall;
    }
}
