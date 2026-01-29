using UnityEngine;
using UnityEngine.InputSystem;


public class RayManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask raycastLayer;


    private Vector2 screenPos => Mouse.current.position.ReadValue();
    private bool isRaycast = false;
    private Vector3 raycastPosition;
    private IDamagable currentTarget;

    void Start()
    {
        isRaycast = true;
        currentTarget = null;
    }


    private void Update()
    {
        if (!isRaycast) return;
        //PointerMove();
        PointerDamageIfCheck();
    }
    private void PointerMove()
    {
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

    private void PointerDamageIfCheck()
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50f, raycastLayer))
        {
            raycastPosition = hit.point;
            GameEvent.Input.PublishPointerMove(raycastPosition);
            var target = hit.collider.GetComponent<IDamagable>();
            if (target != null && target != currentTarget)
            {
                currentTarget = target;
                GameEvent.Input.PublishPointerDamage(target);
            }
            else if (target == null && currentTarget != null)
            {
                currentTarget = null;
                GameEvent.Input.PublishPointerDamage(null);
            }
        }
        else
        {
            if (currentTarget != null)
            {
                currentTarget = null;
                GameEvent.Input.PublishPointerDamage(null);
            }
        }
    }



}
