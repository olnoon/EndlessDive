using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public PlayerStatsSetSO stat;
    public GaugeStatRuntime HP;
    public SingleStatRuntime ATK;
    public RatioStatRuntime Cri;
    public RatioStatRuntime Dam;
    public GameObject bulletPrefab;
    public GameObject targetEnemy;
    public float findEnemyRange;
    public GameManager GM;
    [SerializeField] float bulletCooldown = 0.5f;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] List<GameObject> bullets = new List<GameObject>();

    void Awake()
    {
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
        if (HP.MaxFinal <= 0)//사망 판정
        {
            GameOver();
        }
        if (targetEnemy != null && !targetEnemy.activeSelf)//타겟팅된 적이 사망했는지 판단
        {
            targetEnemy = null;
        }
    }

    void GameOver()//게임오버
    {
        Debug.Log("GameOver");
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

            if (targetEnemy == null)
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
            theBullet.GetComponent<Bullet>().target = targetEnemy;
            theBullet.GetComponent<Bullet>().Reset();
            theBullet.GetComponent<Bullet>().ATK = ATK;

        flag:
            yield return new WaitForSeconds(bulletCooldown);
        }
    }
}
