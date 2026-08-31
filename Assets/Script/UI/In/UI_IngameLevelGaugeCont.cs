using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Numerics;
using UniRx;
using DG.Tweening;



public class UI_IngameLevelGaugeCont : MonoBehaviour
{

    [SerializeField] Image gauge_exp;
    [SerializeField] TextMeshProUGUI tmp_level;
    [SerializeField] TextMeshProUGUI tmp_exp;
    [SerializeField] SimpleAnimation anim;
    [SerializeField] ParticleSystem eff_levelUp;






    private void Awake()
    {
        GameEvent.IngameStageLevel.IsActive.Subscribe(Set_IsActive).AddTo(this);
        GameEvent.IngameStageLevel.Changed.Subscribe(x => Set_IngameLevelChanged(x.level, x.breakCountInLevel, x.breaksToNext)).AddTo(this);
        GameEvent.IngameStageLevel.LevelUp.Subscribe(_ => Set_IngameBlockCountUp()).AddTo(this);
    }

    private void Set_IsActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    private void Set_IngameLevelChanged(int level, int blockCount, int blockCountToNext)
    {
        tmp_level.SetText($"<size=80%>Lv.{level}</size>");

        // 最大レベル到達時は必要数が 0 になるため、ゲージを満タン表示で固定する
        if (blockCountToNext <= 0)
        {
            tmp_exp.SetText($"<size=80%>MAX</size>");
            DOTween.To(() => gauge_exp.fillAmount, x => gauge_exp.fillAmount = x, 1f, 0.1f);
            return;
        }

        tmp_exp.SetText($"{blockCount} <size=80%>/ {blockCountToNext}");
        DOTween.To(() => gauge_exp.fillAmount, x => gauge_exp.fillAmount = x, (float)blockCount / (float)blockCountToNext, 0.1f);
    }

    private void Set_IngameBlockCountUp()
    {
        anim.Rewind();
        anim.Play("LevelUp");
        eff_levelUp.Play();
    }





    [ContextMenu("DEBUG_LevelUpAnim")]
    public void DEBUG_LevelUpAnim()
    {
        anim.Rewind();
        anim.Play("LevelUp");
        eff_levelUp.Play();
    }


}
