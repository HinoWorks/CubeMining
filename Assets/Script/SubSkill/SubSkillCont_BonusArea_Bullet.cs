using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SubSkillCont_BonusArea_Bullet : MonoBehaviour
{
    [SerializeField] TriggerSender triggerSender;
    [SerializeField] GameObject sizeRateObject;
    [SerializeField] TextMeshPro text_bonusRate;
    private readonly HashSet<MiningTarget_Cube> targetsInArea = new HashSet<MiningTarget_Cube>();
    private float bonusRate;
    private float sizeRate;




    void Awake()
    {
        triggerSender.OnEnter += OnEnter;
        triggerSender.OnExit += OnExit;
    }

    void OnDestroy()
    {
        triggerSender.OnEnter -= OnEnter;
        triggerSender.OnExit -= OnExit;
        ClearAllBonuses();
    }

    public void Init(float _bonusRate, float _sizeRate)
    {
        bonusRate = _bonusRate;
        sizeRate = _sizeRate;

        text_bonusRate.SetText($"+{bonusRate * 100f}%");
        sizeRateObject.transform.localScale = Vector3.one * sizeRate;
    }



    private void OnEnter(Collider other)
    {
        if (!other.TryGetComponent(out MiningTarget_Cube cube)) return;
        if (!cube.isAlive) return;
        if (!targetsInArea.Add(cube)) return;
        cube.AddValueBonusRate(bonusRate);
    }

    private void OnExit(Collider other)
    {
        if (!other.TryGetComponent(out MiningTarget_Cube cube)) return;
        RemoveBonus(cube);
    }

    private void RemoveBonus(MiningTarget_Cube cube)
    {
        if (!targetsInArea.Remove(cube)) return;
        cube.RemoveValueBonusRate();
    }

    private void ClearAllBonuses()
    {
        foreach (var cube in targetsInArea)
        {
            if (cube == null) continue;
            cube.RemoveValueBonusRate();
        }
        targetsInArea.Clear();
    }
}
