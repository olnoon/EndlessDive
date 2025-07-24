using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Stats Set")]
public class PlayerStatsSetSO : ScriptableObject
{
    [Header("아이디 (id)")]
    public int character_id;
    [Header("레벨 (Level)")]
    public SingleStatSO Level;
    [Header("체력 (HP)")]
    public GaugeStatSO hp;

    [Header("공격력 (ATK)")]
    public RatioStatSO atk;

    [Header("물리 기반 공격력 (phyATK)")]
    public SingleStatSO phyAtk;

    [Header("에너지 기반 공격력 (enATK)")]
    public SingleStatSO enAtk;

    [Header("치명타확률 (CRI)")]
    public RatioStatSO cri;

    [Header("치명타 대미지 (CATK)")]
    public RatioStatSO catK;

    [Header("쿨타임 (DEX)")]
    public RatioStatSO dex;

    [Header("근접사거리 (MeleeRange)")]
    public RatioStatSO meleeRange;

    [Header("방어력 (ARM)")]
    public SingleStatSO ARM;
    
    [Header("아이템 획득 범위 (PickupRange)")]
    public RatioStatSO pickupRange;
    
    [Header("채굴량 (Mining)")]
    public RatioStatSO mining;

    public void InitializeFromSelf()
    {
        var data = DataManager.Instance.GetCharacterData(character_id);
        if (data == null)
        {
            Debug.LogWarning($"[PlayerStatsSetSO] ID {character_id}에 해당하는 캐릭터 데이터를 찾을 수 없습니다.");
            return;
        }

        if (hp != null) hp.baseMax = data.ch_HP_base;
        if (atk != null) atk.baseRatio = data.ch_ATK_base;
        if (phyAtk != null) phyAtk.baseValue = data.ch_PhyATK_base;
        if (enAtk != null) enAtk.baseValue = data.ch_EnATK_base;
        if (cri != null) cri.baseRatio = data.ch_CRI_base;
        if (catK != null) catK.baseRatio = data.ch_CATK_base;
        if (dex != null) dex.baseRatio = data.ch_DEX_base;
        if (meleeRange != null) meleeRange.baseRatio = data.ch_MeleeRange_base;
        if (ARM != null) ARM.baseValue = data.ch_ARM_base;
        if (pickupRange != null) pickupRange.baseRatio = data.ch_PickupRange_base;
        if (mining != null) mining.baseRatio = data.ch_Mining_base;

        Debug.Log($"[PlayerStatsSetSO] [{data.name}] 캐릭터 데이터를 기반으로 초기화 완료.");
    }
}