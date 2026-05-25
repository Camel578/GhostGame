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
        // ѕаттерн Singleton: объект существует только в ќƒЌќћ экземпл€ре на всю игру
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // ”ничтожаем дубликаты, если они создались
        }
    }

    public void RegisterDeath()
    {
        CurrentDeaths++;
        Debug.Log($"—мерть зарегистрирована: {CurrentDeaths} из {MaxDeaths}");

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

    // Ётот метод теперь просто обнул€ет цифры, не лома€ сам объект
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        CurrentDeaths = 0;   // ќЅЌ”Ћя≈ћ счетчик смертей!
        MaxDeaths = 0;      // —брасываем сложность
        SceneManager.LoadScene(0); // ѕереходим в меню
    }
}