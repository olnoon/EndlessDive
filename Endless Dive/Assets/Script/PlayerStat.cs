using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    public PlayerStatsSetSO stat;
    public GaugeStatRuntime HP;
    public SingleStatRuntime ATK;
    public RatioStatRuntime Cri;
    public RatioStatRuntime Dam;
    public GameObject bulletPrefab;
    public GameObject specialBulletPrefab;
    public GameObject targetEnemy;
    public float findEnemyRange;
    public GameManager GM;
    [SerializeField] float bulletCooldown = 0.5f;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] float spcialBulletCooldown = 0.5f;
    [SerializeField] List<GameObject> bullets = new List<GameObject>();
    [SerializeField] List<GameObject> specialBullets = new List<GameObject>();
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

    void Awake()
    {
        mousePos.z = 0f;
        HPBarFilled.fillAmount = 1f;
        XPBarFilled.fillAmount = 0f;
        XPtext.text = $"{currentXp}/{maxXp}";
        GM = FindFirstObjectByType<GameManager>();
        HP = new GaugeStatRuntime(stat.hp.MaxFinal);
        ATK = new SingleStatRuntime(stat.atk.FinalValue);
        Cri = new RatioStatRuntime(stat.cri.FinalRatio);
        Dam = new RatioStatRuntime(stat.criDam.FinalRatio);

        bulletSpawnPoint = transform.GetChild(0);
    }

    void Start()
    {
        StartCoroutine("FindEnemy");
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

            if (Input.GetMouseButtonDown(1) && Time.timeScale != 0)
            {
                SpellSkill();
            }
        }
    }

    void CalculateMouseCoord()//마우스 위치 계산
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void SpellSkill()//강한 탄환 발사하는 함수
    {
        
        GameObject theBullet = null;

        bool reused = false;

        foreach (GameObject bullet in specialBullets)
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
            theBullet = Instantiate(specialBulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            specialBullets.Add(theBullet);
        }

        theBullet.GetComponent<Bullet>().target = mousePos;
        theBullet.GetComponent<Bullet>().Reset();
        theBullet.GetComponent<Bullet>().ATK = new SingleStatRuntime(ATK.FinalValue);
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
        GM.UpgradeOn();
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

    IEnumerator TriggerBullet()//Bullet생성 및 재사용
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
            theBullet.GetComponent<Bullet>().ATK = new SingleStatRuntime(ATK.FinalValue);

        flag:
            yield return new WaitForSeconds(bulletCooldown);
        }
    }
}
