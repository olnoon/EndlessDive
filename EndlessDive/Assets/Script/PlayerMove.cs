using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] int speed;//속도
    [SerializeField] Rigidbody2D rigid;//리지드
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
    }

    void Move()//플레이어 움직임 제어
    {
        float xSpeed = 0;
        float ySpeed = 0;

        if (Input.GetKey(KeyCode.A))
        {
            xSpeed = -1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            ySpeed = 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            xSpeed = 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            ySpeed = -1;
        }
        
        if (isCheckGetUpKey())
        {
            xSpeed = 0;
            ySpeed = 0;
        }

        rigid.linearVelocity = new Vector2(xSpeed * speed, ySpeed * speed);
    }

    bool isCheckGetUpKey()//WASD키를 입력하지 않고 있는지 확인
    {
        return Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D);
    }
}
