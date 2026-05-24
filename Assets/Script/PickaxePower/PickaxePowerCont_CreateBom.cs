using UnityEngine;
using System.Collections.Generic;

public class PickaxePowerCont_CreateBom : PickaxePowerCont_Base
{
    [SerializeField] GameObject pf_Bom;
    private Vector3 offsetPosition = new Vector3(0, 5.5f, 0); // 発射位置オフセット

    private int hp_Bom => 15;
    private List<MiningTarget_Bomb> list_bomBlocks = new List<MiningTarget_Bomb>();


    public override void Activate()
    {
        Debug.Log("Power == CreateBom");
        for (int i = 0; i < EquippedLevelData.value_1; i++)
        {
            CreateBom();
        }
    }

    private void CreateBom()
    {
        var createTargetPosition = BlockGenerateManager.Inst.Get_RandomTargetArea();
        var createPosition = createTargetPosition + offsetPosition;
        var newBomBlock = Get_FreeBomBlock();
        newBomBlock.transform.position = createPosition;
        newBomBlock.Init_SkillBom(hp_Bom);
    }


    private MiningTarget_Bomb Get_FreeBomBlock()
    {
        var freeBomBlock = list_bomBlocks.Find(x => !x.gameObject.activeSelf);
        if (freeBomBlock == null)
        {
            var newBomBlock = Instantiate(pf_Bom, InGameManager.Inst.ParentPool) as GameObject;
            freeBomBlock = newBomBlock.GetComponent<MiningTarget_Bomb>();
            list_bomBlocks.Add(freeBomBlock);
        }
        return freeBomBlock;
    }
}
