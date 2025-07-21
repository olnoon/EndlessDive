using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

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
    [SerializeField] bool isSpecialATKable = true;//쿨타임 지났는지의 여부
    [SerializeField] Transform bulletSpawnPoint;//불렛 스폰위치
    [SerializeField] float skillCoolingTimer;//스킬의 남은 쿨타임
    [SerializeField] int skillCharges;//스킬의 남은 충전 횟수
    [SerializeField] Text SkillCooltext;//스킬 쿨타임을 보여주는 텍스트
    [SerializeField] bool canUse;//스킬 사용 가능 여부
    [SerializeField] List<SkillSO> skillSOSets;//스킬 관련 변수가 담긴 SO(초기화용)
    [SerializeField] List<SkillSO> skillSOs;//스킬 관련 변수가 담긴 SO(보관용)
    public List<GameObject> Bullets;//필드에 나와있는 탄환들(재사용을 위한)
    Action skillEffect;


    void Start()
    {
        skillSOs = new List<SkillSO>(skillSOSets);
    }

    void Update()
    {
        if (!canUse)
        {
            return;
        }
        canUse = false;
        if (skillSOs[0].skillType == SkillType.Gether)
        {
            if (GetComponent<PlayerMoveSet>().mineral == null || !Input.GetKey(KeyCode.Space))
            {
                GetComponent<PlayerStat>().isDisableATK = false;
            }
        }
        if (!skillSOs[0].isGetKey && Input.GetKeyDown(key) && Time.timeScale != 0)
        {
            DetermineSkill();
        }
        else if (skillSOs[0].isGetKey && Input.GetKey(key) && Time.timeScale != 0)
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
        // SkillType Temp = skillType;
        // bool isGetKeyTemp = isGetKey;

        // skillType = skillType2;
        // isGetKey = isGetKey2;

        // skillType2 = Temp;
        // isGetKey2 = isGetKeyTemp;
    }

    void DetermineSkill()//어떤 스킬을 실행할지 판단
    {
        switch (skillSOs[0].skillType)
        {
            case SkillType.Poison:
                if (!isSpecialATKable || GetComponent<PlayerStat>().isDisableATK)
                {
                    break;
                }
                skillEffect = SpellSkill;
                break;
            case SkillType.NewSkill:
                if (!isSpecialATKable || GetComponent<PlayerStat>().isDisableATK)
                {
                    break;
                }
                skillEffect = SkillA;
                break;
            case SkillType.NewSkill2:
                if (!isSpecialATKable || GetComponent<PlayerStat>().isDisableATK)
                {
                    break;
                }
                skillEffect = SkillB;
                break;
            case SkillType.Gether:
                skillEffect = MineMineral;
                break;
        }
        StartCoroutine(RepeatSkill());
    }

    IEnumerator RepeatSkill()//skillEffect에 구독된 함수를 skillSOs의 skillRepeat_Now번 만큼 skillRepeatCooldown_Now마다 반복해줌
    {
        int RepeatNum = skillSOs[0].skillRepeat_Now;
        while (RepeatNum > 0)
        {
            skillEffect();
            yield return new WaitForSeconds(skillSOs[0].skillRepeatCooldown_Now);
            RepeatNum--;
        }
    }

    void MineMineral()
    {
        if (GetComponent<PlayerMoveSet>().mineral == null)//광물이 없다고 판단 하면 다시 공격을 할 수 있게 해줌
        {
            GetComponent<PlayerStat>().isDisableATK = false;
            return;
        }

        GameObject mineral = GetComponent<PlayerMoveSet>().mineral;//로컬 변수 미네랄에 플레이어무브셋의 미네랄을 할당

        GetComponent<PlayerStat>().isDisableATK = true;//공격을 못하게 제한 시킴

        if (Time.time - skillSOs[0].skillCooldown_Now >= skillCoolingTimer)//델타 타임과 getherCooldown을 비교해서 lastGetherTime보다 크면 광물을 채굴 시킴
        {
            skillCoolingTimer = Time.time;
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

        foreach (GameObject bullet in Bullets)
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
            theBullet = Instantiate(skillSOs[0].bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            Bullets.Add(theBullet);
        }

        theBullet.GetComponent<Bullet>().target = GetComponent<PlayerStat>().mousePos;
        theBullet.GetComponent<Bullet>().Reset();
        theBullet.GetComponent<Bullet>().ATK = new RatioStatRuntime(GetComponent<PlayerStat>().ATK.FinalRatio);
        theBullet.GetComponent<Bullet>().phyATK = new SingleStatRuntime(GetComponent<PlayerStat>().phyAtk.FinalValue);
        theBullet.GetComponent<Bullet>().EnATK = new SingleStatRuntime(GetComponent<PlayerStat>().enAtk.FinalValue);
        theBullet.GetComponent<Bullet>().GM = GetComponent<PlayerStat>().GM;
    }

    IEnumerator SpecialSkillColling()//스킬 쿨타임
    {
        while (true)
        {
            if (skillCoolingTimer == skillSOs[0].skillCooldown_Now)
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
            skillCoolingTimer++;
            SkillCooltext.text = $"{skillCoolingTimer}/{skillSOs[0].skillCooldown_Now}";
        }
        skillCoolingTimer = 1;
        isSpecialATKable = true;
    }
}
