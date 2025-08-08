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
    public Text SkillCooltext;//스킬 쿨타임을 보여주는 텍스트
    public Text SkillLvltext;//스킬 쿨타임을 보여주는 텍스트
    [SerializeField] bool canUse = true;//스킬 사용 가능 여부
    public List<SkillSO> skillSOSets;//스킬 관련 변수가 담긴 SO(초기화용)
    public List<SkillSO> skillSOs;//스킬 관련 변수가 담긴 SO(보관용)
    public List<GameObject> bullets;//필드에 나와있는 탄환들(재사용을 위한)
    public bool isDisableATK = false;
    public bool isDisableOperation = false;
    Action skillEffect;
    public bool isCooling;//스킬에 대한 쿨타임 중 인지의 여부
    [SerializeField] int repeat;//스킬 반복 횟수
    [SerializeField] float repeatCooldown;//효과적용 간격
    [SerializeField] float skillCoolingRepeatTimer;//스킬 최소 사용 간격
    [SerializeField] bool isUsageCooling;//반복에 대한 쿨타임 중 인지의 여부
    [SerializeField] bool isUsingSkill;//스킬을 쓰고 있는지에 대한 여부

    void Start()
    {
        bulletSpawnPoint = GetComponent<PlayerStat>().bulletSpawnPoint;
        skillSOs = new List<SkillSO>(skillSOSets);
        skillCoolingTimer = skillSOs[0].skillCooldown_Now;

        SkillCooltext.text = $"{skillCharges}/{skillSOs[0].skillMaxCharges_Now}";
        StartCoroutine(SkillCooling());
    }

    void Update()
    {
        if (SkillLvltext != null)
        {
            SkillLvltext.text = skillSOs[0].skillLvl.ToString();
        }

        if (skillCharges >= 1)
        {
            canUse = true;
        }
        
        //skillType이 Mine이면 모든 플레이어 스킬 스크립트의 공격을 활성화 시켜 줌
        if (skillSOs[0].skillType == SkillType.Mine)
        {
            if (GetComponent<PlayerMoveSet>().mineral == null || !Input.GetKey(key))
            {
                foreach (PlayerSkill playerSkill in GetComponents<PlayerSkill>())
                {
                    playerSkill.isDisableATK = false;
                }
            }
        }

        //key가 누르면 어떤 스킬을 실행할지 판단
        if (Input.GetKeyDown(key) && Time.timeScale != 0 && !isUsingSkill)
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
        //canUse가 false이거나 쿨타임이 돌고 있거나 광물을 채굴중이여서 isDiableATK가 켜져 있거나 조작 불가능 상태가 켜져있으면 리턴
        if (!canUse || isDisableATK || isDisableOperation)
        {
            return;
        }

        canUse = false;

        //스킬타입에 맞는 스킬을 skillEffect 변수에 구독
        switch (skillSOs[0].skillType)
        {
            case SkillType.Basic:
                skillEffect = TriggerBullet;
                break;
            case SkillType.Poison:
                skillEffect = SpellSkill;
                break;
            case SkillType.NewSkill:
                skillEffect = SkillA;
                break;
            case SkillType.NewSkill2:
                skillEffect = SkillB;
                break;
            case SkillType.Mine:
                skillEffect = MineMineral;
                break;
        }

        //구독한 스킬을 반복하기 위해 RepeatSkill코루틴을 실행시켜줌
        StartCoroutine(RepeatSkill());
    }

    IEnumerator RepeatSkill()//skillEffect에 구독된 함수를 skillSOs의 skillRepeat_Now번 만큼 skillRepeatCooldown_Now마다 반복해줌
    {
        if (isUsingSkill)
        {
            yield break;
        }

        isUsingSkill = true;

        // skillEffect 미할당 방지
        if (skillEffect == null)
        {
            Debug.LogWarning("skillEffect is not set!");
            yield break;
        }
        
        foreach (PlayerSkill playerSkill in GetComponents<PlayerSkill>())//모든 플레이어 스킬 스크립트의 공격을 비활성화 시켜 줌.
        {
            if (skillSOs[0].isMultiple)
            {
                break;
            }
            
            if (playerSkill != this)
            {
                playerSkill.isDisableATK = true;
            }
        }

        while (true)
        {
            //광물 채굴중이여서 isDisableATK가 켜져있거나 키가 눌려있지 않으면 코루틴을 끝내줌
            if (isDisableATK)
            {
                isUsingSkill = false;

                StartCoroutine(RecoverSkills());
                
                yield break;
            }

            yield return new WaitUntil(() => !isUsageCooling);//isUsageCooling 거짓이 될때 까지 대기
            yield return new WaitUntil(() => skillCharges > 0);//skillCharges가 양수가 될때 까지 대기

            if (!Input.GetKey(key))
            {
                StartCoroutine(RecoverSkills());

                isUsingSkill = false;
                yield break;
            }

            if (skillSOs[0].cooldownTriggerType == CooldownTriggerType.OnCastStart)//쿨타임
            {
                StartCoroutine(SkillCooling());
            }

            yield return new WaitForSeconds(skillSOs[0].startUp);

            repeat = skillSOs[0].skillRepeat_Now;

            for (int i = 0; i < repeat; i++)//한 반복 루틴안에서 SkillEffect 반복
            {
                skillEffect?.Invoke();
                if (i == 0)
                {
                    skillCharges--;//skillCharges에서 1을 뺌
                    if (skillSOs[0].cooldownTriggerType == CooldownTriggerType.OnFirstEffect)
                    {
                        StartCoroutine(SkillCooling());
                    }
                }

                if (i == repeat - 1 && skillSOs[0].cooldownTriggerType == CooldownTriggerType.OnLastEffect)
                {
                    StartCoroutine(SkillCooling());
                }
                yield return new WaitForSeconds(skillSOs[0].skillRepeatCooldown_Now);
            }

            SkillCooltext.text = $"{skillCharges}/{skillSOs[0].skillMaxCharges_Now}";

            StartCoroutine(SkillRepeatCooling());//MinUsageInterval만큼의 시간을 기다린 후 스킬을 쓸 수 있게 해줌

            if (!skillSOs[0].autoUse)//autoUse가 아니면 한번만 실행
            {
                isUsingSkill = false;

                StartCoroutine(RecoverSkills());
                
                yield break;
            }
        }
    }

    void MineMineral()
    {
        if (GetComponent<PlayerMoveSet>().mineral == null)//광물이 없다고 판단 하면 다시 공격을 할 수 있게 해줌
        {
            StartCoroutine(RecoverSkills());
            return;
        }
        
        GetComponent<PlayerMoveSet>().mineral.GetComponent<Mineral>().Mined(GetComponent<PlayerStat>().mineAmount);
        Debug.Log($"{GetComponent<PlayerMoveSet>().mineral} 캐는 중");
    }

    IEnumerator RecoverSkills()//모든 플레이어 스킬 스크립트의 공격을 활성화 시켜 줌.
    {
        yield return new WaitForSeconds(skillSOs[0].recovery);

        foreach (PlayerSkill playerSkill in GetComponents<PlayerSkill>())
        {
            if (skillSOs[0].isMultiple)
            {
                break;
            }

            playerSkill.isDisableATK = false;
        }

        if (skillSOs[0].cooldownTriggerType == CooldownTriggerType.OnCastEnd)
        {
            StartCoroutine(SkillCooling());
        }
    }

    void SkillA()
    {
        if (isDisableATK)
            return;

        Debug.Log($"{key} 스킬 A발동");
        // 스킬 A의 고유 효과 실행 코드 추가 가능
        canUse = false;

    }

    void SkillB()
    {
        if (isDisableATK)
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
            if (bullet.GetComponent<Bullet>() == null)
            {
                continue;
            }
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
 
    public IEnumerator SkillRepeatCooling()//스킬 반복 쿨타임
    {
        isUsageCooling = true;

        yield return new WaitForSeconds(skillSOs[0].MinUsageInterval);

        isUsageCooling = false;
    }
    
    public IEnumerator SkillCooling()//스킬 쿨타임
    {
        if (isCooling)//쿨타임 돌리기 중복 방지
        {
            yield break;
        }
        isCooling = true;
        while (true)
        {
            skillCoolingTimer = skillSOs[0].skillCooldown_Now;//쿨타임 초기화

            if (skillSOs[0].skillMaxCharges_Now <= skillCharges)
            {
                break;
            }

            while (true)
            {
                if (skillCoolingTimer <= 0)//쿨타임이 0일 시 브레이크
                {
                    break;
                }
                yield return new WaitForSeconds(0.1f);
                skillCoolingTimer--;//0.1초후 쿨타임에 -1
            }

            if (skillSOs[0].chargeAll)
            {
                skillCharges = skillSOs[0].skillMaxCharges_Now;
            }
            else
            {
                skillCharges++;
            }

            SkillCooltext.text = $"{skillCharges}/{skillSOs[0].skillMaxCharges_Now}";

            yield return null;
        }
        isCooling = false;
    }

    void TriggerBullet()//Bullet생성 및 재사용, 또한 불렛의 변수들을 올바르게 초기화 시킴
    {
        GameObject theBullet = null;

        bool reused = false;

        if (isDisableATK)
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