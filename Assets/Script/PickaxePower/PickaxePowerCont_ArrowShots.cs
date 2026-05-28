using System.Collections.Generic;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using System;
using UniRx;



public class PickaxePowerCont_ArrowShots : PickaxePowerCont_Base
{
    [SerializeField] GameObject obj_bow;
    private SimpleAnimation anim_bow;

    [SerializeField] GameObject pf_Arrow;

    private float damageRate => EquippedLevelData.value_1;
    private float sizeRate => EquippedLevelData.value_2;
    private int arrowCount => (int)EquippedLevelData.value_3;

    private List<PickaxePowerCont_ArrowUnit> list_arrowUnits = new List<PickaxePowerCont_ArrowUnit>();


    private int damage => (int)(AttackManager.Inst.currentPickaxeDamage * damageRate);
    private float arrowSpeed = 30f;

    // awwrowPosiiton
    private float offsetPosition_Y => UnityEngine.Random.Range(3.5f, 4f);
    private float offsetPosition_XZ => UnityEngine.Random.Range(1f, 2.5f);
    //private float arrowPosition_overArea => currentLayerSize + offsetPosition_XZ;
    private float arrowPosition_underArea => UnityEngine.Random.Range(1, currentLayerSize);
    private float arrowPosition_Y => -BlockGenerateManager.Inst.currentLayerIndex + offsetPosition_Y;


    private Vector3 arrowPosition_base => new Vector3(currentLayerSize / 2f, arrowPosition_Y, -currentLayerSize / 2f);

    /*
        private Vector3 arrowPosition_base_ => UnityEngine.Random.Range(0, 2) % 2 == 0 ?
            new Vector3(arrowPosition_overArea, arrowPosition_Y, -arrowPosition_underArea)
            : new Vector3(arrowPosition_underArea, arrowPosition_Y, -arrowPosition_overArea);
    */
    private int currentLayerSize => BlockGenerateManager.Inst.currentLayerSize;



    private void Awake()
    {
        anim_bow = obj_bow.GetComponent<SimpleAnimation>();
        obj_bow.SetActive(false);
    }

    public override void Activate()
    {
        Debug.Log("Power == ArrowShots");

        var basePosition = arrowPosition_base;
        obj_bow.transform.position = basePosition;
        obj_bow.SetActive(false);
        obj_bow.SetActive(true);
        for (int i = 0; i < arrowCount; i++)
        {
            ArrowReady(basePosition);
        }
        CameraManager.Inst.ShakeCamera_Large();
        StaticManager.SlowGameTime_PickaxePower();


        Observable.Timer(TimeSpan.FromSeconds(0.25f)).Subscribe(_ =>
        {
            anim_bow.Play("Shot");
            foreach (var arrowUnit in list_arrowUnits)
            {
                arrowUnit.ShotArrow();
            }

            Observable.Timer(TimeSpan.FromSeconds(0.25f)).Subscribe(_ =>
            {
                anim_bow.Play("Return");
            }).AddTo(this);
        }).AddTo(this);
    }

    void OnDestroy()
    {
        list_arrowUnits.Clear();
    }



    private void ArrowReady(Vector3 _basePosition)
    {
        var targetBlock = BlockGenerateManager.Inst.Get_RandomTargetBlock();
        var arrowPposition = _basePosition;
        var arrowDirection = (targetBlock.transform.position - arrowPposition).normalized;


        var newArrowUnit = Get_FreeArrowUnit();
        newArrowUnit.transform.position = arrowPposition;
        newArrowUnit.transform.rotation = Quaternion.LookRotation(arrowDirection);
        newArrowUnit.Init(damage, sizeRate, arrowSpeed * arrowDirection);
    }



    private PickaxePowerCont_ArrowUnit Get_FreeArrowUnit()
    {
        var freeArrowUnit = list_arrowUnits.Find(x => !x.gameObject.activeSelf);
        if (freeArrowUnit == null)
        {
            freeArrowUnit = CreateArrowUnit();
        }
        return freeArrowUnit;
    }

    private PickaxePowerCont_ArrowUnit CreateArrowUnit()
    {
        var newArrow = Instantiate(pf_Arrow, transform) as GameObject;
        var newArrowUnit = newArrow.GetComponent<PickaxePowerCont_ArrowUnit>();
        list_arrowUnits.Add(newArrowUnit);
        return newArrowUnit;
    }
}
