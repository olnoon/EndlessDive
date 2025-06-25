using UnityEngine;

[CreateAssetMenu(menuName = "Stats/SingleStat")]
public class SingleStatSO : ScriptableObject
{
    [Header("기본 수치")]
    public int baseValue;

    [Header("고정 증가량")]
    public int bonusFlat;

    [Header("퍼센트 증가")]
    public float bonusRate;

    [Header("임시 수치")]
    public int tempValue;

    // 계산된 최종 수치
    public int FinalValue => (int)((baseValue + bonusFlat) * (1f + bonusRate)) + tempValue;
}