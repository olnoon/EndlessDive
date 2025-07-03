using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public List<GameObject> enemies;
    public List<GameObject> orbs;

    public GameObject enemyPrefab;

    public GameObject UpgradeScreen;

    public GameObject orbPrefab;

    void Start()
    {
        StartCoroutine(SpawnTempEnemy());
    }

    IEnumerator SpawnTempEnemy()
    {
        while (true)
        {
            Vector2 pos = new Vector2(1, 1);
            string kind = "A";
            int num = 2;
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
    public void UpgradeOn()//UpgradeScreen활성화
    {
        PauseTime(0);
        UpgradeScreen.SetActive(true);
    }

    public void PauseTime(int isStop)//시간 활성화 및 시간 정지
    {
        Time.timeScale = isStop;
    }

    public void GenerateXPorb(GameObject enemy)//적 사망시 경험치 오브를 생성함
    {
        bool reused = false;

        foreach (GameObject orb in orbs)
        {
            var orbScript = orb.GetComponent<ExpOrb>();
            if (!orb.activeSelf)
            {
                orb.transform.position = enemy.transform.position;
                orb.SetActive(true);
                reused = true;
                break;
            }
        }

        if (!reused)
        {
            GameObject orb = Instantiate(orbPrefab, enemy.transform.position, Quaternion.identity);
            orbs.Add(orb);
        }
    }

}