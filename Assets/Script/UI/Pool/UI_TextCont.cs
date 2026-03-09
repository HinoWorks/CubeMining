using UnityEngine;
using TMPro;

public class UI_TextCont : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_text;

    public void SetText(string _text)
    {
        tmp_text.SetText(_text);
        this.gameObject.SetActive(true);
    }

    public void SetNotActive_FromAnim()
    {
        this.gameObject.SetActive(false);
    }
}
