using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    public Coroutine buffing;

    void Awake()
    {
        GM = FindAnyObjectByType<GameManager>();
        HPBarFilled.fillAmount = 1f;
        Revive();
    }

    public void Revive()//부활 시킬 때 해당 오브젝트의 변수들을 초기화 시켜주는 메서드
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
            GM.IncreaseMissionRemain(GetComponent<EnemyMove>().kind);
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
                // Debug.Log($"{gameObject.name} > {collision.gameObject.name}에게 {ATK.FinalValue}의 대미지. 남은 HP {collision.gameObject.GetComponent<PlayerStat>().HP.Current}");
                lastAttackTime = Time.time;
            }
        }
    }

    public void StartPoissonCour(GameObject bullet, int bulletAtk, float buffDuration)//버프 코루틴을 활성화 시켜주는 메서드
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
}
