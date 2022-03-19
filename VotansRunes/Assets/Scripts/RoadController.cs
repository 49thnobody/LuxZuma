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

    private PathContoller _path;

    private void Start()
    {
        _path = GetComponent<PathContoller>();
        _ballsOnRoad = new LinkedList<BallController>();

        if (GameManager.instance.GameState != GameState.Play) return;

        StartCoroutine(SpawnBalls());
    }

    public void Restart()
    {
        StartCoroutine(SpawnBalls());

        StartCoroutine(SpawningBalls());
    }

    public void Continue()
    {
        StartCoroutine(SpawningBalls());
    }

    private IEnumerator SpawnBalls()
    {
        for (int i = 0; i < 25; i++)
        {
            _ballsOnRoad.AddLast(BallSpawner.instance.SpawnOnRoad());
            var currentNode = _ballsOnRoad.Last;
            while (currentNode.Previous != null)
            {
                currentNode.Previous.Value.transform.position += new Vector3(-0.2f, 0, 0);
                currentNode = currentNode.Previous;
            }

            yield return new WaitForSeconds(0.1f);
        }

        SetBalls();
    }

    private void SetBalls()
    {
        var currentNode = _ballsOnRoad.First;

        while (currentNode != null)
        {
            currentNode.Value.SetNode(currentNode);
            currentNode.Value.OnMovingBallCollision += OnMovingBallCollision;
            currentNode.Value.OnBallDestroy += OnBallDestroy;
            currentNode = currentNode.Next;
        }
    }

    private void OnBallDestroy(BallController ball)
    {
        _ballsOnRoad.Remove(ball);
        Destroy(ball.gameObject);
    }

    private IEnumerator SpawningBalls()
    {
        while (GameManager.instance.GameState == GameState.Play)
        {
            _ballsOnRoad.AddLast(BallSpawner.instance.SpawnOnRoad());
            _ballsOnRoad.Last.Value.OnMovingBallCollision += OnMovingBallCollision;
            _ballsOnRoad.Last.Value.OnBallDestroy += OnBallDestroy;
            _ballsOnRoad.Last.Value.SetNode(_ballsOnRoad.Last);
            _ballsOnRoad.Last.Value.ChaseBall();

            yield return new WaitForSeconds(2f);
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
        if (roadBall.MovingTo == Direction.Down || roadBall.MovingTo == Direction.Up)
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
        mbInList.Value.SetNode(mbInList);
        mbInList.Value.transform.position = newPos;
        mbInList.Value.SetState(BallState.Road);
        mbInList.Value.OnMovingBallCollision += OnMovingBallCollision;
        mbInList.Value.OnBallDestroy += OnBallDestroy;
        mbInList.Value.NextPoint = roadBall.NextPoint;

        // чек если рядом есть 3 шарика одного цвета
        CheckForMatches(mbInList);
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

        if (ballsInChain.Count < 3)
        {
            return;
        }
        else
        {
            GameManager.instance.AddScore(100 * ballsInChain.Count);
        }

        var prevBall = _ballsOnRoad.Find(ballsInChain[0]).Previous;
        var nextBall = _ballsOnRoad.Find(ballsInChain[ballsInChain.Count - 1]).Next;

        for (int i = ballsInChain.Count - 1; i >= 0; i--)
        {
            BallController ball = ballsInChain[i];
            _ballsOnRoad.Remove(ball);
            Destroy(ball.gameObject);
        }

        if (_ballsOnRoad.Count == 0)
            return; // win

        // убрать дыру между шарами
        StartCoroutine(RemoveGap(prevBall, nextBall));

        CheckForMatches(nextBall ?? prevBall);
    }

    private IEnumerator RemoveGap(LinkedListNode<BallController> prevBall, LinkedListNode<BallController> nextBall)
    {
        if (nextBall == null) yield break;

        var currentNode = prevBall;
        while (currentNode != null && currentNode.Next != null)
        {
            currentNode.Value.MoveBack();
            currentNode = currentNode.Previous;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            GameManager.instance.Pause();
        }
    }
}