using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadController : MonoBehaviour
{
    public static RoadController instance;

    private void Awake()
    {
        instance = this;
    }

    private LinkedList<BallController> _ballsOnRoad;

    private float _baseSpeed = 1f;

    private float _ballSize = 0.7f;

    private PathContoller _path;

    private void Start()
    {
        _path = GetComponent<PathContoller>();
        _ballsOnRoad = new LinkedList<BallController>();

        StartCoroutine(SpawnBalls());
    }

    private IEnumerator SpawnBalls()
    {
        for (int i = 0; i < 25; i++)
        {
            _ballsOnRoad.AddLast(BallSpawner.instance.SpawnOnRoad());
            _ballsOnRoad.Last.Value.SetSpeed(_baseSpeed);
            _ballsOnRoad.Last.Value.OnMovingBallCollision += OnMovingBallCollision;

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnMovingBallCollision(BallController roadBall, BallController movingBall)
    {
        var ballInList = _ballsOnRoad.Find(roadBall);

        if (ballInList == null) return;

        // определить впихнуть до или после
        var rbPos = roadBall.transform.position;
        var mbPos = movingBall.transform.position;

        var leftPos = rbPos.x - _ballSize;
        var rightPos = rbPos.x + _ballSize;

        var leftPosDif = Mathf.Abs(leftPos) - Mathf.Abs(mbPos.x);
        var rightPosDif = Mathf.Abs(rightPos) - Mathf.Abs(mbPos.x);

        movingBall.transform.SetParent(roadBall.transform.parent);
        if (leftPosDif < rightPosDif)
        {

            movingBall.transform.position = new Vector3(leftPos, rbPos.y, 0f);
            _ballsOnRoad.AddBefore(ballInList, movingBall);
        }
        else
        {
            movingBall.transform.position = new Vector3(rightPos, rbPos.y, 0f);
            _ballsOnRoad.AddAfter(ballInList, movingBall);
        }

        movingBall.SetState(BallState.Road);
        movingBall.SetSpeed(_baseSpeed);
        movingBall.NextPoint = roadBall.NextPoint;
        // впихнуть

        // чек если рядом есть 3 шарика одного цвета
        CheckForMatches(movingBall);
    }

    private void CheckForMatches(BallController movingBall)
    {
        var ballInList = _ballsOnRoad.Find(movingBall);

        if (ballInList == null) return;

        var firstBallInChain = ballInList;

        while (firstBallInChain.Previous != null && firstBallInChain.Previous.Value.Color == ballInList.Value.Color)
        {
            firstBallInChain = firstBallInChain.Previous;
        }

        List<BallController> ballsInChain = new List<BallController>();

        var currentNode = firstBallInChain;

        do
        {
            ballsInChain.Add(currentNode.Value);
            currentNode = currentNode.Next;
        } while (currentNode != null && currentNode.Value.Color == firstBallInChain.Value.Color);

        if (ballsInChain.Count < 4) return;

        // увеличить счет за количество шаров в цепочке

        foreach (var ball in ballsInChain)
        {
            Destroy(ball);
        }

        // убрать дыру между шарами
    }
}
