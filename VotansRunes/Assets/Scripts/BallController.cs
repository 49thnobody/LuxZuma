using System.Collections.Generic;
using UnityEngine;


public class BallController : MonoBehaviour
{
    private CircleCollider2D _collider;
    private Rigidbody2D _body2D;
    public SpriteRenderer Sprite;
    public Color Color;
    private BallState _state;
    private float _speed = 12f;

    public delegate void BallCollision(BallController ballOnRoad, BallController movingBall);
    public event BallCollision OnMovingBallCollision;

    private PathContoller Path;

    private List<Transform> _pathPoints;
    private int nextPoint = 0;

    private void Awake()
    {
        Sprite = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponentInChildren<CircleCollider2D>();
        _body2D = GetComponentInChildren<Rigidbody2D>();
    }

    private void Start()
    {
        if (_state == BallState.Active) _collider.enabled = false;

        if (_state != BallState.Road) return;
        Path = FindObjectOfType<RoadController>().gameObject.GetComponent<PathContoller>(); // i hate this line, realy

        _pathPoints = Path.Elements;

        transform.position = _pathPoints[nextPoint].position;
    }

    private void Update()
    {
        if (_state == BallState.Moving)
        {
            Vector3 pos = transform.position;
            pos.y += _speed * Time.deltaTime;
            transform.position = pos;
        }
        if (_state == BallState.Road)
        {
            if (_pathPoints == null) return;
            transform.position = Vector3.MoveTowards(transform.position, _pathPoints[nextPoint].position, Time.deltaTime * _speed);

            var distanceSquare = (transform.position - _pathPoints[nextPoint].position).sqrMagnitude;

            if (distanceSquare < 0.1f * 0.1f)
            {
                if (nextPoint == _pathPoints.Count - 1)
                    return; // actually lost

                nextPoint++;
            }

            if (nextPoint < _pathPoints.Count - 1)
            {
                if (_pathPoints[nextPoint].position.y == _pathPoints[nextPoint + 1].position.y) // not down
                {
                    _body2D.constraints = RigidbodyConstraints2D.FreezePositionX;
                }
                else // down
                {
                    _body2D.constraints = RigidbodyConstraints2D.FreezePositionY;
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
    }

    public void Set(Color color, Sprite sprite)
    {
        Color = color;
        Sprite.sprite = sprite;
    }

    public void Set(BallController ball)
    {
        Color = ball.Color;
        Sprite.sprite = ball.Sprite.sprite;
    }

    public void SetState(BallState state)
    {
        _state = state;

        if (state != BallState.Active)
            _collider.enabled = true;
        else
            _collider.enabled = false;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}
