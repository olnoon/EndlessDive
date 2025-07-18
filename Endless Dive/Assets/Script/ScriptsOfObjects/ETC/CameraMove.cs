using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] GameObject player;//플레이어
    public float minX;//갈 수 있는 x방향 최솟값
    public float maxX = 10f;//갈 수 있는 x방향 최댓값
    public float minY = -5f;//갈 수 있는 y방향 최솟값
    public float maxY = 5f;//갈 수 있는 y방향 최댓값

    void Awake()
    {
        player = FindAnyObjectByType<PlayerMoveSet>().gameObject;
        minX = player.GetComponent<PlayerMoveSet>().minX + 8.6f;
        maxX = player.GetComponent<PlayerMoveSet>().maxX - 8.6f;
        minY = player.GetComponent<PlayerMoveSet>().minY + 4.37f;
        maxY = player.GetComponent<PlayerMoveSet>().maxY - 4.37f;
    }
    void LateUpdate()//제한된 위치 안에서 플레이어 위치로 부드럽게 이동
    {
        Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
    }
}
