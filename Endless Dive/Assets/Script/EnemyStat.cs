using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    public PlayerStatsSetSO stat;
    public GaugeStatRuntime HP;
    public SingleStatRuntime ATK;
    public RatioStatRuntime Cri;
    public RatioStatRuntime Dam;
    public float attackCooldown = 1.0f;
    private float lastAttackTime;

    void Awake()
    {
        HP = new GaugeStatRuntime(stat.hp.MaxFinal);
        ATK = new SingleStatRuntime(stat.atk.FinalValue);
        Cri = new RatioStatRuntime(stat.cri.FinalRatio);
        Dam = new RatioStatRuntime(stat.criDam.FinalRatio);
    }

    void Update()
    {
        if (HP.Current <= 0)//사망 판정
        {
            gameObject.SetActive(false);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerStat>() != null)//대미지 주는 용도
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                collision.gameObject.GetComponent<PlayerStat>().HP.TakeDamage(ATK.FinalValue);
                Debug.Log($"{collision.gameObject.name}에게 {ATK.FinalValue}의 대미지를 가해서 체력이 {collision.gameObject.GetComponent<PlayerStat>().HP.Current}만큼 남았습니다.");
                lastAttackTime = Time.time;
            }
        }
    }
}
