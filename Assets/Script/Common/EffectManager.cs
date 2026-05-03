using UnityEngine;
using System.Collections.Generic;



public enum EffectType
{
    None,
    BlockDamage = 1,
    BlockBreak = 2,

    ThunderStrike = 100,
    ThunderStrike_Cross = 101,

    ThunderStrike_Red = 110,
    ThunderStrike_Red_Circle = 111,
}

public class EffectManager : MonoBehaviour
{
    public static EffectManager Inst;

    [SerializeField] GameObject pf_eff_blockDamage;
    [SerializeField] GameObject pf_eff_blockBreak;
    [SerializeField] GameObject pf_eff_thunderStrike;
    [SerializeField] GameObject pf_eff_thunderStrike_cross;
    [SerializeField] GameObject pf_eff_thunderStrike_red;
    [SerializeField] GameObject pf_eff_thunderStrike_red_circle;
    private List<GameObject> pool_eff_blockDamage = new List<GameObject>();
    private List<GameObject> pool_eff_blockBreak = new List<GameObject>();
    private List<GameObject> pool_eff_thunderStrike = new List<GameObject>();
    private List<GameObject> pool_eff_thunderStrike_cross = new List<GameObject>();
    private List<GameObject> pool_eff_thunderStrike_red = new List<GameObject>();
    private List<GameObject> pool_eff_thunderStrike_red_circle = new List<GameObject>();
    private int createCountInit = 20;
    private int createCountInit_thunderStrike = 10;

    void Awake()
    {
        if (Inst == null) Inst = this;
        else { Destroy(this); }
    }

    void Start()
    {
        for (int i = 0; i < createCountInit; i++)
        {
            var newUnit = Instantiate(pf_eff_blockBreak, InGameManager.Inst.ParentPool) as GameObject;
            newUnit.SetActive(false);
            pool_eff_blockBreak.Add(newUnit);
        }
        for (int i = 0; i < createCountInit; i++)
        {
            var newUnit = Instantiate(pf_eff_blockDamage, InGameManager.Inst.ParentPool) as GameObject;
            newUnit.SetActive(false);
            pool_eff_blockDamage.Add(newUnit);
        }
        for (int i = 0; i < createCountInit_thunderStrike; i++)
        {
            var newUnit = Instantiate(pf_eff_thunderStrike, InGameManager.Inst.ParentPool) as GameObject;
            newUnit.SetActive(false);
            pool_eff_thunderStrike.Add(newUnit);
        }
        for (int i = 0; i < createCountInit_thunderStrike; i++)
        {
            var newUnit = Instantiate(pf_eff_thunderStrike_red, InGameManager.Inst.ParentPool) as GameObject;
            newUnit.SetActive(false);
            pool_eff_thunderStrike_red.Add(newUnit);
        }
    }


    public GameObject Get_Effect(EffectType _effectType)
    {
        var (pf, pool) = Get_EffectData(_effectType);
        var selectUnit = pool.Find(d => d.gameObject.activeSelf == false);
        if (selectUnit == null)
        {
            selectUnit = Instantiate(pf, InGameManager.Inst.ParentPool) as GameObject;
            pool.Add(selectUnit);
        }
        return selectUnit;
    }

    private (GameObject, List<GameObject>) Get_EffectData(EffectType _effectType)
    {
        switch (_effectType)
        {
            case EffectType.BlockDamage:
                return (pf_eff_blockDamage, pool_eff_blockDamage);
            case EffectType.BlockBreak:
                return (pf_eff_blockBreak, pool_eff_blockBreak);
            case EffectType.ThunderStrike:
                return (pf_eff_thunderStrike, pool_eff_thunderStrike);
            case EffectType.ThunderStrike_Cross:
                return (pf_eff_thunderStrike_cross, pool_eff_thunderStrike_cross);
            case EffectType.ThunderStrike_Red:
                return (pf_eff_thunderStrike_red, pool_eff_thunderStrike_red);
            case EffectType.ThunderStrike_Red_Circle:
                return (pf_eff_thunderStrike_red_circle, pool_eff_thunderStrike_red_circle);
            default:
                return (null, new List<GameObject>());
        }
    }




}
