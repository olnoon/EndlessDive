using UnityEngine;
using DG.Tweening;
using System.Collections;

public enum OrbKind
{
    HP,
    XP
}

public class OrbScript : MonoBehaviour
{
    Coroutine deSpawnRoutine;//디스폰루틴이 충돌되는 걸 방지하기 위한 루틴
    public OrbKind orbKind;//오브의 종류
    public bool isFinded;//발견상태의 여부
    public GameObject player;//해당 오브젝트가 점점 빨라지는 속도로 다가갈 플레이어

    void Awake()
    {
        Debug.Log($"orb가 {transform.position}에 생성");
    }

    void Start()
    {
        ChangeOrbColor();
    }

    public void MoveToPlayer()//플레이어 방향으로 다가가는 메서드
    {
        if (!isFinded || player == null) return;

        if (deSpawnRoutine != null)
        {
            StopCoroutine(deSpawnRoutine);
            deSpawnRoutine = null;
        }

        StartCoroutine(MoveTowardPlayerRoutine());
    }

    IEnumerator MoveTowardPlayerRoutine()//플레이어에게 다가가며 플레이어의 속도를 넘을 때 까지 가속해줌
    {
        float acceleration = 5f;
        float currentSpeed = 0f;
        float speedBuffer = 2f;

        while (true)
        {
            if (player == null) yield break;

            // 플레이어 속도 가져오기
            float playerSpeed = 0f;
            if (player.TryGetComponent<Rigidbody2D>(out var rb2d))
                playerSpeed = rb2d.linearVelocity.magnitude;

            float targetSpeed = playerSpeed + speedBuffer;

            // 가속
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, targetSpeed);

            // 방향 계산 및 이동
            Vector3 dir = (player.transform.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;

            yield return null;
        }
    }
    void OnEnable()
    {
        if (deSpawnRoutine != null)//일정 시간 후 디스폰해주는 루틴 활성화
        {
            StopCoroutine(deSpawnRoutine);
        }
        deSpawnRoutine = StartCoroutine(DeSpawnOrb());
        ChangeOrbColor();
    }

    void ChangeOrbColor()//orbKind에 따라 오브의 색깔 변경
    {
        if (orbKind == OrbKind.HP)//체력 오브라고 판단시 빨간색으로 변경
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.7f);
        }
        else if (orbKind == OrbKind.XP)//경험치 오브라고 판단시 테니공색?으로 변경
        {
            GetComponent<SpriteRenderer>().color = new Color(0.8282f, 1, 0, 0.7f);
        }
    }
    
    void OnDisable()//비활성화 될 때 deSpawnRoutine 초기화
    {
        if (deSpawnRoutine != null)
        {
            StopCoroutine(deSpawnRoutine);
            deSpawnRoutine = null;
        }
    }

    IEnumerator DeSpawnOrb()//디스폰 루틴
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (orbKind == OrbKind.XP)//플레이어와 충돌했을 때 플레이어의 xp 증가
            {
                collision.GetComponent<PlayerStat>().addXP();
            }
            else if (orbKind == OrbKind.HP)//플레이어와 충돌했을 때 플레이어의 체력 회복
            {
                collision.GetComponent<PlayerStat>().HP.Heal(1);
            }
            gameObject.SetActive(false);
        }
    }
}
