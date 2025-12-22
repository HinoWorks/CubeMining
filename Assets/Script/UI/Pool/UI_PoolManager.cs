using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UI_PoolManager : MonoBehaviour
{

    public static UI_PoolManager Inst;
    [SerializeField] Transform parent_base;
    [SerializeField] Transform parent_over;

    [Space(10)]
    [Header(" -- pf set --")]
    [SerializeField] GameObject pf_circleGauge;
    [SerializeField] GameObject pf_getCoinText;
    [SerializeField] GameObject pf_moveIcon_Coin;
    [SerializeField] GameObject pf_moveIcon_Gem;
    [SerializeField] GameObject pf_ballCounter;
    [SerializeField] GameObject pf_speechBubble;



    [Space(10)]
    [Header(" -- target point set --")]
    [SerializeField] Transform target_coin;
    [SerializeField] Transform target_gem;


    private List<UI_CircleTimer> pool_gauge_circle = new List<UI_CircleTimer>();
    private List<UI_TextCoinGet> pool_textCoinGet = new List<UI_TextCoinGet>();
    private List<UI_GetResourceMove> pool_moveItemCont_coin = new List<UI_GetResourceMove>(15);
    private List<UI_SpeechBubble> pool_speechBubble = new List<UI_SpeechBubble>(10);


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
            var newUnit = Instantiate(pf_getCoinText, parent_base) as GameObject;
            var selectUnit = newUnit.GetComponent<UI_TextCoinGet>();
            pool_textCoinGet.Add(selectUnit);
            selectUnit.gameObject.SetActive(false);
        }

    }

    private void Set_UIScaleChange(float _zoomRate)
    {
        parent_base.localScale = _zoomRate * Vector3.one;
    }



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

    #region -- Get Item icon --
    public void Set_GetItemIcon_Coin(int _count, Vector3 _buttonPosition)
    {
        for (int i = 0; i < _count; i++)
        {
            UI_GetResourceMove selectUnit = null;
            selectUnit = pool_moveItemCont_coin.Find(d => d.isActive == false);
            if (selectUnit == null)
            {
                var newUnit = Instantiate(pf_moveIcon_Coin, parent_over) as GameObject;
                selectUnit = newUnit.GetComponent<UI_GetResourceMove>();
                newUnit.gameObject.SetActive(false);
                pool_moveItemCont_coin.Add(selectUnit);
            }
            selectUnit.UnitActivate_SetPosi(target_coin.position, _buttonPosition);
        }
    }

    #endregion --


}
