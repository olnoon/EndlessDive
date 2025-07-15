using System.Collections.Generic;
using UnityEngine;
public interface IStatusTarget
{
    public GaugeStatRuntime HP { get; }
    public SingleStatRuntime ATK { get; }
    public RatioStatRuntime Cri { get; }
    public RatioStatRuntime Dam { get; }

    public void DetectDamage(); // 체력 UI 업데이트 등
}

public abstract class StatusEffectBase
{
    public float Duration;
    protected float elapsed;

    public bool IsExpired => elapsed >= Duration;

    public virtual void OnApply(IStatusTarget target) { }
    public virtual void OnUpdate(IStatusTarget target) { }
    public virtual void OnRemove(IStatusTarget target) { }

    public void Tick(float delta)
    {
        elapsed += delta;
    }
}

public class DotDamageEffect : StatusEffectBase
{
    float tickInterval = 1f;
    float tickTimer = 0f;
    int damage = 5;

    public override void OnUpdate(IStatusTarget target)
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            tickTimer = 0f;
            target.HP.TakeDamage(damage);
            target.DetectDamage();
        }
    }
}

/*public class AbnormalStatus : MonoBehaviour
{
    public void StartPoissonRoutine(GameObject bullet, int bulletAtk, float buffDuration)//버프 코루틴을 활성화 시켜주는 메서드
    {
        GetComponent<EnemyMove>().state = State.Poisoned;
        buffing = StartCoroutine(BuffEnemy(bullet, bulletAtk, 0.5f));
        StartCoroutine(ClearBuffEnemy(buffDuration));
    }

    public IEnumerator BuffEnemy(GameObject bullet, int dam = 5, float weight = 1)//플레이어에서 해당 오브젝트에게 디버프를 거는 코루틴
    {
        Debug.Log("디버프 시작");
        Debug.Log($"대미지와 가중치 : {dam} {weight}");
        while (true)
        {
            GetComponent<EnemyStat>().HP.TakeDamage(Mathf.RoundToInt(dam * weight));
            GetComponent<EnemyStat>().DetectDamage();
            Debug.Log($"{bullet.name} > {gameObject.name}에게 {ATK.FinalValue}의 대미지. 남은 HP {HP.Current}");
            yield return new WaitForSeconds(1);
        }
    }

    public IEnumerator ClearBuffEnemy(float buffDuration)//해당 오브젝트의 디버프를 제거하는 코루틴
    {
        yield return new WaitForSeconds(buffDuration);
        StopCoroutine(buffing);
        buffing = null;
        GetComponent<EnemyMove>().state = State.Normal;
        Debug.Log("디버프 끝");
    }
}*/