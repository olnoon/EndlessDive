using UnityEngine;

public enum State
{
    Normal,
    Death
}
public enum EnemyKind
{
    A,
    B
}
public class EnemyMove : MonoBehaviour
{
    public State state;
    public EnemyKind kind;
    public SingleStatRuntime speed;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;
    [SerializeField] bool isKnockAble = true;
    [SerializeField] float knockbackPower;
    public GameObject player;
    [SerializeField] Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindAnyObjectByType<PlayerMoveSet>().gameObject;
        speed = new SingleStatRuntime(GetComponent<EnemyStat>().stat.speed.FinalValue);
        SetKnockBack();
    }

    void Update()
    {
        Move();
    }

    public void Revive(Vector2 spawnPos)//재사용시 상태, 위치등을 초기화 시켜주는 함수
    {
        GetComponent<EnemyStat>().Revive();
        speed = new SingleStatRuntime(GetComponent<EnemyStat>().stat.speed.FinalValue);
        transform.position = spawnPos;
        state = State.Normal;
        gameObject.SetActive(true);
        SetKnockBack();
    }

    void Move()//움직임 제어
    {
        // 현재 위치와 목표 위치
        Vector2 currentPos = rb.position;
        Vector2 targetPos = player.transform.position;

        // 목표까지의 방향 벡터
        Vector2 dir = (targetPos - currentPos).normalized;

        // 속도 적용
        rb.linearVelocity = dir * speed.FinalValue;
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
