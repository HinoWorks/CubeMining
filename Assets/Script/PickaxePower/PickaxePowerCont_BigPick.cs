using UnityEngine;

public class PickaxePowerCont_BigPick : PickaxePowerCont_Base
{
    [SerializeField] GameObject pf_bigPick;
    private Vector3 offsetPosition = new Vector3(2.75f, 0.35f, 0); // 発射位置オフセット
    private Vector3 targetPosition;

    private float damageRate => EquippedLevelData.value_1;
    private float sizeRate => EquippedLevelData.value_2;



    public override void Activate()
    {
        Debug.Log("Power == Create BigPickaxe");
        CreateBigPickaxe();
    }

    private void CreateBigPickaxe()
    {
        targetPosition = AttackManager.Inst.currentPickaxePosition;
        var pickaxePosition = targetPosition + offsetPosition;

        var newBigPick = Instantiate(pf_bigPick, transform) as GameObject;
        var bigPickCont = newBigPick.GetComponent<PickaxePowerCont_BigPickUnit>();
        newBigPick.transform.position = pickaxePosition;

        var damage = (int)(AttackManager.Inst.currentPickaxeDamage * damageRate);
        bigPickCont.Init(damage, sizeRate, targetPosition);
    }




}
