using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;



public class HButton : Button
{


    const float duration_scaleChange = 0.2f;
    const float scale_push = 0.85f; // ボタンが押されたときのスケール
    const float scale_over = 1.1f; // ボタンにカーソルが乗ったときのスケール

    //private HButtonConnect hButtonConnect;
    public UnityAction<bool> onMouseOver;
    public UnityAction<bool> onSelect;
    public UnityEvent rightClick;
    //public UnityAction onRightClick;




    protected override void Awake()
    {
        base.Awake();
        //hButtonConnect = this.GetComponent<HButtonConnect>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (base.interactable == false) return;
        Anim_ScaleChange_PointerOn();
        if (onMouseOver != null)
        {
            onMouseOver(true);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (base.interactable == false) return;
        Anim_ScaleChange_ToNormal();
        if (onMouseOver != null)
        {
            onMouseOver(false);
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
            if (rightClick != null)
            {
                rightClick.Invoke();
            }
        }
    }


    public void Set_SelectActive(bool _active)
    {
        if (onSelect != null)
        {
            onSelect(_active);
        }
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



