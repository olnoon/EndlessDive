using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 싱글턴 인스턴스
    public List<GameObject> enemies;//필드에 나와있는 활성화/비활성화 되어있는 모든 적들
    public List<GameObject> orbs;//필드에 나와있는 활성화/비활성화 되어있는 모든 오브들

    public List<GameObject> enemyPrefabs;//적의 프리팹들

    public GameObject UpgradeScreen;//업그레이드 선택창

    public GameObject orbPrefab;//오브의 프리팹
    public EnemyKind missionTarget;//미션에서 필요로 하는 적의 종류
    public int missionNum;//미션에서 필요로 하는 적의 갯수
    public int currentMissionNum;//현재 잡은 미션에서 필요로 하는 적의 갯수
    public Text missionText;//미션현황을 보여주는 텍스트
    public List<Action> upgrades;//업그레이드할 갯수
    public Image blackScreen;
    public float fadeDuration = 1f;// - 씬이 시작될 때 호출됨
    public Coroutine enemySpawnRoutine;//씬이 바뀔때 눌익셉션과 바뀐씬에서의 적 스폰 방지를 위한 변수
    public List<GameObject> players;
    public GameObject statWindow;
    [Header("디버그용 스폰 가능/불가능 여부")]
    public bool isSpawnAble;

    void Awake()
    {
        // 싱글턴 패턴 적용: 인스턴스가 없다면 자신을 등록
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }
        else
        {
            Instance.ResetVars(this);
            Destroy(gameObject); // 기존 인스턴스가 있다면 제거
        }
    }

    void ResetVars(GameManager disappearGM)
    {
        UpgradeScreen = disappearGM.UpgradeScreen;//업그레이드 선택창을 없앨 GM에서 가져와서 초기화시켜 줌
        blackScreen = disappearGM.blackScreen;//암전화면을 없앨 GM에서 가져와서 초기화시켜 줌
        missionText = disappearGM.missionText;//미션텍스트를 없앨 GM에서 가져와서 초기화시켜 줌

        StartCoroutine(InitAfterSceneLoad());
    }

    void Start()
    {
        enemySpawnRoutine = StartCoroutine(SpawnTempEnemy());
        GiveMainMission();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))//Tab키 감지
        {
            ShowStatWindow();
        }
    }

    void OnEnable()
    {
        // 씬이 로드될 때마다 호출됨
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // 이벤트 등록 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드되었을 때 실행되는 메서드
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 특정 씬 이름일 때만 동작
        if (scene.name == "UpgradeScene")
        {
            ClearVars();
        }
    }

    IEnumerator InitAfterSceneLoad()
    {
        yield return null; // 씬 완전히 초기화되길 기다림

        GameObject newPlayer = null;
        GameObject oldPlayer = null;

        foreach (GameObject Obj in players)
        {
            if (Obj.activeSelf)
            {
                newPlayer = Obj;
            }
            else
            {
                oldPlayer = Obj;
            }
        }
        oldPlayer.SetActive(true);
        oldPlayer.GetComponent<PlayerStat>().SetAfterReturnToMain(newPlayer);

        players.Remove(newPlayer);

        enemySpawnRoutine = StartCoroutine(SpawnTempEnemy());
        GiveMainMission();
    }

    void ClearVars()//눌익셉션 방지를 위해 변수들을 Clear해주는 메서드
    {
        enemies.Clear();
        orbs.Clear();
        // upgrades.Clear();
        StopCoroutine(enemySpawnRoutine);
        enemySpawnRoutine = null;
    }

    public void FadeOut(bool isUpgrade)//isUpgrade는 Upgrade씬으로 이동할건지를 뜻함
    {
        StartCoroutine(Fade(0f, 1f, isUpgrade)); // 화면을 암전
    }

    public void FadeIn(bool isUpgrade)//isUpgrade는 Upgrade씬으로 이동할건지를 뜻함
    {
        StartCoroutine(Fade(1f, 0f, isUpgrade)); // 화면을 복구
    }

    IEnumerator Fade(float startAlpha, float endAlpha, bool isUpgrade)//화면을 암전/밝힘을 해주는 메서드
    {
        float time = 0f;
        Color color = blackScreen.color;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            blackScreen.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        blackScreen.color = new Color(color.r, color.g, color.b, endAlpha);

        yield return null;

        if (isUpgrade)
        {
            SceneLoader.Instance.LoadScene("UpgradeScene");
        }
    }

    IEnumerator SpawnTempEnemy()//임시로 만든 SummonEnemy 반복시켜주는 코루틴
    {
        while (true)
        {
            if (!isSpawnAble)
            {
                yield return null;
                continue;
            }
            Vector2 pos = new Vector2(1, 1);
            EnemyKind kind = EnemyKind.A;
            int num = 1;
            SummonEnemy(pos, kind, num);
            yield return new WaitForSeconds(20);
        }
    }

    public void SummonEnemy(Vector2 pos, EnemyKind kind, int num)//생성위치, 적 종류, 갯수를 받아서 갯수만큼 생성위치에 알맞은 적의 종류를 생성 혹은 재사용하는 함수
    {
        for (int i = 0; i < num; i++)
        {
            bool reused = false;

            foreach (GameObject enemy in enemies)
            {
                var move = enemy.GetComponent<EnemyMove>();
                if (move.state == State.Death && move.kind == kind)
                {
                    move.Revive(pos);
                    enemy.transform.position = pos;
                    enemy.SetActive(true);
                    reused = true;
                    break;
                }
            }

            if (!reused)
            {
                GameObject investedEnemy = null;
                foreach (GameObject enemyPrefab in enemyPrefabs)
                {
                    if (enemyPrefab.GetComponent<EnemyMove>().kind == kind)
                    {
                        investedEnemy = enemyPrefab;
                    }
                }
                GameObject enemy = Instantiate(investedEnemy, pos, Quaternion.identity);
                enemies.Add(enemy);
            }
        }
    }

    public void UpgradeOn()//UpgradeScreen활성화, 레벨 아이콘이 없을시 미션 초기화
    {
        if (upgrades.Count == 0)
        {
            EndFinalUpgrade();
            return;
        }
        upgrades.RemoveAt(0);
        UpgradeScreen.SetActive(true);
    }

    public void EndFinalUpgrade()//미션을 완료한 후 쌓여있던 업그레이드를 모두 끝냈을 때 사용하는 메서드
    {
        PauseTime(1);
        currentMissionNum = 0;
        missionText.text = $"{missionTarget}을 {missionNum}만큼 잡으시오.({currentMissionNum}/{missionNum})";
        Vector3 resetPos = Vector3.zero;
        GameObject.FindGameObjectWithTag("Player").transform.position = resetPos;
    }
    public void PauseTime(int isStop)//시간 활성화 및 시간 정지
    {
        Time.timeScale = isStop;
    }

    public void GenerateOrb(GameObject enemy, OrbKind orbKind)//적 사망시 경험치 혹은 체력 오브를 생성/재사용함
    {
        bool reused = false;

        foreach (GameObject orb in orbs)
        {
            if (!orb.activeSelf)
            {
                orb.transform.position = enemy.transform.position;
                orb.GetComponent<OrbScript>().orbKind = orbKind;
                orb.SetActive(true);
                reused = true;
                break;
            }
        }

        if (!reused)
        {
            GameObject orb = Instantiate(orbPrefab, enemy.transform.position, Quaternion.identity);
            orb.GetComponent<OrbScript>().orbKind = orbKind;
            orbs.Add(orb);
        }
    }

    public void GiveMainMission()//미션타겟을 에너미 카인드와 수량을 정해서 미션을 주고 미션텍스트의 텍스틀 변경 시켜 줌
    {
        missionTarget = EnemyKind.A;
        currentMissionNum = 0;
        missionNum = 5;

        missionText.text = $"{missionTarget}을 {missionNum}만큼 잡으시오.({currentMissionNum}/{missionNum})";
    }

    public void IncreaseMissionRemain(EnemyKind enemyKind)//missionTarget증가 및 미션완료여부 판단
    {
        if (enemyKind == missionTarget)
        {
            currentMissionNum++;
            if (currentMissionNum == missionNum)
            {
                CompleteMission();
            }
            missionText.text = $"{missionTarget}을 {missionNum}만큼 잡으시오.({currentMissionNum}/{missionNum})";
        }
    }

    void CompleteMission()//미션을 완료하면 플레이어 무브셋의 이탈가능하게 하는 변수인 isExitable을 true로 바꿔줌
    {
        // foreach (GameObject enemy in enemies)
        // {
        //     enemy.SetActive(false);
        // }
        FindAnyObjectByType<PlayerMoveSet>().isExitable = true;
    }

    public void DealDamage(GameObject target, int ATK, int weight = 1)//에너미 게임 오브젝트에게 대미지를 가하는 메서드
    {
        if (target.GetComponent<EnemyStat>() != null)
        {
            target.GetComponent<EnemyStat>().HP.TakeDamage(ATK * weight);
            target.GetComponent<EnemyStat>().DetectDamage();
        }
        else
        {
            if (target.GetComponent<PlayerStat>().isInvincibility)
            {
                return;
            }
            target.GetComponent<PlayerStat>().HP.TakeDamage(ATK * weight);
        }
    }

    public void ShowMinerals(Text remainMineralText)
    {
        remainMineralText.text = $"현재 보유 자원 : {players[0].GetComponent<PlayerStat>().currentAether}";
    }

    

    void ShowStatWindow()
    {
        if (statWindow == null)//statWindow가 비어있을시 할당시켜줌
        {
            GameObject canvas = null;

            foreach (Canvas each in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                if (each != null && each.transform.parent == null)
                {
                    canvas = each.gameObject;
                    break;
                }
            }

            foreach (Transform child in canvas.transform)
            {
                if (child.gameObject.name == "StatWindowBackground")
                {
                    statWindow = child.gameObject;
                    Debug.Log(FindAnyObjectByType<Canvas>().name);
                    break;
                }
            }
        }
        
        if (statWindow.activeSelf)//statWindow가 활성화 되어있다면 꺼 줌(편의성)
        {
            HideStatWindow();
            return;
        }

        if (players[0].GetComponent<PlayerMoveSet>().isDisableOperation && SceneManager.GetActiveScene().name == "MainScene")//만약의 경우에 씬 넘어갈때 Tab키를 눌렀을 때 게임이 고장나는 것을 방지
        {
            return;
        }

        //StatWindow를 활성화, Stat들을 업데이트 시켜줌, StatWindow를 비활성화시키는 버튼에 HideStatWidow를 구독 시켜줌, 시간을 멈춤
        statWindow.SetActive(true);
        statWindow.transform.GetChild(0).GetComponent<StatWindow>().UpdateStat(players[0]);
        statWindow.transform.GetChild(0).GetComponent<StatWindow>().exitBTN.onClick.RemoveAllListeners();
        statWindow.transform.GetChild(0).GetComponent<StatWindow>().exitBTN.onClick.AddListener(HideStatWindow);
        PauseTime(0);
    }

    void HideStatWindow()//StatWidow 비활성화 및 시간을 다시 흐르게 함
    {
        statWindow.SetActive(false);
        PauseTime(1);
    }
}