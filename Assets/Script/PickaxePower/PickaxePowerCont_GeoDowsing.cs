using UnityEngine;

public class PickaxePowerCont_GeoDowsing : PickaxePowerCont_Base
{
    private int convertCount => (int)EquippedLevelData.value_1;

    public override void Activate()
    {
        Debug.Log("Power == GeoDowsing");

        var converted = BlockGenerateManager.Inst.ConvertDirtToOre(convertCount);
        Debug.Log($"GeoDowsing converted: {converted} / request: {convertCount}");

        CameraManager.Inst.ShakeCamera_Large();
        StaticManager.SlowGameTime_PickaxePower();
    }
}
