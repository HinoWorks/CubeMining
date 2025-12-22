using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UI_GetResourceMove : MonoBehaviour
{
    public bool isActive { get; private set; } = true;
    [Header("MoveSetting")]
    [SerializeField] Ease setEase;
    [SerializeField] float duration;
    [SerializeField] bool isMoveEndNotActive = false;
    [SerializeField] float positionRange;
    [SerializeField] float moveRate;
    [SerializeField] float waitActiveRange;

    Sequence moveTarget;

    public void UnitActivate(Transform _moveTarget, Transform _basePosition)
    {
        isActive = true;

        var randomAngle = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
        var randomPosi = positionRange * Random.Range(0.25f, 1f) * new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f);
        transform.position = _basePosition.position + randomPosi;

        this.gameObject.SetActive(true);
        moveTarget = DOTween.Sequence()
            .Append(transform.DOMove(_basePosition.position + randomPosi * moveRate, 0.25f).SetEase(Ease.OutSine))
            .Append(DOVirtual.DelayedCall(Random.Range(0f, waitActiveRange), () => { }))
            .Append(transform.DOMove(_moveTarget.position, duration).SetEase(setEase))
            .OnComplete(() =>
                    {
                        if (isMoveEndNotActive)
                        {
                            isActive = false;
                            this.gameObject.SetActive(false);
                        }
                    }).Play();
    }

    public void UnitActivate_SetPosi(Vector3 _moveTarget, Vector3 _basePosition)
    {
        isActive = true;

        var randomAngle = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
        var randomPosi = positionRange * Random.Range(0.25f, 1f) * new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f);
        transform.position = _basePosition;

        this.gameObject.SetActive(true);
        moveTarget = DOTween.Sequence()
            .Append(transform.DOMove(_basePosition + randomPosi * moveRate, 0.25f).SetEase(Ease.OutSine))
            .Append(DOVirtual.DelayedCall(Random.Range(0f, waitActiveRange), () => { }))
            .Append(transform.DOMove(_moveTarget, duration).SetEase(setEase))
            .OnComplete(() =>
                    {
                        if (isMoveEndNotActive)
                        {
                            isActive = false;
                            this.gameObject.SetActive(false);
                        }
                    }).Play();
    }
}
