using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class UI_GetArtifactCont : UI_GetIconBase
{


    public void SetInit(int _artifactIndex, Vector3 _basePosition)
    {
        var artifactData = SOLoader.ArtifactData.Get_ArtifactData(_artifactIndex);
        var targetPosition = UIManager_InGame.Inst.Get_ArtifactTargetPosition(
                InGameManager.Inst.Get_ArtifactCount() - 1).position;

        Init(artifactData.icon, _basePosition, targetPosition);
    }


}
