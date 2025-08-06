using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillSO", menuName = "Scriptable Objects/SkillSO")]
public class SkillSO : ScriptableObject
{
    public int skillId;
    public string skillName;//스킬 이름
    public Image skillcon;//스킬 아이콘 이미지
    public int skillLvl;//스킬 레벨
    public bool isGetKey;//키가 눌려있을 때 스킬을 실행시킬지의 여부
    public float skillCooldown_Origin;//쿨타임
    public float skillCooldown_Now;//버프/보너스를 통해 변경된 스킬의 최종 쿨타임
    public int skillRepeat_Origin; //스킬의 연속 사용 횟수.
    public int skillRepeat_Now;//버프나 보너스를 통해 변경된, 스킬의 최종 연속 사용 횟수.
    public float skillRepeatCooldown_Origin;//스킬의 연속 사용 쿨타임
    public float skillRepeatCooldown_Now;//버프나 보너스를 통해 변경된, 스킬의 최종 연속 사용 쿨타임
    public int skillMaxCharges_Origin;//스킬의 최대 충전 횟수
    public int skillMaxCharges_Now;//버프나 보너스를 통해 변경된, 스킬의 최종 최대 충전 횟수
    public SkillType skillType;//스킬의 스킬타입
    public GameObject bulletPrefab;//발사될 불렛의 프리팹
    public bool chargeAll;
    public float MinUsageInterval;//스킬 최소 사용 간격
    public bool isMultiple;//스킬 동시사능 가능 여부

    public void InitializeFromSelf()
    {
        SkillData data = DataManager.Instance.GetSkillData(skillId);
        if (data == null)
        {
            Debug.LogWarning($"SkillSO 초기화 실패: ID {skillId}의 스킬 데이터를 찾을 수 없습니다.");
            return;
        }

        skillCooldown_Origin = data.sk_Cooldown_base;
        skillCooldown_Now = data.sk_Cooldown_base;

        skillRepeat_Origin = data.sk_Repeat_base;
        skillRepeat_Now = data.sk_Repeat_base;

        skillRepeatCooldown_Origin = data.sk_RepeatCooldown_base;
        skillRepeatCooldown_Now = data.sk_RepeatCooldown_base;

        skillMaxCharges_Origin = data.sk_MaxCharges_base;
        skillMaxCharges_Now = data.sk_MaxCharges_base;

        chargeAll = data.sk_ChargeAll;

        Debug.Log($"[SkillSO] ID {skillId} 초기화 완료");
    }

}
