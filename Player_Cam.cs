using UnityEngine;

public class Player_Cam : MonoBehaviour
{
    [SerializeField] private Transform _Orientation;
    [SerializeField] private float sensX = 50f;
    [SerializeField] private float sensY = 50f;

    private float xRot;
    private float yRot;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * (sensX * 10) * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * (sensY * 10) * Time.deltaTime;

        yRot += mouseX;
        xRot -= mouseY;

        xRot = Mathf.Clamp(xRot, -80, 90);

        transform.rotation = Quaternion.Euler(xRot, yRot, 0f);
        _Orientation.rotation = Quaternion.Euler(0f, yRot, 0f);
    }
}
