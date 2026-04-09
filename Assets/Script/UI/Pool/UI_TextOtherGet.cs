using UnityEngine;
using TMPro;

public class UI_TextOtherGet : UI_Gauge
{
    [SerializeField] TextMeshProUGUI tmp_getCoin;

    private Vector3 initialPosition;
    private float posiRange = 20f;

    void Awake()
    {
        initialPosition = tmp_getCoin.transform.localPosition;
    }

    public void SetText(string _setText)
    {
        tmp_getCoin.SetText($"{_setText}");
    }

    public void SetText(string _setText, Color _setColor)
    {
        tmp_getCoin.transform.localPosition = new Vector3(Random.Range(-posiRange, posiRange), Random.Range(-posiRange, posiRange), 0f);
        tmp_getCoin.SetText($"{_setText}");
        tmp_getCoin.color = _setColor;
    }

    public override void Return()
    {
        target = null;
        this.gameObject.SetActive(false);
        tmp_getCoin.transform.localPosition = initialPosition;
        tmp_getCoin.alpha = 1f;
    }
}
