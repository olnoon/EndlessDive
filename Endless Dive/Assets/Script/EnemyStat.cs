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
        if (HP.Current <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerStat>() != null)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                collision.gameObject.GetComponent<PlayerStat>().HP.TakeDamage(ATK.FinalValue);
                lastAttackTime = Time.time;
            }
        }
    }
}
