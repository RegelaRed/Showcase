using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private Transform _Orientation;
    [SerializeField] private float _sensX = 50f;
    [SerializeField] private float _sensY = 50f;

    private float _xRot;
    private float _yRot;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        float _mouseX = Input.GetAxisRaw("Mouse X") * (_sensX * 10) * Time.deltaTime;
        float _mouseY = Input.GetAxisRaw("Mouse Y") * (_sensY * 10) * Time.deltaTime;

        _yRot += _mouseX;
        _xRot -= _mouseY;

        _xRot = Mathf.Clamp(_xRot, -80, 90);

        transform.rotation = Quaternion.Euler(_xRot, _yRot, 0f);
        _Orientation.rotation = Quaternion.Euler(0f, _yRot, 0f);
    }
}
