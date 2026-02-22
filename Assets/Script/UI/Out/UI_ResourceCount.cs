using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ResourceCount : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI tmp_resourceCount;


    public void SetData(Sprite _icon, string _resourceCount, Color _color = default)
    {
        icon.sprite = _icon;
        tmp_resourceCount.text = _resourceCount;
        tmp_resourceCount.color = _color == default ? Color.white : _color;
        this.gameObject.SetActive(true);
    }

    public void NotActive()
    {
        this.gameObject.SetActive(false);
    }

}
