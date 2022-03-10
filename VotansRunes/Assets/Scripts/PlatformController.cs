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

    private BallController ActiveBall;

    private float _speed = 10f;

    private void Start()
    {
        ActiveBall = BallSpawner.instance.SpawnOnPlatform();
    }

    void Update()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            pos.x -= _speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            pos.x += _speed * Time.deltaTime;
        }

        transform.position = pos;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PushBall();
        }
    }

    private void PushBall()
    {
        // выпустить мяч
        var movingBall = Instantiate(ActiveBall);
        movingBall.transform.position = ActiveBall.transform.position;
        movingBall.SetState(BallState.Moving);
        // заспавнить новый
        ActiveBall = BallSpawner.instance.SpawnOnPlatform();
    }
}
