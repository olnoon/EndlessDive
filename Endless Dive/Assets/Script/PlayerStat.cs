using UnityEngine;
using System.Collections.Generic;

public class PlayerStat : MonoBehaviour
{
    public int maxHP;
    public int currectHP;
    public PlayerStatsSetSO stat;

    void Start()
    {
        
    }

    void Update()
    {
        if (currectHP <= 0)
        {
            GameOver();
        }
    }
    
    void GameOver()
    {
        Debug.Log("GameOver");
    }
}
