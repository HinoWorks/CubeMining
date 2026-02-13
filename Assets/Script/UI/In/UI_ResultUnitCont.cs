using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Numerics;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class UI_ResultUnitCont : MonoBehaviour
{
    [SerializeField] GameObject obj_resourceCount;
    [SerializeField] TextMeshProUGUI tmp_resourceCount;
    [SerializeField] Image icon;

    [Space(5)]
    [SerializeField] GameObject obj_resourceTotal;
    [SerializeField] TextMeshProUGUI tmp_resourceTotal;
    [SerializeField] Image icon_total;


    public async UniTask SetData(ResourceType _resourceType, BigInteger _getCount, BigInteger _currentTotal)
    {
        // icon
        var getIcon = SOLoader.ItemData.GetItemUnitData((int)_resourceType).icon;
        icon.sprite = getIcon;
        icon_total.sprite = getIcon;

        // color
        var textColor = SOLoader.UISetting.GetTextColor(_resourceType);
        tmp_resourceCount.color = textColor;
        tmp_resourceTotal.color = textColor;

        Set_ResourceCount(tmp_resourceCount, _getCount);
        Set_ResourceCount(tmp_resourceTotal, _currentTotal);
        this.gameObject.SetActive(true);

        await UniTask.Delay(200);
        var modCoin_total = StaticManager.Get_BigintegerToUnit(_currentTotal + _getCount);
        tmp_resourceTotal.transform.DOScale(1.05f, 0.05f).SetEase(Ease.OutSine);

        var currentResourceFloat = StaticManager.Get_BigintegerToUnit(_currentTotal + _getCount).num;
        DOTween.To(() => currentResourceFloat, x => currentResourceFloat = x, modCoin_total.num, 0.35f)
            .OnUpdate(() =>
            {
                if (modCoin_total.unit == "")
                {
                    tmp_resourceTotal.text = $"{currentResourceFloat.ToString("F0")} {modCoin_total.unit}";
                }
                else
                {
                    tmp_resourceTotal.text = $"{currentResourceFloat.ToString("F2")} {modCoin_total.unit}";
                }
            }).OnComplete(() =>
        {
            tmp_resourceCount.transform.DOScale(1f, 0.05f).SetEase(Ease.OutSine);
        });

        await UniTask.Delay(100);
    }


    private void Set_ResourceCount(TextMeshProUGUI _tmp_resourceCount, BigInteger _getCount)
    {
        var modCoin_get = StaticManager.Get_BigintegerToUnit(_getCount);
        if (modCoin_get.unit == "")
        {
            _tmp_resourceCount.text = $"{modCoin_get.num.ToString("F0")} {modCoin_get.unit}";
        }
        else
        {
            _tmp_resourceCount.text = $"{modCoin_get.num.ToString("F2")} {modCoin_get.unit}";
        }
    }
}
