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
    public List<BallController> _ballsForInspector
    {
        get
        {
            List<BallController> result = new List<BallController>();

            foreach (BallController ball in _ballsOnRoad)
            {
                result.Add(ball);
            }

            return result;
        }
    }

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
            _ballsOnRoad.Last.Value.OnMovingBallCollision += OnMovingBallCollision;
            if (_ballsOnRoad.Last.Previous != null)
            {
                _ballsOnRoad.Last.Previous.Value.SetNext(_ballsOnRoad.Last.Value);
                var currentNode = _ballsOnRoad.Last;
                while (currentNode.Previous != null)
                {
                    currentNode.Previous.Value.transform.position += new Vector3(-0.2f, 0, 0);
                    currentNode = currentNode.Previous;
                }
            }


            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnMovingBallCollision(BallController roadBall, BallController movingBall)
    {
        var ballInList = _ballsOnRoad.Find(roadBall);

        if (ballInList == null) return;

        PlaceOnRoad(roadBall, movingBall, ballInList);
    }

    private void PlaceOnRoad(BallController roadBall, BallController movingBall, LinkedListNode<BallController> ballInList)
    {
        // определить впихнуть до или после
        var rbPos = roadBall.transform.position;
        var mbPos = movingBall.transform.position;

        Vector3 newPos = movingBall.transform.position;
        movingBall.transform.SetParent(roadBall.transform.parent);
        if (roadBall.MovingTo == Direction.Down)
        {
            newPos = new Vector3(rbPos.x, rbPos.y - 0.3f, 0f);
            _ballsOnRoad.AddBefore(ballInList, movingBall);
        }
        else
        {
            var prevPos = ballInList.Previous == null ? ballInList.Value.transform.position : ballInList.Previous.Value.transform.position;
            var nextPos = ballInList.Next == null ? ballInList.Value.transform.position : ballInList.Next.Value.transform.position;

            var prevDistanceSquare = (prevPos - mbPos).sqrMagnitude;
            var nextDistanceSquare = (nextPos - mbPos).sqrMagnitude;

            if (roadBall.MovingTo == Direction.Left)
            {
                if (prevDistanceSquare < nextDistanceSquare)
                {
                    newPos = new Vector3(prevPos.x + 0.2f, rbPos.y, 0f);
                    _ballsOnRoad.AddBefore(ballInList, movingBall);
                }
                else
                {
                    newPos = new Vector3(nextPos.x - 0.2f, rbPos.y, 0f);
                    _ballsOnRoad.AddAfter(ballInList, movingBall);
                }
            }
            else
            {
                if (prevDistanceSquare < nextDistanceSquare)
                {
                    newPos = new Vector3(prevPos.x - 0.2f, rbPos.y, 0f);
                    _ballsOnRoad.AddBefore(ballInList, movingBall);
                }
                else
                {
                    newPos = new Vector3(nextPos.x + 0.2f, rbPos.y, 0f);
                    _ballsOnRoad.AddAfter(ballInList, movingBall);
                }
            }
        }

        var mbInList = _ballsOnRoad.Find(movingBall);
        mbInList.Value.transform.position = newPos;
        mbInList.Value.SetState(BallState.Road);
        mbInList.Value.OnMovingBallCollision += OnMovingBallCollision;
        mbInList.Value.NextPoint = roadBall.NextPoint;

        ResetNextToMe();

        // чек если рядом есть 3 шарика одного цвета
        CheckForMatches(mbInList);
    }

    private void ResetNextToMe()
    {
        var currentNode = _ballsOnRoad.First;
        while (currentNode.Next != null)
        {
            currentNode.Value.SetNext(currentNode.Next.Value);
            currentNode = currentNode.Next;
        }
    }

    private void CheckForMatches(LinkedListNode<BallController> movingBall)
    {
        if (movingBall == null) return;

        var firstBallInChain = movingBall;

        while (firstBallInChain.Previous != null && firstBallInChain.Previous.Value.Color == movingBall.Value.Color)
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

        if (ballsInChain.Count < 3) return;

        var prevBall = _ballsOnRoad.Find(ballsInChain[0]).Previous;
        var nextBall = _ballsOnRoad.Find(ballsInChain[ballsInChain.Count - 1]).Next;

        for (int i = ballsInChain.Count - 1; i >= 0; i--)
        {
            BallController ball = ballsInChain[i];
            _ballsOnRoad.Remove(ball);
            Destroy(ball.gameObject);
        }

        // убрать дыру между шарами


    }
}