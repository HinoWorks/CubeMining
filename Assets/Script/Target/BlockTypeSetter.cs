using UnityEngine;

public class BlockTypeSetter : MonoBehaviour
{
    [SerializeField] ResourceType resourceType;
    public void Set_BlockTypeObject(ResourceType _resourceType)
    {
        this.gameObject.SetActive(_resourceType == resourceType);
    }
}
