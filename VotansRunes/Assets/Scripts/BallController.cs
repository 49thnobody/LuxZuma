using System.Collections.Generic;
using UnityEngine;


public class BallController : MonoBehaviour
{
    private const float _roadSpeed = 1f;
    private const float _movingSpeed = 12f;
    private CircleCollider2D _collider;
    private Rigidbody2D _body2D;
    private SpriteRenderer _sprite;
    public Color Color;
    private BallState _state;
    private float _speed = 12f;

    public delegate void BallCollision(BallController ballOnRoad, BallController otherBall);
    public event BallCollision OnMovingBallCollision;
    public event BallCollision OnRoadBallCollision;
    public delegate void BallDestroy(BallController ball);  
    public event BallDestroy OnBallDestroy;

    private PathContoller Path;

    private List<Transform> _pathPoints;
    public int NextPoint = 0;
    private bool movingBack = false;

    private void Awake()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponentInChildren<CircleCollider2D>();
        _body2D = GetComponentInChildren<Rigidbody2D>();
    }

    private void Start()
    {
        if (_state == BallState.Active) _collider.enabled = false;


        Path = FindObjectOfType<RoadController>().gameObject.GetComponent<PathContoller>(); // i hate this line, realy

        _pathPoints = Path.Elements;

        if (_state == BallState.Road)
            transform.position = _pathPoints[NextPoint].position;
    }

    private void Update()
    {
        if (GameManager.instance.GameState != GameState.Play) return;

        if (_state == BallState.Moving)
        {
            Vector3 pos = transform.position;
            pos.y += _speed * Time.deltaTime;
            transform.position = pos;

            if (pos.y > 6) Destroy(gameObject);
        }
        if (_state == BallState.Road)
        {
            if (_pathPoints == null) return;
            float distanceSquare;

            transform.position = Vector3.MoveTowards(transform.position, _pathPoints[NextPoint].position, Time.deltaTime * _speed * _currentVelocity);
            distanceSquare = (transform.position - _pathPoints[NextPoint].position).sqrMagnitude;
            if (movingBack)
            {
                if (distanceSquare < 0.1f * 0.1f)
                {
                    NextPoint--;
                    if (NextPoint < 0) NextPoint = 0;
                }
            }
            else
            {
                if (distanceSquare < 0.1f * 0.1f)
                {
                    if (NextPoint == _pathPoints.Count - 1)
                    {
                        GameManager.instance.TakeDamage();

                        OnBallDestroy(this);
                        return;
                    }

                    NextPoint++;
                }
            }

            if (NextPoint < _pathPoints.Count - 1)
            {
                if (MovingTo == Direction.Down || MovingTo == Direction.Up) //  down
                {
                    if (transform.position.x != _pathPoints[NextPoint].position.x)
                        transform.position = new Vector3(_pathPoints[NextPoint].position.x, transform.position.y, 0f);
                    _body2D.constraints = RigidbodyConstraints2D.FreezePositionX;
                }
                else //not down
                {
                    if (transform.position.y != _pathPoints[NextPoint].position.y)
                        transform.position = new Vector3(transform.position.x, _pathPoints[NextPoint].position.y, 0f);
                    _body2D.constraints = RigidbodyConstraints2D.FreezePositionY;
                }
            }
            else
            {
                _body2D.constraints = RigidbodyConstraints2D.FreezePositionY;
            }

            if (MyNode.Previous == null)
                _currentVelocity = 1f;
        }
    }

    private float _currentVelocity = 1.5f;
    public LinkedListNode<BallController> MyNode;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_state != BallState.Road) return;

        var collisionBall = collision.gameObject.GetComponent<BallController>();
        if (collisionBall == null) return;

        if (collisionBall._state == BallState.Moving)
            OnMovingBallCollision?.Invoke(this, collisionBall);

        if (collisionBall._state == BallState.Road)
        {
            if (MyNode != null && MyNode.Next != null && collisionBall == MyNode.Next.Value)
            {
                if (movingBack)
                {
                    _currentVelocity = 1f;
                    movingBack = false;
                    NextPoint++;
                }
                else
                    _currentVelocity = 4f;
            }
            else if (isChasing)
            {
                if (MyNode != null && MyNode.Next != null && collisionBall == MyNode.Previous.Value)
                {
                    _currentVelocity = 1f;
                    isChasing = false;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var collisionBall = collision.gameObject.GetComponent<BallController>();
        if (collisionBall == null) return;

        if (MyNode != null && MyNode.Next != null && collisionBall == MyNode.Next.Value)
            _currentVelocity = 1f;
    }

    public Direction MovingTo
    {
        get
        {
            Vector3 startPoint;
            Vector3 endPoint;
            if (movingBack)
            {
                if (NextPoint + 1 >= _pathPoints.Count) return Direction.Right; // shouldn happen
                startPoint = _pathPoints[NextPoint + 1].position;
                endPoint = _pathPoints[NextPoint].position;
            }
            else
            {
                if (NextPoint - 1 <= 0) return Direction.Left; // shouldn happen
                startPoint = _pathPoints[NextPoint - 1].position;
                endPoint = _pathPoints[NextPoint].position;
            }

            if (startPoint.y > endPoint.y) return Direction.Down;
            if (startPoint.y < endPoint.y) return Direction.Up;
            if (startPoint.x > endPoint.x) return Direction.Left;
            if (startPoint.x < endPoint.x) return Direction.Right;

            return Direction.Left; // shouldn happen
        }
    }

    public void MoveBack()
    {
        NextPoint--;
        if (NextPoint < 0) NextPoint = 0;
        movingBack = true;
        _currentVelocity = 4f;
    }

    bool isChasing = false;
    public void ChaseBall()
    {
        _currentVelocity = 4f;
        isChasing = true;
    }

    public void Set(Color color, Sprite sprite)
    {
        Color = color;
        _sprite.sprite = sprite;
    }

    public void Set(BallController ball)
    {
        Color = ball.Color;
        _sprite.sprite = ball._sprite.sprite;
    }

    public void SetState(BallState state)
    {
        _state = state;

        switch (_state)
        {
            case BallState.Active:
                _collider.enabled = false;
                break;
            case BallState.Moving:
                _collider.enabled = true;
                _speed = _movingSpeed;
                break;
            case BallState.Road:
                _collider.enabled = true;
                _speed = _roadSpeed;
                break;
            default:
                break;
        }
    }

    public void SetNode(LinkedListNode<BallController> node)
    {
        MyNode = node;
        _currentVelocity = 1f;
    }
}
