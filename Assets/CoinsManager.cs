using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    public static int TotalCoins; // Сколько монет всего на уровне
    public static int CollectedCoins; // Сколько собрали

    [Header("UI")]
    [SerializeField] private GameObject _winWindow; // Перетащи сюда свой WinWindow из Hierarchy

    void Start()
    {
        // Считаем все объекты с тегом "Coin" на сцене
        TotalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
        CollectedCoins = 0;
        _winWindow.SetActive(false); // Скрываем окно при старте
    }

    public void CollectCoin()
    {
        CollectedCoins++;
        Debug.Log("Монет собрано: " + CollectedCoins + " из " + TotalCoins);

        if (CollectedCoins >= TotalCoins)
        {
            ShowWinWindow();
        }
    }

    void ShowWinWindow()
    {
        _winWindow.SetActive(true); // Включаем окно
        Time.timeScale = 0f; // Останавливаем игру (опционально)
        Cursor.lockState = CursorLockMode.None; // Разблокируем курсор
        Cursor.visible = true;
    }
}