using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ArtifactDetailUnit : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_skillName;
    [SerializeField] TextMeshProUGUI tmp_description;
    [SerializeField] TextMeshProUGUI tmp_paramNow;
    [SerializeField] TextMeshProUGUI tmp_paramNext;
    private ArtifactUnitData so;

    public void SetData(ArtifactUnitData _so = null)
    {
        if (_so == null)
        {
            this.gameObject.SetActive(false);
            so = null;
            return;
        }
        so = _so;
        SetData_Base();
    }

    private void SetData_Base()
    {
        tmp_skillName.SetText(so.artifactName);
        tmp_description.SetText(so.artifactDescription);

        var paramNow = so.value;
        tmp_paramNow.SetText(paramNow.ToString("F2"));
        tmp_paramNext.SetText("xxxx");

        this.gameObject.SetActive(true);
    }


}
