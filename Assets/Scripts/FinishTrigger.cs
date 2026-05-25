using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что наступил именно игрок
        if (other.GetComponent<Sample.GhostScript>() != null)
        {
            // Находим скрипт интерфейса на сцене и даем команду "Победа!"
            GameOverUi ui = FindObjectOfType<GameOverUi>();
            if (ui != null)
            {
                ui.ShowWinWindow();
            }
        }
    }
}