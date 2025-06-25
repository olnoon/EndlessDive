using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] GameObject player;
    public float minX;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    void Awake()
    {
        player = FindAnyObjectByType<PlayerMove>().gameObject;
        minX = player.GetComponent<PlayerMove>().minX + 8.6f;
        maxX = player.GetComponent<PlayerMove>().maxX - 8.6f;
        minY = player.GetComponent<PlayerMove>().minY + 4.37f;
        maxY = player.GetComponent<PlayerMove>().maxY - 4.37f;
    }
    void Update()//카메라를 자동으로 playerTransform으로 이동시킴
    {
        Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y, this.transform.position.z);

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        this.transform.position = targetPos;
    }
}
