using UnityEngine;
using UnityEngine.InputSystem;


public class RayManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask raycastLayer;


    private Vector2 screenPos => Mouse.current.position.ReadValue();
    private bool isRaycast = false;

    private Vector3 raycastPosition;

    void Start()
    {
        isRaycast = true;
    }


    private void Update()
    {
        if (!isRaycast) return;
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50f, raycastLayer))
        {
            raycastPosition = hit.point;
            GameEvent.Input.PublishPointerAreaIn(true);
            GameEvent.Input.PublishPointerMove(raycastPosition);
        }
        else
        {
            GameEvent.Input.PublishPointerAreaIn(false);
        }
    }



}
