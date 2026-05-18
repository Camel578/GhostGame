using Sample;
using UnityEngine;

public class camera_position_by_firstface : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Transform ghostTransform;
    [SerializeField] private float ghostSpeed = 0.03f;

    void Start()
    {
        ghostTransform = FindObjectOfType<GhostScript>().transform;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(ghostTransform.position.x, ghostTransform.position.y, ghostTransform.position.z), ghostSpeed);
    }
}
