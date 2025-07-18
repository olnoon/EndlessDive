using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] KeyCode key;//스킬의 키코드
    [SerializeField] SkillType skillType;//1번째 스킬의 스킬타입
    [SerializeField] SkillType skillType2;//2번째 스킬의 스킬타입
    [SerializeField] bool isSpecialATKable = true;//쿨타임 지났는지의 여부
    [SerializeField] List<GameObject> specialBullets = new List<GameObject>();//활성화/비활성화된 필드의 모든 특별탄환들
    public GameObject bulletPrefab;//발사될 불렛의 프리팹
    public GameObject specialBulletPrefab;//발사될 트별불렛의 프리팹
    [SerializeField] Transform bulletSpawnPoint;//불렛 스폰위치
    [SerializeField] int spcialBulletCooldown;//쿨타임(단위 0.1초)
    [SerializeField] int spcialCurrectTime = 1;//현재 지난 쿨타임(단위 0.1초)
    public float getherCooldown = 1.0f;//채굴 쿨타임
    float lastGetherTime;//남은 채굴 쿨타임
    [SerializeField] bool isGetKey;//첫번째 스킬에서 키가 눌려있을 때 스킬을 실행시킬지의 여부
    [SerializeField] bool isGetKey2;//두번째 스킬에서 키가 눌려있을 때 스킬을 실행시킬지의 여부
    [SerializeField] Text SkilCooltext;//스킬 쿨타임을 보여주는 텍스트

    void Start()
    {
        bulletPrefab = GetComponent<PlayerStat>().bulletPrefab;
        specialBulletPrefab = GetComponent<PlayerStat>().specialBulletPrefab;
        bulletSpawnPoint = GetComponent<PlayerStat>().bulletSpawnPoint;
        spcialBulletCooldown = GetComponent<PlayerStat>().spcialBulletCooldown;
        StartCoroutine(SpecialSkillColling());
    }

    void Update()
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
            ChangeSkillOneTwo();
        }
    }

    void ChangeSkillOneTwo()//첫번째 스킬과 두번째 스킬을 체인지
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

    
    void Gether()//, , , 델타 타임과 비교해서 
    {
        if (GetComponent<PlayerMoveSet>().mineral == null)//광물이 없다고 판단 하면 다시 공격을 할 수 있게 해줌
        {
            GetComponent<PlayerStat>().isDisableATK = false;
            return;
        }

        GameObject mineral = GetComponent<PlayerMoveSet>().mineral;//로컬 변수 미네랄에 플레이어무브셋의 미네랄을 할당
        
        GetComponent<PlayerStat>().isDisableATK = true;//공격을 못하게 제한 시킴

        if (Time.time - getherCooldown >= lastGetherTime)//델타 타임과 getherCooldown을 비교해서 lastGetherTime보다 크면 광물을 채굴 시킴
        {
            lastGetherTime = Time.time;
            GetComponent<PlayerStat>().mineralNum++;
            mineral.GetComponent<Mineral>().Gathered();
            Debug.Log($"{mineral} 캐는 중");
        }
    }

    void SkillA()
    {
        if (!isSpecialATKable || GetComponent<PlayerStat>().isDisableATK)
            return;

        Debug.Log($"{key} 스킬 A발동");
        // 스킬 A의 고유 효과 실행 코드 추가 가능
        isSpecialATKable = false;

        StartCoroutine(SpecialSkillColling());
    }
    
    void SkillB()
    {
        if (!isSpecialATKable || GetComponent<PlayerStat>().isDisableATK)
            return;

        Debug.Log($"{key} 스킬 B발동");
        // 스킬 B의 고유 효과 실행 코드 추가 가능
        isSpecialATKable = false;

        StartCoroutine(SpecialSkillColling());
    }


    void SpellSkill()//Skill 탄환 생성/재사용 및 불렛 스크립트의 변수들을 초기화 시켜주는 함수
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
        theBullet.GetComponent<Bullet>().GM = GetComponent<PlayerStat>().GM;
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
            SkilCooltext.text = $"{spcialCurrectTime}/{spcialBulletCooldown}";
        }
        spcialCurrectTime = 1;
        isSpecialATKable = true;
    }
}
