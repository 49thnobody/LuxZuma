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
    private Vector3 _velocity;

    public delegate void BallCollision(BallController ballOnRoad, BallController otherBall);
    public event BallCollision OnMovingBallCollision;
    public event BallCollision OnRoadBallCollision;

    private PathContoller Path;

    private List<Transform> _pathPoints;
    public int NextPoint = 0;

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

            transform.position = Vector3.MoveTowards(transform.position, _pathPoints[NextPoint].position + _velocity, Time.deltaTime * _speed);

            var distanceSquare = (transform.position - _pathPoints[NextPoint].position).sqrMagnitude;

            if (distanceSquare < 0.1f * 0.1f)
            {
                if (NextPoint == _pathPoints.Count - 1)
                    return; // actually lost

                NextPoint++;
            }

            if (NextPoint < _pathPoints.Count - 1)
            {
                if (MovingTo == Direction.Down) //  down
                {
                    _body2D.constraints = RigidbodyConstraints2D.FreezePositionY;
                }
                else //not down
                {
                    _body2D.constraints = RigidbodyConstraints2D.FreezePositionX;
                }
            }
            else
            {
                _body2D.constraints = RigidbodyConstraints2D.FreezePositionY;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_state != BallState.Road) return;

        var collisionBall = collision.gameObject.GetComponent<BallController>();
        if (collisionBall == null) return;

        if (collisionBall._state == BallState.Moving)
            OnMovingBallCollision?.Invoke(this, collisionBall);

        if (collisionBall._state == BallState.Road)
            OnRoadBallCollision?.Invoke(this, collisionBall);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _velocity = Vector3.zero;
    }

    public Direction MovingTo
    {
        get
        {
            if (_pathPoints[NextPoint].position.y != _pathPoints[NextPoint + 1].position.y)
                return Direction.Down;
            if (_pathPoints[NextPoint].position.x < _pathPoints[NextPoint + 1].position.x)
                return Direction.Left;
            return Direction.Right;
        }
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

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}
