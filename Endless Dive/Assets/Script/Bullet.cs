using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public Vector2 target;
    [SerializeField] int spendingTime;//target 까지 가는데 걸리는 시간
    [SerializeField] Vector2 direction;
    Coroutine deSpawnRoutine;
    public SingleStatRuntime ATK;
    Rigidbody2D rb;
    [SerializeField] BuffKind buffKind;
    [SerializeField] float buffDuration;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        MoveBullet();
    }

    void MoveBullet()//총알 움직임 제어
    {
        float distance = 5f; // 이동할 거리
        float speed = distance / spendingTime;

        Vector2 targetPos = (Vector2)transform.position + direction * distance;
        rb.MovePosition(Vector2.MoveTowards(rb.position, targetPos, speed * Time.fixedDeltaTime));
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")//적과의 충돌 판정으로 대미지
        {
            this.gameObject.SetActive(false);
            transform.DOKill();
            if (deSpawnRoutine != null)
            {
                StopCoroutine(deSpawnRoutine);
                deSpawnRoutine = null;
            }
            
            DealDamage(collision.gameObject);

            if (buffDuration > 0)
            {
                collision.gameObject.GetComponent<EnemyStat>().StartPoissonCour(gameObject, ATK.FinalValue, buffDuration);
            }
        }
    }

    void DealDamage(GameObject enemy, int weight = 1)//대미지를 가하는 메서드
    {
        enemy.GetComponent<EnemyStat>().HP.TakeDamage(ATK.FinalValue * weight);
        enemy.GetComponent<EnemyStat>().DetectDamage();
        // Debug.Log($"{gameObject.name} > {enemy.name}에게 {ATK.FinalValue * weight}의 대미지. 남은 HP {enemy.GetComponent<EnemyStat>().HP.Current}");
    }

    public void Reset()//Bullet이 생성 또는 재사용 될 때 초기화 시켜주는 메서드
    {
        ResetDirection();
        if (deSpawnRoutine != null)
        {
            StopCoroutine(deSpawnRoutine);
        }
        deSpawnRoutine = StartCoroutine(DeSpawnBullet());
    }

    void ResetDirection()//힘이 가해지는 방향 초기화
    {
        if (target == null)
        {
            direction = new Vector2(0, 0);
            return;
        }
        Vector3 diff = target - (Vector2)transform.position;
        direction = new Vector2(diff.x, diff.y).normalized;
    }

    IEnumerator DeSpawnBullet()//불렛 디스폰
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (deSpawnRoutine != null)//불렛이 디스폰 될 때 다음 재사용에서 디스폰 루틴이 충돌하지 않게 하기 위해 만듦
        {
            StopCoroutine(deSpawnRoutine);
            deSpawnRoutine = null;
        }
        transform.DOKill();
    }
}
