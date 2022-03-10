using System.Collections;
using UnityEngine;


public class BallController : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private Color _color;
    private BallState _state;
    private float _speed = 5f;

    void Awake()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (_state == BallState.Moving)
        {
            Vector3 pos = transform.position;
            pos.y += _speed * Time.deltaTime;
            transform.position = pos;
        }
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
