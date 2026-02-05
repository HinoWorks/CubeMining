using UnityEngine;

public class BlockTypeSetter : MonoBehaviour
{
    [SerializeField] BlockType blockType;
    public void Set_BlockTypeObject(BlockType _blockType)
    {
        this.gameObject.SetActive(_blockType == blockType);
    }
}
