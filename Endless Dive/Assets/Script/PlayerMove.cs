using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] int speed;//속도
    [SerializeField] Rigidbody2D rb;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
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
        
        // 이동 방향 계산
        Vector2 moveDir = new Vector2(xSpeed, ySpeed).normalized;

        // 목표 위치 계산
        Vector2 targetPos = (Vector2)transform.position + moveDir * speed * Time.deltaTime;

        // 위치 제한 적용
        float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);
        Vector2 clampedTarget = new Vector2(clampedX, clampedY);

        // 제한된 위치로 향하는 속도 계산
        Vector2 velocity = (clampedTarget - (Vector2)transform.position) / Time.fixedDeltaTime;

        // 속도 적용
        rb.linearVelocity = velocity;
    }

    bool isCheckGetUpKey()//WASD키를 입력하지 않고 있는지 확인
    {
        return Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D);
    }
}
