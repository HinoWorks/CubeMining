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
    [SerializeField] GameObject pf_damageText;
    //[SerializeField] GameObject pf_moveIcon_Coin;
    [SerializeField] GameObject pf_getResourceCont;
    [SerializeField] GameObject pf_getTime;
    [SerializeField] GameObject pf_speechBubble;



    [Space(10)]
    [Header(" -- target point set --")]
    [SerializeField] Transform target_coin;
    [SerializeField] Transform target_gem;
    [SerializeField] Transform target_time;


    private List<UI_CircleTimer> pool_gauge_circle = new List<UI_CircleTimer>();
    private List<UI_TextCoinGet> pool_textCoinGet = new List<UI_TextCoinGet>();
    private List<UI_TextDamage> pool_damageText = new List<UI_TextDamage>();
    private List<UI_GetResourceCont> pool_getResourceCont = new List<UI_GetResourceCont>();
    private List<UI_SpeechBubble> pool_speechBubble = new List<UI_SpeechBubble>(10);
    private List<UI_TextCont> pool_getTime = new List<UI_TextCont>();

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

        /*
        for (int i = 0; i < 50; i++)
        {
            var newUnit = Instantiate(pf_damageText, parent_base) as GameObject;
            var selectUnit = newUnit.GetComponent<UI_TextDamage>();
            pool_damageText.Add(selectUnit);
            selectUnit.gameObject.SetActive(false);
        }
        */

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

    public UI_TextDamage Get_TextDamage()
    {
        UI_TextDamage selectUnit = null;
        selectUnit = pool_damageText.Find(d => d.gameObject.activeSelf == false);
        if (selectUnit == null)
        {
            var newUnit = Instantiate(pf_damageText, parent_base) as GameObject;
            selectUnit = newUnit.GetComponent<UI_TextDamage>();
            pool_damageText.Add(selectUnit);
        }
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
    #endregion --


}
