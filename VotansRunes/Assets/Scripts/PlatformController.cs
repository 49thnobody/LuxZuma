using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public static PlatformController instance;

    void Awake()
    {
        instance = this;

        _transform = GetComponent<Transform>();
    }

    private Transform _transform;
    private BallController ActiveBall;

    private float _speed = 10f;

    private void Start()
    {
        ActiveBall = BallSpawner.instance.Spawn();
    }

    void Update()
    {
        Vector3 pos = _transform.position;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            pos.x -= _speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            pos.x += _speed * Time.deltaTime;
        }

        ActiveBall.transform.position = pos;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            // выпустить мяч

            // заспавнить новый
            ActiveBall = BallSpawner.instance.Spawn();
        }
    }
}
