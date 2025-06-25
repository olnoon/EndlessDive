using UnityEngine;

[CreateAssetMenu(menuName = "Stats/GaugeStat")]
public class GaugeStatSO : ScriptableObject
{
    [Header("기본 최대치")]
    public int baseMax;

    [Header("고정 증가량")]
    public int bonusFlat;

    [Header("퍼센트 증가")]
    public float bonusRate;

    [Header("임시 최대치")]
    public int tempMax;

    // 계산된 최종 최대치
    public int MaxFinal => (int)((baseMax + bonusFlat) * (1f + bonusRate)) + tempMax;
}
