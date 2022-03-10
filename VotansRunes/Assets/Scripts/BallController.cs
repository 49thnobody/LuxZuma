using System.Collections;
using UnityEngine;


public class BallController : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private Color _color;
    private BallState _state;

    void Awake()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (_state == BallState.Moving)
        {
            // двигать шар вверх
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
