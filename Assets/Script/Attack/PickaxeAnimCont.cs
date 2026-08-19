using UnityEngine;
using UniRx;
using DG.Tweening;

public class PickaxeAnimCont : MonoBehaviour
{
    [SerializeField] Transform modelRoot;
    [SerializeField] SimpleAnimation anim;
    [SerializeField] string animName_attack = "Attack";

    [Space(5)]
    [SerializeField] Vector3 offsetPosition = new Vector3(0.45f, 0.2f, 0f);
    [SerializeField] Vector3 modelLocalEuler = new Vector3(0f, 0f, -35f);
    [SerializeField] float modelLocalScale = 1f;

    //[Space(5)]
    //[SerializeField] Vector3 attackRotate = new Vector3(0f, 0f, -80f);
    //[SerializeField] float attackAnimDuration = 0.12f;

    private GameObject currentModel;
    private Transform followTarget;
    private Quaternion idleLocalRotation;
    private bool isReady;


    void Awake()
    {
        followTarget = transform;
        if (GetComponent<AttackManager>() != null)
        {
            var pivot = new GameObject("PickaxeFollow");
            pivot.transform.SetParent(transform, false);
            followTarget = pivot.transform;
        }
        if (modelRoot == null) modelRoot = followTarget;
        idleLocalRotation = modelRoot.localRotation;
        GameEvent.Input.PointerMove.Subscribe(pos => PointerMove(pos)).AddTo(this);
        GameEvent.Input.PointerAreaIn.Subscribe(isAreaIn => PointerAreaIn(isAreaIn)).AddTo(this);
    }

    void Start()
    {
        AttackManager.Inst.OnPickaxeAttackTiming.Subscribe(_ => PlayAttack()).AddTo(this);
    }

    public void SetModel(GameObject modelPrefab)
    {
        ClearModel();
        if (modelPrefab == null)
        {
            isReady = false;
            return;
        }
        followTarget.localRotation = Quaternion.Euler(modelLocalEuler);

        currentModel = Instantiate(modelPrefab, modelRoot);
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.Euler(Vector3.zero);
        currentModel.transform.localScale = Vector3.one * modelLocalScale;

        if (anim == null)
            anim = currentModel.GetComponent<SimpleAnimation>()
                ?? currentModel.GetComponentInChildren<SimpleAnimation>();

        isReady = true;
    }

    public void ClearModel()
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
            currentModel = null;
        }
        isReady = false;
    }

    private void PlayAttack()
    {
        if (!isReady) return;

        if (anim != null)
        {
            anim.Rewind();
            anim.Play(animName_attack);
            return;
        }

        /*
                modelRoot.DOKill();
                modelRoot.localRotation = idleLocalRotation;
                var seq = DOTween.Sequence().SetTarget(modelRoot);
                seq.Append(modelRoot.DOLocalRotate(attackRotate, attackAnimDuration * 0.4f, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.OutQuad));
                seq.Append(modelRoot.DOLocalRotate(idleLocalRotation.eulerAngles, attackAnimDuration * 0.6f)
                    .SetEase(Ease.InOutQuad));
                    */
    }


    #region -- position fix --
    private void PointerMove(Vector3 pos)
    {
        followTarget.position = pos + offsetPosition;
    }

    private void PointerAreaIn(bool isAreaIn)
    {
        if (currentModel == null) return;
        if (isAreaIn == currentModel.activeSelf) return;
        currentModel.SetActive(isAreaIn);
    }
    #endregion
}
