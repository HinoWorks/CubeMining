using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class HButton : Button
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        Anim_ScaleChange_ToSmall();
        base.OnPointerDown(eventData);
    }

    // Button is released
    public override void OnPointerUp(PointerEventData eventData)
    {
        Anim_ScaleChange_ToNormal();
        base.OnPointerUp(eventData);
    }




    private void Anim_ScaleChange_ToSmall()
    {
        this.gameObject.transform.DOScale(0.75f * Vector3.one, 0.2f).Play();
    }
    private void Anim_ScaleChange_ToNormal()
    {
        this.gameObject.transform.DOScale(1f * Vector3.one, 0.2f).Play();
    }
}



