using UnityEngine;

public class UI_StarLevel : MonoBehaviour
{
    [SerializeField] GameObject obj_star;


    public void Set_StarLevel(bool _isActive)
    {
        obj_star.SetActive(_isActive);
    }


}
