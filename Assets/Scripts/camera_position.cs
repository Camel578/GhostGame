using Sample;
using UnityEngine;

public class camera_position : MonoBehaviour
{

    private Transform ghostTransform;
    [SerializeField] private float ghostSpeed = 0.03f;

    void Start()
    {
        ghostTransform = FindObjectOfType<GhostScript>().transform;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(ghostTransform.position.x, transform.position.y, ghostTransform.position.z), ghostSpeed);
    }
}
