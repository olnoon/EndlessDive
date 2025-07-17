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
    BuffSO buffSO;

    void OnTriggerEnter2D(Collider2D collision)//AbnormalStatus스크립트를 추가 시켜줌
    {
        //나중에 다시 활성화 시킬 예정
        // collision.AddComponent<AbnormalStatus>().buffSetSO = Instantiate(buffSO);
    }
}
