using UnityEngine;
using UnityEngine.UI;

public class EnemyStat : MonoBehaviour
{
    public PlayerStatsSetSO stat;
    public GaugeStatRuntime HP;
    public SingleStatRuntime ATK;
    public RatioStatRuntime Cri;
    public RatioStatRuntime Dam;
    public float attackCooldown = 1.0f;
    private float lastAttackTime;
    GameManager GM;
    [SerializeField] GameObject HPBarBackground;
    [SerializeField] Image HPBarFilled;
    [SerializeField] Text HPtext;

    void Awake()
    {
        GM = FindAnyObjectByType<GameManager>();
        HPBarFilled.fillAmount = 1f;
        Revive();
    }

    public void Revive()
    {
        HP = new GaugeStatRuntime(stat.hp.MaxFinal);
        ATK = new SingleStatRuntime(stat.atk.FinalValue);
        Cri = new RatioStatRuntime(stat.cri.FinalRatio);
        Dam = new RatioStatRuntime(stat.criDam.FinalRatio);
        DetectDamage();
    }

    void Update()
    {
        if (HP.Current <= 0)//사망 판정
        {
            GetComponent<EnemyMove>().state = State.Death;
            int probability = Random.Range(0, 101);
            if (probability >= 0 && probability <= 39)
            {
                GM.GenerateOrb(gameObject, OrbKind.XP);
            }
            else if (probability >= 40 && probability <= 54)
            {
                GM.GenerateOrb(gameObject, OrbKind.HP);
            }
            gameObject.SetActive(false);
        }
    }

    public void DetectDamage()//대미지를 받을 때 감지해서 체력바를 줄여주는 메서드
    {
        HPBarFilled.fillAmount = (float)HP.Current / HP.MaxFinal;
        HPtext.text = $"{HP.Current}/{HP.MaxFinal}";
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
