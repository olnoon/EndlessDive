using System;
using UnityEngine;

public class PlayerMoveSet : MonoBehaviour
{
    public SingleStatRuntime speed;//이동속도
    [SerializeField] Rigidbody2D rb;//리지드바디
    public float minX = -10f;//X좌표 최소제한
    public float maxX = 10f;//X좌표 최대제한
    public float minY = -5f;//Y좌표 최소제한
    public float maxY = 5f;//Y좌표 최대제한
    public GameObject mineral;//타겟팅된 미네랄

    void Awake()
    {
        speed = new SingleStatRuntime(GetComponent<PlayerStat>().stat.speed.FinalValue);
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (Time.timeScale != 0)
        {
            Move();
            UpdateFacing();
        }
    }

    void UpdateFacing()//마우스 방향 보는 함수
    {
        float currectScaleX = Math.Abs(transform.localScale.x);
        float lookingDirec = GetComponent<PlayerStat>().mousePos.x > transform.position.x ? currectScaleX : -currectScaleX;

        Vector2 toSize = new Vector2(lookingDirec, transform.localScale.y);

        transform.localScale = toSize;
    }

    void Move()//플레이어 움직임을 WASD에 따라 제어
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
        Vector2 targetPos = rb.position + moveDir * speed.FinalValue * Time.fixedDeltaTime;

        float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);
        Vector2 clampedTarget = new Vector2(clampedX, clampedY);

        rb.MovePosition(clampedTarget);
    }

    bool isCheckGetUpKey()//WASD키를 입력하지 않고 있는지 확인
    {
        return Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D);
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Mineral")//mineral에 캐고 있는 광물 오브젝트 할당
        {
            mineral = collision.transform.parent.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Mineral")//mineral에 캐고 있는 광물 오브젝트 초기화
        {
            mineral = null;
        }
    }
}