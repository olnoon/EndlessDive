using UnityEngine;
using UnityEngine.UI;

public class StatWindow : MonoBehaviour
{
    public Text HPtext;
    public Text ATKtext;
    public Text speedText;
    public Text criText;
    public Text CATKtext;
    public Button exitBTN;
    
    public void UpdateStat(GameObject player)//받아온 매게변수 player로 부터 능력치들을 Text에 업데이트 해 줌.
    {
        var stat = player.GetComponent<PlayerStat>();
        var move = player.GetComponent<PlayerMoveSet>();
        HPtext.text = $"최대 체력 : {stat.HP.MaxFinal}";
        ATKtext.text = $"공격력 : {stat.ATK.FinalRatio}";
        speedText.text = $"속도 : {move.basicSpeed}";
        criText.text = $"치명타율 : {stat.Cri.FinalRatio}";
        CATKtext.text = $"치명타 대미지 : {stat.Dam.FinalRatio}";
    }
}
