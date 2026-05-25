using Sample;
using UnityEngine;

public class DeathOnLava : MonoBehaviour
{
    private GhostScript _ghostScript;

    private void OnTriggerEnter(Collider other)
    {
        if(_ghostScript = other.GetComponent<GhostScript>())
        {
            _ghostScript.Damage();
        }
    }

}
