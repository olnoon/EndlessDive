using System.Collections;
using System.Collections.Generic;
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
    public int currentLvl;//현재 레벨
    public int maxXp;//다음 레벨로 넘어가기까지의 XP
    public int currentXp;//현재 xp
    public int mineralNum;//캔 미네랄 갯수
    public Text mineralText;//미네랄 갯수를 표시할 UI
    [SerializeField] GameObject HPBarBackground;//HP를 표시할 UI의 배경
    [SerializeField] Image HPBarFilled;//HP를 표시할 UI
    [SerializeField] Text HPtext;//HP를 글로 표시할 textUI
    [SerializeField] GameObject XPBarBackground;
    [SerializeField] Text lvltext;//레벨을 글로 표시할 textUI
    public Vector3 mousePos;//보는 방향을 결정하거나 불렛이 나갈 방향을 결정지을 마우스 위치
    [SerializeField] float gainRange;//아이템(오브)를 획득할 수 있게 하는 범위
    [SerializeField] bool invincibility;

    void Awake()
    {
        mousePos.z = 0f;
        HPBarFilled.fillAmount = 1f;
        // XPBarFilled.fillAmount = 0f;
        
        // XPtext.text = $"{currentXp}/{maxXp}";
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
        lvltext.text = currentLvl.ToString();
        mineralText.text = mineralNum.ToString();
    }

    void Start()
    {
        stat.InitializeFromSelf();
        StartCoroutine("FindEnemy");
        StartCoroutine("FindOrb");
    }

    void Update()
    {
        HPBarFilled.fillAmount = (float)HP.Current / HP.MaxFinal;
        HPtext.text = $"{HP.Current}/{HP.MaxFinal}";

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

    public void addXP()//경험치 증가
    {
        currentXp++;
        if (currentXp >= maxXp)
        {
            levelUP();
        }
        // XPBarFilled.fillAmount = (float)currentXp / maxXp;
        // XPtext.text = $"{currentXp}/{maxXp}";
    }

    void levelUP()//레벨 증가 및 업그레이드 화면 띄우기
    {
        currentXp -= maxXp;
        currentLvl++;
        lvltext.text = currentLvl.ToString();
        GM.levelUP();
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

    IEnumerator FindOrb()//0.2초 마다 gainRange안의 오브들을 찾음, 오브들을 발견됨 상태로 만드나 체력오브이고 최대체력이면 발견됨 상태로 만들지 않음
    {
        while (true)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, gainRange * stat.mining.FinalRatio, LayerMask.GetMask("Orb"));
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
