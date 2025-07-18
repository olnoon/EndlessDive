using UnityEngine;

[CreateAssetMenu(fileName = "BuffSO", menuName = "Scriptable Objects/BuffSO")]
public class BuffSO : ScriptableObject
{
    public float damagePerTick;//tickInterval당 들어가는 버프 혹은 디버프의 세기
    public float tickInterval;//한번 버프가 들어갈 때 마다의 시간
    public int repeatCount;//버프가 들어가는 반복 횟수
    public BuffToWhat buffKind;//들어가는 버프의 종류(Ex.HP, 공격력)
}
