using System.Collections.Generic;
using UnityEngine;

public class PickaxePowerCont_ChainLightning : PickaxePowerCont_Base
{
    [SerializeField] GameObject pf_Lightning;

    private float damageRate => EquippedLevelData.value_1;
    private float sizeRate => EquippedLevelData.value_2;

    private readonly List<PickaxePowerCont_ChainLightningUnit> list_lightningUnits = new List<PickaxePowerCont_ChainLightningUnit>();

    private int damage => (int)(AttackManager.Inst.currentPickaxeDamage * damageRate);
    private const float BoltSpeed = 150f;

    private static readonly Vector2[] ViewportCorners =
    {
        new Vector2(0f, 0f),
        new Vector2(0f, 1f),
        new Vector2(1f, 0f),
        new Vector2(1f, 1f),
    };

    public override void Activate()
    {
        Debug.Log("Power == ChainLightning");

        var targetPosition = AttackManager.Inst.currentPickaxePosition;
        foreach (var corner in ViewportCorners)
        {
            var startPosition = ViewportCornerToWorld(corner, targetPosition.y);
            ShotLightning(startPosition, targetPosition);
        }

        CameraManager.Inst.ShakeCamera_Large();
        StaticManager.SlowGameTime_PickaxePower();
    }

    private void ShotLightning(Vector3 startPosition, Vector3 targetPosition)
    {
        var direction = targetPosition - startPosition;
        if (direction.sqrMagnitude < 0.001f) return;

        var unit = Get_FreeLightningUnit();
        unit.transform.position = startPosition;
        unit.transform.rotation = Quaternion.LookRotation(direction.normalized);
        unit.Init(damage, sizeRate, direction.normalized * BoltSpeed, targetPosition);
    }

    private static Vector3 ViewportCornerToWorld(Vector2 viewport, float targetY)
    {
        var cam = Camera.main;
        if (cam == null)
        {
            return new Vector3(viewport.x * 20f - 10f, targetY, viewport.y * 20f - 10f);
        }

        var ray = cam.ViewportPointToRay(new Vector3(viewport.x, viewport.y, 0f));
        var plane = new Plane(Vector3.up, new Vector3(0f, targetY, 0f));
        if (plane.Raycast(ray, out var enter))
        {
            return ray.GetPoint(enter);
        }

        var world = cam.ViewportToWorldPoint(new Vector3(viewport.x, viewport.y, cam.nearClipPlane + 10f));
        return new Vector3(world.x, targetY, world.z);
    }

    private PickaxePowerCont_ChainLightningUnit Get_FreeLightningUnit()
    {
        var freeUnit = list_lightningUnits.Find(x => !x.gameObject.activeSelf);
        if (freeUnit == null)
        {
            freeUnit = CreateLightningUnit();
        }
        return freeUnit;
    }

    private PickaxePowerCont_ChainLightningUnit CreateLightningUnit()
    {
        var newLightning = Instantiate(pf_Lightning, transform) as GameObject;
        var newUnit = newLightning.GetComponent<PickaxePowerCont_ChainLightningUnit>();
        list_lightningUnits.Add(newUnit);
        return newUnit;
    }

    void OnDestroy()
    {
        list_lightningUnits.Clear();
    }
}
