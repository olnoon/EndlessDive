using UnityEngine;
using DG.Tweening;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] int speed;//속도
    [SerializeField] Rigidbody2D rigid;//리지드
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;
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

        Vector2 moveDir = new Vector2(xSpeed, ySpeed).normalized;
        Vector2 targetPos = (Vector2)transform.position + moveDir * speed * Time.deltaTime;

        //위치 제한
        float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);
        Vector2 clampedTarget = new Vector2(clampedX, clampedY);

        //DOTween을 이용한 이동
        transform.DOMove(clampedTarget, 0.1f); 
    }

    bool isCheckGetUpKey()//WASD키를 입력하지 않고 있는지 확인
    {
        return Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D);
    }
}
