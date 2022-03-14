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
            _ballsOnRoad.Last.Value.OnRoadBallCollision += OnRoadBallCollision;

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnRoadBallCollision(BallController ballOnRoad, BallController movingBall)
    {
       // Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
       // body.velocity = pushDir * pushPower;
    }

    private void OnMovingBallCollision(BallController roadBall, BallController movingBall)
    {
        var ballInList = _ballsOnRoad.Find(roadBall);

        if (ballInList == null) return;

        PlaceOnRoad(roadBall, movingBall, ballInList);

        // чек если рядом есть 3 шарика одного цвета
        CheckForMatches(movingBall);
    }

    private void PlaceOnRoad(BallController roadBall, BallController movingBall, LinkedListNode<BallController> ballInList)
    {
        // определить впихнуть до или после
        var rbPos = roadBall.transform.position;
        var mbPos = movingBall.transform.position;

        var prevPos = ballInList.Previous.Value.transform.position;
        var nextPos = ballInList.Next.Value.transform.position;

        var prevDistanceSquare = (prevPos - mbPos).sqrMagnitude;
        var nextDistanceSquare = (nextPos - mbPos).sqrMagnitude;

        movingBall.transform.SetParent(roadBall.transform.parent);

        Vector3 newPos = movingBall.transform.position;
        // близжайший нод - предыдущий тому, в которого врезались
        if (prevDistanceSquare < nextDistanceSquare)
        {
            newPos = new Vector3(prevPos.x, rbPos.y, 0f);

            // ooooo
            // 01234  
            if (roadBall.MovingTo == Direction.Left)
                _ballsOnRoad.AddBefore(ballInList, movingBall);
            // ooooo
            // 43210  
            else if (roadBall.MovingTo == Direction.Right)
                _ballsOnRoad.AddAfter(ballInList, movingBall);
        }
        // близжайший нод - следующий после того, в которого врезались
        else
        {
            newPos = new Vector3(nextPos.x, rbPos.y, 0f);

            // ooooo
            // 01234  
            if (roadBall.MovingTo == Direction.Left)
                _ballsOnRoad.AddAfter(ballInList, movingBall);
            // ooooo
            // 43210  
            else if (roadBall.MovingTo == Direction.Right)
                _ballsOnRoad.AddBefore(ballInList, movingBall);
        }

        movingBall.transform.position = newPos;
        movingBall.SetState(BallState.Road);
        movingBall.SetSpeed(_baseSpeed);
        movingBall.NextPoint = roadBall.NextPoint;
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

        for (int i = ballsInChain.Count - 1; i >= 0; i--)
        {
            BallController ball = ballsInChain[i];
            Destroy(ball.gameObject);
        }

        // убрать дыру между шарами
    }
}