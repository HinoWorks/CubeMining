using UnityEngine;

public class AroundLayerUnit : MonoBehaviour
{
    [SerializeField] MeshRenderer mesh_1;
    [SerializeField] MeshRenderer mesh_2;

    public void Init()
    {
        this.gameObject.SetActive(true);
    }

}
