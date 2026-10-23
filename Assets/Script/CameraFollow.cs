using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The target the camera will follow
    public Vector3 offset = new Vector3(0,0,-10); // Offset from the target position

    // Update is called once per frame
    void Update()
    {
        if (target != null) {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, 0.1f);
        }
    }
}
