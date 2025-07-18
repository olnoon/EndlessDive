using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//TODO 기본 공격 플레이어스킬로 이동시키기
public class PlayerStat : MonoBehaviour
{
    public PlayerStatsSetSO stat;//스탯의 초깃값
    public GaugeStatRuntime HP;//체력
    public RatioStatRuntime ATK;//공격력
    public SingleStatRuntime phyATK;//물리 기반 공격력
    public SingleStatRuntime EnATK;//에너지 기반 공격력
    public RatioStatRuntime Cri;//크리티컬 확률
    public RatioStatRuntime Dam;//크리티컬 대미지
    public GameObject bulletPrefab;//불렛 프리펩
    public GameObject targetEnemy;//타겟팅중인 적
    public float findEnemyRange;//적 타겟팅 범위
    public GameManager GM;//게임메니저
    [SerializeField] float bulletCooldown = 0.5f;
    public Transform bulletSpawnPoint;
    public int spcialBulletCooldown = 10;//단위 0.1초
    [SerializeField] List<GameObject> bullets = new List<GameObject>();
    [SerializeField] bool isToggleATK = true;
    public bool isDisableATK;
    public int currentLvl;
    public int maxXp;
    public int currentXp;
    public int mineralNum;
    [SerializeField] GameObject HPBarBackground;
    [SerializeField] Image HPBarFilled;
    [SerializeField] Text HPtext;
    [SerializeField] GameObject XPBarBackground;
    [SerializeField] Image XPBarFilled;
    [SerializeField] Text XPtext;
    public Vector3 mousePos;
    [SerializeField] float gainRange;

    void Awake()
    {
        mousePos.z = 0f;
        HPBarFilled.fillAmount = 1f;
        XPBarFilled.fillAmount = 0f;
        XPtext.text = $"{currentXp}/{maxXp}";
        GM = FindFirstObjectByType<GameManager>();
        HP = new GaugeStatRuntime(stat.hp.MaxFinal);
        ATK = new RatioStatRuntime(stat.atk.FinalRatio);
        phyATK = new SingleStatRuntime(stat.phyAtk.FinalValue);
        EnATK = new SingleStatRuntime(stat.enAtk.FinalValue);
        Cri = new RatioStatRuntime(stat.cri.FinalRatio);
        Dam = new RatioStatRuntime(stat.catK.FinalRatio);

        bulletSpawnPoint = transform.GetChild(0);
    }

    void Start()
    {
        StartCoroutine("FindEnemy");
        StartCoroutine("FindOrb");
        StartCoroutine("TriggerBullet");
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
        XPBarFilled.fillAmount = (float)currentXp / maxXp;
        XPtext.text = $"{currentXp}/{maxXp}";
    }

    void levelUP()//레벨 증가 및 업그레이드 화면 띄우기
    {
        currentXp -= maxXp;
        currentLvl++;
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

    IEnumerator FindOrb()//0.2초 마다 gainRange안의 오브들을 찾음, 체력오브이고 최대체력이면 발견됨 상태로 만들지 않음
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

    IEnumerator TriggerBullet()//Bullet생성 및 재사용, 또한 불렛의 변수들을 올바르게 초기화 시킴
    {
        while (true)
        {
            GameObject theBullet = null;

            bool reused = false;

            if (isDisableATK || !isToggleATK)
            {
                goto flag;
            }

            foreach (GameObject bullet in bullets)
            {
                var move = bullet.GetComponent<Bullet>();
                if (!bullet.activeSelf)
                {
                    theBullet = bullet;
                    move.transform.position = bulletSpawnPoint.position;
                    bullet.SetActive(true);
                    reused = true;
                    break;
                }
            }

            if (!reused)
            {
                theBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
                bullets.Add(theBullet);
            }
            CalculateMouseCoord();
            theBullet.GetComponent<Bullet>().target = mousePos;
            theBullet.GetComponent<Bullet>().Reset();
            theBullet.GetComponent<Bullet>().ATK = new RatioStatRuntime(ATK.FinalRatio);
            theBullet.GetComponent<Bullet>().phyATK = new SingleStatRuntime(phyATK.FinalValue);
            theBullet.GetComponent<Bullet>().EnATK = new SingleStatRuntime(EnATK.FinalValue);
            theBullet.GetComponent<Bullet>().GM = GM;

        flag:
            yield return new WaitForSeconds(bulletCooldown);
        }
    }
}
