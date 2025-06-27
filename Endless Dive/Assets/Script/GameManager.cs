using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public List<GameObject> enemies;

    public GameObject enemyPrefab;

    void Start()
    {
        Vector2 pos = new Vector2(1, 1);
        string kind = "A";
        int num = 2;
        SummonEnemy(pos, kind, num);
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

}
