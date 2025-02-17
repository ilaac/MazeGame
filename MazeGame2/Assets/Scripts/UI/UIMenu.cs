using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    void Awake()
    {
        Time.timeScale = 1f; // Ensures game starts with normal time
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f; // Ensure time is reset before loading
        SceneManager.LoadScene("MainMenu"); // Change "MainMenu" to your scene name
    }
    
    public void StartGame()
    {
        Time.timeScale = 1f; // Ensure time is reset before loading
        SceneManager.LoadScene("Main"); // Change "MainMenu" to your scene name
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Ensure it stops in the editor
#endif
    }
}