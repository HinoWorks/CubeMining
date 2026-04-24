using UnityEngine;
using TMPro;

public class DEBUG_UI_ResourceUnit : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_counter;

    public void SetResourceCount(int _resourceCount)
    {
        if (_resourceCount <= 0)
        {
            this.gameObject.SetActive(false);
            return;
        }
        this.gameObject.SetActive(true);
        tmp_counter.text = _resourceCount.ToString();
    }
}
