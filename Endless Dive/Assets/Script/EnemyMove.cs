using UnityEngine;

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
    [SerializeField] int speed;//속도
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    void Update()
    {
        Move();
    }

    void Move()
    {
        Debug.Log("움직이는 중");
    }
}
