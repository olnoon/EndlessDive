using UnityEngine;

public enum State
{
    Normal,
    Poisoned,
    Death
}
public enum EnemyKind
{
    A,
    B
}
public class EnemyMove : MonoBehaviour
{
    public State state;//적의 현재 상태
    public EnemyKind kind;//적의 종류
    public float basicSpeed;//기본 이동속도
    public RatioStatRuntime SPD;//이동 속도에 영향을 주는 능력치
    public float minX = -10f;//움직일 수 있는 곳 제한(X의 최솟값)
    public float maxX = 10f;//움직일 수 있는 곳 제한(X의 최댓값)
    public float minY = -5f;//움직일 수 있는 곳 제한(y의 최솟값)
    public float maxY = 5f;//움직일 수 있는 곳 제한(y의 최댓값)
    [SerializeField] bool isKnockAble = true;//넉백 가능/불가능 여부
    [SerializeField] float knockbackPower;//넉백을 얼마나 시킬지의 수치
    public GameObject player;//감지되고 다가갈 플레이어
    [SerializeField] Rigidbody2D rb;//리지드바디

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindAnyObjectByType<PlayerMoveSet>().gameObject;
        SetKnockBack();
    }

    void FixedUpdate()
    {
        Move();
    }

    public void Revive(Vector2 spawnPos)//재사용시 상태, 위치 등을 초기화 시켜주는 함수
    {
        GetComponent<EnemyStat>().Revive();
        transform.position = spawnPos;
        state = State.Normal;
        gameObject.SetActive(true);
        SetKnockBack();
    }

    void Move()//움직임 제어(플레이어와 해당 오브젝트 사이의 방향을 알아내서 해당 방향으로 향함)
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerMoveSet>().gameObject;
            return;
        }
        Vector2 currentPos = rb.position;
        Vector2 targetPos = player.transform.position;
        Vector2 dir = (targetPos - currentPos).normalized;
        
        float FinalSpeed = SPD != null ? basicSpeed * SPD.FinalRatio: basicSpeed;

        rb.MovePosition(currentPos + dir * FinalSpeed * Time.fixedDeltaTime);
    }

    void SetKnockBack()//넉백 설정
    {
        if (!isKnockAble)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = knockbackPower;
        }
    }
}
