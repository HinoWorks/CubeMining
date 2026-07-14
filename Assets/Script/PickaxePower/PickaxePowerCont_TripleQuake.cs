using UnityEngine;

public class PickaxePowerCont_TripleQuake : PickaxePowerCont_Base
{
    [SerializeField] GameObject pf_Quake;

    private float damageRate => EquippedLevelData.value_1;
    private float sizeRate => EquippedLevelData.value_2;

    private PickaxePowerCont_TripleQuakeUnit activeUnit;

    public override void Activate()
    {
        Debug.Log("Power == TripleQuake");
        CreateQuake();
    }

    private void CreateQuake()
    {
        // 前のユニットが残っている場合は止める
        if (activeUnit != null)
        {
            activeUnit.CancelAndDestroy();
            activeUnit = null;
        }

        var targetPosition = AttackManager.Inst.currentPickaxePosition;
        var damage = (int)(AttackManager.Inst.currentPickaxeDamage * damageRate);

        var newQuake = Instantiate(pf_Quake, transform) as GameObject;
        activeUnit = newQuake.GetComponent<PickaxePowerCont_TripleQuakeUnit>();
        activeUnit.transform.position = targetPosition;
        activeUnit.Init(damage, sizeRate, targetPosition, OnUnitFinished);
    }

    private void OnUnitFinished()
    {
        activeUnit = null;
    }

    public override void GameEndCall()
    {
        if (activeUnit == null) return;
        activeUnit.CancelAndDestroy();
        activeUnit = null;
    }

    public override void OnDestroyCall()
    {
        GameEndCall();
        base.OnDestroyCall();
    }
}
