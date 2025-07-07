using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public List<GameObject> enemies;
    public List<GameObject> orbs;

    public GameObject enemyPrefab;

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
            string kind = "A";
            int num = 1;
            SummonEnemy(pos, kind, num);
            yield return new WaitForSeconds(5);
        }
    }

    public void SummonEnemy(Vector2 pos, string kind, int num)
    {
        //생성위치, 적 종류, 갯수를 받아서 갯수만큼 생성위치에 알맞은 적의 종류를 생성 혹은 재사용하는 함수
        for (int i = 0; i < num; i++)
        {
            bool reused = false;

            foreach (GameObject enemy in enemies)
            {
                var move = enemy.GetComponent<EnemyMove>();
                if (move.state == State.Death && move.kind.ToString() == kind)
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
                GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
                enemies.Add(enemy);
            }
        }
    }

    public void levelUP()//원형 아이콘 추가
    {
        //TODO 나중에 재사용 가능하게 바꿔 주기
        Instantiate(lvlDisplay, Vector2.zero, Quaternion.identity, lvlParent.transform);
    }
    public void UpgradeOn()//UpgradeScreen활성화
    {
        if(upgrades.Count == 0)
        {
            PauseTime(1);
            return;
        }
        upgrades.RemoveAt(0);
        UpgradeScreen.SetActive(true);
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
            enemy.GetComponent<EnemyStat>().HP.TakeDamage(enemy.GetComponent<EnemyStat>().HP.Current);
        }
        for (int i = 0; i < lvlParent.transform.childCount; i++)
        {
            upgrades.Add(UpgradeOn);
        }
        foreach (Transform child in lvlParent.transform)//TODO 나중에 비활성화로 바꿔줄 것
        {
            Destroy(child.gameObject);
        }
        PauseTime(0);
        upgrades[0]();
    }
}