using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ResourceCount : MonoBehaviour
{
    [SerializeField] GameObject obj_main;
    [SerializeField] GameObject obj_lock;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI tmp_resourceCount;


    public void SetData(Sprite _icon, string _resourceCount, Color _color = default)
    {
        icon.sprite = _icon;
        tmp_resourceCount.text = _resourceCount;
        tmp_resourceCount.color = _color == default ? Color.white : _color;
        obj_main.SetActive(true);
        obj_lock.SetActive(false);

        this.gameObject.SetActive(true);
    }
    public void SetLock()
    {
        obj_main.SetActive(false);
        obj_lock.SetActive(true);

        this.gameObject.SetActive(true);
    }

    public void NotActive()
    {
        this.gameObject.SetActive(false);
    }

}
