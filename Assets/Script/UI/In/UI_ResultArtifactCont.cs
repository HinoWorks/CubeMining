using UnityEngine;
using UnityEngine.UI;

public class UI_ResultArtifactCont : MonoBehaviour
{
    [SerializeField] Image icon;


    public void SetData(ArtifactUnitData _artifactData)
    {
        icon.sprite = _artifactData.icon;
        this.gameObject.SetActive(true);
    }
}
