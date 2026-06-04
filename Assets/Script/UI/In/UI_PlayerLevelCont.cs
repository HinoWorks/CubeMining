using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Numerics;
using UniRx;
using DG.Tweening;

public class UI_PlayerLevelCont : MonoBehaviour
{
    [SerializeField] Image gauge_exp;
    [SerializeField] TextMeshProUGUI tmp_level;
    [SerializeField] TextMeshProUGUI tmp_exp;
    [SerializeField] SimpleAnimation anim;
    [SerializeField] ParticleSystem eff_levelUp;



    private void Awake()
    {
        GameEvent.PlayerLevel.PlayerLevelChanged.Subscribe(x => Set_PlayerLevelChanged(x.expInLevel, x.level, x.expToNext)).AddTo(this);
        GameEvent.PlayerLevel.LevelUp.Subscribe(x => Set_PlayerLevelUp(x.newLevel, x.pointsGained)).AddTo(this);
    }

    private void Set_PlayerLevelChanged(BigInteger expInLevel, int level, BigInteger expToNext)
    {
        tmp_level.SetText($"{level}");
        tmp_exp.SetText($"{expInLevel} <size=80%>/ {expToNext}");
        DOTween.To(() => gauge_exp.fillAmount, x => gauge_exp.fillAmount = x, (float)expInLevel / (float)expToNext, 0.1f);
    }


    private void Set_PlayerLevelUp(int newLevel, int pointsGained)
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
