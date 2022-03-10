using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadController : MonoBehaviour
{
    public static RoadController instance;

    private List<Vector2> _angles;
    public List<BoxCollider2D> Turns;
    private List<BallController> _ballsOnRoad;

    private float _baseSpeed = 3f;
    private float _speedOnStart = 15f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //_ballsOnRoad = new();
        //_angles = new()
        //{
        //    new Vector2(9.5f, 5),
        //    new Vector2(-8f, 5),
        //    new Vector2(-8f, 3),
        //    new Vector2(8f, 3),
        //    new Vector2(8f, 1),
        //    new Vector2(-8f, 1),
        //    new Vector2(-8f, -1),
        //    new Vector2(8f, -1),
        //    new Vector2(8f, -3),
        //    new Vector2(-8f, -3)
        //};

        for (int i = 0; i < 25; i++)
        {
            _ballsOnRoad.Add(BallSpawner.instance.SpawnOnPlatform());
        }
    }

    private void Update()
    {
        
    }
}
