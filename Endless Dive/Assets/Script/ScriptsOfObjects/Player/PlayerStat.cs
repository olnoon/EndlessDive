using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    public SingleStatSO Level;//레벨
    public PlayerStatsSetSO stat;//플레이어 스텟 초깃값
    public GaugeStatRuntime HP;//플레이어 체력
    public RatioStatRuntime ATK;//모든 공격력에 영향을 주는 능력치
    public SingleStatRuntime phyAtk;//물리 기반 공격력
    public SingleStatRuntime enAtk;//에너지 기반 공격력
    public RatioStatRuntime Cri;//크리티컬 대미지
    public RatioStatRuntime Dam;//크리티컬 확률
    public RatioStatRuntime Dex;// 쿨타임 감소에 영향을 주는 능력치
    public RatioStatRuntime MeleeRange;// 근접 공격 사거리에 영향을 주는 능력치
    public SingleStatRuntime ARM;// 받는 피해를 줄이는 방어력 스탯
    public RatioStatRuntime PickupRange;// 아이템을 자동으로 줍는 거리 범위
    public RatioStatRuntime Mining;// 자원 채굴 효율 또는 채굴량에 영향을 주는 능력치
    public GameObject bulletPrefab;//불렛 프리펩
    public GameObject targetEnemy;//타겟팅중인 적
    public float findEnemyRange;//적 타겟팅 범위
    public GameManager GM;//게임메니저
    public Transform bulletSpawnPoint;//불렛이 생성되는 위치
    [SerializeField] bool isToggleATK = true;
    public bool isDisableATK;
    public int currentaAether;//현재 Ather에너지
    public int mineralNum;//캔 미네랄 갯수
    public Text mineralText;//미네랄 갯수를 표시할 UI
    [SerializeField] GameObject HPBarBackground;//HP를 표시할 UI의 배경
    [SerializeField] Image HPBarFilled;//HP를 표시할 UI
    [SerializeField] Text HPtext;//HP를 글로 표시할 textUI
    [SerializeField] GameObject XPBarBackground;
    [SerializeField] Text lvltext;//레벨을 글로 표시할 textUI
    public Vector3 mousePos;//보는 방향을 결정하거나 불렛이 나갈 방향을 결정지을 마우스 위치
    [SerializeField] float gainRange;//아이템(오브)를 획득할 수 있게 하는 범위
    public bool isInvincibility;//무적 상태 체크
    float savedMass;//저장해둘 RB의 Mass
    float savedLinear;//저장해둘 RB의 LinearDamping
    float savedAngular;//저장해둘 RB의 AngularDamping
    public int mineAmount;//한번에 채굴하는 광물양

    void Awake()
    {
        savedMass = GetComponent<Rigidbody2D>().mass;
        savedAngular = GetComponent<Rigidbody2D>().angularDamping;
        savedLinear = GetComponent<Rigidbody2D>().linearDamping;

        mousePos.z = 0f;
        HPBarFilled.fillAmount = 1f;
        GM = FindFirstObjectByType<GameManager>();
        HP = new GaugeStatRuntime(stat.hp.MaxFinal);
        ATK = new RatioStatRuntime(stat.atk.FinalRatio);
        phyAtk = new SingleStatRuntime(stat.phyAtk.FinalValue);
        enAtk = new SingleStatRuntime(stat.enAtk.FinalValue);
        Cri = new RatioStatRuntime(stat.cri.FinalRatio);
        Dam = new RatioStatRuntime(stat.catK.FinalRatio);
        Dex = new RatioStatRuntime(stat.dex.FinalRatio);
        MeleeRange = new RatioStatRuntime(stat.meleeRange.FinalRatio);
        ARM = new SingleStatRuntime(stat.ARM.FinalValue);
        PickupRange = new RatioStatRuntime(stat.pickupRange.FinalRatio);
        Mining = new RatioStatRuntime(stat.mining.FinalRatio);

        bulletSpawnPoint = transform.GetChild(0);
        lvltext.text = currentaAether.ToString();
        mineralText.text = mineralNum.ToString();
        GM.players.Add(gameObject);
    }

    void Start()
    {
        stat.InitializeFromSelf();
        StartCoroutine("FindEnemy");
        StartCoroutine("FindOrb");
    }

    public void SetAfterReturnToMain(GameObject newPlayer)//UI를 새로운 플레이어 오브젝트로 부터 받아와서 초기화시켜줌
    {
        PlayerStat newStat = newPlayer.GetComponent<PlayerStat>();
        PlayerMoveSet newMove = newPlayer.GetComponent<PlayerMoveSet>();

        // UI 참조 연결
        mineralText = newStat.mineralText;
        HPBarBackground = newStat.HPBarBackground;
        HPBarFilled = newStat.HPBarFilled;
        HPtext = newStat.HPtext;
        XPBarBackground = newStat.XPBarBackground;
        lvltext = newStat.lvltext;
        GetComponent<PlayerMoveSet>().exitTimeBarBG = newMove.exitTimeBarBG;
        GetComponent<PlayerMoveSet>().exitTimeBarFilled = newMove.exitTimeBarFilled;
        GetComponent<PlayerMoveSet>().exitTimeText = newMove.exitTimeText;

        if (mineralText != null)
            mineralText.text = $"{mineralNum}";

        if (lvltext != null)
            lvltext.text = $"{newStat.Level}";

        for (int i = 0; i < GetComponents<PlayerSkill>().Length; i++)
        {
            GetComponents<PlayerSkill>()[i].SkillCooltext = newPlayer.GetComponents<PlayerSkill>()[i].SkillCooltext;
            GetComponents<PlayerSkill>()[i].SkillLvltext = newPlayer.GetComponents<PlayerSkill>()[i].SkillLvltext;
            GetComponents<PlayerSkill>()[i].bullets.Clear();
            StartCoroutine(GetComponents<PlayerSkill>()[i].SkillCooling());
        }

        //무적 상태와 조작 불능 상태 해제
        isInvincibility = false;
        foreach (PlayerSkill skill in GetComponents<PlayerSkill>())
        {
            skill.isDisableOperation = false;
        }
        GetComponent<PlayerMoveSet>().isDisableOperation = false;

        StartCoroutine(FindOrb());//FindOrb코루틴을 다시 시작 해 줌.

        Destroy(newPlayer);//새로운 플레이어 삭제
    }

    void Update()
    {
        if (HPBarFilled != null && HPtext != null)
        {
            HPBarFilled.fillAmount = (float)HP.Current / HP.MaxFinal;
            HPtext.text = $"{HP.Current}/{HP.MaxFinal}";
        }

        if (isInvincibility)
        {
            //무적일시 넉백 무시
            GetComponent<Rigidbody2D>().mass = 1000000;
            GetComponent<Rigidbody2D>().linearDamping = 1000000;
            GetComponent<Rigidbody2D>().angularDamping = 1000000;
            
            //무적일시 모든 디버프 제거
            foreach (AbnormalStatus buff in GetComponents<AbnormalStatus>())
            {
                if (buff.buffSetSO.damagePerTick > 0)
                {
                    Destroy(buff);
                }
            }
        }
        else
        {
            //넉백을 원래대로 돌려 놓음
            GetComponent<Rigidbody2D>().mass = savedMass;
            GetComponent<Rigidbody2D>().linearDamping = savedLinear;
            GetComponent<Rigidbody2D>().angularDamping = savedAngular;
        }

        if (HP.MaxFinal <= 0)//사망 판정
        {
            GameOver();
        }

        if (targetEnemy != null && !targetEnemy.activeSelf)//타겟팅된 적이 사망했는지 판단
        {
            targetEnemy = null;
        }

        AttackMethod();

        CalculateMouseCoord();
    }

    void AttackMethod()//마우스 오른쪽, 왼쪽 입력 감지
    {
        if (!isDisableATK)
        {
            if (Input.GetMouseButtonDown(0) && Time.timeScale != 0)
            {
                isToggleATK = !isToggleATK;
            }
        }
    }

    void CalculateMouseCoord()//마우스 위치 계산
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void GameOver()//게임 오버
    {
        Debug.Log("GameOver");
    }

    public void AddAether()//에테르 증가
    {
        currentaAether++;
        lvltext.text = currentaAether.ToString();
    }

    IEnumerator FindEnemy()//0.2초 마다 주변의 적들을 찾음
    {
        while (true)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, findEnemyRange, LayerMask.GetMask("Enemy"));
            foreach (var hitCollider in hitColliders)
            {
                if (GM.enemies.Contains(hitCollider.gameObject))
                {
                    if (targetEnemy == null)
                    {
                        targetEnemy = hitCollider.gameObject;
                    }
                    if (Vector2.Distance(hitCollider.transform.position, transform.position) < Vector2.Distance(targetEnemy.transform.position, transform.position))
                    {
                        targetEnemy = hitCollider.gameObject;
                    }
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator FindOrb()//0.2초 마다 gainRange안의 오브들을 찾음, 오브들을 발견됨 상태로 만드나 체력오브이고 최대 체력이면 발견됨 상태로 만들지 않음
    {
        while (true)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, gainRange * stat.pickupRange.FinalRatio, LayerMask.GetMask("Orb"));
            foreach (var hitCollider in hitColliders)
            {
                if (GM.orbs.Contains(hitCollider.gameObject) && hitCollider.gameObject.activeSelf)
                {
                    if (HP.Current >= HP.MaxFinal && hitCollider.GetComponent<OrbScript>().orbKind == OrbKind.HP)
                    {
                        break;
                    }
                    hitCollider.GetComponent<OrbScript>().isFinded = true;
                    hitCollider.GetComponent<OrbScript>().player = gameObject;
                    hitCollider.GetComponent<OrbScript>().MoveToPlayer();
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
