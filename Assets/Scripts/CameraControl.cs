using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Actor actor;
    private Vector3 defaultPosition;

    void Start()
    {
        defaultPosition = transform.position;
        actor = FindObjectOfType<Actor>();
    }

    void Update()
    {
        transform.position = actor.transform.position + defaultPosition;
    }
}
