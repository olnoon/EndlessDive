using UnityEngine;
using System.Collections;

public enum OrbKind
{
    HP,
    XP
}
public class OrbScript : MonoBehaviour
{
    Coroutine deSpawnRoutine;
    public OrbKind orbKind; 

    void Awake()
    {
        Debug.Log($"orb가 {transform.position}에 생성됨");
    }

    void Start()
    {
        ChangeOrbColor();
    }

    void OnEnable()
    {
        if (deSpawnRoutine != null)//일정시간 후 디스폰해주는 루틴 활성화
        {
            StopCoroutine(deSpawnRoutine);
        }
        deSpawnRoutine = StartCoroutine(DeSpawnOrb());
        ChangeOrbColor();
    }

    void ChangeOrbColor()
    {
        if (orbKind == OrbKind.HP)//체력오브라고 판단시 빨간색으로 변경
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.7f);
        }
        else if (orbKind == OrbKind.XP)//경험치오브라고 판단시 테니공색?으로 변경
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
        if (collision.tag == "Player")//플레이어와 충돌했을 때 플레이어의 xp증가
        {
            if (orbKind == OrbKind.XP)
            {
                collision.GetComponent<PlayerStat>().addXP();
                Debug.Log("경험치 증가");
            }
            else if (orbKind == OrbKind.HP)
            {
                collision.GetComponent<PlayerStat>().HP.Heal(1);
                Debug.Log("체력 회복");
            }
            gameObject.SetActive(false);
        }
    }
}
