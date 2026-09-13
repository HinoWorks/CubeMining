using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PickaxePowerCont_ChainLightningOrigin : MonoBehaviour
{
    [SerializeField] SimpleAnimation anim;
    [SerializeField] string spawnAnimName = "Start";
    [SerializeField] string shotAnimName = "Shot";
    [SerializeField] string endAnimName = "End";
    [SerializeField] ParticleSystem eff_shot;


    void Awake()
    {
        if (anim == null)
        {
            anim = GetComponent<SimpleAnimation>();
        }
    }

    public void PlaySpawn()
    {
        Play(spawnAnimName);
    }

    public void PlayShot()
    {
        Play(shotAnimName);
    }
    public void PlayShotEff()
    {
        eff_shot.Stop();
        eff_shot.Play();
    }

    public void PlayEndAndDestroy()
    {
        Play(endAnimName);
        DestroyAfterEnd().Forget();
    }

    private async UniTaskVoid DestroyAfterEnd()
    {
        var delay = GetClipLength(endAnimName);
        if (delay <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        var canceled = await UniTask.Delay(
                TimeSpan.FromSeconds(delay),
                cancellationToken: this.GetCancellationTokenOnDestroy())
            .SuppressCancellationThrow();
        if (canceled || this == null) return;

        Destroy(gameObject);
    }

    private void Play(string animName)
    {
        if (anim == null || string.IsNullOrEmpty(animName)) return;
        anim.Rewind();
        anim.Play(animName);
    }

    private float GetClipLength(string animName)
    {
        if (anim == null || string.IsNullOrEmpty(animName)) return 0f;
        var state = anim.GetState(animName);
        if (state == null || !state.isValid) return 0f;
        return state.length;
    }
}
