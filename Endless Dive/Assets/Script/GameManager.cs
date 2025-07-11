using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public List<GameObject> enemies;
    public List<GameObject> orbs;

    public List<GameObject> enemyPrefabs;

    public GameObject UpgradeScreen;

    public GameObject orbPrefab;
    public GameObject lvlParent;
    public GameObject lvlDisplay;
    public EnemyKind missionTarget;
    public int missionNum;
    public int currentMissionNum;
    public Text missionText;
    public List<Action> upgrades;
    void Start()
    {
        StartCoroutine(SpawnTempEnemy());
        GiveMainMission();
    }

    IEnumerator SpawnTempEnemy()
    {
        while (true)
        {
            Vector2 pos = new Vector2(1, 1);
            EnemyKind kind = EnemyKind.A;
            int num = 1;
            SummonEnemy(pos, kind, num);
            yield return new WaitForSeconds(7);
        }
    }

    public void SummonEnemy(Vector2 pos, EnemyKind kind, int num)
    {
        //생성위치, 적 종류, 갯수를 받아서 갯수만큼 생성위치에 알맞은 적의 종류를 생성 혹은 재사용하는 함수
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

    public void levelUP()//원형 아이콘 추가
    {
        foreach (Transform child in lvlParent.transform)
        {
            if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
                return;
            }
        }
        Instantiate(lvlDisplay, Vector2.zero, Quaternion.identity, lvlParent.transform);
    }
    public void UpgradeOn()//UpgradeScreen활성화
    {
        if(upgrades.Count == 0)
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

    public void GenerateOrb(GameObject enemy, OrbKind orbKind)//적 사망시 경험치 혹은 체력 오브를 생성함
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

    public void GiveMainMission()//미션을 줌
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

    void CompleteMission()//upgrades에 UpgradeOn함수를 구독해 줌, 또한 레벨 초기화
    {
        upgrades = new List<Action>();
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(false);
        }
        for (int i = 0; i < lvlParent.transform.childCount; i++)
        {
            if (lvlParent.transform.GetChild(i).gameObject.activeSelf)
            {
                upgrades.Add(UpgradeOn);
            }
        }
        if (upgrades.Count <= 0)
        {
            Debug.Log("미션완료, 레벨 낮아서 업그레이드 불가");
            GiveMainMission();
            return;
        }
        foreach (Transform child in lvlParent.transform)
        {
            child.gameObject.SetActive(false);
        }
        PauseTime(0);
        upgrades[0]();
    }
}