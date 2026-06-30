using UnityEngine;

public class PickaxePowerCont_CollectArea : PickaxePowerCont_Base
{
    [SerializeField] GameObject pf_CollectArea;
    private PickaxePowerCont_CollectAreaUnit ActiveFlowerCont;

    private Vector3 offsetPosition => new Vector3(0f, 0.1f, 0f); // 発射位置オフセット
    private float sizeRate => EquippedLevelData.value_1;
    private float aliveTime => EquippedLevelData.value_2;
    private float collectPower => EquippedLevelData.value_3;




    public override void Activate()
    {
        Debug.Log("Power == CollectFlower");
        CreateCollectFlower();
    }

    private void CreateCollectFlower()
    {
        if (ActiveFlowerCont != null) // アクティブ中の花は削除
        {
            ActiveFlowerCont.DestroyCall();
            ActiveFlowerCont = null;
        }

        var targetPosition = AttackManager.Inst.currentPickaxePosition;
        var flowerPosition = targetPosition + offsetPosition;

        var newCollectArea = Instantiate(pf_CollectArea, transform) as GameObject;
        ActiveFlowerCont = newCollectArea.GetComponent<PickaxePowerCont_CollectAreaUnit>();
        ActiveFlowerCont.transform.position = flowerPosition;
        ActiveFlowerCont.Init(sizeRate, aliveTime, collectPower);
        Debug.Log($"CreateCollectFlower ==  sizeRate: {sizeRate} / aliveTime: {aliveTime} / collectPower: {collectPower}");
    }

    public override void GameEndCall()
    {
        if (ActiveFlowerCont != null)
        {
            ActiveFlowerCont.DestroyCall();
            ActiveFlowerCont = null;
        }
    }



}
