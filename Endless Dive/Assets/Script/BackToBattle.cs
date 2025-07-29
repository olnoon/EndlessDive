using UnityEngine;

//임시로 작성한 스크립트로 나중에 게임메니저로 옮길 예정
public class BackToBattle : MonoBehaviour
{
    public void BackToMainScene()//메인씬으로 복귀
    {
        SceneLoader.Instance.LoadScene("MainScene");
    }
}
