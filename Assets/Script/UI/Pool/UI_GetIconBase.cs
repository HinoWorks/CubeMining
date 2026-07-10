using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_GetIconBase : MonoBehaviour
{

    [SerializeField] protected Image icon;
    [SerializeField] protected float duration_wait;
    [SerializeField] protected float duration_endPoint;
    [SerializeField] protected Ease setEase;
    [SerializeField] protected SimpleAnimation anim;

    private Vector3 targetPosition;
    private Sequence moveTarget;




    protected virtual void Init(Sprite _setIcon, Vector3 _basePosition, Vector3 _targetPosition)
    {
        icon.sprite = _setIcon;
        var setPosition = Camera.main.WorldToScreenPoint(_basePosition);
        setPosition.z = 0f;
        transform.position = setPosition;
        targetPosition = _targetPosition;

        this.gameObject.SetActive(true);
        anim.Rewind();
        anim.Play("Default");

        moveTarget = DOTween.Sequence()
           .Append(DOVirtual.DelayedCall(duration_wait, () => { }))
            .Append(transform.DOMove(targetPosition, duration_endPoint).SetEase(setEase))
            .OnComplete(() => { }).Play();
    }


}
