using System.Collections.Generic;
using UnityEngine;
//버프 및 디버프 캐릭터스탯에 반환해주는 애
/// <summary>
/// 치명타 확률, 회피율 등 0~1 사이의 비율 능력치를 관리하는 클래스.
/// 버프/디버프 적용 가능.
/// </summary>
public class RatioStatRuntime
{
    private float baseRatio;
    private List<StatModifier> modifiers = new();

    public RatioStatRuntime(float baseRatio)
    {
        this.baseRatio = baseRatio;
    }

    public void AddModifier(StatModifier mod)
    {
        modifiers.Add(mod);
    }

    public void RemoveModifiersFromSource(object source)
    {
        modifiers.RemoveAll(m => m.Source == source);
    }

    /// <summary>
    /// 계산된 최종 비율 (0 ~ 1로 클램프)
    /// </summary>
    public float FinalRatio
    {
        get
        {
            float final = baseRatio;
            float percentBonus = 0f;

            foreach (var mod in modifiers)
            {
                if (mod.Type == StatModType.Flat)
                    final += mod.Value;
                else if (mod.Type == StatModType.Percent)
                    percentBonus += mod.Value;
            }

            final *= (1f + percentBonus);
            return Mathf.Clamp01(final);
        }
    }
}
