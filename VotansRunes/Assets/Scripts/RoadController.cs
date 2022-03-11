using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadController : MonoBehaviour
{
    public static RoadController instance;

    public List<BoxCollider2D> Turns;
    private LinkedList<BallController> _ballsOnRoad;

    private float _baseSpeed = 3f;
    private float _speedOnStart = 15f;

    private float _ballSize = 0.7f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _ballsOnRoad = new LinkedList<BallController>();

        StartCoroutine(SpawnBalls());
    }

    private IEnumerator SpawnBalls()
    {
        for (int i = 0; i < 25; i++)
        {
            _ballsOnRoad.AddLast(BallSpawner.instance.SpawnOnRoad());
            _ballsOnRoad.Last.Value.SetSpeed(1f);
            _ballsOnRoad.Last.Value.OnMovingBallCollision += OnMovingBallCollision;
            _ballsOnRoad.Last.Value.OnRoadBallCollision += OnRoadBallCollision;

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnRoadBallCollision(BallController ballOnRoad, BallController movingBall)
    {
        ReplaceBalls();
    }

    private void OnMovingBallCollision(BallController ballOnRoad, BallController movingBall)
    {
        var ballInList = _ballsOnRoad.Find(ballOnRoad);

        if (ballInList != null) return;

        // определить впихнуть до или после

        // впихнуть

        // передвинуть
        ReplaceBalls();
    }

    private void ReplaceBalls()
    {
        // план: от последнего элемента к первому передвинуть шары друг к другу
        var currentNode = _ballsOnRoad.Last;

        for (int i = 0; i < _ballsOnRoad.Count - 1; i++)
        {
            
        }

        // если рядом есть 3 шара одного цвета - уничтожить их

    }
}
