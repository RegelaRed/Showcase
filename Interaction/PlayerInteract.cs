using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("References")]
    public Transform cameraMain;

    [Header("Interact values")]
    public KeyCode interact_key = KeyCode.E;
    private float interact_distance = 4.7f;
    private CanInteract currentInteractable;


    private void Update()
    {
        CheckInteractable();
    }

    private void CheckInteractable()
    {
        RaycastHit hit;
        Debug.DrawRay(cameraMain.position, cameraMain.forward * interact_distance, Color.red);
        if (Physics.Raycast(cameraMain.position + Vector3.forward * 0.2f, cameraMain.forward, out hit, interact_distance))
        {
            var interactable = hit.collider.gameObject.GetComponent<CanInteract>();
            if (interactable != currentInteractable)
            {
                Debug.Log("player currently looking at", hit.collider.gameObject);
                currentInteractable?.SetHighlight(false);
                currentInteractable = interactable;
                currentInteractable?.SetHighlight(true);
            }
            if (currentInteractable != null && Input.GetKeyDown(interact_key))
            {
                Debug.Log("player stopped looking at", hit.collider.gameObject);
                currentInteractable.Call();
            }
        }
        else
        {
            currentInteractable?.SetHighlight(false);
            currentInteractable = null;
        }
    }
}
