using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class UI_GetArtifactCont : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] float duration_wait;
    [SerializeField] float duration_endPoint;
    [SerializeField] Ease setEase;
    [SerializeField] SimpleAnimation anim;

    private Vector3 targetPosition;
    private Sequence moveTarget;



    public void SetInit(int _artifactIndex, Vector3 _basePosition)
    {
        var artifactData = SOLoader.ArtifactData.Get_ArtifactData(_artifactIndex);
        icon.sprite = artifactData.icon;

        var setPosition = Camera.main.WorldToScreenPoint(_basePosition);
        setPosition.z = 0f;
        transform.position = setPosition;
        targetPosition = UIManager_InGame.Inst.Get_ArtifactTargetPosition(
                InGameManager.Inst.Get_ArtifactCount() - 1).position;

        this.gameObject.SetActive(true);
        anim.Rewind();
        anim.Play("Default");

        moveTarget = DOTween.Sequence()
           .Append(DOVirtual.DelayedCall(duration_wait, () => { }))
            .Append(transform.DOMove(targetPosition, duration_endPoint).SetEase(setEase))
            .OnComplete(() => { }).Play();
    }


}
