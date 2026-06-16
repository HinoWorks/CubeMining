using UnityEngine;
using System.Collections.Generic;



public enum EffectType
{
    None,
    BlockDamage = 1,
    BlockBreak = 2,

    ThunderStrike = 100,

    ThunderStrike_Chain = 101,

    ThunderStrike_Red = 110,
    ThunderStrike_Red_Chain = 111,


    ArrowHit = 200,
    LasaerHit = 201,
}



public class EffectManager : MonoBehaviour
{
    public static EffectManager Inst;


    [SerializeField] GameObject pf_eff_blockDamage;
    [SerializeField] GameObject pf_eff_blockBreak;
    [SerializeField] GameObject pf_eff_thunderStrike;
    [SerializeField] GameObject pf_eff_thunderStrike_chain;
    [SerializeField] GameObject pf_eff_thunderStrike_red;
    [SerializeField] GameObject pf_eff_thunderStrike_chain_red;
    [SerializeField] GameObject pf_eff_arrowHit;
    [SerializeField] GameObject pf_eff_lasaerHit;
    private List<GameObject> pool_eff_blockDamage = new List<GameObject>();
    private List<GameObject> pool_eff_blockBreak = new List<GameObject>();
    private List<GameObject> pool_eff_thunderStrike = new List<GameObject>();
    private List<EffectCont> pool_eff_thunderStrike_chain = new List<EffectCont>();
    private List<GameObject> pool_eff_thunderStrike_red = new List<GameObject>();
    private List<EffectCont> pool_eff_thunderStrike_chain_red = new List<EffectCont>();
    private List<GameObject> pool_eff_arrowHit = new List<GameObject>();
    private List<GameObject> pool_eff_lasaerHit = new List<GameObject>();
    private int createCountInit = 20;
    private int createCountInit_thunderStrike = 10;


    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
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
            case EffectType.ThunderStrike_Red:
                return (pf_eff_thunderStrike_red, pool_eff_thunderStrike_red);
            case EffectType.ArrowHit:
                return (pf_eff_arrowHit, pool_eff_arrowHit);
            case EffectType.LasaerHit:
                return (pf_eff_lasaerHit, pool_eff_lasaerHit);
            default:
                return (null, new List<GameObject>());
        }
    }


    public EffectCont Get_EffectCont(EffectType _effectType)
    {
        var (pf, pool) = Get_EffectDataCont(_effectType);
        var selectUnit = pool.Find(d => d.gameObject.activeSelf == false);
        if (selectUnit == null)
        {
            var newUnit = Instantiate(pf, InGameManager.Inst.ParentPool) as GameObject;
            selectUnit = newUnit.GetComponent<EffectCont>();
            pool.Add(selectUnit);
        }
        return selectUnit;
    }

    private (GameObject, List<EffectCont>) Get_EffectDataCont(EffectType _effectType)
    {
        switch (_effectType)
        {
            case EffectType.ThunderStrike_Chain:
                return (pf_eff_thunderStrike_chain, pool_eff_thunderStrike_chain);
            case EffectType.ThunderStrike_Red_Chain:
                return (pf_eff_thunderStrike_chain_red, pool_eff_thunderStrike_chain_red);
            default:
                return (null, new List<EffectCont>());
        }
    }



}
