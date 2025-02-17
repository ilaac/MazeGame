using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Method to start the game
    public void StartGame()
    {
        // Load the game scene (Change "GameScene" to the actual name of your game scene)
        SceneManager.LoadScene("Main");
    }

    // Method to quit the game
    public void QuitGame()
    {
        Application.Quit();
        
#if UNITY_EDITOR
        // If in the Unity editor, stop the play mode
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}