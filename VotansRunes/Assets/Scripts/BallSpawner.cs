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

    public Transform PlatformPosition;
    public BallController SpawnOnPlatform()
    {
        return SpawnOnPlace(PlatformPosition, BallState.Active);
    }

    public void SpawnMoving(BallController template)
    {
        var movingBall = Instantiate(BallPrefab);
        movingBall.transform.position = PlatformPosition.position;
        movingBall.Set(template);
        movingBall.SetState(BallState.Moving);
    }

    public Transform RoadSpawnPosition;
    public BallController SpawnOnRoad()
    {
        return SpawnOnPlace(RoadSpawnPosition, BallState.Road);
    }

    private BallController SpawnOnPlace(Transform parent, BallState ballState)
    {
        var newBall = Instantiate(BallPrefab, parent);
        int color = Random.Range(0, BallSprites.Count - 1);
        newBall.Set((Color)color, BallSprites[color]);
        newBall.SetState(ballState);
        return newBall;
    }
}
