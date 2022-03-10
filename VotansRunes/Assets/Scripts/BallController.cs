using System.Collections;
using UnityEngine;


public class BallController : MonoBehaviour
{
    private CircleCollider2D _collider;
    private SpriteRenderer _sprite;
    private Color _color;
    private BallState _state;
    private float _speed = 12f;

    private Direction _currentDirection;

    public delegate void BallCollision(BallController ballOnRoad, BallController movingBall);
    public event BallCollision OnMovingBallCollision;
    public event BallCollision OnRoadBallCollision;

    void Awake()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponentInChildren<CircleCollider2D>();
    }

    void Update()
    {
        if (_state == BallState.Moving)
        {
            Vector3 pos = transform.position;
            pos.y += _speed * Time.deltaTime;
            transform.position = pos;
        }
        if (_state == BallState.OnRoad)
        {
            Vector3 pos = transform.position;

            switch (_currentDirection)
            {
                case Direction.Left:
                    pos.x -= _speed * Time.deltaTime;
                    break;
                case Direction.Rigth:
                    pos.x += _speed * Time.deltaTime;
                    break;
                case Direction.Bottom:
                    pos.y -= _speed * Time.deltaTime;
                    break;
                default:
                    break;
            }

            transform.position = pos;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_state != BallState.OnRoad) return;

        if (_collider.tag == "GameOver")
            ;//gameover

        var collisionBall = collision.gameObject.GetComponent<BallController>();
        if (collisionBall == null) return;

        if (collisionBall._state == BallState.OnRoad)
            OnRoadBallCollision?.Invoke(this, collisionBall);
        else if (collisionBall._state == BallState.Moving)
            OnMovingBallCollision?.Invoke(this, collisionBall);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_state != BallState.OnRoad) return;

        var collisionTurn = collision.gameObject.GetComponent<TurnController>();
        if (collisionTurn == null) return;

        _currentDirection = collisionTurn.NextDirection;
    }

    public void Set(Color color, Sprite sprite)
    {
        _color = color;
        _sprite.sprite = sprite;
    }
    public void SetState(BallState state)
    {
        _state = state;
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
