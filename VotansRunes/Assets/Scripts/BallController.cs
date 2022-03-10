using System.Collections;
using UnityEngine;


public class BallController : MonoBehaviour
{
    private BoxCollider2D _collider;
    private SpriteRenderer _sprite;
    private Color _color;
    private BallState _state;
    private float _speed = 5f;

    private Vector2 CurrentPath;

    void Awake()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponentInChildren<BoxCollider2D>();
    }

    void Update()
    {
        if (_state == BallState.Moving)
        {
            Vector3 pos = transform.position;
            pos.y += _speed * Time.deltaTime;
            transform.position = pos;
        }
        if(_state == BallState.OnRoad)
        {

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }

    public void SetState(BallState state)
    {
        _state = state;
    }

    public void Set(Color color, Sprite sprite)
    {
        _color = color;
        _sprite.sprite = sprite;
    }
}
