using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using DG.Tweening;


public class UI_PickaxePower : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text_counter;
    [SerializeField] Image icon_power;

    [SerializeField] GameObject gaugeParent;
    [SerializeField] Image gauge;
    [SerializeField] Image gauge_forCT;
    [SerializeField] GameObject obj_powerReady;
    [SerializeField] ParticleSystem eff_powerActive;




    void Start()
    {
        PickaxePowerManager.Inst.PowerGaugeRateChanged.Subscribe(x => Set_PowerGaugeRate(x)).AddTo(this);
        PickaxePowerManager.Inst.PowerActivate.Subscribe(x => Set_PowerActivate(x.Item1, x.Item2)).AddTo(this);
        GameEvent.GameState.SetGameState.Subscribe(Set_GameState).AddTo(this);
    }


    private void Set_GameState(GameStateType state)
    {
        switch (state)
        {
            case GameStateType.InGame_Ready:
                Init();
                break;
        }
    }

    private void Init()
    {
        var equippedIndex = SaveLoader.Inst.PickaxePowerEquipedIndex;
        if (equippedIndex <= 0)
        {
            this.gameObject.SetActive(false);
            return;
        }

        var equippedBase = SOLoader.PickaxePowerData.GetPickaxePowerBase(equippedIndex);
        icon_power.sprite = equippedBase.icon;

        text_counter.SetText("0");
        gaugeParent.SetActive(false);
        obj_powerReady.SetActive(false);
        eff_powerActive.Stop();
        gauge.fillAmount = 0f;
        gauge_forCT.fillAmount = 0f;

        this.gameObject.SetActive(true);
    }



    // Event:ゲージ変更
    private void Set_PowerGaugeRate(float rate)
    {
        text_counter.SetText(PickaxePowerManager.Inst.CurrentGauge.ToString());
        gauge.fillAmount = rate;
        obj_powerReady.SetActive(rate >= 1f);
    }

    // Event:スキル発動
    private void Set_PowerActivate(int index, int CT)
    {
        text_counter.SetText(PickaxePowerManager.Inst.CurrentGauge.ToString());
        eff_powerActive.Play();
        obj_powerReady.SetActive(false);
        gauge.fillAmount = 0f;
        gaugeParent.SetActive(true);
        gauge_forCT.fillAmount = 1f;
        gauge_forCT.DOKill();
        DOTween.To(() => gauge_forCT.fillAmount, x => gauge_forCT.fillAmount = x, 0f, CT)
            .SetEase(Ease.Linear)
            .OnComplete(() => gaugeParent.SetActive(false))
            .Play();
    }

}
