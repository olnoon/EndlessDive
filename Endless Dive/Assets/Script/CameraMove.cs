using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    void Update()//카메라를 자동으로 playerTransform으로 이동시킴
    {
        Vector3 moveToPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, -10);
        this.transform.position = moveToPosition;
    }
}
