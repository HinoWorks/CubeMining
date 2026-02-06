using UnityEngine;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class AroundLayerCont : MonoBehaviour
{
    [SerializeField] int layerIndex;
    [SerializeField] Transform parent_units;
    [SerializeField] GameObject pf_unit;
    [SerializeField] Vector3 edgePosition = new Vector3(-2, 0, 2);

    [Space(5)]
    [Header(" -- side unit setting --")]
    [SerializeField] Transform sideUnit_left;
    [SerializeField] Transform sideUnit_right;
    private Vector3 sideUnit_basePosition_left = new Vector3(-4, 0, 1);
    private Vector3 sideUnit_basePosition_right = new Vector3(-1, 0, 4);
    private List<AroundLayerUnit> list_units = new List<AroundLayerUnit>();



    public void Init(int _layerIndex, int _layerSize)
    {
        layerIndex = _layerIndex;
        var cntUnit = CreateUnits();
        cntUnit.transform.localPosition = edgePosition;

        for (int i = 0; i < _layerSize; i++)
        {
            var unitCont = CreateUnits();
            unitCont.transform.localPosition = edgePosition + -1 * new Vector3(0, 0, i + 1);
        }
        for (int i = 0; i < _layerSize; i++)
        {
            var unitCont = CreateUnits();
            unitCont.transform.localPosition = edgePosition + new Vector3(i + 1, 0, 0);
        }
        sideUnit_left.localPosition = sideUnit_basePosition_left + new Vector3(0, 0, -_layerSize);
        sideUnit_right.localPosition = sideUnit_basePosition_right + new Vector3(_layerSize, 0, 0);

        transform.localPosition = new Vector3(0, -layerIndex, 0);
        this.gameObject.SetActive(true);
    }


    private AroundLayerUnit CreateUnits()
    {
        var freeUnit = list_units.Find(x => x.gameObject.activeSelf == false);
        if (freeUnit == null)
        {
            GameObject newUnit;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // エディタ内でプレハブ接続を保持したまま生成
                newUnit = PrefabUtility.InstantiatePrefab(pf_unit, parent_units) as GameObject;
            }
            else
            {
                newUnit = Instantiate(pf_unit, parent_units);
            }
#else
            newUnit = Instantiate(pf_unit, parent_units);
#endif
            freeUnit = newUnit.GetComponent<AroundLayerUnit>();
            list_units.Add(freeUnit);
        }
        freeUnit.transform.localRotation = Quaternion.Euler(0, 45, 0);
        freeUnit.Init();
        return freeUnit;
    }

    public void NotActivate()
    {
        foreach (var unit in list_units)
        {
            unit.gameObject.SetActive(false);
        }
        this.gameObject.SetActive(false);
    }
}
