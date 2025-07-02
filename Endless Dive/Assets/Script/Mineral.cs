using UnityEngine;
using UnityEngine.UI;

public class Mineral : MonoBehaviour
{
    [SerializeField] float range;
    public bool isGetherable;
    public GameObject getherer;
    [SerializeField] int currectQuantity;
    [SerializeField] int totalQuantity;
    [SerializeField] GameObject quantityBarBackground;
    [SerializeField] Image quantityBarFilled;
    [SerializeField] Text quantityText;

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

    public void Gathered()//currectQuantity을 줄이고 플레이어스텟의 mineralNum을 그만큼 늘려줌
    {
        currectQuantity--;
        if (getherer.GetComponent<PlayerStat>() != null)
        {
            getherer.GetComponent<PlayerStat>().mineralNum++;
        }
        quantityBarFilled.fillAmount = (float)currectQuantity / totalQuantity;
        quantityText.text = $"{currectQuantity}/{totalQuantity}";
        if (currectQuantity == 0)
        {
            if (getherer.GetComponent<PlayerMoveSet>() != null)
            {
                getherer.GetComponent<PlayerMoveSet>().mineral = null;
            }
            gameObject.SetActive(false);
        }
    }
}