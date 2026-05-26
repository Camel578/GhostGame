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
        Debug.Log("Игрок нажали кнопку: ВЫХОД ИЗ ИГРЫ");

        // 1. Эта строчка закроет игру, если она запущена как отдельная программа (.exe)
        Application.Quit();

        // 2. Эта строчка сработает ТОЛЬКО внутри редактора Unity, чтобы ты видел, что кнопка работает
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}