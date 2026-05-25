using UnityEngine;

public class GameOverUi : MonoBehaviour
{
    [SerializeField] private GameObject _panel;

    void OnEnable()
    {
        // Подписываемся на событие смерти в менеджере
        GameDifficultyManager.OnGameOverTriggered += ShowGameOverWindow;
    }

    void OnDisable()
    {
        // Обязательно отписываемся при выходе, чтобы не было багов памяти
        GameDifficultyManager.OnGameOverTriggered -= ShowGameOverWindow;
    }

    void Start()
    {
        if (_panel != null) _panel.SetActive(false);
    }

    public void ShowGameOverWindow()
    {
        if (_panel != null)
        {
            _panel.SetActive(true); // Включаем темное окно победы/проигрыша
            Time.timeScale = 0f;    // Стопаем игру
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void OnRestartButtonPressed()
    {
        if (GameDifficultyManager.Instance != null)
        {
            GameDifficultyManager.Instance.GoToMainMenu();
        }
    }
}