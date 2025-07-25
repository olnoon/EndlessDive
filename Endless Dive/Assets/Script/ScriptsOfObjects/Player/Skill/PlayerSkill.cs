using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public enum SkillType
{
    Basic,
    Poison,
    Mine,
    NewSkill,
    NewSkill2
}

public class PlayerSkill : MonoBehaviour
{
    [SerializeField] KeyCode key;//스킬의 키코드
    [SerializeField] Transform bulletSpawnPoint;//불렛 스폰위치
    [SerializeField] float skillCoolingTimer;//스킬의 남은 쿨타임
    [SerializeField] int skillCharges;//스킬의 남은 충전 횟수
    [SerializeField] Text SkillCooltext;//스킬 쿨타임을 보여주는 텍스트
    [SerializeField] Text SkillLvltext;//스킬 쿨타임을 보여주는 텍스트
    [SerializeField] bool canUse = true;//스킬 사용 가능 여부
    [SerializeField] bool isCooling;//스킬 쿨타임 가능 여부
    [SerializeField] List<SkillSO> skillSOSets;//스킬 관련 변수가 담긴 SO(초기화용)
    [SerializeField] List<SkillSO> skillSOs;//스킬 관련 변수가 담긴 SO(보관용)
    public List<GameObject> bullets;//필드에 나와있는 탄환들(재사용을 위한)
    public bool isDisableATK = false;
    Action skillEffect;


    void Start()
    {
        bulletSpawnPoint = GetComponent<PlayerStat>().bulletSpawnPoint;
        skillSOs = new List<SkillSO>(skillSOSets);
        skillCoolingTimer = skillSOs[0].skillCooldown_Now;
        StartCoroutine(SkillCooling());
    }

    void Update()
    {
        SkillLvltext.text = skillSOs[0].skillLvl.ToString();
        if (skillCharges >= 1)
        {
            canUse = true;
        }
        else
        {
            canUse = false;
        }
        if (skillSOs[0].skillType == SkillType.Mine)
        {
            if (GetComponent<PlayerMoveSet>().mineral == null || !Input.GetKey(key))
            {
                GetComponent<PlayerStat>().isDisableATK = false;
            }
        }
        if (Input.GetKeyDown(key) && Time.timeScale != 0)
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

    void DetermineSkill()//어떤 스킬을 실행할지 판단 및 스킬을 반복시켜주는 코루틴 실행
    {
        if (!canUse || isCooling || isDisableATK)
        {
            return;
        }
        switch (skillSOs[0].skillType)
        {
            case SkillType.Basic:
                skillEffect = TriggerBullet;
                break;
            case SkillType.Poison:
                if (!canUse || GetComponent<PlayerStat>().isDisableATK)
                {
                    break;
                }
                skillEffect = SpellSkill;
                break;
            case SkillType.NewSkill:
                if (!canUse || GetComponent<PlayerStat>().isDisableATK)
                {
                    break;
                }
                skillEffect = SkillA;
                break;
            case SkillType.NewSkill2:
                if (!canUse || GetComponent<PlayerStat>().isDisableATK)
                {
                    break;
                }
                skillEffect = SkillB;
                break;
            case SkillType.Mine:
                skillEffect = MineMineral;
                break;
        }
        StartCoroutine(RepeatSkill());
    }

    IEnumerator RepeatSkill()//skillEffect에 구독된 함수를 skillSOs의 skillRepeat_Now번 만큼 skillRepeatCooldown_Now마다 반복해줌
    {
        int RepeatNum = skillSOs[0].skillRepeat_Now;

        // skillEffect 미할당 방지
        if (skillEffect == null)
        {
            Debug.LogWarning("skillEffect is not set!");
            yield break;
        }

        while (RepeatNum > 0)
        {
            if (isDisableATK)
            {
                yield break;
            }
            
            // 쿨다운이 끝날 때까지 대기
            while (isCooling)
                yield return null;

            // 키 입력 체크: 필요 없으면 제거 가능
            if (!Input.GetKey(key))
                break;

            // 실제 효과 실행
            skillEffect?.Invoke();
            //쿨타임 돌리기
            StartCoroutine(SkillCooling());
            skillCharges--;

            RepeatNum--;

            // 다음 반복 전 대기
            yield return new WaitForSeconds(skillSOs[0].skillRepeatCooldown_Now);
        }

        foreach (PlayerSkill playerSkill in GetComponents<PlayerSkill>())//모든 플레이어 스킬 스크립트의 공격을 활성화 시켜 줌.
        {
            playerSkill.isDisableATK = false;
        }
    }

    void MineMineral()
    {
        foreach (PlayerSkill playerSkill in GetComponents<PlayerSkill>())//모든 플레이어 스킬 스크립트의 공격을 비활성화 시켜 줌.
        {
            if (playerSkill != this)
            {
                playerSkill.isDisableATK = true;
            }
        }

        if (GetComponent<PlayerMoveSet>().mineral == null)//광물이 없다고 판단 하면 다시 공격을 할 수 있게 해줌
        {
            foreach (PlayerSkill playerSkill in GetComponents<PlayerSkill>())//모든 플레이어 스킬 스크립트의 공격을 활성화 시켜 줌.
            {
                playerSkill.isDisableATK = false;
            }
            return;
        }

        GetComponent<PlayerStat>().isDisableATK = true;//공격을 못하게 제한 시킴
        
        GetComponent<PlayerMoveSet>().mineral.GetComponent<Mineral>().Mined();
        Debug.Log($"{GetComponent<PlayerMoveSet>().mineral} 캐는 중");
    }

    void SkillA()
    {
        if (!canUse || GetComponent<PlayerStat>().isDisableATK)
            return;

        Debug.Log($"{key} 스킬 A발동");
        // 스킬 A의 고유 효과 실행 코드 추가 가능
        canUse = false;

    }

    void SkillB()
    {
        if (!canUse || GetComponent<PlayerStat>().isDisableATK)
            return;

        Debug.Log($"{key} 스킬 B발동");
        // 스킬 B의 고유 효과 실행 코드 추가 가능
        canUse = false;

    }

    void SpellSkill()//Skill 탄환 생성/재사용 및 불렛 스크립트의 변수들을 초기화 시켜주는 함수
    {
        GameObject theBullet = null;

        bool reused = false;

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
            theBullet = Instantiate(skillSOs[0].bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            bullets.Add(theBullet);
        }
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        theBullet.GetComponent<Bullet>().target = mousePos;
        theBullet.GetComponent<Bullet>().Reset();
        theBullet.GetComponent<Bullet>().ATK = new RatioStatRuntime(GetComponent<PlayerStat>().ATK.FinalRatio);
        theBullet.GetComponent<Bullet>().phyATK = new SingleStatRuntime(GetComponent<PlayerStat>().phyAtk.FinalValue);
        theBullet.GetComponent<Bullet>().EnATK = new SingleStatRuntime(GetComponent<PlayerStat>().enAtk.FinalValue);
        theBullet.GetComponent<Bullet>().GM = GetComponent<PlayerStat>().GM;
    }

    IEnumerator SkillCooling()//스킬 쿨타임
    {
        isCooling = true;

        skillCoolingTimer = skillSOs[0].skillCooldown_Now;//쿨타임 초기화

        while (true)
        {
            if (skillCoolingTimer <= 0)//쿨타임이 0일 시 브레이크
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
            skillCoolingTimer--;//0.1초후 쿨타임에 -1
            SkillCooltext.text = $"{skillCoolingTimer}/{skillSOs[0].skillCooldown_Now}";
        }
        isCooling = false;
    }

    void TriggerBullet()//Bullet생성 및 재사용, 또한 불렛의 변수들을 올바르게 초기화 시킴
    {
        GameObject theBullet = null;

        bool reused = false;

        if (GetComponent<PlayerStat>().isDisableATK)
        {
            return;
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
            theBullet = Instantiate(skillSOs[0].bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            bullets.Add(theBullet);
        }
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        theBullet.GetComponent<Bullet>().target = mousePos;
        theBullet.GetComponent<Bullet>().Reset();
        theBullet.GetComponent<Bullet>().ATK = new RatioStatRuntime(GetComponent<PlayerStat>().ATK.FinalRatio);
        theBullet.GetComponent<Bullet>().phyATK = new SingleStatRuntime(GetComponent<PlayerStat>().phyAtk.FinalValue);
        theBullet.GetComponent<Bullet>().EnATK = new SingleStatRuntime(GetComponent<PlayerStat>().enAtk.FinalValue);
        theBullet.GetComponent<Bullet>().GM = GetComponent<PlayerStat>().GM;
    }
}