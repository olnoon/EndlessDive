using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public float bulletCooldown = 0.5f;
    public Transform bulletSpawnPoint;
    public int spcialBulletCooldown = 10;//단위 0.1초
    public bool isToggleATK = true;
    public bool isDisableATK;
    public int currentLvl;
    public int maxXp;
    public int currentXp;
    public int mineralNum;
    public GameObject HPBarBackground;
    public Image HPBarFilled;
    public Text HPtext;
    public GameObject XPBarBackground;
    public Image XPBarFilled;
    public Text XPtext;
    public Text SkillCooltext;
    public float gainRange;
    public Vector3 mousePos;
    [SerializeField] List<GameObject> bullets = new List<GameObject>();

    void Awake()
    {
        HPBarFilled.fillAmount = 1f;
        XPBarFilled.fillAmount = 0f;
        XPtext.text = $"{currentXp}/{maxXp}";
        GM = FindFirstObjectByType<GameManager>();
        HP = new GaugeStatRuntime(stat.hp.MaxFinal);
        ATK = new SingleStatRuntime(stat.atk.FinalValue);
        Cri = new RatioStatRuntime(stat.cri.FinalRatio);
        Dam = new RatioStatRuntime(stat.criDam.FinalRatio);

        bulletSpawnPoint = transform.GetChild(0);
        mousePos.z = 0f;
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

    IEnumerator FindOrb()//0.2초 마다 주변의 오브들을 찾음
    {
        while (true)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, gainRange, LayerMask.GetMask("Orb"));
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
    
    IEnumerator TriggerBullet()//Bullet생성 및 재사용
    {
        while (true)
        {
            GameObject theBullet = null;

            bool reused = false;

            if (GetComponent<PlayerStat>().isDisableATK || !GetComponent<PlayerStat>().isToggleATK)
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
            GetComponent<PlayerStat>().CalculateMouseCoord();
            theBullet.GetComponent<Bullet>().target = GetComponent<PlayerStat>().mousePos;
            theBullet.GetComponent<Bullet>().Reset();
            theBullet.GetComponent<Bullet>().ATK = new SingleStatRuntime(GetComponent<PlayerStat>().ATK.FinalValue);

        flag:
            yield return new WaitForSeconds(GetComponent<PlayerStat>().bulletCooldown);
        }
    }

    public void CalculateMouseCoord()//마우스 위치 계산
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

}
