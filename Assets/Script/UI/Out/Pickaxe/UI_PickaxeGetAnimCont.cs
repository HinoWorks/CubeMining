using UnityEngine;
using UnityEngine.UI;

public class UI_PickaxeGetAnimCont : MonoBehaviour
{
    [SerializeField] Image icon;

    public void SetIcon(Sprite _sprite)
    {
        icon.sprite = _sprite;
        this.gameObject.SetActive(true);
    }

    public void AnimEnd()
    {
        this.gameObject.SetActive(false);
    }
}
