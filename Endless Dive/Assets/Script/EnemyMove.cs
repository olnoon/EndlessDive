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
    [SerializeField] float moveDuration;//속도
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindAnyObjectByType<PlayerMoveSet>().gameObject;
    }

    void Update()
    {
        Move();
    }

    public void Revive(Vector2 spawnPos)//spawnPos로 가서 해당 오브젝트를 활성화 되게 하는 함수
    {
        transform.position = spawnPos;
        state = State.Normal;
        gameObject.SetActive(true);
    }

    void Move()//움직임 제어
    {
        // 현재 위치와 목표 위치
        Vector2 currentPos = rb.position;
        Vector2 targetPos = player.transform.position;

        // 목표까지의 방향 벡터
        Vector2 dir = (targetPos - currentPos).normalized;

        // 목표까지 거리
        float distance = Vector2.Distance(currentPos, targetPos);

        // moveDuration 동안 도달하기 위한 속도 계산
        float speed = distance / moveDuration;

        // 속도 적용
        rb.linearVelocity = dir * speed;
    }
}
