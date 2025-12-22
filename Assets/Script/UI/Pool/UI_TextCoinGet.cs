using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class UI_TextCoinGet : UI_Gauge
{
    [SerializeField] TextMeshProUGUI tmp_getCoin;
    private float posiRange = 20f;
    private Sequence seq;


    public void SetText(string _setText, Color _setColor)
    {
        tmp_getCoin.transform.localPosition = new Vector3(Random.Range(-posiRange, posiRange), Random.Range(-posiRange, posiRange), 0f);
        tmp_getCoin.SetText($"<size=75%>$</size>{_setText}");
        tmp_getCoin.color = _setColor;
        StartTextAnim();
    }
    public void SetText(string _setText)
    {
        tmp_getCoin.transform.localPosition = new Vector3(Random.Range(-posiRange, posiRange), Random.Range(-posiRange, posiRange), 0f);
        tmp_getCoin.SetText($"<size=75%>$</size>{_setText}");
        StartTextAnim();
    }

    private void StartTextAnim()
    {
        if (seq == null)
        {
            seq = DOTween.Sequence();
            seq.Append(tmp_getCoin.transform.DOLocalMoveY(50f, 0.75f).SetEase(Ease.OutSine));
            seq.Join(DOTween.To(() => tmp_getCoin.alpha, x => tmp_getCoin.alpha = x, 0f, 0.15f).SetDelay(0.6f)
            .OnComplete(() =>
            {
                Return();
            }));
            seq.SetAutoKill(false).SetLink(this.gameObject).Pause();
        }
        this.gameObject.SetActive(true);
        seq.Restart();
    }

    public override void Return()
    {
        target = null;
        this.gameObject.SetActive(false);
        tmp_getCoin.transform.localPosition = Vector3.zero;
        tmp_getCoin.alpha = 1f;
    }

}
