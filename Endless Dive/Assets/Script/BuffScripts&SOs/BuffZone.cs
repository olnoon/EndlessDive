using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum BuffToWhat
{
    ATK,
    speed,
    maxHP,
    currentHP
}

public class BuffZone : MonoBehaviour
{
    [SerializeField] BuffSO buffSO;//해당 스크립트의 변수들의 초깃값을 갖고 있는 SO

    void OnTriggerEnter2D(Collider2D collision)//AbnormalStatus스크립트를 추가 시켜줌
    {
        //나중에 다시 활성화 시킬 예정
        if(collision.GetComponent<EnemyStat>() != null || collision.GetComponent<PlayerStat>() != null)
        {
            collision.AddComponent<AbnormalStatus>().buffSetSO = Instantiate(buffSO);
        }
    }
}
