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
    public int weight = 1;

    void FixedUpdate()
    {
        MoveBullet();
    }

    void MoveBullet()//총알 움직임 제어
    {
        float distance = 5f; // 이동할 거리

        Vector2 targetPos = (Vector2)transform.position + direction * distance;
        transform.DOMove(targetPos, spendingTime);
    }

    void OnTriggerStay2D(Collider2D collision)
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
            collision.gameObject.GetComponent<EnemyStat>().HP.TakeDamage(ATK.FinalValue * weight);
            Debug.Log($"{collision.gameObject.name}에게 {ATK.FinalValue * weight}의 대미지를 가해서 체력이 {collision.gameObject.GetComponent<EnemyStat>().HP.Current}만큼 남았습니다.");
        }
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

    IEnumerator DeSpawnBullet()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (deSpawnRoutine != null)
        {
            StopCoroutine(deSpawnRoutine);
            deSpawnRoutine = null;
        }
        transform.DOKill();
    }
}
