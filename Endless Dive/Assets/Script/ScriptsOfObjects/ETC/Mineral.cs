using UnityEngine;
using UnityEngine.UI;

public class Mineral : MonoBehaviour
{
    [SerializeField] float range;//채굴 가능 범위
    public bool isGetherable;//채굴 가능 여부
    public GameObject miner;//채굴중인 캐릭터
    [SerializeField] int currectQuantity;//현재 양
    [SerializeField] int totalQuantity;//캘 수 있는 최대 양
    [SerializeField] GameObject quantityBarBackground;//양을 표시하는 바의 배경
    [SerializeField] Image quantityBarFilled;//양을 표시하는 바
    [SerializeField] Text quantityText;//양을 숫자로 표시하는 텍스트

    void Awake()
    {
        currectQuantity = totalQuantity;
        quantityBarFilled.fillAmount = 1f;
        quantityText.text = $"{currectQuantity}/{totalQuantity}";
    }

    void Update()
    {
        transform.GetChild(0).localScale = new Vector2(range, range);//콜라이더를 갖고 있는 자식오브젝트의 크기를 range만큼 수정
    }

    public void Mined()//currectQuantity을 줄이고 플레이어스텟의 mineralNum을 그만큼 늘려줌, 또한 양이 0이면 해당 오브젝트를 비활성화 시켜줌
    {
        currectQuantity--;
        if (miner.GetComponent<PlayerStat>() != null)
        {
            miner.GetComponent<PlayerStat>().mineralNum++;
            miner.GetComponent<PlayerStat>().mineralText.text = miner.GetComponent<PlayerStat>().mineralNum.ToString();

            // getherer.GetComponent<PlayerStat>().addXP();
        }
        quantityBarFilled.fillAmount = (float)currectQuantity / totalQuantity;
        quantityText.text = $"{currectQuantity}/{totalQuantity}";
        if (currectQuantity == 0)
        {
            if (miner.GetComponent<PlayerMoveSet>() != null)
            {
                miner.GetComponent<PlayerMoveSet>().mineral = null;
            }
            gameObject.SetActive(false);
        }
    }
}