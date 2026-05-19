using UnityEngine;
using System;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;


public class UI_PickaxePowerUnit : MonoBehaviour
{
    [SerializeField] Image image_icon;
    [SerializeField] UI_StarLevel[] ui_starLevelUnits;
    [SerializeField] GameObject obj_equip;
    [SerializeField] GameObject obj_enhanceReady;
    [SerializeField] GameObject obj_enhanceComplete;
    [SerializeField] GameObject obj_selectFlame;
    [SerializeField] ParticleSystem eff_enhance;

    public PickaxePowerBase so_base { get; private set; }
    public PickaxePowerLevel so_level { get; private set; }
    public int currentLevel { get; private set; } = 0;
    public bool isEnhanceReady { get; private set; } = false;
    private Action<UI_PickaxePowerUnit> onClick_Select;



    public async void Init_Once(int _index, Action<UI_PickaxePowerUnit> _onClick_Select)
    {
        this.onClick_Select = _onClick_Select;
        so_base = SOLoader.PickaxePowerData.GetPickaxePowerBase(_index);
        image_icon.sprite = so_base.icon;

        // 初回のみレベルを保持しておく
        var pickaxePowerData = await SaveLoader.Inst.Get_PickaxePowerData(so_base.index);
        if (pickaxePowerData != null)
        {
            currentLevel = pickaxePowerData.level;
        }
        else
        {
            currentLevel = 0;
        }

        var count = 1;
        foreach (var star in ui_starLevelUnits)
        {
            star.Set_StarLevel(count >= currentLevel);
            count++;
        }
    }


    /// <summary>
    /// 初期化(アウトゲーム移行時に毎回呼ばれる)
    /// </summary>
    public void Init(int _currentPoints)
    {
        // level max?
        if (currentLevel >= so_base.maxLevel)
        {
            isEnhanceReady = false;
            obj_enhanceReady.SetActive(isEnhanceReady);
            return;
        }

        // リソース見て、強化可能か確認
        so_level = SOLoader.PickaxePowerData.GetPickaxePowerLevel(so_base.index, currentLevel);
        if (_currentPoints < so_level.req_point)
        {
            isEnhanceReady = false;
            obj_enhanceReady.SetActive(isEnhanceReady);
            return;
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
    }



    public void EquipMark_Update(int _equipedIndex)
    {
        var isEquiped = _equipedIndex == so_base.index;
        obj_equip.SetActive(isEquiped);
        SelectMark_Update(isEquiped);
    }
    public void SelectMark_Update(bool _isSelect)
    {
        obj_selectFlame.SetActive(_isSelect);
    }



    #region -- OnClick --
    public void OnClick_Select()
    {
        onClick_Select?.Invoke(this);
    }
    #endregion
}
