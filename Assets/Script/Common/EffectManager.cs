using UnityEngine;
using System.Collections.Generic;



public enum EffectType
{
    None,
    BlockBreak,

    ThunderStrike = 100,
}

public class EffectManager : MonoBehaviour
{
    public static EffectManager Inst;

    [SerializeField] GameObject pf_eff_blockBreak;
    [SerializeField] GameObject pf_eff_thunderStrike;
    private List<GameObject> pool_eff_blockBreak = new List<GameObject>();
    private List<GameObject> pool_eff_thunderStrike = new List<GameObject>();


    private int createCountInit = 20;


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
            var newUnit = Instantiate(pf_eff_thunderStrike, InGameManager.Inst.ParentPool) as GameObject;
            newUnit.SetActive(false);
            pool_eff_thunderStrike.Add(newUnit);
        }
    }


    public GameObject Get_Effect(EffectType _effectType)
    {
        var (pf, pool) = Get_EffectData(_effectType);
        var selectUnit = pool.Find(d => d.gameObject.activeSelf == false);
        if (selectUnit == null)
        {
            var newUnit = Instantiate(pf, InGameManager.Inst.ParentPool) as GameObject;
            pool.Add(newUnit);
        }
        return selectUnit;
    }

    private (GameObject, List<GameObject>) Get_EffectData(EffectType _effectType)
    {
        switch (_effectType)
        {
            case EffectType.BlockBreak:
                return (pf_eff_blockBreak, pool_eff_blockBreak);
            case EffectType.ThunderStrike:
                return (pf_eff_thunderStrike, pool_eff_thunderStrike);
            default:
                return (null, new List<GameObject>());
        }
    }




}
