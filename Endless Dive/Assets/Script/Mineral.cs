using UnityEngine;

public class Mineral : MonoBehaviour
{
    [SerializeField] float range;
    public bool isGetherable;
    public GameObject getherer;

    void Update()
    {
        transform.GetChild(0).localScale = new Vector2(range, range);//콜라이더를 갖고 있는 자식오브젝트의 크기를 range만큼 수정
    }
}
