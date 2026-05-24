using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


public enum UI_ResourceUnitSize
{
    Max2, Max, Mid, Min,
}

public class UI_PoolManager : MonoBehaviour
{

    public static UI_PoolManager Inst;
    [SerializeField] Transform parent_base;
    [SerializeField] Transform parent_over;

    [Space(10)]
    [Header(" -- pf set --")]
    [SerializeField] GameObject pf_getCoinText;
    [SerializeField] GameObject pf_otherText;
    [SerializeField] GameObject pf_getResourceCont;
    [SerializeField] GameObject pf_getArtifactCont;
    [SerializeField] GameObject pf_getTime;
    [SerializeField] GameObject pf_criticalText;
    [SerializeField] GameObject pf_luckText;


    [Space(10)]
    [Header(" -- target point set --")]
    [SerializeField] Transform target_coin;
    [SerializeField] Transform target_gem;
    [SerializeField] Transform target_time;


    private List<UI_CircleTimer> pool_gauge_circle = new List<UI_CircleTimer>();
    private List<UI_TextCoinGet> pool_textCoinGet = new List<UI_TextCoinGet>();
    private List<UI_TextOtherGet> pool_otherText = new List<UI_TextOtherGet>();
    private List<UI_GetResourceCont> pool_getResourceCont = new List<UI_GetResourceCont>();
    private List<UI_GetArtifactCont> pool_getArtifactCont = new List<UI_GetArtifactCont>();
    private List<UI_TextCont> pool_getTime = new List<UI_TextCont>();
    private List<UI_TextOtherGet> pool_luckText = new List<UI_TextOtherGet>();
    private List<UI_TextOtherGet> pool_criticalText = new List<UI_TextOtherGet>();
    //  -- screen out limit setting --
    public float screenOut_min_w { get; private set; }
    public float screenOut_max_w { get; private set; }
    public float screenOut_min_h { get; private set; }
    public float screenOut_max_h { get; private set; }



    void Awake()
    {
        if (Inst == null) Inst = this;
        else { Destroy(this); }
    }
    void Start()
    {
        screenOut_min_w = -100;
        screenOut_max_w = Screen.width + 100;
        screenOut_min_h = -100;
        screenOut_max_h = Screen.height + 100;


        // -- initialCreate --
        for (int i = 0; i < 50; i++)
        {
            var newUnit = Instantiate(pf_getResourceCont, parent_base) as GameObject;
            var selectUnit = newUnit.GetComponent<UI_GetResourceCont>();
            pool_getResourceCont.Add(selectUnit);
            selectUnit.gameObject.SetActive(false);
        }

        for (int i = 0; i < 50; i++)
        {
            var newUnit = Instantiate(pf_getCoinText, parent_base) as GameObject;
            var selectUnit = newUnit.GetComponent<UI_TextCoinGet>();
            pool_textCoinGet.Add(selectUnit);
            selectUnit.gameObject.SetActive(false);
        }

        for (int i = 0; i < 10; i++)
        {
            var newUnit = Instantiate(pf_otherText, parent_base) as GameObject;
            var selectUnit = newUnit.GetComponent<UI_TextOtherGet>();
            pool_otherText.Add(selectUnit);
            selectUnit.gameObject.SetActive(false);
        }

        for (int i = 0; i < 10; i++)
        {
            var newUnit = Instantiate(pf_criticalText, parent_base) as GameObject;
            var selectUnit = newUnit.GetComponent<UI_TextOtherGet>();
            pool_criticalText.Add(selectUnit);
            selectUnit.gameObject.SetActive(false);
        }
        for (int i = 0; i < 10; i++)
        {
            var newUnit = Instantiate(pf_luckText, parent_base) as GameObject;
            var selectUnit = newUnit.GetComponent<UI_TextOtherGet>();
            pool_luckText.Add(selectUnit);
            selectUnit.gameObject.SetActive(false);
        }

        GameEvent.GameState.SetGameState.Subscribe(Init_GameStateChange).AddTo(this);
    }

    private void Init_GameStateChange(GameStateType _state)
    {
        switch (_state)
        {
            case GameStateType.InGame_Ready:
                foreach (var unit in pool_getArtifactCont)
                {
                    unit.gameObject.SetActive(false);
                }
                break;
            case GameStateType.InGame:
                break;
            case GameStateType.InGame_End:
                break;
        }
    }


    private void Set_UIScaleChange(float _zoomRate)
    {
        parent_base.localScale = _zoomRate * Vector3.one;
    }



    /*
        public UI_CircleTimer Set_Gauge(Transform _target, Vector3 _offset)
        {
            UI_CircleTimer selectUnit = null;
            selectUnit = pool_gauge_circle.Find(d => d.gameObject.activeSelf == false);
            if (selectUnit == null)
            {
                var newUnit = Instantiate(pf_circleGauge, parent_base) as GameObject;
                selectUnit = newUnit.GetComponent<UI_CircleTimer>();
                pool_gauge_circle.Add(selectUnit);
            }
            selectUnit.Initialize(_target, _offset);
            return selectUnit;
        }
        */


    public UI_TextCoinGet Set_TextCoinGet(Transform _target, Vector3 _offset)
    {
        UI_TextCoinGet selectUnit = null;
        selectUnit = pool_textCoinGet.Find(d => d.gameObject.activeSelf == false);
        if (selectUnit == null)
        {
            var newUnit = Instantiate(pf_getCoinText, parent_base) as GameObject;
            selectUnit = newUnit.GetComponent<UI_TextCoinGet>();
            pool_textCoinGet.Add(selectUnit);
        }
        selectUnit.Initialize(_target, _offset);
        return selectUnit;
    }

    public UI_TextOtherGet Get_OtherText(Transform _target, Vector3 _offset)
    {
        UI_TextOtherGet selectUnit = null;
        selectUnit = pool_otherText.Find(d => d.gameObject.activeSelf == false);
        if (selectUnit == null)
        {
            var newUnit = Instantiate(pf_otherText, parent_base) as GameObject;
            selectUnit = newUnit.GetComponent<UI_TextOtherGet>();
            pool_otherText.Add(selectUnit);
        }
        selectUnit.Initialize(_target, _offset);
        return selectUnit;
    }

    /*
        public UI_SpeechBubble Set_SpeechBubbleGet(Transform _target, Vector3 _offset)
        {
            UI_SpeechBubble selectUnit = null;
            selectUnit = pool_speechBubble.Find(d => d.gameObject.activeSelf == false);
            if (selectUnit == null)
            {
                var newUnit = Instantiate(pf_speechBubble, parent_base) as GameObject;
                selectUnit = newUnit.GetComponent<UI_SpeechBubble>();
                pool_speechBubble.Add(selectUnit);
            }
            selectUnit.Initialize(_target, _offset);
            return selectUnit;
        }
    */


    #region -- Get Resource Cont --
    public UI_GetResourceCont Set_GetResourceCont()
    {
        UI_GetResourceCont selectUnit = null;
        selectUnit = pool_getResourceCont.Find(d => d.gameObject.activeSelf == false);
        if (selectUnit == null)
        {
            var newUnit = Instantiate(pf_getResourceCont, parent_over) as GameObject;
            selectUnit = newUnit.GetComponent<UI_GetResourceCont>();
            pool_getResourceCont.Add(selectUnit);
        }
        return selectUnit;
    }

    public UI_GetArtifactCont Set_GetArtifactCont()
    {
        UI_GetArtifactCont selectUnit = null;
        selectUnit = pool_getArtifactCont.Find(d => d.gameObject.activeSelf == false);
        if (selectUnit == null)
        {
            var newUnit = Instantiate(pf_getArtifactCont, parent_over) as GameObject;
            selectUnit = newUnit.GetComponent<UI_GetArtifactCont>();
            pool_getArtifactCont.Add(selectUnit);
        }
        return selectUnit;
    }

    /// <summary>
    /// インゲーム時間増加通知用, 右上のHUD付近に表示される
    /// </summary>
    public UI_TextCont Set_TimeText()
    {
        UI_TextCont selectUnit = null;
        selectUnit = pool_getTime.Find(d => d.gameObject.activeSelf == false);
        if (selectUnit == null)
        {
            var newUnit = Instantiate(pf_getTime, parent_base) as GameObject;
            selectUnit = newUnit.GetComponent<UI_TextCont>();
            pool_getTime.Add(selectUnit);
        }
        selectUnit.transform.position = target_time.position;
        return selectUnit;
    }

    public UI_TextOtherGet Set_LuckText()
    {
        UI_TextOtherGet selectUnit = null;
        selectUnit = pool_luckText.Find(d => d.gameObject.activeSelf == false);
        if (selectUnit == null)
        {
            var newUnit = Instantiate(pf_luckText, parent_base) as GameObject;
            selectUnit = newUnit.GetComponent<UI_TextOtherGet>();
            pool_luckText.Add(selectUnit);
        }
        return selectUnit;
    }

    public UI_TextOtherGet Set_CriticalText()
    {
        UI_TextOtherGet selectUnit = null;
        selectUnit = pool_criticalText.Find(d => d.gameObject.activeSelf == false);
        if (selectUnit == null)
        {
            var newUnit = Instantiate(pf_criticalText, parent_base) as GameObject;
            selectUnit = newUnit.GetComponent<UI_TextOtherGet>();
            pool_criticalText.Add(selectUnit);
        }
        return selectUnit;
    }
    #endregion --


}
