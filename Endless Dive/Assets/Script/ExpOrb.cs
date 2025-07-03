using UnityEngine;
using System.Collections;

public class ExpOrb : MonoBehaviour
{
    Coroutine deSpawnRoutine;

    void Awake()
    {
        Debug.Log($"orb가 {transform.position}에 생성됨");
    }

    void OnEnable()//일정시간 후 디스폰해주는 루틴 활성화
    {
        if (deSpawnRoutine != null)
        {
            StopCoroutine(deSpawnRoutine);
        }
        deSpawnRoutine = StartCoroutine(DeSpawnOrb());
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
        if (collision.tag == "Player")//플레이어와 충돌했을 때 플레이어의 xp증가
        {
            collision.GetComponent<PlayerStat>().addXP();
            gameObject.SetActive(false);
        }
    }
}
