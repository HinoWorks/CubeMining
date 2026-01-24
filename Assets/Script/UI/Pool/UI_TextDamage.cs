using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UI_TextDamage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_getCoin;
    private float posiRange = 20f;
    private Sequence seq;

    public virtual void SetPosition(Vector3 _position)
    {
        transform.position = Camera.main.WorldToScreenPoint(_position);// * CameraManager.Inst.zoomRate;
    }

    public void SetText(string _setText, Color _setColor)
    {
        tmp_getCoin.transform.localPosition = new Vector3(Random.Range(-posiRange, posiRange), Random.Range(-posiRange, posiRange), 0f);
        tmp_getCoin.SetText($"{_setText}");
        tmp_getCoin.color = _setColor;
        StartTextAnim();
    }
    public void SetText(string _setText)
    {
        tmp_getCoin.transform.localPosition = new Vector3(Random.Range(-posiRange, posiRange), Random.Range(-posiRange, posiRange), 0f);
        tmp_getCoin.SetText($"{_setText}");
        StartTextAnim();
    }

    private void StartTextAnim()
    {
        if (seq == null)
        {
            seq = DOTween.Sequence();
            seq.Append(transform.DOScale(1.5f, 0.1f).SetEase(Ease.OutBack));
            seq.Append(DOTween.To(() => tmp_getCoin.alpha, x => tmp_getCoin.alpha = x, 0f, 0.15f).SetDelay(0.3f)
            .OnComplete(() =>
            {
                Return();
            }));
            seq.SetAutoKill(false).SetLink(this.gameObject).Pause();
        }
        this.gameObject.SetActive(true);
        transform.localScale = Vector3.one;
        seq.Restart();
    }

    public void Return()
    {
        this.gameObject.SetActive(false);
        tmp_getCoin.transform.localPosition = Vector3.zero;
        tmp_getCoin.alpha = 1f;
    }



}
