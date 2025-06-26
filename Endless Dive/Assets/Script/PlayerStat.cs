using UnityEngine;
using System.Collections.Generic;

public class PlayerStat : MonoBehaviour
{
    public PlayerStatsSetSO stat;
    public GaugeStatRuntime HP;
    public SingleStatRuntime ATK;
    public RatioStatRuntime Cri;
    public RatioStatRuntime Dam;

    void Awake()
    {
        HP = new GaugeStatRuntime(stat.hp.MaxFinal);
        ATK = new SingleStatRuntime(stat.atk.FinalValue);
        Cri = new RatioStatRuntime(stat.cri.FinalRatio);
        Dam = new RatioStatRuntime(stat.criDam.FinalRatio);
    }
    
    void Update()
    {
        if (HP.MaxFinal <= 0)
        {
            GameOver();
        }
    }

    void GameOver()//게임오버
    {
        Debug.Log("GameOver");
    }
}
