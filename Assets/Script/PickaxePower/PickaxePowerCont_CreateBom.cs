using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;

public class PickaxePowerCont_CreateBom : PickaxePowerCont_Base
{
    [SerializeField] GameObject pf_Bom;
    private Vector3 offsetPositionBase = new Vector3(0, 3f, 0); // 発射位置オフセット
    private float offsetPositionY_delta = 0.5f;

    private int delayGenerate = 150;
    private int hp_Bom = 15; // ダミー、MiningTarget_Bomb_PickaxePowerで攻撃回数で破壊するよう設定中
    private float damageRate => EquippedLevelData.value_1;
    private float sizeRate => EquippedLevelData.value_2;
    private int bomCount => (int)EquippedLevelData.value_3;


    private List<MiningTarget_Bomb_PickaxePower> list_bomBlocks = new List<MiningTarget_Bomb_PickaxePower>();


    public override async void Activate()
    {
        Debug.Log("Power == CreateBom");
        for (int i = 0; i < bomCount; i++)
        {
            CreateBom(i);
            await UniTask.Delay(delayGenerate);
        }
    }

    private void CreateBom(int _index)
    {
        var createTargetPosition = BlockGenerateManager.Inst.Get_RandomTargetArea();
        var createPosition = createTargetPosition // xz位置のみ
                                - new Vector3(0, BlockGenerateManager.Inst.currentLayerIndex, 0)
                                + offsetPositionBase
                                + new Vector3(0, _index * offsetPositionY_delta, 0);
        var newBomBlock = Get_FreeBomBlock();
        newBomBlock.transform.position = createPosition;

        var damage = (int)(AttackManager.Inst.currentPickaxeDamage * damageRate);
        newBomBlock.Init_SkillBom(hp_Bom, damage, sizeRate);
    }


    private MiningTarget_Bomb_PickaxePower Get_FreeBomBlock()
    {
        var freeBomBlock = list_bomBlocks.Find(x => !x.gameObject.activeSelf);
        if (freeBomBlock == null)
        {
            var newBomBlock = Instantiate(pf_Bom, InGameManager.Inst.ParentPool) as GameObject;
            freeBomBlock = newBomBlock.GetComponent<MiningTarget_Bomb_PickaxePower>();
            list_bomBlocks.Add(freeBomBlock);
        }
        return freeBomBlock;
    }
}
