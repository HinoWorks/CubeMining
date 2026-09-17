using UnityEngine;
using UniRx;
using System;
using DG.Tweening;

public class MiningTarget_Bomb_PickaxePower : MonoBehaviour
{
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private const string EmissionKeyword = "_EMISSION";

    [SerializeField] float baseExplosionSize = 3f;
    [SerializeField] private GameObject pf_bomb;
    [SerializeField] private Material emissionMaterial;

    private int index_SE_Break => 25;
    private float explodeTimeMin = 1.5f;
    private float explodeTimeMax = 2.5f;
    private float blinkIntervalMax = 0.35f;
    private float blinkIntervalMin = 0.06f;
    private int damage;
    private float sizeRate;
    private IDisposable explodeTimer;
    private Material emissionMatInstance;
    private Color emissionColorOn = Color.black;

    public void Init(int _damage, float _sizeRate)
    {
        damage = _damage;
        sizeRate = _sizeRate;
        transform.localRotation = Quaternion.identity;
        transform.DOKill();
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack).SetLink(gameObject).Play();
        SetupEmissionMaterial();
        StartTimer();
    }

    private void Awake()
    {
        SetupEmissionMaterial();
    }

    private void OnDestroy()
    {
        explodeTimer?.Dispose();
        explodeTimer = null;
        if (emissionMatInstance != null)
            Destroy(emissionMatInstance);
    }

    private void StartTimer()
    {
        explodeTimer?.Dispose();
        var explodeTime = UnityEngine.Random.Range(explodeTimeMin, explodeTimeMax);
        var startTime = Time.time;
        var nextToggleTime = startTime;
        var emissionOn = false;

        explodeTimer = Observable.EveryUpdate().Subscribe(_ =>
        {
            var elapsed = Time.time - startTime;
            if (elapsed >= explodeTime)
            {
                explodeTimer?.Dispose();
                explodeTimer = null;
                SetEmission(true);
                Explode();
                return;
            }

            if (Time.time < nextToggleTime) return;

            emissionOn = !emissionOn;
            SetEmission(emissionOn);
            var t = Mathf.Clamp01(elapsed / explodeTime);
            var interval = Mathf.Lerp(blinkIntervalMax, blinkIntervalMin, t * t);
            nextToggleTime = Time.time + interval;
        }).AddTo(this);
    }

    private void Explode()
    {
        explodeTimer?.Dispose();
        explodeTimer = null;

        SoundManager.Inst.PlaySE(index_SE_Break);
        CameraManager.Inst?.ShakeCamera_BlockBreak();

        var newBomb = Instantiate(pf_bomb, InGameManager.Inst.ParentPool) as GameObject;
        var bomb = newBomb.GetComponent<MiningTarget_BombAttackArea>();
        bomb.transform.position = transform.position;

        var explodeSize = baseExplosionSize * sizeRate * (1f + ArtifactManager.Inst.bomb_sizeRate);
        bomb.Explode(damage, explodeSize);
        NotActivate();
    }

    private void NotActivate()
    {
        explodeTimer?.Dispose();
        explodeTimer = null;
        SetEmission(false);
        gameObject.SetActive(false);
    }

    private void SetupEmissionMaterial()
    {
        if (emissionMatInstance != null || emissionMaterial == null) return;

        emissionMatInstance = new Material(emissionMaterial);
        emissionColorOn = emissionMatInstance.HasProperty(EmissionColorId)
            ? emissionMatInstance.GetColor(EmissionColorId)
            : Color.red;

        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var renderer in renderers)
        {
            var shared = renderer.sharedMaterials;
            var changed = false;
            for (int i = 0; i < shared.Length; i++)
            {
                if (shared[i] != emissionMaterial) continue;
                shared[i] = emissionMatInstance;
                changed = true;
            }
            if (changed)
                renderer.sharedMaterials = shared;
        }

        SetEmission(false);
    }

    private void SetEmission(bool on)
    {
        if (emissionMatInstance == null) return;

        if (on)
        {
            emissionMatInstance.EnableKeyword(EmissionKeyword);
            emissionMatInstance.SetColor(EmissionColorId, emissionColorOn);
        }
        else
        {
            emissionMatInstance.DisableKeyword(EmissionKeyword);
            emissionMatInstance.SetColor(EmissionColorId, Color.black);
        }
    }
}
