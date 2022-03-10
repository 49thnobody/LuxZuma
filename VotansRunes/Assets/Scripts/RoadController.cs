using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadController : MonoBehaviour
{
    public static RoadController instance;

    public List<BoxCollider2D> Turns;
    private LinkedList<BallController> _ballsOnRoad;

    private float _baseSpeed = 3f;
    private float _speedOnStart = 15f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _ballsOnRoad = new LinkedList<BallController>();


        for (int i = 0; i < 25; i++)
        {
            _ballsOnRoad.AddLast(BallSpawner.instance.SpawnOnRoad());
            
        }
    }

    private void Update()
    {
        
    }
}
