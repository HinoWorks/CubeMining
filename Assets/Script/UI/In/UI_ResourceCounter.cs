using UnityEngine;
using DG.Tweening;
using System.Numerics;
using TMPro;
using UniRx;
using UnityEngine.UI;


public class UI_ResourceCounter : MonoBehaviour
{
    [SerializeField] ResourceType resourceType;
    public ResourceType ResourceType => resourceType;

    [Space(10)]
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI tmp_resourceCount;
    private float currentResourceFloat;

    public Transform targetPosition => icon.transform;


    public void AwakeCall()
    {
        GameEvent.UI.ResourceMod.Subscribe(Set_ResourceMod).AddTo(this);
    }

    public void Set_Init()
    {
        currentResourceFloat = 0;
        tmp_resourceCount.text = "0";
        var initialActive = resourceType == ResourceType.Stone ? true : false;
        this.gameObject.SetActive(initialActive);
    }

    private void Set_ResourceMod((ResourceType, BigInteger) _resourceMod)
    {
        if (_resourceMod.Item1 != resourceType) return;
        if (!this.gameObject.activeSelf) this.gameObject.SetActive(true);

        var modCoin = StaticManager.Get_BigintegerToUnit(_resourceMod.Item2);
        tmp_resourceCount.transform.DOScale(1.05f, 0.05f).SetEase(Ease.OutSine);
        DOTween.To(() => currentResourceFloat, x => currentResourceFloat = x, modCoin.num, 0.2f)
            .OnUpdate(() =>
            {
                if (modCoin.unit == "")
                {
                    tmp_resourceCount.text = $"{currentResourceFloat.ToString("F0")} {modCoin.unit}";
                }
                else
                {
                    tmp_resourceCount.text = $"{currentResourceFloat.ToString("F2")} {modCoin.unit}";
                }
            }).OnComplete(() =>
        {
            currentResourceFloat = modCoin.num;
            tmp_resourceCount.transform.DOScale(1f, 0.05f).SetEase(Ease.OutSine);
        });
    }

}
