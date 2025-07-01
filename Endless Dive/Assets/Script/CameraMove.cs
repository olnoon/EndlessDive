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
        player = FindAnyObjectByType<PlayerMoveSet>().gameObject;
        minX = player.GetComponent<PlayerMoveSet>().minX + 8.6f;
        maxX = player.GetComponent<PlayerMoveSet>().maxX - 8.6f;
        minY = player.GetComponent<PlayerMoveSet>().minY + 4.37f;
        maxY = player.GetComponent<PlayerMoveSet>().maxY - 4.37f;
    }
    void LateUpdate()
    {
        Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
    }
}
