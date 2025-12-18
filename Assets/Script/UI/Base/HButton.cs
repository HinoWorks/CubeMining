using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class HButton : Button
{


    const float duration_scaleChange = 0.2f;
    const float scale_push = 0.85f; // ボタンが押されたときのスケール
    const float scale_over = 1.1f; // ボタンにカーソルが乗ったときのスケール

    private HButtonConnect hButtonConnect;


    protected override void Awake()
    {
        base.Awake();
        hButtonConnect = this.GetComponent<HButtonConnect>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (base.interactable == false) return;
        Anim_ScaleChange_PointerOn();
        if (hButtonConnect != null)
        {
            hButtonConnect.Set_MouseOverActive(true);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (base.interactable == false) return;
        Anim_ScaleChange_ToNormal();
        if (hButtonConnect != null)
        {
            hButtonConnect.Set_MouseOverActive(false);
        }
    }

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

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            base.OnPointerClick(eventData);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (hButtonConnect == null) return;
            hButtonConnect.RightClickAction();
        }
    }


    public void Set_SelectActive(bool _active)
    {
        if (hButtonConnect == null) return;
        hButtonConnect.Set_SelectActive(_active);
    }


    private void Anim_ScaleChange_PointerOn()
    {
        this.gameObject.transform.DOScale(scale_over * Vector3.one, duration_scaleChange).Play();
    }
    private void Anim_ScaleChange_ToSmall()
    {
        this.gameObject.transform.DOScale(scale_push * Vector3.one, duration_scaleChange).Play();
    }
    private void Anim_ScaleChange_ToNormal()
    {
        this.gameObject.transform.DOScale(1f * Vector3.one, duration_scaleChange).Play();
    }
}



