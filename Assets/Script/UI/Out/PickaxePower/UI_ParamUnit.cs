using UnityEngine;
using TMPro;

public class UI_ParamUnit : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_paramName;
    [SerializeField] TextMeshProUGUI tmp_paramNow;
    [SerializeField] GameObject obj_vec;
    [SerializeField] TextMeshProUGUI tmp_paramNext;




    public void SetData(string _paramName, string _paramNow, string _paramNext)
    {
        this.gameObject.SetActive(true);

        tmp_paramName.SetText(_paramName);
        tmp_paramNow.SetText(_paramNow);
        tmp_paramNext.SetText(_paramNext);
        obj_vec.SetActive(true);
        tmp_paramNext.gameObject.SetActive(true);
    }

    public void SetData_OnlyNow(string _paramName, string _paramNow)
    {
        this.gameObject.SetActive(true);

        tmp_paramName.SetText(_paramName);
        tmp_paramNow.SetText(_paramNow);
        obj_vec.SetActive(false);
        tmp_paramNext.gameObject.SetActive(false);
    }


}
