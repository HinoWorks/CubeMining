using UnityEngine;
using System;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;


public class UI_PickaxePowerUnit : MonoBehaviour
{
    [SerializeField] Image image_icon;
    [SerializeField] UI_StarLevel[] ui_starLevelUnits;
    [SerializeField] GameObject obj_locked;
    [SerializeField] GameObject obj_equip;
    [SerializeField] GameObject obj_enhanceReady;
    [SerializeField] GameObject obj_enhanceComplete;
    [SerializeField] GameObject obj_selectFlame;
    [SerializeField] ParticleSystem eff_enhance;

    public PickaxePowerBase so_base { get; private set; }
    public PickaxePowerLevel so_level { get; private set; }
    public int currentLevel { get; private set; } = 0;
    public bool isEnhanceReady { get; private set; } = false;
    public bool isEnoughPlayerLevel { get; private set; } = false;

    private SimpleAnimation anim;
    private Action<UI_PickaxePowerUnit> onClick_Select;



    public async void Init_Once(int _index, Action<UI_PickaxePowerUnit> _onClick_Select)
    {
        anim = GetComponent<SimpleAnimation>();
        so_base = SOLoader.PickaxePowerData.GetPickaxePowerBase(_index);
        if (so_base == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        this.onClick_Select = _onClick_Select;
        image_icon.sprite = so_base.icon;

        var pickaxePowerData = await SaveLoader.Inst.Get_PickaxePowerData(so_base.index);
        currentLevel = pickaxePowerData == null ? 0 : pickaxePowerData.level;
        so_level = SOLoader.PickaxePowerData.GetPickaxePowerLevel(so_base.index, currentLevel);

    }


    /// <summary>
    /// 初期化(アウトゲーム移行時に毎回呼ばれる)
    /// </summary>
    public async void Init(int _currentPoints, int _currentPlayerLevel)
    {
        if (so_base == null) return;

        isEnoughPlayerLevel = _currentPlayerLevel >= so_base.unlockLevel;
        obj_locked.SetActive(!isEnoughPlayerLevel);
        if (!isEnoughPlayerLevel)
        {
            isEnhanceReady = false;
            obj_enhanceReady.SetActive(isEnhanceReady);
            return;
        }

        var pickaxePowerData = await SaveLoader.Inst.Get_PickaxePowerData(so_base.index);
        currentLevel = pickaxePowerData == null ? 0 : pickaxePowerData.level;
        so_level = SOLoader.PickaxePowerData.GetPickaxePowerLevel(so_base.index, currentLevel);


        // level max?
        if (currentLevel >= so_base.maxLevel)
        {
            isEnhanceReady = false;
            obj_enhanceReady.SetActive(isEnhanceReady);
        }
        else
        {
            // リソース見て、強化可能か確認
            ResourceCheck(_currentPoints);
        }
    }

    public void StartIdleAnim()
    {
        anim.Rewind();
        anim.Play("Default");
        anim["Default"].normalizedTime = UnityEngine.Random.Range(0f, 1f);
    }

    public bool ResourceCheck(int _currentPoints)
    {
        if (!isEnoughPlayerLevel || _currentPoints < so_level.req_point)
        {
            isEnhanceReady = false;
            obj_enhanceReady.SetActive(isEnhanceReady);
            return isEnhanceReady;
        }

        var requredResources = new ResourceCount[7];
        requredResources[0] = new ResourceCount() { resourceType = ResourceType.Stone, requiredCount = so_level.req_stone };
        requredResources[1] = new ResourceCount() { resourceType = ResourceType.Iron, requiredCount = so_level.req_iron };
        requredResources[2] = new ResourceCount() { resourceType = ResourceType.Gold, requiredCount = so_level.req_gold };
        requredResources[3] = new ResourceCount() { resourceType = ResourceType.Emerald, requiredCount = so_level.req_emerald };
        requredResources[4] = new ResourceCount() { resourceType = ResourceType.Ruby, requiredCount = so_level.req_ruby };
        requredResources[5] = new ResourceCount() { resourceType = ResourceType.Sapphire, requiredCount = so_level.req_sapphire };
        requredResources[6] = new ResourceCount() { resourceType = ResourceType.Diamond, requiredCount = so_level.req_diamond };
        isEnhanceReady = StaticManager.IsResourceEnough(requredResources);
        obj_enhanceReady.SetActive(isEnhanceReady);
        return isEnhanceReady;
    }



    public void EquipMark_Update(int _equipedIndex)
    {
        if (so_base == null) return;

        var isEquiped = _equipedIndex == so_base.index;
        obj_equip.SetActive(isEquiped);
        SelectMark_Update(isEquiped);
    }
    public void SelectMark_Update(bool _isSelect)
    {
        obj_selectFlame.SetActive(_isSelect);
    }


    public void Callback_Enhanced(int _currentLevel, int _currentPoints)
    {
        currentLevel = _currentLevel;
        /*
        var count = 1;
        foreach (var star in ui_starLevelUnits)
        {
            star.Set_StarLevel(count >= currentLevel);
        }*/

        // リソース見て、強化可能か確認
        so_level = SOLoader.PickaxePowerData.GetPickaxePowerLevel(so_base.index, currentLevel);
        ResourceCheck(_currentPoints);
    }



    #region -- OnClick --
    public void OnClick_Select()
    {
        onClick_Select?.Invoke(this);
    }
    #endregion
}
