using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDifficultyManager : MonoBehaviour
{
    public static GameDifficultyManager Instance;

    public int MaxDeaths;
    public int CurrentDeaths;

    public static event Action OnGameOverTriggered;

    void Awake()
    {
        // Паттерн Singleton: объект существует только в ОДНОМ экземпляре на всю игру
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Уничтожаем дубликаты, если они создались
        }
    }

    public void RegisterDeath()
    {
        CurrentDeaths++;
        Debug.Log($"Смерть зарегистрирована: {CurrentDeaths} из {MaxDeaths}");

        if (CurrentDeaths >= MaxDeaths)
        {
            if (OnGameOverTriggered != null)
            {
                OnGameOverTriggered.Invoke();
            }
            else
            {
                GoToMainMenu();
            }
        }
    }

    // Этот метод теперь просто обнуляет цифры, не ломая сам объект
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        DataContainer._deaths = 0;
        DataContainer._coins = 0;
        MaxDeaths = 0;      // Сбрасываем сложность
        SceneManager.LoadScene(0); // Переходим в меню
    }
}