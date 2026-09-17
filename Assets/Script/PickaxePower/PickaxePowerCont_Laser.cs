using UnityEngine;
using System.Collections.Generic;


public class PickaxePowerCont_Laser : PickaxePowerCont_Base
{
    [SerializeField] GameObject pf_Laser;
    private float damageRate => EquippedLevelData.value_1;
    private float laserSize => EquippedLevelData.value_2;
    private float rotateAngle => EquippedLevelData.value_3;


    private int maxCount = 50;




    public override void Activate()
    {
        Debug.Log("Power == Laser");
        CreateLaser();
    }

    private void CreateLaser()
    {
        var laserPosition = AttackManager.Inst.currentPickaxePosition;

        var newLaser = Instantiate(pf_Laser, transform) as GameObject;
        var newLaserUnit = newLaser.GetComponent<PickaxePowerCont_LaserUnit>();
        newLaserUnit.transform.position = laserPosition;

        var damage = (int)(AttackManager.Inst.currentPickaxeDamage * damageRate);
        newLaserUnit.Init(damage, maxCount, laserSize, rotateAngle);
    }


    private void CreateLaser_Old()
    {
        var targetLayerIndex = Mathf.Abs(Mathf.FloorToInt(AttackManager.Inst.currentPickaxePosition.y));
        var so_layer = SOLoader.BlockLayerData.GetBlockLayerData(targetLayerIndex);
        var blockSize = so_layer.layerSize;
        var laserPosition = new Vector3(0f, AttackManager.Inst.currentPickaxePosition.y, 0f)
                            + new Vector3(blockSize + 1.5f, 0f, -blockSize / 2f); // 右側から発射

        var newLaser = Instantiate(pf_Laser, transform) as GameObject;
        var newLaserUnit = newLaser.GetComponent<PickaxePowerCont_LaserUnit>();
        newLaserUnit.transform.position = laserPosition;

        var damage = (int)(AttackManager.Inst.currentPickaxeDamage * damageRate);
        newLaserUnit.Init(damage, maxCount, laserSize, rotateAngle);
    }


}
