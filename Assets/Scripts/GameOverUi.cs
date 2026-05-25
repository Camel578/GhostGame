using UnityEngine;
using UnityEngine.SceneManagement; // ОБЯЗАТЕЛЬНО для работы со сценами

public class GameOverUi : MonoBehaviour
{
    [Header("Окна интерфейса")]
    [SerializeField] private GameObject _panel;     // Сюда перетащи GameOverWindow
    [SerializeField] private GameObject _winPanel;  // Сюда перетащи WinWindow

    void OnEnable()
    {
        GameDifficultyManager.OnGameOverTriggered += ShowGameOverWindow;
    }

    void OnDisable()
    {
        GameDifficultyManager.OnGameOverTriggered -= ShowGameOverWindow;
    }

    void Start()
    {
        // При старте уровня прячем оба окна
        if (_panel != null) _panel.SetActive(false);
        if (_winPanel != null) _winPanel.SetActive(false);
    }

    // Метод для вызова проигрыша
    public void ShowGameOverWindow()
    {
        if (_panel != null)
        {
            _panel.SetActive(true);
            StopGameAndShowCursor();
        }
    }

    // МЕТОД ДЛЯ ВЫЗОВА ПОБЕДЫ (Вызывай его из триггера финиша)
    public void ShowWinWindow()
    {
        if (_winPanel != null)
        {
            DataContainer.isLevelWon = true; // Запоминаем победу
            DataContainer._deaths = 0;       // Сбрасываем смерти

            _winPanel.SetActive(true);       // Включаем окно победы
            StopGameAndShowCursor();
        }
    }

    // Вспомогательный метод, чтобы не дублировать код паузы
    private void StopGameAndShowCursor()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Кнопка "В меню" (при проигрыше)
    public void OnRestartButtonPressed()
    {
        if (GameDifficultyManager.Instance != null)
        {
            GameDifficultyManager.Instance.GoToMainMenu();
        }
    }

    // КНОПКА "СЛЕДУЮЩИЙ УРОВЕНЬ" (Для твоей новой кнопки RestartButton под WinWindow)
    public void OnNextLevelButtonPressed()
    {
        Time.timeScale = 1f; // Возвращаем время в норму перед загрузкой!

        // Получаем индекс следующей сцены
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Если следующий уровень существует в Build Settings — загружаем его
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Если уровни кончились, отправляем в главное меню
            SceneManager.LoadScene(0);
        }
    }
}