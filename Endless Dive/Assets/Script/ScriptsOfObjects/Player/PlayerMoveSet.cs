using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoveSet : MonoBehaviour
{
    public float basicSpeed;//기본 이동속도
    public RatioStatSO SPD;//이동 속도에 영향을 주는 능력치
    [SerializeField] Rigidbody2D rb;//리지드바디
    public float minX = -10f;//X좌표 최소제한
    public float maxX = 10f;//X좌표 최대제한
    public float minY = -5f;//Y좌표 최소제한
    public float maxY = 5f;//Y좌표 최대제한
    public GameObject mineral;//타겟팅된 미네랄
    public bool isDisableOperation;//조작 불가 상태
    public bool isExitable;//지역 이탈 가능 여부
    public bool isTriedExit;//지역 이탈 시도
    public GameObject exitTimeBarBG;//필드를 이탈하기까지의 시간을 표시할 UI의 배경
    public Image exitTimeBarFilled;//필드를 이탈하기까지의 시간을 표시할 UI
    public Text exitTimeText;//필드를 이탈하기까지의 시간을 표시할 UI의 텍스트

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        DontDestroyOnLoad(gameObject);
    }

    void FixedUpdate()
    {
        if (Time.timeScale != 0)
        {
            Move();
            UpdateFacing();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isExitable && !isTriedExit)
        {
            StartCoroutine(CheckExitZone());
        }
    }

    IEnumerator CheckExitZone()
    {
        int repeatAmount = 0;
        exitTimeBarBG.SetActive(true);//이탈시간을 보여주는 UI활성황
        GetComponent<PlayerStat>().isInvincibility = true;//무적 활성화
        foreach (PlayerSkill skill in GetComponents<PlayerSkill>())//스킬발동 비활성화
        {
            skill.isDisableOperation = true;
        }
        while (repeatAmount < 20)//2초 기다림
        {
            exitTimeBarFilled.fillAmount = (float)repeatAmount/20;
            exitTimeText.text = $"지역 이탈 까지 : {repeatAmount}/20";
            if (!Input.GetKey(KeyCode.F))//지역 이탈 포기
            {
                exitTimeBarBG.SetActive(false);
                foreach (PlayerSkill skill in GetComponents<PlayerSkill>())//스킬발동 활성화
                {
                    skill.isDisableOperation = false;
                }
                GetComponent<PlayerStat>().isInvincibility = false;//무적 비활성화
                yield break;
            }

            repeatAmount++;
            yield return new WaitForSeconds(0.1f);
        }
        isTriedExit = true;
        StartCoroutine(ExitZone());//지역 이탈 루틴 실행
    }

    IEnumerator ExitZone()
    {
        exitTimeBarBG.SetActive(false);
        isDisableOperation = true;//조작불가 상태 활성화
        //TODO 애니메이션 넣기
        yield return new WaitForSeconds(3);

        foreach (AbnormalStatus abnormalStatus in GetComponents<AbnormalStatus>())
        {
            if (!abnormalStatus.isDontDestroyOnLoad)
            {
                Destroy(abnormalStatus);
            }
        }

        gameObject.SetActive(false);//업그레이드씬에서 방해되지 않게 비활성화

        isExitable = false;//지역 이탈 비활성화

        isTriedExit = false;//지역 이탈 시도 비활성화

        GetComponent<PlayerStat>().GM.FadeOut(true);//암전 효과
    }

    void UpdateFacing()//마우스 방향 보는 함수
    {
        float currectScaleX = Math.Abs(transform.localScale.x);
        float lookingDirec = GetComponent<PlayerStat>().mousePos.x > transform.position.x ? currectScaleX : -currectScaleX;

        Vector2 toSize = new Vector2(lookingDirec, transform.localScale.y);

        transform.localScale = toSize;
    }

    void Move()//플레이어 움직임을 WASD에 따라 제어
    {
        if (isDisableOperation)//조작 불가능 상태가 켜져있으면 리턴
        {
            return;
        }

        float xSpeed = 0;
        float ySpeed = 0;

        if (Input.GetKey(KeyCode.A))
        {
            xSpeed = -1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            ySpeed = 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            xSpeed = 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            ySpeed = -1;
        }

        if (isCheckGetUpKey())
        {
            xSpeed = 0;
            ySpeed = 0;
        }

        Vector2 moveDir = new Vector2(xSpeed, ySpeed).normalized;

        float FinalSpeed = SPD != null ? basicSpeed * SPD.FinalRatio: basicSpeed;

        Vector2 targetPos = rb.position + moveDir * FinalSpeed * Time.fixedDeltaTime;

        float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);
        Vector2 clampedTarget = new Vector2(clampedX, clampedY);

        rb.MovePosition(clampedTarget);
    }

    bool isCheckGetUpKey()//WASD키를 입력하지 않고 있는지 확인
    {
        return Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D);
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Mineral")//mineral에 캐고 있는 광물 오브젝트 할당
        {
            mineral = collision.transform.parent.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Mineral")//mineral에 캐고 있는 광물 오브젝트 초기화
        {
            mineral = null;
        }
    }
}