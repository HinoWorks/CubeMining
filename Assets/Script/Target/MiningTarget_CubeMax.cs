using UnityEngine;



/// <summary>
/// ==================================================
/// 現在使用していない
/// ==================================================
/// </summary>



public class MiningTarget_CubeMax : MiningTarget_Cube
{
    [SerializeField] MeshRenderer[] meshRenderers_Max;
    [SerializeField] Material[] setMaterials_Max;

    private Material selectedMaterial;

    public override void Init(int _hp, int _value, int _index, int _layerIndex)
    {
        base.Init(_hp, _value, _index, _layerIndex);
    }



    public override void Set_BlockType(BaseBlockType _baseBlockType, ResourceType _resourceType)
    {
        base.Set_BlockType(_baseBlockType, _resourceType);

        selectedMaterial = Get_Material(_resourceType);
        foreach (var meshRenderer in meshRenderers_Max)
        {
            if (meshRenderer == null) continue;
            var mats = meshRenderer.sharedMaterials;
            if (mats.Length == 0) continue;
            mats[0] = selectedMaterial;
            meshRenderer.sharedMaterials = mats;
        }
    }


    private Material Get_Material(ResourceType _resourceType)
    {
        switch (_resourceType)
        {
            case ResourceType.Iron:
                return setMaterials_Max[0];
            case ResourceType.Gold:
                return setMaterials_Max[1];
            case ResourceType.Emerald:
                return setMaterials_Max[2];
            case ResourceType.Ruby:
                return setMaterials_Max[3];
            case ResourceType.Sapphire:
                return setMaterials_Max[4];
            case ResourceType.Diamond:
                return setMaterials_Max[5];
            default:
                return setMaterials_Max[0];
        }
    }
}
