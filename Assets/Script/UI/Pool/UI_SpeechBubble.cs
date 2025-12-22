using UnityEngine;
using TMPro;
using DG.Tweening;

public class UI_SpeechBubble : UI_Gauge
{
    [SerializeField] Transform parent_base;
    [SerializeField] TextMeshProUGUI tmp_text;
    private Sequence seq;


    public void SetText(string _setText)
    {
        parent_base.localPosition = Vector3.zero;
        tmp_text.SetText($"{_setText}");
        this.gameObject.SetActive(true);
        SetAnim();
    }

    private void SetAnim()
    {
        if (seq == null)
        {
            seq = DOTween.Sequence()
                .Append(parent_base.DOLocalMoveY(25f, 0.5f).SetEase(Ease.OutSine))
                .Append(parent_base.DOLocalMoveY(0f, 0.5f).SetEase(Ease.OutSine))
                .SetAutoKill(false).SetLink(this.gameObject).SetLoops(-1).Pause();
        }
        seq.Restart();
    }


    public override void Return()
    {
        if (seq.IsPlaying()) seq.Pause();
        base.Return();
    }



}
