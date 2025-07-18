using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyStat : MonoBehaviour
{
    public PlayerStatsSetSO stat;//플레이어 스텟 초깃값
    public GaugeStatRuntime HP;//플레이어 체력
    public SingleStatRuntime ATK;//플레이어 공격력
    public RatioStatRuntime Cri;//크리티컬 대미지
    public RatioStatRuntime Dam;//크리티컬 확률
    public float attackCooldown = 1.0f;//공격력 쿨타임
    private float lastAttackTime;//현재 공격력 쿨타임
    GameManager GM;//게임매니저
    [SerializeField] GameObject HPBarBackground;//체력바 배경
    [SerializeField] Image HPBarFilled;//체력바
    [SerializeField] Text HPtext;//체력 텍스트

    void Awake()
    {
        GM = FindAnyObjectByType<GameManager>();
        HPBarFilled.fillAmount = 1f;
        Revive();
    }

    public void Revive()//부활 시킬 때 해당 오브젝트의 변수들을 초기화 시켜주는 메서드
    {
        HP = new GaugeStatRuntime(stat.hp.MaxFinal);
        // ATK = new SingleStatRuntime(stat.atk.FinalValue);
        Cri = new RatioStatRuntime(stat.cri.FinalRatio);
        // Dam = new RatioStatRuntime(stat.criDam.FinalRatio);
        DetectDamage();
    }

    void Update()
    {
        if (HP.Current <= 0)//사망 판정 후 확률에 따라 오브를 생성시킴, 또한 미션에 필요한 오브젝트인지 판단하는 메서드를 실행시킴
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
        if (collision.gameObject.GetComponent<PlayerStat>() != null)//lastAttackTime과 델타타임을 비교한 후 대미지 주는 용도
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                collision.gameObject.GetComponent<PlayerStat>().HP.TakeDamage(ATK.FinalValue);
                // Debug.Log($"{gameObject.name} > {collision.gameObject.name}에게 {ATK.FinalValue}의 대미지. 남은 HP {collision.gameObject.GetComponent<PlayerStat>().HP.Current}");
                lastAttackTime = Time.time;
            }
        }
    }
}
