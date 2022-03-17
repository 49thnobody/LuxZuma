using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public static PlatformController instance;

    void Awake()
    {
        instance = this;
    }

    private BallController _activeBall;

    private float _speed = 10f;

    private void Start()
    {
        _activeBall = BallSpawner.instance.SpawnOnPlatform();
    }

    void Update()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            pos.x -= _speed * Time.deltaTime;
            if (pos.x < -8) pos.x = -8;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            pos.x += _speed * Time.deltaTime;
            if (pos.x > 8) pos.x = 8;
        }

        transform.position = pos;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PushBall();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeBall();
        }
    }

    private void ChangeBall()
    {
        DestroyImmediate(_activeBall.gameObject);
        // заспавнить новый
        _activeBall = BallSpawner.instance.SpawnOnPlatform(_activeBall.Color);
    }

    private void PushBall()
    {
        // выпустить мяч
        BallSpawner.instance.SpawnMoving(_activeBall);
        DestroyImmediate(_activeBall.gameObject);
        // заспавнить новый
        _activeBall = BallSpawner.instance.SpawnOnPlatform();
    }
}
