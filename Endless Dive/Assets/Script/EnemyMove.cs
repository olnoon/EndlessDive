using UnityEngine;
using DG.Tweening;

public enum State
{
    Normal,
    Death
}
public enum EnemyKind
{
    A,
    B
}
public class EnemyMove : MonoBehaviour
{
    public State state;
    public EnemyKind kind;
    [SerializeField] int moveDuration;//속도
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;
    [SerializeField] GameObject player;

    void Update()
    {
        Move();
    }

    public void Revive(Vector2 spawnPos)
    {
        //spawnPos로 가서 해당 오브젝트를 활성화 되게 하는 함수
        transform.position = spawnPos;
        state = State.Normal;
        gameObject.SetActive(true);
    }

    void Move()//움직임 제어
    {
        player = FindAnyObjectByType<PlayerMove>().gameObject;
        transform.DOMove(player.transform.position, moveDuration);
    }
}
