using UnityEngine;

public class CamPositioner : MonoBehaviour
{
    [SerializeField] public Transform CamPos;
    // Update is called once per frame
    void Update()
    {
        transform.position = CamPos.position;
    }
}
