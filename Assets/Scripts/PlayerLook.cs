using Unity.Cinemachine;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform camTarget; // The child of the player used as the camera target
    public Transform Orientation;
    public float sensitivityX = 2f;
    public float sensitivityY = 2f;
    public float minY = -60f;
    public float maxY = 75f;

    private float yaw;
    private float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        camTarget.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        Orientation.localRotation = Quaternion.Euler(0f, yaw, 0f);
    }
}
