using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum SkillType
{
    Poison,
    Gether,
    NewSkill,
    NewSkill2
}
public class PlayerSkill : MonoBehaviour
{
    [SerializeField] KeyCode key;
    [SerializeField] SkillType skillType;
    [SerializeField] SkillType skillType2;
    [SerializeField] bool isSpecialATKable = true;
    [SerializeField] List<GameObject> specialBullets = new List<GameObject>();
    public GameObject bulletPrefab;
    public GameObject specialBulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] int spcialBulletCooldown;//단위 0.1초
    [SerializeField] int spcialCurrectTime = 1;//단위 0.1초
    public float getherCooldown = 1.0f;
    float lastGetherTime;
    [SerializeField] bool isGetKey;
    [SerializeField] bool isGetKey2;

    void Start()
    {
        bulletPrefab = GetComponent<PlayerStat>().bulletPrefab;
        specialBulletPrefab = GetComponent<PlayerStat>().specialBulletPrefab;
        bulletSpawnPoint = GetComponent<PlayerStat>().bulletSpawnPoint;
        spcialBulletCooldown = GetComponent<PlayerStat>().spcialBulletCooldown;
        StartCoroutine(SpecialSkillColling());
    }

    void FixedUpdate()
    {
        if (skillType == SkillType.Gether)
        {
            if (GetComponent<PlayerMoveSet>().mineral == null || !Input.GetKey(KeyCode.Space))
            {
                GetComponent<PlayerStat>().isDisableATK = false;
            }
        }
        if (!isGetKey && Input.GetKeyDown(key) && Time.timeScale != 0)
        {
            DetermineSkill();
        }
        else if (isGetKey && Input.GetKey(key) && Time.timeScale != 0)
        {
            DetermineSkill();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeSkillOntTwo();
        }
    }

    void ChangeSkillOntTwo()//첫번째 스킬과 두번째 스킬을 체인지
    {
        SkillType Temp = skillType;
        bool isGetKeyTemp = isGetKey;

        skillType = skillType2;
        isGetKey = isGetKey2;
        
        skillType2 = Temp;
        isGetKey2 = isGetKeyTemp;
    }

    void DetermineSkill()//어떤 스킬을 실행할지 판단
    {
        switch (skillType)
        {
            case SkillType.Poison:
                if (!isSpecialATKable || GetComponent<PlayerStat>().isDisableATK)
                {
                    break;
                }
                SpellSkill();
                break;
            case SkillType.NewSkill:
                if (!isSpecialATKable || GetComponent<PlayerStat>().isDisableATK)
                {
                    break;
                }
                SkillA();
                break;
            case SkillType.NewSkill2:
                if (!isSpecialATKable || GetComponent<PlayerStat>().isDisableATK)
                {
                    break;
                }
                SkillB();
                break;
            case SkillType.Gether:
                Gether();
                break;
        }
    }

    
    void Gether()//광물 캐기
    {
        if (GetComponent<PlayerMoveSet>().mineral == null)
        {
            GetComponent<PlayerStat>().isDisableATK = false;
            return;
        }

        GameObject mineral = GetComponent<PlayerMoveSet>().mineral;
        
        GetComponent<PlayerStat>().isDisableATK = true;
        if (Time.time - getherCooldown >= lastGetherTime)
        {
            lastGetherTime = Time.time;
            GetComponent<PlayerStat>().mineralNum++;
            mineral.GetComponent<Mineral>().Gathered();
            Debug.Log($"{mineral} 캐는 중");
        }
    }

    void SkillA()
    {
        isSpecialATKable = false;
        Debug.Log($"{key} 스킬 A발동");
        StartCoroutine(SpecialSkillColling());
    }
    
    void SkillB()
    {
        isSpecialATKable = false;
        Debug.Log($"{key} 스킬 B발동");
        StartCoroutine(SpecialSkillColling());
    }

    void SpellSkill()//강한 탄환 발사하는 함수
    {
        
        isSpecialATKable = false;

        StartCoroutine(SpecialSkillColling());

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

        theBullet.GetComponent<Bullet>().target = GetComponent<PlayerStat>().mousePos;
        theBullet.GetComponent<Bullet>().Reset();
        theBullet.GetComponent<Bullet>().ATK = new SingleStatRuntime(GetComponent<PlayerStat>().ATK.FinalValue);
    }

    IEnumerator SpecialSkillColling()//스킬 쿨타임
    {
        while (true)
        {
            if (spcialCurrectTime == spcialBulletCooldown)
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
            spcialCurrectTime++;
            // GetComponent<PlayerStat>().SkillCooltext.text = $"{spcialCurrectTime}/{spcialBulletCooldown}";
            //임시로 비활성화
        }
        spcialCurrectTime = 1;
        isSpecialATKable = true;
    }

}
