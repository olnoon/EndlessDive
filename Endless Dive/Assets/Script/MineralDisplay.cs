using UnityEngine;

public class MineralDisplay : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)//해당 스크립트의 부모의 getherer할당
    {
        transform.parent.GetComponent<Mineral>().isGetherable = true;
        transform.parent.GetComponent<Mineral>().getherer = collision.gameObject;
    }

    void OnTriggerExit2D(Collider2D collision)//해당 스크립트의 부모의 getherer초기화
    {
        transform.parent.GetComponent<Mineral>().isGetherable = false;
        transform.parent.GetComponent<Mineral>().getherer = null;
    }
}
