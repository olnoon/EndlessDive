using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public List<GameObject> enemies;

    public GameObject enemyPrefab;

    public void SummonEnemy(Vector2 pos, string kind, int num)
    {
        for (int i = 0; i < num; i++)
        {
            bool reused = false;

            foreach (GameObject enemy in enemies)
            {
                var move = enemy.GetComponent<EnemyMove>();
                if (move.state == State.Death && move.kind.ToString() == kind)
                {
                    enemy.transform.position = pos;
                    enemy.SetActive(true);
                    reused = true;
                    break;
                }
            }

            if (!reused)
            {
                Instantiate(enemyPrefab, pos, Quaternion.identity);
            }
        }
    }

}
