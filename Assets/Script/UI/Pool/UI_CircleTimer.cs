using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_CircleTimer : UI_Gauge
{
    [SerializeField] Image image_gauge;

    public void Set_Color(Color _setcolor)
    {
        image_gauge.color = _setcolor;
    }
    public void Set_FillGauge(float _rate)
    {
        if (_rate == 0)
        {
            image_gauge.fillAmount = 0f;
            return;
        }
        image_gauge.fillAmount = _rate;
        //DOTween.To(() => image_gauge.fillAmount, x => image_gauge.fillAmount = x, _rate, 0.15f).Play();
    }

}
