using UnityEngine;

public class AroundLayerUnit : MonoBehaviour
{
    [SerializeField] MeshRenderer mesh_1;
    [SerializeField] MeshRenderer mesh_2;

    public void Init(Material _setMaterial)
    {
        this.gameObject.SetActive(true);
        mesh_1.material = _setMaterial;
        mesh_2.material = _setMaterial;
    }

}
