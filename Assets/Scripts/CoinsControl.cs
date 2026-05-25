using UnityEngine;
using TMPro;

public class CoinsControl : MonoBehaviour
{
    [SerializeField] private TMP_Text coins;
    [SerializeField] private TMP_Text deaths;
    [SerializeField] private GameObject winerUI;

    void Update()
    {
        coins.text = DataContainer._coins.ToString();
        deaths.text = DataContainer._deaths.ToString();
    }


    public void FinishGame()
    {
        winerUI.SetActive(true);
    }
}
