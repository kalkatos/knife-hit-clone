using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool skipMenu { get; private set; }

    private void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void RestartGame ()
    {
        skipMenu = true;
        SceneManager.LoadScene(0);
    }

    public void GoToMenu ()
    {
        skipMenu = false;
        SceneManager.LoadScene(0);
    }
}
