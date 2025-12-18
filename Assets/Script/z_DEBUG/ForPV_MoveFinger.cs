using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ForPV_MoveFinger : MonoBehaviour
{
    [SerializeField] private RectTransform canvasTrans;
    [SerializeField] private RectTransform trans_finger;
    [SerializeField] CanvasGroup CG_circle;
    [SerializeField] private GameObject obj_finger;
    private Vector3 InitialPosition;
    private Vector2 ToPosition;
    private Vector2 screenPoint;



    [Header("MoveSetting")]
    public bool flag_x_Lock = false;
    public bool flag_y_Lock = false;

    void Start()
    {
        InitialPosition = trans_finger.localPosition;
    }


    void Update()
    {
        MoveObject();
    }



    private void MoveObject()
    {
        screenPoint = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTrans, screenPoint, GetComponent<Camera>(), out ToPosition);

        if (flag_x_Lock) ToPosition.x = InitialPosition.x;
        if (flag_y_Lock) ToPosition.y = InitialPosition.y;

        trans_finger.localPosition = ToPosition;

        if (Input.GetMouseButtonDown(0))
        {
            obj_finger.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.2f).Play();
            DOTween.To(() => CG_circle.alpha, x => CG_circle.alpha = x, 1f, 0.25f).Play();

        }
        else if (Input.GetMouseButtonUp(0))
        {
            obj_finger.transform.DOLocalRotate(new Vector3(0f, 0f, -10f), 0.2f).Play();
            DOTween.To(() => CG_circle.alpha, x => CG_circle.alpha = x, 0f, 0.25f).Play();
        }
    }
}
