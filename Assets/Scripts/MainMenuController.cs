using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void SetDifficultyAndStart(int lives)
    {
        // Если менеджера почему-то нет на сцене меню, создаем его прямо из кода
        if (GameDifficultyManager.Instance == null)
        {
            GameObject brain = new GameObject("GameDifficultyManager");
            brain.AddComponent<GameDifficultyManager>();
        }

        // Записываем выбранную сложность в "живой" менеджер
        GameDifficultyManager.Instance.MaxDeaths = lives;
        GameDifficultyManager.Instance.CurrentDeaths = 0; // Гарантированный сброс в 0

        // Загружаем игру
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}