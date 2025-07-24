using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    void Awake()
    {
        // - 싱글턴 패턴: 게임 내에서 하나의 SceneLoader만 존재하도록 보장
        if (Instance == null)
            Instance = this;
    }

    // 씬 이동 함수
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 게임 종료 함수
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("게임 종료");
    }
}